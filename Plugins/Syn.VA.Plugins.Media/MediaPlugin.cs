using System;
using System.Windows.Forms;
using System.Windows.Threading;
using Syn.Bot.Siml;
using Syn.Utility;
using Syn.VA.Controls;
using Syn.VA.Libraries.Language.Locale;
using Syn.VA.Libraries.Multimedia.Music;
using Syn.VA.Libraries.Multimedia.Video;
using Syn.VA.Plugins.Media.Adapter;
using Syn.VA.Plugins.Media.Set;
using Syn.VA.Plugins.Media.View;
using Syn.VA.Plugins.Media.ViewModel;
using Syn.VA.Utility.Extensions;

namespace Syn.VA.Plugins.Media
{
    public class MediaPlugin : Plugin
    {
        public MediaPlugin()
        {
            try
            {
                var bot = VA.Components.Get<SimlBot>();
                MusicControl = new MusicControl();
                VideoWindow = new VideoWindow();

                VideoWindow.Closing += (sender, args) =>
                {
                    try
                    {
                        args.Cancel = true;
                        VideoWindow.Hide();
                        VideoWindow.Player.Stop();
                    }
                    catch (Exception exception)
                    {
                        VA.Logger.Error(exception);
                    }
                };

                VA.Loaded += (sender, args) =>
                {
                    try
                    {
                        var dispatcher = VA.Components.Get<Dispatcher>();
                        MusicControl.Player.Indexer.IndexingCompleted += (sender1, args1) =>
                        {
                            dispatcher.Invoke(() =>
                            {
                                bot.Trigger("Music-Directory-Indexed");
                            });
                        };

                        VideoWindow.Player.Indexer.IndexingCompleted += (sender1, args1) =>
                        {
                            dispatcher.Invoke(() =>
                            {
                                bot.Trigger("Video-Directory-Indexed");
                            });
                        };
                    }
                    catch (Exception exception)
                    {
                        VA.Logger.Error(exception);
                    }
                };
                
                var musicDirectoryVariable = Settings["Music-Directory"];
                var videoDirectoryVariable = Settings["Video-Directory"];

                if (string.IsNullOrEmpty(musicDirectoryVariable.Value))
                {
                    var defaultMusicDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic);;
                    musicDirectoryVariable.Value = defaultMusicDirectory;
                    MusicControl.Player.Indexer.ClearCache();
                }

                if (string.IsNullOrEmpty(videoDirectoryVariable.Value))
                {
                    var defaultVideoDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonVideos);
                    videoDirectoryVariable.Value = defaultVideoDirectory;
                    VideoWindow.Player.Indexer.ClearCache();
                }

                if (!string.IsNullOrEmpty(musicDirectoryVariable.Value))
                {
                    MusicControl.Player.Indexer.StartIndexing(musicDirectoryVariable.Value);
                }

                if (!string.IsNullOrEmpty(videoDirectoryVariable.Value))
                {
                    VideoWindow.Player.Indexer.StartIndexing(videoDirectoryVariable.Value);
                }

                //TopMost because Song player should stay on the top until closed.
                MusicPlayerDisplayItem = new DisplayItem(MusicControl) { TopMost = true };
                bot.Adapters.Add(new MusicAdapter(this));
                bot.Sets.Add(new SongGenreSet(this));
                bot.Sets.Add(new SongTitleSet(this));
                bot.Sets.Add(new SongArtistSet(this));
                bot.Sets.Add(new SongAlbumSet(this));
                bot.Adapters.Add(new VideoAdapter(this));
                bot.Sets.Add(new VideoTitleSet(this));
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        public MusicControl MusicControl { get; }
        public VideoWindow VideoWindow { get; }
        public DisplayItem MusicPlayerDisplayItem { get; }

        public override string Name => "Media";
        public override string DisplayName => StringResource.MediaPlugin_DisplayName;

        public override T GetPanel<T>(params object[] parameters)
        {
            try
            {
                var mediaPanel = new MediaPanel();
                var context = mediaPanel.DataContext as MediaPanelContext;
                var settingsWindow = VA.Components.Get<SettingsWindow>();
                if (context != null)
                {
                    Settings.ApplyToProperties(context, "Music-Directory", "Video-Directory");

                    context.MusicLabel = string.Format(StringResource.MediaPlugin_MusicDirectoryLabelContentFormat, MusicControl.Player.Indexer.SongList.Count);
                    context.VideoLabel = string.Format(StringResource.MediaPlugin_VideoDirectoryLabelContentFormat, VideoWindow.Player.Indexer.VideoList.Count);

                    context.MusicBrowseAction = () =>
                    {
                        try
                        {
                            var dialog = new FolderBrowserDialog();
                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                var selectedPath = dialog.SelectedPath;
                                Settings["Music-Directory"].Value = context.MusicDirectory = selectedPath;
                                settingsWindow.DisplayMessage(Name, StringResource.MediaPlugin_IndexingMessage);
                                MusicControl.Player.Indexer.ClearCache();
                                MusicControl.Player.Indexer.StartIndexing(selectedPath);
                                VA.SettingsManager.Save(Settings);
                            }
                        }
                        catch (Exception exception)
                        {
                            VA.Logger.Error(exception);
                        }
                    };

                    context.VideoBrowseAction = () =>
                    {
                        try
                        {
                            var dialog = new FolderBrowserDialog();
                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                var selectedPath = dialog.SelectedPath;
                                Settings["Video-Directory"].Value = context.VideoDirectory = selectedPath;
                                settingsWindow.DisplayMessage(Name, Settings["Indexing-Message"].Value);
                                VideoWindow.Player.Indexer.ClearCache();
                                VideoWindow.Player.Indexer.StartIndexing(selectedPath);
                                VA.SettingsManager.Save(Settings);
                            }
                        }
                        catch (Exception exception)
                        {
                            VA.Logger.Error(exception);
                        }
                    };
                }

                return mediaPanel as T;
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }

            return null;
        }
    }
}