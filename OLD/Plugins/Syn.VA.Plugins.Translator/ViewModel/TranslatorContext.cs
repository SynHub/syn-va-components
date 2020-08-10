using System.Collections.ObjectModel;
using Syn.Utility;

namespace Syn.VA.Plugins.Translator.ViewModel
{
    class TranslatorContext : ViewModelBase
    {
        private string _selectedLanguage;

        public TranslatorContext()
        {
            LanguageCollection = new ObservableCollection<string>();
        }

        public ObservableCollection<string> LanguageCollection { get; set; }

        public string SelectedLanguage
        {
            get { return _selectedLanguage; }
            set { _selectedLanguage = value; OnPropertyChanged("SelectedLanguage");  }
        }
    }
}