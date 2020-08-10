using Syn.VA.Utility.Extensions;

namespace Syn.VA.Plugins.Launcher.Model
{
    public class FolderModel : StringConstructor
    {
        public FolderModel(string name, string location): base(name)
        {
            this.Name = name;
            this.Location = location;
        }
        public string Name { get; set; }
        public string Location { get; set; }
    }
}