using System;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;

namespace Syn.VA.Plugins.Loader.Adapter
{
    class CreditAdapter : IAdapter
    {
        public bool IsRecursive => false;
        public XName TagName => SimlSpecification.Namespace.O + "Credit";

        public string Evaluate(Context context)
        {
            var va = VirtualAssistant.Instance;
            try
            {
                //Syn Virtual Assistant automatically removes the 'Response' settings after a response is processed.
                if (!va.SettingsManager.Contains("Response"))
                {
                    va.SettingsManager.Create("Response");
                }

                var titleAttribute = context.Element.Attribute("Title");
                var urlAttribute = context.Element.Attribute("Url");
                if (titleAttribute != null && urlAttribute != null)
                {
                    var title = titleAttribute.Value;
                    var url = urlAttribute.Value;

                    va.SettingsManager["Response"]["Credit-Title"].Value = title;
                    va.SettingsManager["Response"]["Credit-Url"].Value = url;
                }
            }
            catch (Exception exception)
            {
                va.Logger.Error(exception);
            }

            return string.Empty;
        }
    }
}