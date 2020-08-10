using System;
using System.Collections.ObjectModel;
using Prism.Commands;
using Syn.Utility;
using Syn.VA.Plugins.Skype.Model;

namespace Syn.VA.Plugins.Skype.ViewModel
{
    class SkypeContext : ViewModelBase
    {
        private ObservableCollection<SkypeUser> _users;

        public SkypeContext()
        {
            Users = new ObservableCollection<SkypeUser>();

            TestCommand = new DelegateCommand(() =>
            {
                SkypeCommandLauncher.StartCall("echo123");
            });

            AddCommand = new DelegateCommand(() =>
            {
                AddAction?.Invoke();
            });

            RemoveCommand = new DelegateCommand(() =>
            {
                RemoveAction?.Invoke();
            });
        }

        public ObservableCollection<SkypeUser> Users
        {
            get { return _users; }
            set { _users = value; OnPropertyChanged("Users"); }
        }

        public DelegateCommand TestCommand { get; set; }

        public Action AddAction { get; set; }
        public DelegateCommand AddCommand { get; set; }

        public Action RemoveAction { get; set; }
        public DelegateCommand RemoveCommand { get; set; }
    }
}
