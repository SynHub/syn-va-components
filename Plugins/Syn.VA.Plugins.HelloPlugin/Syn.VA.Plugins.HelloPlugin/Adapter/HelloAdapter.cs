using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;

namespace Syn.VA.Plugins.HelloPlugin.Adapter
{
    class HelloAdapter : IAdapter
    {
        public XName TagName => SimlSpecification.Namespace.X + "Hello";
        public bool IsRecursive => true;
        public string Evaluate(Context context)
        {
            return "HelloPlugin says Hello!";
        }
    }
}