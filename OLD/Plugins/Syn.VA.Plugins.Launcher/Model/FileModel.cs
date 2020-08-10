
using Syn.VA.Utility.Extensions;

namespace Syn.VA.Plugins.Launcher.Model
{

    public class FileModel : StringConstructor
    {

        public FileModel(string name): base(name)
        {
            Name = Location = name;
        }
        public FileModel(string name, string path): base(name)
        {
            Name = name;
            Location = path;
        }

        public string Name { get; set; }
        public string Location { get; set; }
    }
}
