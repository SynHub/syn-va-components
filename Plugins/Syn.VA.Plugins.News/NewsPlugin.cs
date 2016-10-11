using System;
using Syn.Bot.Siml;
using Syn.VA.Plugins.News.Adapter;

namespace Syn.VA.Plugins.News
{
    public class NewsPlugin : Plugin
    {
        public NewsPlugin()
        {
            try
            {
                var bot = VA.Components.Get<SimlBot>();
                bot.Adapters.Add(new NewsAdapter(this));
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        public override string Name => "News";

        public override T GetPanel<T>(params object[] parameters)
        {
            return null;
        }
    }
}