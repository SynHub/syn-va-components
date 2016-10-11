using System;
using System.IO;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;
using Syn.Controls;

namespace Syn.VA.Plugins.Loader.Adapter
{
    public class HelpWindowAdapter : IAdapter
    {
        public bool IsRecursive => true;
        public XName TagName => SimlSpecification.Namespace.O + "HelpWindow";
        public string Evaluate(Context context)
        {
            try
            {
                var va = VirtualAssistant.Instance;
                var vaSettings = va.SettingsManager["VA"];
                var titleAttribute = context.Element.Attribute("Title");
                var helpDirectory = vaSettings["Help-Directory"].Value;
                var helpWindow = new HelpWindow(helpDirectory);
                var markdownCssFile = Path.Combine(helpDirectory, "markdown.css");
                if (File.Exists(markdownCssFile))
                {
                    helpWindow.CssString = File.ReadAllText(markdownCssFile);
                }
                if (titleAttribute != null)
                {
                    helpWindow.Title = titleAttribute.Value;
                }
                helpWindow.ArrangeSequence.Add("Introduction");
                helpWindow.ShowDialog();
            }
            catch (Exception exception)
            {
                VirtualAssistant.Instance.Logger.Error(exception);
            }
            return string.Empty;
        }
    }
}