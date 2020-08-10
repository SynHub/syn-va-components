using System;
using Syn.Bot.Siml;
using Syn.VA.Plugins.Browser.Adapter;

namespace Syn.VA.Plugins.Browser
{
    public class BrowserPlugin : Plugin
    {
        public BrowserPlugin()
        {
            try
            {
                var bot = VA.Components.Get<SimlBot>();
                bot.Adapters.Add(new BrowserAdapter());
                bot.Adapters.Add(new WebPanelAdapter());
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