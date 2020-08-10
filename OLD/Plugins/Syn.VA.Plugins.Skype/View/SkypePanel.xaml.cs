using Syn.Utility;

namespace Syn.VA.Plugins.Skype.View
{
    /// <summary>
    /// Interaction logic for SkypePanel.xaml
    /// </summary>
    public partial class SkypePanel
    {
        public SkypePanel()
        {
            InitializeComponent();
            SynUtility.Wpf.RestoreDesignBackground(this);
        }
    }
}
