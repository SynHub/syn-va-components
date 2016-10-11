using Syn.Utility;

namespace Syn.VA.Plugins.Windows.View
{
    /// <summary>
    /// Interaction logic for WindowsPanel.xaml
    /// </summary>
    public partial class WindowsPanel
    {
        public WindowsPanel()
        {
            InitializeComponent();
            SynUtility.Wpf.RestoreDesignBackground(this);
        }
    }
}
