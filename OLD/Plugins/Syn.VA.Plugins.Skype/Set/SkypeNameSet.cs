using System.Collections.Generic;
using Syn.Bot.Siml.Interfaces;

namespace Syn.VA.Plugins.Skype.Set
{
    class SkypeNameSet : ISet
    {
        private readonly SkypePlugin _plugin;
        public SkypeNameSet(SkypePlugin plugin)
        {
            _plugin = plugin;
        }
        public string Name => "Skype-Name";

        public IEnumerable<string> GetValues(string parameter = "")
        {
         return _plugin.GetUsers().Keys; 
        }

        public bool Contains(string item, string parameter)
        {
            return _plugin.GetUsers().ContainsKey(item);
        }
    }
}