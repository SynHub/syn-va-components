using System;
using System.Collections.ObjectModel;
using Prism.Commands;
using Syn.Utility;

namespace Syn.VA.Plugins.Loader.ViewModel
{
    public class UserContext : ViewModelBase
    {
        private string _firstName;
        private string _lastName;
        private string _middleName;
        private ObservableCollection<string> _genderList;
        private ObservableCollection<string> _countryList;
        private string _email;
        private string _name;

        public UserContext()
        {
            GenderList = new ObservableCollection<string>();
            CountryList = new ObservableCollection<string>();
            SaveCommand = new DelegateCommand(() =>
            {
                SaveAction?.Invoke();
            });
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                OnPropertyChanged("FirstName");
            }
        }

        public string MiddleName
        {
            get { return _middleName; }
            set
            {
                _middleName = value;
                OnPropertyChanged("MiddleName");
            }
            
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                OnPropertyChanged("LastName");
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged("Email");
            }
        }

        public ObservableCollection<string> GenderList
        {
            get { return _genderList; }
            set
            {
                _genderList = value;
                OnPropertyChanged("GenderList");
            }
        }

        public ObservableCollection<string> CountryList
        {
            get { return _countryList; }
            set
            {
                _countryList = value;
                OnPropertyChanged("CountryList");
            }
        }

        public Action SaveAction { get; set; }
        public DelegateCommand SaveCommand { get; private set; }
    }
}