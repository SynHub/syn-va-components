using Syn.Utility;

namespace Syn.VA.Plugins.Email.View
{
    /// <summary>
    /// Interaction logic for EmailPanel.xaml
    /// </summary>
    public partial class EmailPanel
    {
        public EmailPanel()
        {
            InitializeComponent();
            SynUtility.Wpf.RestoreDesignBackground(this);
        }
    }
}
