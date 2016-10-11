using System.Collections.Generic;
using Syn.Bot.Siml.Interfaces;

namespace Syn.VA.Plugins.Media.Set
{
    class SongGenreSet : ISet
    {
        public SongGenreSet(MediaPlugin mediaPlugin)
        {
            Plugin = mediaPlugin;
        }

        private MediaPlugin Plugin { get; }

        public string Name => "Song-Genre";

        public IEnumerable<string> GetValues(string parameter = "")
        {
            var table = Plugin.MusicControl.Player.Indexer.TagTable;
            if (table.ContainsKey("Genre"))
            {
                return table["Genre"];
            }
            
            return new List<string>();
        }

        public bool Contains(string value, string parameter = "")
        {
            var table = Plugin.MusicControl.Player.Indexer.TagTable;
            if (table.ContainsKey("Genre"))
            {
                return table["Genre"].Contains(value);
            }
            return false;
        }
    }
}