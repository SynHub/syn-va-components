using Syn.Utility;

namespace Syn.VA.Plugins.Messaging.View
{
    /// <summary>
    /// Interaction logic for MessagingPanel.xaml
    /// </summary>
    public partial class MessagingPanel
    {
        public MessagingPanel()
        {
            InitializeComponent();
            SynUtility.Wpf.RestoreDesignBackground(this);
        }
    }
}
