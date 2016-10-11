using System;
using System.Diagnostics;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;

namespace Syn.VA.Plugins.Launcher.Adapter
{
    internal class FolderAdapter : IAdapter
    {
        public bool IsRecursive => true;

        public XName TagName => SimlSpecification.Namespace.O + "Folder";

        public string Evaluate(Context context)
        {
            try
            {
                Process.Start(context.Element.Value);
            }
            catch (Exception exception)
            {
                VirtualAssistant.Instance.Logger.Error(exception);
            }   
            return string.Empty;
        }
    }
}
