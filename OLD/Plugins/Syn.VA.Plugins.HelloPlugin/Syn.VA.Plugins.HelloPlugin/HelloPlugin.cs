using Syn.Bot.Siml;
using Syn.VA.Plugins.HelloPlugin.Adapter;

namespace Syn.VA.Plugins.HelloPlugin
{
    public class HelloPlugin : Plugin
    {
        public HelloPlugin()
        {
            SimlBot.Instance.Adapters.Add(new HelloAdapter());
        }

        public override string Name => "Hello";
    }
}