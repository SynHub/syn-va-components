using System;
using System.IO;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;
using Syn.VA.Controls;

namespace Syn.VA.Plugins.HtmlPanel.Adapter
{
    public class HtmlPanelAdapter : IAdapter
    {
        public bool IsRecursive => true;

        public XName TagName => SimlSpecification.Namespace.O + "HtmlPanel";

        public string Evaluate(Context context)
        {
            try
            {
                var va = VirtualAssistant.Instance;
                var displayPanel = VirtualAssistant.Instance.Components.Get<DisplayPanel>();
                var htmlValue = context.Element.Value;
                var resourceDirectory = va.SettingsManager["VA"]["resources-directory"].Value;
                var cssFile = Path.Combine(resourceDirectory, "Html", "dark.css");
                var newString = htmlValue;
                if (!htmlValue.StartsWith("<html>", StringComparison.InvariantCultureIgnoreCase) && File.Exists(cssFile))
                {
                    newString = $"<html><style>{File.ReadAllText(cssFile)}</style><body>{htmlValue}</body></html>";
                }
                var htmlPanel = new TheArtOfDev.HtmlRenderer.WPF.HtmlPanel { Text = newString };

                var displayItem = new DisplayItem(htmlPanel);
                displayPanel.Add(displayItem);
            }
            catch (Exception exception)
            {
                VirtualAssistant.Instance.Logger.Error(exception);
            }

            return string.Empty;
        }
    }
}
