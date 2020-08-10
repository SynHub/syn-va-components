using Syn.Utility;

namespace Syn.VA.Plugins.Horoscope.View
{
    /// <summary>
    /// Interaction logic for HoroscopePanel.xaml
    /// </summary>
    public partial class HoroscopePanel
    {
        public HoroscopePanel()
        {
            InitializeComponent();
            SynUtility.Wpf.RestoreDesignBackground(this);
        }
    }
}