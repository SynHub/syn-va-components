using System.Collections.Generic;
using Syn.Bot.Siml.Interfaces;

namespace Syn.VA.Plugins.Skype.Set
{
    class SkypeIdSet : ISet
    {
        private readonly SkypePlugin _plugin;
        public SkypeIdSet(SkypePlugin plugin)
        {
            _plugin = plugin;
        }
        public string Name => "Skype-Id";


        public IEnumerable<string> GetValues(string parameter = "")
        {
           return _plugin.GetUsers().Values; 
        }

  
        public bool Contains(string item, string parameter)
        {
            return _plugin.GetUsers().ContainsValue(item);
        }

    }
}