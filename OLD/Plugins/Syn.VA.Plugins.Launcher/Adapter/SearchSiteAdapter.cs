using System;
using System.Diagnostics;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;

namespace Syn.VA.Plugins.Launcher.Adapter
{
    public class SearchSiteAdapter : IAdapter
    {

        public bool IsRecursive => true;

        public XName TagName => SimlSpecification.Namespace.O + "SearchSite";

        public string Evaluate(Context context)
        {
            try
            {
                var linkAttribute = context.Element.Attribute("Link");
                if (linkAttribute != null)
                {
                    var linkString = string.Format(linkAttribute.Value, context.Element.Value);
                    Process.Start(linkString);
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