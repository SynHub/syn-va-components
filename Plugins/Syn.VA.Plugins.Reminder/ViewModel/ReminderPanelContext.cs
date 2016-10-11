using System;
using System.Collections.ObjectModel;
using Prism.Commands;
using Syn.Utility;

namespace Syn.VA.Plugins.Reminder.ViewModel
{
    class ReminderPanelContext : ViewModelBase
    {
        private ReminderItem _selectedItem;

        public ReminderPanelContext()
        {
            RemoveItemCommand = new DelegateCommand(() =>
            {
                RemoveItemAction?.Invoke();
            });
            ReminderList = new ObservableCollection<ReminderItem>();
        }

        public ObservableCollection<ReminderItem> ReminderList { get; set; }

        public ReminderItem SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; OnPropertyChanged("SelectedItem"); }
        }
            
        internal Action RemoveItemAction { get; set; }
        public DelegateCommand RemoveItemCommand { get; }
    }
}