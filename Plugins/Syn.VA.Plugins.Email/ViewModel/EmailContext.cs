using System;
using Prism.Commands;
using Syn.Utility;

namespace Syn.VA.Plugins.Email.ViewModel
{
    class EmailContext : ViewModelBase
    {
        private string _botEmail;
        private string _serviceProvider;

        public EmailContext()
        {
            SaveCommand = new DelegateCommand(() =>
            {
                SaveAction?.Invoke();
            });
        }

        public string BotEmail
        {
            get { return _botEmail; }
            set { _botEmail = value; OnPropertyChanged("BotEmail"); }
        }

        public string ServiceProvider
        {
            get { return _serviceProvider; }
            set { _serviceProvider = value; OnPropertyChanged("ServiceProvider"); }
        }

        public Action SaveAction { get; set; }
        public DelegateCommand SaveCommand { get; set; }
    }
}