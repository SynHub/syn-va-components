using System;
using System.Windows.Controls;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;
using Syn.VA.Controls;

namespace Syn.VA.Plugins.Browser.Adapter
{
    class WebPanelAdapter : IAdapter
    {
        public bool IsRecursive => true;

        public XName TagName => SimlSpecification.Namespace.O + "WebPanel";

        public string Evaluate(Context context)
        {
            try
            {
                var displayPanel = VirtualAssistant.Instance.Components.Get<DisplayPanel>();
                var webBrowser = new WebBrowser();
                var urlAttribute = context.Element.Attribute("Url");
                if (urlAttribute != null)
                {
                    webBrowser.Navigate(urlAttribute.Value);
                    var displayItem = new DisplayItem(webBrowser);
                    displayPanel.Add(displayItem);
                }
                else
                {
                    webBrowser.Navigate(context.Element.Value);
                    var displayItem = new DisplayItem(webBrowser);
                    displayPanel.Add(displayItem);
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