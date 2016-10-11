using System;
using System.Collections.Generic;
using Syn.Bot.Siml.Interfaces;
using Syn.VA.Extensions;

namespace Syn.VA.Plugins.Media.Set
{
    class VideoTitleSet : ISet
    {
        public VideoTitleSet(MediaPlugin mediaPlugin)
        {
            Plugin = mediaPlugin;
        }

        private MediaPlugin Plugin { get; }

        public string Name => "Video-Title";

        public IEnumerable<string> GetValues(string parameter = "")
        {
            return Plugin.VideoWindow.Player.Indexer.TagTable["Title"];
        }

        public bool Contains(string value, string parameter = "")
        {
            var titleTable = Plugin.VideoWindow.Player.Indexer.TagTable["Title"];
            foreach (var item in titleTable)
            {
                if (item.ContainsWithoutCase(value)) return true;
            }
            return false;
        }
    }
}