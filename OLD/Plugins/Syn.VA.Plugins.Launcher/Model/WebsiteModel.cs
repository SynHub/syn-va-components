using Syn.Utility;

namespace Syn.VA.Plugins.Launcher.Model
{
    public class WebsiteModel : ViewModelBase
    {
        public WebsiteModel(string name, string link)
        {
            Name = name;
            Link = link;
        }

        public string Name { get; set; }
        public string Link { get; set; }
    }
}
