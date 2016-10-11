using System;
using System.IO;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Helper;
using Syn.Bot.Siml.Interfaces;
using Syn.Utility;

namespace Syn.VA.Plugins.Windows.Adapter
{
    class MachineAdapter : IAdapter
    {
        public bool IsRecursive => true;

        public XName TagName => SimlSpecification.Namespace.O + "Machine";

        public string Evaluate(Context context)
        {
            try
            {
                var plugin = VirtualAssistant.Instance.Plugins.GetPluginByType<WindowsPlugin>();
                var taskAttribute = context.Element.Attribute(Tag.TaskAttribute);
                var getAttribute = context.Element.Attribute(Tag.GetAttribute);
                if (taskAttribute != null)
                {
                    var taskValue = taskAttribute.Value.ToLower();
                    if (taskValue == "lock")
                    {
                        SynUtility.Computer.Lock();
                    }
                    else if (taskValue == "restart" || taskValue == "reboot")
                    {
                        SynUtility.Computer.Restart();
                    }
                    else if (taskValue == "shutdown")
                    {
                        SynUtility.Computer.Shutdown();
                    }
                    else if (taskValue == "logoff")
                    {
                        SynUtility.Computer.LogOff();
                    }
                    else if (taskValue == "clear-trash")
                    {
                        SynUtility.Win32.EmptyRecycleBin();
                    }
                    else if (taskValue == "screenshot")
                    {
                        var screenshot = SynUtility.Screen.GetScreenshot();
                        var screenshotDirectory = plugin.Settings["Screenshot-Directory"].Value;
                        var fileName = SynUtility.File.FileNameFromTime("screenshot", ".bmp");
                        var screenshotPath = Path.Combine(screenshotDirectory, fileName);
                        if (!File.Exists(screenshotPath))
                        {
                            screenshot.Save(screenshotPath);
                        }
                    }
                    else if (taskValue == "change-wallpaper")
                    {
                        var wallpaperDirectory = plugin.Settings["Wallpaper-Directory"].Value;
                        var fileName = SynUtility.Directory.RandomFile(wallpaperDirectory, searchPattern:"*.jpg", searchOption: SearchOption.AllDirectories);
                        SynUtility.Win32.SetWallpaper(fileName);
                    }
                }
                else if (getAttribute != null)
                {
                    var getValue = getAttribute.Value.ToLower();
                    if (getValue == "ip")
                    {
                        return SynUtility.Network.GetExternalIp();
                    }
                    if (getValue == "user-name")
                    {
                        return Environment.UserName;
                    }
                    if (getValue == "os-version")
                    {
                        return Environment.OSVersion.Version.Major.ToString(context.Bot.Culture);
                    }
                    if (getValue == "processor-count")
                    {
                        return Environment.ProcessorCount.ToString(context.Bot.Culture);
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
