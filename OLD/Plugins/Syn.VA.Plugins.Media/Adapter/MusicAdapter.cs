using System;
using System.Linq;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;
using Syn.VA.Controls;
using Syn.VA.Extensions;
using Syn.VA.Libraries.Multimedia.Music;

namespace Syn.VA.Plugins.Media.Adapter
{
    public class MusicAdapter : IAdapter
    {
        public MusicAdapter(MediaPlugin mediaPlugin)
        {
            MediaPlugin = mediaPlugin;
        }

        private MediaPlugin MediaPlugin { get; }

        public bool IsRecursive => true;

        public XName TagName => SimlSpecification.Namespace.O + "Music";

        public string Evaluate(Context context)
        {
            try
            {
                var songPlayer = MediaPlugin.MusicControl.Player;
                var genreAttribute = context.Element.Attribute("Genre");
                var titleAttribute = context.Element.Attribute("Title");
                var artistAttribute = context.Element.Attribute("Artist");
                var albumAttribute = context.Element.Attribute("Album");
                var taskAttribute = context.Element.Attribute("Task");

                if (genreAttribute != null)
                {
                    var songList = songPlayer.Indexer.GetSongs(SongTag.Genre, genreAttribute.Value);
                    context.User.Vars["music-genre"].Value = genreAttribute.Value;
                    if (songList.Any())
                    {
                        songPlayer.Play(songList);
                        AddPlayer();
                        context.Bot.Trigger("music-genre-played", context.User);
                    }
                    else
                    {
                        context.Bot.Trigger("music-genre-not-found", context.User);
                    }
                }
                else if (titleAttribute != null)
                {
                    var songList = songPlayer.Indexer.GetSongs(SongTag.Title, titleAttribute.Value);
                    context.User.Vars["music-title"].Value = titleAttribute.Value;
                    if (songList.Any())
                    {
                        songPlayer.Play(songList);
                        AddPlayer();
                        context.Bot.Trigger("music-title-played", context.User);
                    }
                    else
                    {
                        context.Bot.Trigger("music-title-not-found", context.User);
                    }
                }
                else if (artistAttribute != null)
                {
                    var songList = songPlayer.Indexer.GetSongs(SongTag.Artist, artistAttribute.Value);
                    context.User.Vars["music-artist"].Value = artistAttribute.Value;
                    if (songList.Any())
                    {
                        songPlayer.Play(songList);
                        AddPlayer();
                        context.Bot.Trigger("music-artist-played", context.User);
                    }
                    else
                    {
                        context.Bot.Trigger("music-artist-not-found", context.User);
                    }
                }
                else if (albumAttribute != null)
                {
                    var songList = songPlayer.Indexer.GetSongs(SongTag.Album, albumAttribute.Value);
                    context.User.Vars["music-album"].Value = albumAttribute.Value;
                    if (songList.Any())
                    {
                        songPlayer.Play(songList);
                        AddPlayer();
                        context.Bot.Trigger("music-album-played", context.User);
                    }
                    else
                    {
                        context.Bot.Trigger("music-album-not-found", context.User);
                    }
                }
                else if (taskAttribute != null)
                {
                    if (songPlayer.State == MusicPlayerState.Idle)
                    {
                        context.Bot.Trigger("music-idle", context.User);
                        return string.Empty;
                    }

                    if (taskAttribute.Value.EqualsWithoutCase("Resume"))
                    {
                        songPlayer.Play();
                        context.Bot.Trigger("music-resumed", context.User);
                    }
                    else if (taskAttribute.Value.EqualsWithoutCase("Stop"))
                    {
                        songPlayer.Stop();
                        songPlayer.PlayList.Clear();
                        RemovePlayer();
                        context.Bot.Trigger("music-stopped", context.User);
                    }
                    else if (taskAttribute.Value.EqualsWithoutCase("Pause"))
                    {
                        songPlayer.Pause();
                        context.Bot.Trigger("music-paused", context.User);
                    }
                    else if (taskAttribute.Value.EqualsWithoutCase("Next"))
                    {
                        songPlayer.PlayNext();
                        context.Bot.Trigger("Music-Next-Track", context.User);
                    }
                    else if (taskAttribute.Value.EqualsWithoutCase("Previous"))
                    {
                        songPlayer.PlayPrevious();
                        songPlayer.PlayNext();
                        context.Bot.Trigger("Music-Previous-Track", context.User);
                    }
                }
                else
                {
                    if (songPlayer.PlayList.Count == 0)
                    {
                        songPlayer.PlayList.AddRange(songPlayer.Indexer.SongList);
                    }

                    if (songPlayer.PlayList.Any())
                    {
                        songPlayer.Play();
                        AddPlayer();
                        context.Bot.Trigger("Music-Played", context.User);
                    }
                    else
                    {
                        context.Bot.Trigger("Music-No-Media", context.User);
                    }
                }
            }
            catch (Exception exception)
            {
                VirtualAssistant.Instance.Logger.Error(exception);
            }

            return string.Empty;
        }

        private void AddPlayer()
        {
            var displayPanel = VirtualAssistant.Instance.Components.Get<DisplayPanel>();
            //Add control only if it's not added already.
            if (!displayPanel.Contains(MediaPlugin.MusicPlayerDisplayItem))
            {
                displayPanel.Add(MediaPlugin.MusicPlayerDisplayItem);
            }
        }

        private void RemovePlayer()
        {
            var displayPanel = VirtualAssistant.Instance.Components.Get<DisplayPanel>();
            //Remove control only if it exist.
            if (displayPanel.Contains(MediaPlugin.MusicPlayerDisplayItem))
            {
                displayPanel.Remove(MediaPlugin.MusicPlayerDisplayItem);
            }
        }
    }
}