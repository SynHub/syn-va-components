using System;
using Syn.Bot.Siml;

namespace Syn.VA.Plugins.MovieInfo
{
    public class MovieInfo: Plugin
    {
        public MovieInfo()
        {
            try
            {
                var bot = VA.Components.Get<SimlBot>();
                bot.Adapters.Add(new MovieInfoAdapter());
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