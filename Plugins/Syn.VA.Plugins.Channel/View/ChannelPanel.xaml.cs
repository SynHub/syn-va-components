using Syn.Utility;

namespace Syn.VA.Plugins.Channel.View
{
    /// <summary>
    /// Interaction logic for ChannelPanel.xaml
    /// </summary>
    public partial class ChannelPanel
    {
        public ChannelPanel()
        {
            InitializeComponent();
            SynUtility.Wpf.RestoreDesignBackground(this);
        }
    }
}