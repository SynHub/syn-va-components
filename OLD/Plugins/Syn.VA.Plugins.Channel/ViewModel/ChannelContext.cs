using System;
using System.Collections.ObjectModel;
using Syn.Utility;
using Syn.VA.Plugins.Channel.Server;

namespace Syn.VA.Plugins.Channel.ViewModel
{
    public class ChannelContext : ViewModelBase
    {
        private int _portNumber;
        private ObservableCollection<string> _messages;
        private bool _enabled;

        public ChannelContext()
        {
            try
            {
                PortNumber = 8885;
                Enabled = false;
                Messages = new ObservableCollection<string>();
                foreach (var item in ChannelServer.StatusList)
                {
                    Messages.Add(item);
                }

                ChannelServer.StatusList.CollectionChanged += (sender, args) =>
                {
                    Messages = ChannelServer.StatusList;
                };
            }
            catch (Exception exception)
            {
                VirtualAssistant.Instance.Logger.Error(exception);
            }
        }

        public int PortNumber
        {
            get { return _portNumber; }
            set
            {
                _portNumber = value;
                OnPropertyChanged("PortNumber");
            }
        }

        public ObservableCollection<string> Messages
        {
            get { return _messages; }
            set { _messages = value; OnPropertyChanged("Messages"); }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; OnPropertyChanged("Enabled"); }
        }
    }
}