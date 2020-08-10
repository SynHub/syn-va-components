using System;
using Prism.Commands;
using Syn.Utility;

namespace Syn.VA.Plugins.Messaging.ViewModel
{
    public class MessagingContext : ViewModelBase
    {
        private string _telegramApiKey;
        private bool _autoConnect;

        public MessagingContext()
        {
            SaveCommand = new DelegateCommand(() =>
            {
                SaveAction?.Invoke();
            });
        }

        public bool AutoConnect
        {
            get { return _autoConnect; }
            set
            {
                _autoConnect = value;
                OnPropertyChanged("AutoConnect");
            }
        }

        public string TelegramApiKey
        {
            get { return _telegramApiKey; }
            set
            {
                _telegramApiKey = value;
                OnPropertyChanged("TelegramApiKey");
            }
        }

        public Action SaveAction { get; set; }
        public DelegateCommand SaveCommand { get; set; }
    }
}