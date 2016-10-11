using System;
using Syn.Bot.Siml;
using Syn.VA.Plugins.HtmlPanel.Adapter;

namespace Syn.VA.Plugins.HtmlPanel
{
    public class HtmlPanelPlugin : Plugin
    {
        public HtmlPanelPlugin()
        {
            try
            {
                var bot = VA.Components.Get<SimlBot>();
                bot.Adapters.Add(new HtmlPanelAdapter());
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        public override T GetPanel<T>(params object[] parameters)
        {
            return null;
        }
    }
}