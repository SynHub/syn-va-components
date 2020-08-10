using System;
using Prism.Commands;
using Syn.Utility;

namespace Syn.VA.Plugins.Weather.ViewModel
{
    public class WeatherContext : ViewModelBase
    {
        private string _location;

        public WeatherContext()
        {
            SaveCommand = new DelegateCommand(() => { SaveAction?.Invoke(); });
        }

        public string Location
        {
            get { return _location; }
            set { _location = value; OnPropertyChanged("Location"); }
        }
        public Action SaveAction { get; set; }
        public DelegateCommand SaveCommand { get; set; }
    }
}