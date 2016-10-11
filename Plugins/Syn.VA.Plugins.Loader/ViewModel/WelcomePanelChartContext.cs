using LiveCharts;
using Syn.Utility;

namespace Syn.VA.Plugins.Loader.ViewModel
{
    class WelcomePanelChartContext : ViewModelBase
    {
        private SeriesCollection _seriesCollection;

        public SeriesCollection SeriesCollection
        {
            get { return _seriesCollection; }
            set { _seriesCollection = value; OnPropertyChanged("SeriesCollection"); }
        }
    }
}