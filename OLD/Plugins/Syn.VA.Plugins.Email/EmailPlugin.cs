using System;
using Syn.Bot.Siml;
using Syn.Utility;
using Syn.VA.Controls;
using Syn.VA.Libraries.Language.Locale;
using Syn.VA.Plugins.Email.Adapter;
using Syn.VA.Plugins.Email.View;
using Syn.VA.Plugins.Email.ViewModel;
using Syn.VA.Utility.Extensions;

namespace Syn.VA.Plugins.Email
{
    public class EmailPlugin : Plugin
    {
        public EmailPlugin()
        {
            try
            {
                var bot = VA.Components.Get<SimlBot>();
                bot.Adapters.Add(new EmailAdapter());

                var checkEmailService = new Service("Email-Service", () =>
                {
                    EmailAdapter.CheckNewEmail(bot.MainUser, true);
                });

                checkEmailService.Description = StringResource.EmailPlugin_ServiceDescription;
                checkEmailService.Interval = TimeSpan.FromMinutes(5);

                VA.Services.Add(checkEmailService);
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        public override string Name => "Email";

        public override string DisplayName => StringResource.EmailPlugin_DisplayName;

        public override T GetPanel<T>(params object[] parameters)
        {
            try
            {
                var emailPanel = new EmailPanel();
                var emailContext = emailPanel.DataContext as EmailContext;
                if (emailContext != null)
                {
                    Settings.ApplyToProperties(emailContext, "Bot-Email", "Service-Provider");
                    if (!string.IsNullOrEmpty(Settings["Password"].Value))
                    {
                        emailPanel.UserPasswordBox.Password = SynUtility.Security.Decrypt(Settings["Password"].Value);
                        emailPanel.UserConfirmPasswordBox.Password = SynUtility.Security.Decrypt(Settings["Password"].Value);
                    }

                    emailContext.SaveAction = () =>
                    {
                        try
                        {
                            var settingsManager = VA.SettingsManager;
                            var settingsWindow = VA.Components.Get<SettingsWindow>();
                            if (emailPanel.UserPasswordBox.Password == emailPanel.UserConfirmPasswordBox.Password)
                            {
                                Settings["Bot-Email"].Value = emailContext.BotEmail;
                                Settings["Service-Provider"].Value = emailContext.ServiceProvider;
                                Settings["Password"].Value = !string.IsNullOrEmpty(emailPanel.UserPasswordBox.Password) ? SynUtility.Security.Encrypt(emailPanel.UserPasswordBox.Password) : string.Empty;
                                settingsManager.Save(Settings, StringResource.EmailPlugin_SettingsSavedMessage);
                            }
                            else
                            {
                                settingsWindow.DisplayMessage(Name, StringResource.EmailPlugin_WrongPasswordMessage);
                            }
                        }
                        catch (Exception exception)
                        {
                            VA.Logger.Error(exception);
                        }
                    };
                }
                return emailPanel as T;
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }

            return null;
        }
    }
}