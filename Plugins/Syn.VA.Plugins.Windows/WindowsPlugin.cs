using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Syn.Bot.Siml;
using Syn.Utility;
using Syn.Utility.Enumerations;
using Syn.VA.Libraries.Language.Locale;
using Syn.VA.Plugins.Windows.Adapter;
using Syn.VA.Plugins.Windows.View;
using Syn.VA.Plugins.Windows.ViewModel;
using Syn.VA.Utility.Extensions;

namespace Syn.VA.Plugins.Windows
{
    public class WindowsPlugin : Plugin
    {
        private readonly List<string> _knownLowDrives;

        public WindowsPlugin()
        {
            try
            {
                var bot = VA.Components.Get<SimlBot>();
                bot.Settings.Add("Internet-Available", ()=> SynUtility.Network.IsInternetAvailable().ToString()); //FunctionalVariable
                bot.Adapters.Add(new WindowAdapter());
                bot.Adapters.Add(new MachineAdapter());
                bot.Adapters.Add(new ClipboardAdapter());

                //If ScreenshotDirectory variable is empty or the directory doesn't exist then create a directory called screenshot within the resources directory and use it.
                if (string.IsNullOrEmpty(Settings["Screenshot-Directory"].Value) || Directory.Exists(Settings["Screenshot-Directory"].Value) == false)
                {
                    var generalSettings = VA.SettingsManager["VA"];
                    var screenshotDirectory = Path.Combine(generalSettings["resources-directory"].Value, "Screenshots");
                    if (Directory.Exists(screenshotDirectory) == false) Directory.CreateDirectory(screenshotDirectory);
                    Settings["Screenshot-Directory"].Value = screenshotDirectory;
                }

                //if WallpaperDirectory variable is empty or the directory doesn't exist then revert to default Windows wallpaper directory
                if (string.IsNullOrEmpty(Settings["Wallpaper-Directory"].Value) || Directory.Exists(Settings["Wallpaper-Directory"].Value) == false)
                {
                    Settings["Wallpaper-Directory"].Value = SynUtility.Directory.GetPath(SpecialFolders.Wallpaper);
                }


                _knownLowDrives = new List<string>();
                VA.Services.Add(GetTimeReminderService());
                VA.Services.Add(GetSystemUptimeService());
                VA.Services.Add(GetDriveSpaceService());
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        private SimlBot Bot => VirtualAssistant.Instance.Components.Get<SimlBot>();

        private Service GetTimeReminderService()
        {

            //Time Reminder Service
            var timeReminderService = new VA.Service("Time-Reminder-Service", () =>
            {
                if (DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
                {
                    Bot.Trigger("Time-Reminder-Service-Event");
                }
            });
            timeReminderService.Interval = TimeSpan.FromSeconds(1);
            timeReminderService.Description = StringResource.ServicePlugin_TimeReminderServiceDescription;
            return timeReminderService;

        }

        private Service GetSystemUptimeService()
        {
            //System Uptime Service
            var systemUptimeService = new VA.Service("System-Uptime-Service", () =>
            {
                var systemUptimeVariable = new PerformanceCounter("System", "System Up Time");
                systemUptimeVariable.NextValue();
                var nextValue = TimeSpan.FromSeconds(systemUptimeVariable.NextValue());
                if (nextValue.Minutes == 0 && nextValue.Seconds == 0)
                {
                    //Only raise when the uptime is 1 or more
                    if (nextValue.Hours > 0)
                    {
                        Bot.MainUser.Vars["system-uptime"].Value = nextValue.Hours.ToString(CultureInfo.InvariantCulture);
                        Bot.Trigger("System-Uptime-Service-Event");
                    }
                }
            });

            systemUptimeService.Interval = TimeSpan.FromSeconds(1);
            systemUptimeService.Description = StringResource.ServicePlugin_SystemUptimeReminderServiceDescription;
            return systemUptimeService;
        }

        private Service GetDriveSpaceService()
        {
            //Low drive space service.
            var driveSpaceServiceId = Guid.NewGuid();
            var driveSpaceReminderService = new VA.Service("Drive-Space-Reminder-Service", () =>
            {
                //Recheck drive space every 1 Minute
                if (SynUtility.Timing.Delay(driveSpaceServiceId, 60))
                {
                    Bot.MainUser.Vars["Drive-Name"].Clear();
                    foreach (var drive in DriveInfo.GetDrives())
                    {
                        var percentFree = 100 * (double)drive.TotalFreeSpace / drive.TotalSize;
                        var driveName = drive.Name.Substring(0, 1);
                        if (percentFree < 10 && !_knownLowDrives.Contains(driveName))
                        {
                            Bot.MainUser.Vars["Drive-Name"].Add(driveName);
                            _knownLowDrives.Add(driveName);
                        }
                        else if (percentFree > 10 && _knownLowDrives.Contains(driveName))
                        {
                            _knownLowDrives.Remove(driveName);
                        }
                    }
                    if (Bot.MainUser.Vars["Drive-Name"].Count > 0)
                    {
                        Bot.Trigger("Drive-Space-Reminder-Event");
                    }
                }
            });
            driveSpaceReminderService.Interval = TimeSpan.FromSeconds(1);
            driveSpaceReminderService.Description = StringResource.ServicePlugin_DriveSpaceReminderServiceDescription;
            return driveSpaceReminderService;
        }

        public override string Name => "Windows";

        public override string DisplayName => StringResource.WindowsPlugin_DisplayName;

        public override T GetPanel<T>(params object[] parameters)
        {
            try
            {
                var toReturn = new WindowsPanel();
                var context = toReturn.DataContext as WindowsContext;
                if (context != null)
                {
                    Settings.ApplyToProperties(context, "Screenshot-Directory", "Wallpaper-Directory");

                    context.BrowseScreenshotAction = () =>
                    {
                        try
                        {
                            var dialog = new FolderBrowserDialog();
                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                Settings["Screenshot-Directory"].Value = context.ScreenshotDirectory = dialog.SelectedPath;
                                VA.SettingsManager.Save(Settings);
                            }
                        }
                        catch (Exception exception)
                        {
                            VA.Logger.Error(exception);
                        }
                    };

                    context.BrowseWallpaperAction = () =>
                    {
                        try
                        {
                            var dialog = new FolderBrowserDialog();
                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                Settings["Wallpaper-Directory"].Value = context.WallpaperDirectory = dialog.SelectedPath;
                                VA.SettingsManager.Save(Settings);
                            }
                        }
                        catch (Exception exception)
                        {
                            VA.Logger.Error(exception);
                        }
                    };
                }
                return toReturn as T;
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }

            return null;
        }
    }
}