using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml.Linq;
using ImapX;
using ImapX.Enums;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Helper;
using Syn.Bot.Siml.Interfaces;
using Syn.Utility;
using Syn.VA.Extensions;
using MailAddress = System.Net.Mail.MailAddress;

namespace Syn.VA.Plugins.Email.Adapter
{
    public class EmailAdapter : IAdapter
    {
        public bool IsRecursive => true;

        public XName TagName => SimlSpecification.Namespace.O + "Email";

        public static Message LastEmail { get; set; }

        public static void CheckNewEmail(BotUser user, bool isServiceCall)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    var va = VirtualAssistant.Instance;
                    var dispatcher = VirtualAssistant.Instance.Components.Get<Dispatcher>();
                    const string hostAddress = "imap.gmail.com";
                    var client = new ImapClient(hostAddress, true);
                    if (client.Connect())
                    {
                        var settings = va.SettingsManager["Email"];
                        var userEmailId = settings["Bot-Email"].Value;
                        var userPassword = SynUtility.Security.Decrypt(settings["Password"].Value);
                        //If login is successful.
                        if (client.Login(userEmailId, userPassword))
                        {
                            var message = client.Folders.Inbox.Search("UNSEEN", MessageFetchMode.Full, 1);
                            //If there are any Unseen messages.
                            if (message.Any())
                            {
                                var emailMessage = message[0];
                                dispatcher.Invoke(() =>
                                {
                                    SetUserVars(user, emailMessage);
                                    LastEmail = emailMessage;
                                    user.Bot.Trigger("Email-New");
                                    emailMessage.Seen = true;
                                    message[0].Remove();
                                });
                            }
                            else //No unseen message found.
                            {
                                dispatcher.Invoke(() =>
                                {
                                    if (!isServiceCall)
                                    {
                                        user.Bot.Trigger("Email-No-New");
                                    }
                                });
                            }
                        }
                        else
                        {
                            if (!isServiceCall)
                            {
                                dispatcher.Invoke(() =>
                                {
                                    user.Bot.Trigger("Email-Login-failed");
                                });
                            }
                        }
                    }
                    else
                    {
                        if (!isServiceCall)
                        {
                            dispatcher.Invoke(() =>
                            {
                                user.Bot.Trigger("Email-Connection-failed");
                            });
                        }
                    }
                });
            }
            catch (Exception exception)
            {
                VirtualAssistant.Instance.Logger.Error(exception);
            }
        }

        private static bool EmptyCredentials()
        {
            var va = VirtualAssistant.Instance;
            var emailSettings = va.SettingsManager["Email"];
            if (string.IsNullOrEmpty(emailSettings["Bot-Email"].Value)) return true;
            if (string.IsNullOrEmpty(emailSettings["Password"].Value)) return true;
            return false;
        }

        private static void SetUserVars(BotUser user, Message emailMessage)
        {
            user.Vars["Email-Subject"].Value = emailMessage.Subject;
            user.Vars["Email-Html"].Value = emailMessage.Body.Html;
            user.Vars["Email-Text"].Value = emailMessage.Body.Text;
            user.Vars["Email-Address"].Value = emailMessage.From.Address;
        }

        public string Evaluate(Context context)
        {
            var va = VirtualAssistant.Instance;
            try
            {
                if (EmptyCredentials())
                {
                    context.Bot.Trigger("Email-Empty-Credentials");
                }
                else
                {
                    var taskAttribute = context.Element.Attribute(Tag.TaskAttribute);
                    if (taskAttribute != null)
                    {
                        var taskValue = taskAttribute.Value;
                        if (taskValue.EqualsWithoutCase("check"))
                        {
                            CheckNewEmail(context.User, false);
                        }
                        else if (taskValue.EqualsWithoutCase("read"))
                        {
                            if (LastEmail != null)
                            {
                                SetUserVars(context.User, LastEmail);
                                context.Bot.Trigger(!LastEmail.Body.HasHtml ? "Email-Read-Text" : "Email-Read-Html");
                            }
                            else
                            {
                                context.Bot.Trigger("Email-No-New");
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                va.Logger.Error(exception);
            }
            return string.Empty;
        }

        // A method for sending messages
        public MailMessage SendMail(string smtpServer, string login, string password, string from, string to, string subject, string text)
        {
            try
            {
                var mail = new MailMessage {From = new MailAddress(@from)};
                mail.To.Add(new MailAddress(to));
                mail.Subject = subject;
                mail.Body = text;

                var client = new SmtpClient
                {
                    Host = smtpServer,
                    Port = 587,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(login, password),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };
                client.Send(mail);

                return mail;
            }
            catch (Exception exception)
            {
                VirtualAssistant.Instance.Logger.Error(exception);
            }
            return null;
        }
    }
}