using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Microsoft.Owin.Hosting;

namespace Syn.VA.Plugins.Channel.Server
{
    public class ChannelServer
    {
        //private const string FromPluginName = "ChannelPlugin";

        public ChannelServer()
        {
            try
            {
                Server = null;
                StatusList = new ObservableCollection<string>();
                ServiceTimer = new DispatcherTimer();
                ServiceTimer.Tick += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(CommandController.ApiSendMessage))
                    {
                        AddStatus($"API Send Message Received: {CommandController.ApiSendMessage}");
                        VirtualAssistant.Instance.Interaction.SendMessage(CommandController.ApiSendMessage);
                        CommandController.ApiSendMessage = string.Empty;
                    }
                };

                ServiceTimer.Interval = TimeSpan.FromMilliseconds(200); //Execute service tick every 0.2 second
            }
            catch (Exception exception)
            {
                VirtualAssistant.Instance.Logger.Error(exception);
            }
        }

        public DispatcherTimer ServiceTimer { get; set; }

        public IDisposable Server { get; set; }

        public void Start()
        {
            try
            {
                var portString = VirtualAssistant.Instance.SettingsManager["Channel"]["Port-Number"].Value;
                int portNumber;
                if (!int.TryParse(portString, out portNumber)) return;
                var baseAddress = $"http://localhost:{portNumber}/";
                Server = WebApp.Start<ServerConfiguration>(baseAddress);
                IsStarted = true;
                ServiceTimer.Start();
            }
            catch (Exception exception)
            {
                VirtualAssistant.Instance.Logger.Error(exception);
            }
        }

        public static ObservableCollection<string> StatusList { get; set; }

        public void Stop()
        {
            try
            {
                if (Server == null) return;
                Server.Dispose();
                IsStarted = false;
                ServiceTimer.Stop();
            }
            catch (Exception exception)
            {
                VirtualAssistant.Instance.Logger.Error(exception);
            }
        }

        public bool IsStarted { get; set; }

        private static void AddStatus(string status)
        {
            var text = $"{DateTime.Now}: {status}";
            StatusList.Insert(0, text);
        }
    }
}