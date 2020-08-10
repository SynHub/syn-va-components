using System.Collections.Generic;
using Syn.Bot.Siml.Interfaces;
using Syn.VA.Extensions;

namespace Syn.VA.Plugins.Media.Set
{
    class SongTitleSet : ISet
    {
        public SongTitleSet(MediaPlugin mediaPlugin)
        {
            Plugin = mediaPlugin;
        }

        private MediaPlugin Plugin { get; }

        public string Name => "Song-Title";

        public IEnumerable<string> GetValues(string parameter = "")
        {
            var table = Plugin.MusicControl.Player.Indexer.TagTable;
            if (table.ContainsKey("Title"))
            {
                return table["Title"];
            }
            
            return new List<string>();
        }

        public bool Contains(string value, string parameter = "")
        {
            var table = Plugin.MusicControl.Player.Indexer.TagTable;
            if (table.ContainsKey("Title"))
            {
                var titleTable = table["Title"];
                foreach (var item in titleTable)
                {
                    if (item.ContainsWithoutCase(value)) return true;
                }
            }
           
            return false;
        }
    }
}