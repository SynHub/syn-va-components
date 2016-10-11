using System.Collections.ObjectModel;
using Syn.Utility;

namespace Syn.VA.Plugins.Horoscope.ViewModel
{
    public class HoroscopeContext : ViewModelBase
    {
        private string _zodiacSign;
        private ObservableCollection<string> _zodiacSignList;

        public HoroscopeContext()
        {
            ZodiacSignList = new ObservableCollection<string>();
        }

        public string ZodiacSign
        {
            get { return _zodiacSign; }
            set { _zodiacSign = value; OnPropertyChanged("ZodiacSign"); }
        }

        public ObservableCollection<string> ZodiacSignList
        {
            get { return _zodiacSignList; }
            set { _zodiacSignList = value; OnPropertyChanged("ZodiacSignList"); }
        }
    }
}