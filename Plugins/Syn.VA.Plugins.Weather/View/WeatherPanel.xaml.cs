using Syn.Utility;

namespace Syn.VA.Plugins.Weather.View
{
    /// <summary>
    /// Interaction logic for WeatherPanel.xaml
    /// </summary>
    public partial class WeatherPanel
    {
        public WeatherPanel()
        {
            InitializeComponent();
            SynUtility.Wpf.RestoreDesignBackground(this);
        }
    }
}