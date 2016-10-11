using System;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;
using Syn.VA.Extensions;
using Syn.VA.Libraries.Multimedia.Video;

namespace Syn.VA.Plugins.Media.Adapter
{
    public class VideoAdapter : IAdapter
    {
        private readonly MediaPlugin _mediaPlugin;

        public VideoAdapter(MediaPlugin mediaPlugin)
        {
            _mediaPlugin = mediaPlugin;
        }

        public bool IsRecursive => true;

        public XName TagName => SimlSpecification.Namespace.O + "Video";

        public string Evaluate(Context context)
        {
            try
            {
                var videoWindow = _mediaPlugin.VideoWindow;
                var titleAttribute = context.Element.Attribute("Title");
                var taskAttribute = context.Element.Attribute("Task");
                if (titleAttribute != null)
                {
                    var titleValue = titleAttribute.Value;
                    var videoInfo = videoWindow.Player.Indexer.GetVideoByName(titleValue);
                    context.User.Vars["video-title"].Value = titleValue;
                    if (videoInfo != VideoInfo.Empty)
                    {
                        videoWindow.Show();
                        videoWindow.Player.Play(videoInfo);
                        context.Bot.Trigger("video-title-played", context.User);
                    }
                    else
                    {
                        context.Bot.Trigger("video-title-not-found", context.User);
                    }
                }
                else if (taskAttribute != null)
                {
                    var taskValue = taskAttribute.Value;
                    if (videoWindow.Player.State == VideoPlayerState.Idle)
                    {
                        context.Bot.Trigger("video-idle", context.User);
                    }
                    else if (taskValue.EqualsWithoutCase("Stop"))
                    {
                        videoWindow.Player.Stop();
                        videoWindow.Close();
                        context.Bot.Trigger("video-stopped", context.User);
                    }
                }
            }
            catch (Exception exception)
            {
                VirtualAssistant.Instance.Logger.Error(exception);
            }

            return string.Empty;
        }
    }
}