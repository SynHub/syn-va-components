using System;
using Syn.Utility;
using Syn.VA.Extensions;
using Syn.VA.Libraries.Language.Locale;
using Syn.VA.Plugins.Messaging.Model;
using Syn.VA.Plugins.Messaging.View;
using Syn.VA.Plugins.Messaging.ViewModel;
using Syn.VA.Utility.Extensions;

namespace Syn.VA.Plugins.Messaging
{
    public class MessagingPlugin : Plugin
    {
        public Messenger Messenger { get; set; }
        public const string ApiVariableName = "telegram-api-key";
        public MessagingPlugin()
        {
            try
            {
                VA.Loaded += (sender, args) =>
                {
                    Messenger = new Messenger();
                    if (CanAutoConnect)
                    {
                        Messenger.Connect();
                    }
                };
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        public override string Name => "Messaging";

        public override string DisplayName => StringResource.MessagingPlugin_DisplayName;

        /// <summary>
        /// Determines whether Settings has all the required credential-variables.
        /// </summary>
        /// <returns></returns>
        public bool CanConnect()
        {
            var hostName = Settings[ApiVariableName].Value;
            return Settings.Contains(ApiVariableName) && !string.IsNullOrEmpty(hostName);
        }

        public bool CanAutoConnect
        {
            get
            {
                var autoConnect = Settings["Auto-Connect"].ValueAs(false);
                return autoConnect && CanConnect();
            }
        }

        public override T GetPanel<T>(params object[] parameters)
        {
            try
            {
                var messagingPanel = new MessagingPanel();
                var messagingContext = messagingPanel.DataContext as MessagingContext;
                if (messagingContext != null)
                {
                    Settings.ApplyToProperties(messagingContext, "Auto-Connect");
                    if (!string.IsNullOrEmpty(Settings[ApiVariableName].Value))
                    {
                        messagingContext.TelegramApiKey = SynUtility.Security.Decrypt(Settings[ApiVariableName].Value);
                    }

                    messagingPanel.ApiBox.TextChanged += (sender, args) =>
                    {
                        if (string.IsNullOrEmpty(messagingPanel.ApiBox.Text))
                        {
                            Settings["Auto-Connect"].Value = messagingContext.AutoConnect.ToString();
                            Settings[ApiVariableName].Value = string.Empty;
                            VA.SettingsManager.Save(Settings, StringResource.MessagingPlugin_ApiClearedMessage);
                        }
                    };

                    messagingContext.SaveAction = () =>
                    {
                        if (!string.IsNullOrEmpty(messagingContext.TelegramApiKey))
                        {
                            Settings["Auto-Connect"].Value = messagingContext.AutoConnect.ToString();
                            Settings[ApiVariableName].Value = SynUtility.Security.Encrypt(messagingContext.TelegramApiKey);
                            VA.SettingsManager.Save(Settings, StringResource.MessagingPlugin_SettingsSaved);
                        }
                    };
                }
                return messagingPanel as T;
            }
            catch (Exception exception)
            {
                VirtualAssistant.Instance.Logger.Error(exception);
            }

            return null;
        }
    }
}