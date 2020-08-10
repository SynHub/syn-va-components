using System;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;

namespace Syn.VA.Plugins.Mock.Adapter
{
    public class MockAdapter : IAdapter
    {
        public bool IsRecursive => true;

        public XName TagName => SimlSpecification.Namespace.O + "Mock";

        public string Evaluate(Context context)
        {
            try
            {
                context.User.Settings["Mock-Data"].Value = context.Element.Value;
            }
            catch (Exception exception)
            {
                VirtualAssistant.Instance.Logger.Error(exception);
            }

            return string.Empty;
        }
    }
}