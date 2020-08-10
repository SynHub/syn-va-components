using System.Collections.Generic;
using Syn.Bot.Siml.Interfaces;
using Syn.VA.Extensions;

namespace Syn.VA.Plugins.Media.Set
{
    class SongArtistSet:ISet
    {
        public SongArtistSet(MediaPlugin mediaPlugin)
        {
            Plugin = mediaPlugin;
        }

        private MediaPlugin Plugin { get; }

        public string Name => "Song-Artist";

        public IEnumerable<string> GetValues(string parameter = "")
        {
            var table = Plugin.MusicControl.Player.Indexer.TagTable;
            if (table.ContainsKey("Artist"))
            {
                return table["Artist"];
            }
            return new List<string>();
        }

        public bool Contains(string value, string parameter = "")
        {
            var table = Plugin.MusicControl.Player.Indexer.TagTable;
            if (table.ContainsKey("Artist"))
            {
                var artistTable = table["Artist"];
                foreach (var item in artistTable)
                {
                    if (item.ContainsWithoutCase(value)) return true;
                }
            }
           
            return false;
        }
    }
}