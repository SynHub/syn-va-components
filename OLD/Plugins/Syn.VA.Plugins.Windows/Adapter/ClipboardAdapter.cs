using System;
using System.Windows;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Helper;
using Syn.Bot.Siml.Interfaces;

namespace Syn.VA.Plugins.Windows.Adapter
{
    class ClipboardAdapter : IAdapter
    {
        public bool IsRecursive => true;

        public XName TagName => SimlSpecification.Namespace.O + "Clipboard";

        public string Evaluate(Context context)
        {
            try
            {
                var getAttribute = context.Element.Attribute(Tag.GetAttribute);
                var setAttribute = context.Element.Attribute(Tag.SetAttribute);
                if (getAttribute != null)
                {
                    var getValue = getAttribute.Value.ToLower();
                    if (getValue == "text")
                    {
                        return Clipboard.GetText();
                    }
                }
                else if (setAttribute != null)
                {
                    var setValue = setAttribute.Value.ToLower();
                    if (setValue == "text")
                    {
                        var setContent = context.Element.Value;
                        Clipboard.SetText(setContent);
                    }
                }
                else if (!string.IsNullOrEmpty(context.Element.Value))
                {
                    Clipboard.SetText(context.Element.Value);
                }
                return Clipboard.GetText();
            }
            catch (Exception exception)
            {
                VirtualAssistant.Instance.Logger.Error(exception);
            }

            return string.Empty;
        }
    }
}
