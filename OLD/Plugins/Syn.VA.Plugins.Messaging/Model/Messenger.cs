using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using Syn.Bot.Siml;
using Syn.Utility;
using Syn.VA.Interaction;
using Telegram.Bot;
using Message = Telegram.Bot.Types.Message;

namespace Syn.VA.Plugins.Messaging.Model
{
    public class Messenger
    {
        private TelegramBotClient _telegramBot;
        private VirtualAssistant VA => VirtualAssistant.Instance;

        public void Connect()
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    var settings = VA.SettingsManager["Messaging"];
                    if (settings.Contains(MessagingPlugin.ApiVariableName))
                    {
                        var apiVariable = settings[MessagingPlugin.ApiVariableName];
                        if (!string.IsNullOrEmpty(apiVariable.Value))
                        {
                            var telegramApi = SynUtility.Security.Decrypt(apiVariable.Value);
                            _telegramBot = new TelegramBotClient(telegramApi);
                            User previousUser = null;
                            Message telegramMessage = null;
                            var simlBot = VA.Components.Get<SimlBot>();
                            var dispatcher = VA.Components.Get<Dispatcher>();

                            dispatcher.Invoke(() =>
                            {
                                VA.Interaction.SendMessage(new Interaction.Message("Messaging-Plugin-Connected", MessageType.EventMessage));
                            });

                            _telegramBot.OnMessage += (sender, args) =>
                            {
                                telegramMessage = args.Message;

                                var message = new Interaction.Message
                                {
                                    User = new User(telegramMessage.Chat.Id.ToString()),
                                    Text = telegramMessage.Text
                                };

                                previousUser = message.User;
                                dispatcher.Invoke(() =>
                                {
                                    simlBot.MainUser.Vars["Telegram-Message"].Value = message.Text;
                                    simlBot.Trigger("Messaging-Input-Received");
                                    var chatResult = simlBot.Chat(message.Text);
                                    _telegramBot.SendTextMessageAsync(telegramMessage.Chat.Id, chatResult.BotMessage);
                                });
                            };

                            _telegramBot.StartReceiving();
                        }
                    }
                });
            }
            catch (Exception exception)
            {
                VirtualAssistant.Instance.Logger.Error(exception);
            }
        }

        public void Disconnect()
        {
            if (_telegramBot != null && _telegramBot.IsReceiving)
            {
                _telegramBot.StopReceiving();
            }
        }
    }
}