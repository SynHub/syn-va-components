using System;
using Syn.Bot.Siml;
using Syn.VA.Plugins.Mock.Adapter;

namespace Syn.VA.Plugins.Mock
{
    public class MockPlugin : Plugin
    {
        public MockPlugin()
        {
            try
            {
                var bot = VA.Components.Get<SimlBot>();
                bot.Adapters.Add(new MockAdapter());
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
