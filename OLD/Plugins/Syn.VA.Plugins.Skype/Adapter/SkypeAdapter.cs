using System;
using System.Diagnostics;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;

namespace Syn.VA.Plugins.Skype.Adapter
{
    class SkypeAdapter : IAdapter
    {
        private readonly SkypePlugin _plugin;
        public SkypeAdapter(SkypePlugin plugin)
        {
            _plugin = plugin;
        }

        public bool IsRecursive => true;

        public XName TagName => SimlSpecification.Namespace.O + "Skype";

        public string Evaluate(Context context)
        {
            try
            {
                string outValue;
                if (_plugin.GetUsers().TryGetValue(context.Element.Value, out outValue))
                {
                    var startString = $"skype:{outValue}?call";
                    Process.Start(startString);
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