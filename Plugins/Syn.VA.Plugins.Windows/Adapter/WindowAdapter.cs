using System;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Helper;
using Syn.Bot.Siml.Interfaces;
using Syn.Utility;

namespace Syn.VA.Plugins.Windows.Adapter
{
    class WindowAdapter : IAdapter
    {
        public bool IsRecursive => true;

        public XName TagName => SimlSpecification.Namespace.O + "Window";

        public string Evaluate(Context context)
        {
            try
            {
                var taskAttribute = context.Element.Attribute(Tag.TaskAttribute);
                if (taskAttribute != null)
                {
                    var taskValue = taskAttribute.Value.ToLower();
                    if (taskValue == "minimize")
                    {
                        SynUtility.Win32.MinimizeWindow(SynUtility.Win32.GetForegroundWindow());
                    }
                    else if (taskValue == "restore")
                    {
                        SynUtility.Win32.RestoreWindow(SynUtility.Win32.GetForegroundWindow());
                    }
                    else if (taskValue == "maximize")
                    {
                        SynUtility.Win32.MaximizeWindow(SynUtility.Win32.GetForegroundWindow());
                    }
                    else if (taskValue == "close")
                    {
                        var window = SynUtility.Win32.GetForegroundWindow();
                        SynUtility.Win32.CloseWindow(window);
                    }
                    else if (taskValue == "tile")
                    {
                        var shellAppType = Type.GetTypeFromProgID("Shell.Application");
                        dynamic shell = Activator.CreateInstance(shellAppType);
                        shell.TileHorizontally();
                    }
                    else if (taskValue == "tile-horizontal")
                    {
                        var shellAppType = Type.GetTypeFromProgID("Shell.Application");
                        dynamic shell = Activator.CreateInstance(shellAppType);
                        shell.TileHorizontally();
                    }
                    else if (taskValue == "tile-vertical")
                    {
                        var shellAppType = Type.GetTypeFromProgID("Shell.Application");
                        dynamic shell = Activator.CreateInstance(shellAppType);
                        shell.TileVertically();
                    }
                    else if (taskValue == "cascade")
                    {
                        var shellAppType = Type.GetTypeFromProgID("Shell.Application");
                        dynamic shell = Activator.CreateInstance(shellAppType);
                        shell.CascadeWindows();
                    }
                    else if (taskValue == "toggle")
                    {
                        var shellAppType = Type.GetTypeFromProgID("Shell.Application");
                        dynamic shell = Activator.CreateInstance(shellAppType);
                        shell.CascadeWindows();
                    }
                    else if (taskValue == "switch")
                    {
                        var shellAppType = Type.GetTypeFromProgID("Shell.Application");
                        dynamic shell = Activator.CreateInstance(shellAppType);
                        shell.WindowSwitcher();
                    }
                    else if (taskValue == "minimize-all")
                    {
                        var shellAppType = Type.GetTypeFromProgID("Shell.Application");
                        dynamic shell = Activator.CreateInstance(shellAppType); 
                        shell.MinimizeAll();
                    }
                    else if (taskValue == "undo-minimize-all")
                    {
                        var shellAppType = Type.GetTypeFromProgID("Shell.Application");
                        dynamic shell = Activator.CreateInstance(shellAppType);
                        shell.UndoMinimizeALL();
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
