using System;
using Syn.Bot.Siml;

namespace Syn.VA.Plugins.Loader
{
    internal class BotPlugin : Plugin
    {
        public BotPlugin()
        {
            try
            {
                var simlBot = VA.Components.Get<SimlBot>();
                Settings.Bind(simlBot.Settings);
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        public override string Name => "Bot";
    }
}