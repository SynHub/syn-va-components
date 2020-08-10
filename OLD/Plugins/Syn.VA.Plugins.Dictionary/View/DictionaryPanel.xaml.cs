using Syn.Utility;

namespace Syn.VA.Plugins.Dictionary.View
{
    /// <summary>
    /// Interaction logic for DictionaryPanel.xaml
    /// </summary>
    public partial class DictionaryPanel
    {
        public DictionaryPanel()
        {
            InitializeComponent();
            SynUtility.Wpf.RestoreDesignBackground(this);
        }
    }
}
