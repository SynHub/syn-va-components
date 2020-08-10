using System.Collections.Generic;
using Syn.Bot.Siml.Interfaces;
using Syn.VA.Extensions;

namespace Syn.VA.Plugins.Media.Set
{
    class SongAlbumSet : ISet
    {
        public SongAlbumSet(MediaPlugin mediaPlugin)
        {
            Plugin = mediaPlugin;
        }

        private MediaPlugin Plugin { get; }

        public string Name => "Song-Album";

        public IEnumerable<string> GetValues(string parameter = "")
        {
            var table = Plugin.MusicControl.Player.Indexer.TagTable;
            if (table.ContainsKey("Ablum"))
            {
                return table["Album"];
            }
            return new List<string>();
        }

        public bool Contains(string value, string parameter = "")
        {
            var table = Plugin.MusicControl.Player.Indexer.TagTable;
            if (table.ContainsKey("Album"))
            {
                var albumTable  = table["Album"];
                foreach (var item in albumTable)
                {
                    if (item.ContainsWithoutCase(value)) return true;
                }
            }
            return false;
        }
    }
}