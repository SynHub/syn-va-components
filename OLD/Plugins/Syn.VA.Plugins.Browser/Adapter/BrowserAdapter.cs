using System;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;
using Syn.VA.Controls;

namespace Syn.VA.Plugins.Browser.Adapter
{
    public class BrowserAdapter : IAdapter
    {
        public bool IsRecursive => true;

        public XName TagName => SimlSpecification.Namespace.O + "Browser";

        public string Evaluate(Context context)
        {
            try
            {
                var browserWindow = new BrowserWindow();
                var type = context.Element.Attribute("Target");
                if (type != null)
                {
                    var typeValue = type.Value;
                    if (typeValue.Equals("html", StringComparison.OrdinalIgnoreCase))
                    {
                        browserWindow.Browser.NavigateToString(context.Element.Value);
                        browserWindow.ShowDialog();
                    }
                    else if (typeValue.Equals("link", StringComparison.OrdinalIgnoreCase))
                    {
                        browserWindow.Browser.Navigate(context.Element.Value);
                        browserWindow.ShowDialog();
                    }
                }
                else
                {
                    browserWindow.Browser.Navigate(context.Element.Value);
                    browserWindow.ShowDialog();
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
