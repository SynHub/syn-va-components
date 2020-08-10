using System;
using System.Diagnostics;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;

namespace Syn.VA.Plugins.Launcher.Adapter
{
    internal class WebsiteAdapter : IAdapter
    {

        public bool IsRecursive => true;

        public XName TagName => SimlSpecification.Namespace.O + "Website";

        public string Evaluate(Context context)
        {
            try
            {
                var websiteName = context.Element.Value;
                if (!string.IsNullOrEmpty(websiteName))
                {
                    //Add "http" if full address is not provided.
                    if (!websiteName.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        websiteName = $"http://{websiteName}";
                    }
                    Process.Start(websiteName);
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