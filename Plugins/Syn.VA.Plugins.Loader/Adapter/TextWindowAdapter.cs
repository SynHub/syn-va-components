using System;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;
using Syn.Controls;
using Syn.Utility;

namespace Syn.VA.Plugins.Loader.Adapter
{
    public class TextWindowAdapter : IAdapter
    {
        public bool IsRecursive => true;
        public XName TagName => SimlSpecification.Namespace.O + "TextWindow";
        public string Evaluate(Context context)
        {
            try
            {
                var innerValue = context.Element.Value;
                var titleAttribute = context.Element.Attribute("Title");
                if (!string.IsNullOrEmpty(innerValue))
                {
                    var window = new TextWindow(SynUtility.File.GetAbsolutePath(innerValue));
                    if (titleAttribute != null)
                    {
                        window.Title = titleAttribute.Value;
                    }
                    window.ShowDialog();
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