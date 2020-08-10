using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Helper;
using Syn.Bot.Siml.Interfaces;
using Syn.Utility;

namespace Syn.VA.Plugins.Launcher.Adapter
{
    public class AppAdapter : IAdapter
    {
        public bool IsRecursive => true;

        public XName TagName => SimlSpecification.Namespace.O + "App";

        public string Evaluate(Context context)
        {
            try
            {
                var taskAttribute = context.Element.Attribute(Tag.TaskAttribute);
                var getAttribute = context.Element.Attribute(Tag.GetAttribute);
                if (taskAttribute != null)
                {
                    var taskValue = taskAttribute.Value.ToLower();
                    if (taskValue == "open")
                    {
                        Process.Start(context.Element.Value);
                    }
                    else if (taskValue == "kill")
                    {
                        SynUtility.Process.Kill(context.Element.Value);
                    }
                    else if (taskValue == "close")
                    {
                        SynUtility.Process.Close(context.Element.Value);
                    }
                }
                else if (getAttribute != null)
                {
                    var getValue = getAttribute.Value.ToLower();
                    if (getValue == "name")
                    {
                        return Path.GetFileNameWithoutExtension(context.Element.Value);
                    }
                }
                else
                {
                    Process.Start(context.Element.Value);
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