using System;
using System.Windows;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Helper;
using Syn.Bot.Siml.Interfaces;
using Syn.Utility;

namespace Syn.VA.Plugins.Loader.Adapter
{
    class AgentAdapter : IAdapter
    {
        public bool IsRecursive => true;

        public XName TagName => SimlSpecification.Namespace.O + "Agent";

        public VirtualAssistant VA => VirtualAssistant.Instance;

        public string Evaluate(Context context)
        {
            try
            {
                var taskAttribute = context.Element.Attribute(Tag.TaskAttribute);
                var getAttribute = context.Element.Attribute(Tag.GetAttribute);
                var setAttribute = context.Element.Attribute(Tag.SetAttribute);

                if (taskAttribute != null)
                {
                    var taskValue = taskAttribute.Value.ToLower();
                    if (taskValue == "exit")
                    {
                        VirtualAssistant.Instance.SettingsManager["VA"].Add("Strict-Close-Override", () => true.ToString());
                        Application.Current.Shutdown();
                    }
                    else if (taskValue == "restart")
                    {
                       SynUtility.Wpf.Restart();
                    }
                }
                else if (getAttribute != null)
                {
                    var getValue = getAttribute.Value;
                    if (getValue.Contains(":"))
                    {
                        var splitValue = getValue.Split(':');
                        return VA.SettingsManager[splitValue[0]][splitValue[1]].Value;
                    }
                }
                else if (setAttribute != null)
                {
                    var setValue = setAttribute.Value;
                    if (setValue.Contains(":"))
                    {
                        var splitValue = setValue.Split(':');
                        VA.SettingsManager[splitValue[0]][splitValue[1]].Value = context.Element.Value;
                        return string.Empty;
                    }
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