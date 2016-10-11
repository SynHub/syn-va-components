using System;
using Syn.Bot.Siml;

namespace Syn.VA.Plugins.Wikipedia
{
    public class WikipediaPlugin : Plugin
    {
        public WikipediaPlugin()
        {
            try
            {
                var bot = VA.Components.Get<SimlBot>();
                bot.Adapters.Add(new WikipediaAdapter());
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
          
        }

        public override T GetPanel<T>(params object[] parameters)
        {
            return default(T);
        }
    }
}
