using System;
using System.IO;
using System.Windows.Forms;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Collections;
using Syn.Utility;
using Syn.Utility.Enumerations;
using Syn.Utility.File;
using Syn.VA.Controls;
using Syn.VA.Libraries.Language.Locale;
using Syn.VA.Plugins.Launcher.Adapter;
using Syn.VA.Plugins.Launcher.Model;
using Syn.VA.Plugins.Launcher.View;
using Syn.VA.Plugins.Launcher.ViewModel;
using Syn.VA.Utility.Extensions;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Syn.VA.Plugins.Launcher
{
    public class LauncherPlugin : Plugin
    {
        public LauncherPlugin()
        {
            try
            {
                var settingsManager = VA.SettingsManager;

                var applicationListSettings = settingsManager[Constants.ApplicationList];
                var directoryListSettings = settingsManager[Constants.DirectoryList];
                var searchEngineListSettings = settingsManager[Constants.SearchEngineList];
                var websiteListSettings = settingsManager[Constants.WebsiteList];

                if (applicationListSettings != null)
                {
                    foreach (var shortcutFile in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "*.lnk", SearchOption.AllDirectories))
                    {
                        var fullPath = ShortcutFile.ResolveShortcut(shortcutFile);
                        var fileName = Path.GetFileNameWithoutExtension(fullPath);
                        if (!string.IsNullOrEmpty(fullPath) && !string.IsNullOrEmpty(fileName))
                        {
                            applicationListSettings.Add(fileName, ()=>fullPath);
                        }
                    }
                }

                if (directoryListSettings != null)
                {
                    directoryListSettings.Add("document", ()=>SynUtility.Directory.GetPath(SpecialFolders.MyDocuments));
                    directoryListSettings.Add("music",()=> SynUtility.Directory.GetPath(SpecialFolders.MyMusic));
                    directoryListSettings.Add("pictures", ()=> SynUtility.Directory.GetPath(SpecialFolders.MyPictures));
                    directoryListSettings.Add("desktop", ()=> SynUtility.Directory.GetPath(SpecialFolders.Desktop));
                    directoryListSettings.Add("wallpapers", ()=> SynUtility.Directory.GetPath(SpecialFolders.Wallpaper));
                    directoryListSettings.Add("videos", ()=> SynUtility.Directory.GetPath(SpecialFolders.MyVideos));
                }

                var bot = VA.Components.Get<SimlBot>();
                bot.Adapters.Add(new AppAdapter());
                bot.Adapters.Add(new FolderAdapter());
                bot.Adapters.Add(new SearchSiteAdapter());
                bot.Adapters.Add(new WebsiteAdapter());

                if (applicationListSettings != null)
                {
                    applicationListSettings.SettingsChanged += delegate
                    {
                        UpdateApplicationView.Invoke();
                        CreateMap(applicationListSettings);
                        VA.SettingsManager.Save(applicationListSettings);
                    };
                }

                if (directoryListSettings != null)
                {
                    directoryListSettings.SettingsChanged += delegate
                    {
                        UpdateDirectoryView.Invoke();
                        CreateMap(directoryListSettings);
                        VA.SettingsManager.Save(directoryListSettings);
                    };
                }

                if (searchEngineListSettings != null)
                {
                    searchEngineListSettings.SettingsChanged += delegate
                    {
                        UpdateSearchEngineView.Invoke();
                        CreateMap(searchEngineListSettings);
                        VA.SettingsManager.Save(searchEngineListSettings);
                    };
                }

                if (websiteListSettings != null)
                {
                    websiteListSettings.SettingsChanged += delegate
                    {
                        UpdateWebsiteView.Invoke();
                        CreateMap(websiteListSettings);
                        VA.SettingsManager.Save(websiteListSettings);
                    };
                }

                RefreshMaps();

            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        public override string DisplayName => StringResource.LauncherPlugin_DisplayName;

        public void RefreshMaps()
        {
            var settingsManager = VA.SettingsManager;
            var applicationListSettings = settingsManager[Constants.ApplicationList];
            var directoryListSettings = settingsManager[Constants.DirectoryList];
            var searchEngineListSettings = settingsManager[Constants.SearchEngineList];
            var websiteListSettings = settingsManager[Constants.WebsiteList];

            CreateMap(applicationListSettings);
            CreateMap(directoryListSettings);
            CreateMap(searchEngineListSettings);
            CreateMap(websiteListSettings);
        }

        public void CreateMap(Settings settings)
        {
            var bot = VA.Components.Get<SimlBot>();
            if (bot.Maps.Contains(settings.Name))
            {
                var foundMap = bot.Maps[settings.Name];
                bot.Maps.Remove(foundMap);
            }

            var map = new Map(settings.Name);
            foreach (var item in settings)
            {
                var mapItem = new MapItem(item.Name, item.Value);
                map.Add(mapItem);
            }
            bot.Maps.Add(map);
        }

        public override string Name => "Launcher";

        public Action UpdateApplicationView
        {
            get
            {
                Action toReturn = () =>
                {
                    var settings = VA.SettingsManager[Constants.ApplicationList];
                    if (settings != null)
                    {
                        LauncherContext.FileList.Clear();
                        foreach (var item in settings)
                        {
                            LauncherContext.FileList.Add(new FileModel(item.Name, item.Value));
                        }
                    }
                };
                return toReturn;
            }
        }

        public Action UpdateDirectoryView
        {
            get
            {
                Action toReturn = () =>
                {
                    var settings = VA.SettingsManager[Constants.DirectoryList];
                    LauncherContext.FolderList.Clear();
                    foreach (var item in settings)
                    {
                        LauncherContext.FolderList.Add(new FolderModel(item.Name, item.Value));
                    }

                };
                return toReturn;
            }
        }

        public Action UpdateSearchEngineView
        {
            get
            {
                Action toReturn = () =>
                {
                    var settings = VA.SettingsManager[Constants.SearchEngineList];
                    LauncherContext.SearchEngineList.Clear();
                    foreach (var item in settings)
                    {
                        LauncherContext.SearchEngineList.Add(new WebsiteModel(item.Name, item.Value));
                    }
                };
                return toReturn;
            }
        }

        public Action UpdateWebsiteView
        {
            get
            {
                Action toReturn = () =>
                {
                    var settings = VA.SettingsManager[Constants.WebsiteList];
                    LauncherContext.WebsiteList.Clear();
                    foreach (var item in settings)
                    {
                        LauncherContext.WebsiteList.Add(new WebsiteModel(item.Name, item.Value));
                    }
                };
                return toReturn;
            }
        }

        LauncherContext LauncherContext { get; set; }

        public override T GetPanel<T>(params object[] parameters)
        {
            try
            {
                var launcherPanel = new LauncherPanel();
                LauncherContext = launcherPanel.DataContext as LauncherContext;

                var settingsManager = VA.SettingsManager;
                var applicationListSettings = settingsManager[Constants.ApplicationList];
                var directoryListSettings = settingsManager[Constants.DirectoryList];
                var searchEngineListSettings = settingsManager[Constants.SearchEngineList];
                var websiteListSettings = settingsManager[Constants.WebsiteList];

                if (LauncherContext != null)
                {
                    LauncherContext.AddFileAction = async () =>
                    {
                        try
                        {
                            var openFileDialog = new OpenFileDialog();
                            if (openFileDialog.ShowDialog() == true)
                            {
                                var window = VA.Components.Get<SettingsWindow>();
                                var selectedPath = openFileDialog.FileName;
                                var selectedName = await window.GetInput(StringResource.LauncherPlugin_AddFileTitle, StringResource.LauncherPlugin_AddFileMessage, Path.GetFileNameWithoutExtension(selectedPath));
                                if (selectedName != null)
                                {
                                    applicationListSettings.Add(new Variable(selectedName, selectedPath));
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            VA.Logger.Error(exception);
                        }
                    };

                    LauncherContext.AddFolderAction = async () =>
                    {
                        try
                        {
                            var folderBrowserDialog = new FolderBrowserDialog();
                            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                            {
                                var window = VA.Components.Get<SettingsWindow>();
                                var selectedPath = folderBrowserDialog.SelectedPath;
                                var selectedName = await window.GetInput(StringResource.LauncherPlugin_AddFolderTitle, StringResource.LauncherPlugin_AddFolderMessage,
                                    SynUtility.Directory.GetName(selectedPath));
                                if (selectedName != null)
                                {
                                    directoryListSettings.Add(new Variable(selectedName, selectedPath));
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            VA.Logger.Error(exception);
                        }
                    };

                    LauncherContext.AddSearchEngineAction = async () =>
                    {
                        try
                        {
                            var window = VA.Components.Get<SettingsWindow>();
                            var searchEngineUrl = await window.GetInput(StringResource.LauncherPlugin_AddSearchEngineAddressTitle, StringResource.LauncherPlugin_AddSearchEngineAddressMessage);
                            if (searchEngineUrl != null)
                            {
                                var searchEngineName = await window.GetInput(StringResource.LauncherPlugin_AddSearchEngineNameTitle, StringResource.LauncherPlugin_AddSearchEngineNameMessage);
                                if (searchEngineName != null)
                                {
                                    searchEngineListSettings.Add(new Variable(searchEngineName, searchEngineUrl));
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            VA.Logger.Error(exception);
                        }
                    };

                    LauncherContext.AddWebsiteAction = async () =>
                    {
                        try
                        {
                            var window = VA.Components.Get<SettingsWindow>();
                            var websiteUrl = await window.GetInput(StringResource.LauncherPlugin_AddWebsiteAddressTitle, StringResource.LauncherPlugin_AddWebsiteAddressMessage);
                            if (websiteUrl != null)
                            {
                                var websiteName = await window.GetInput(StringResource.LauncherPlugin_AddWebsiteNameTitle, StringResource.LauncherPlugin_AddWebsiteNameMessage);
                                if (websiteName != null)
                                {
                                    websiteListSettings.Add(new Variable(websiteName, websiteUrl));
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            VA.Logger.Error(exception);
                        }
                    };

                    UpdateApplicationView.Invoke();
                    UpdateDirectoryView.Invoke();
                    UpdateSearchEngineView.Invoke();
                    UpdateWebsiteView.Invoke();
                    Settings.ApplyToProperties(LauncherContext);
                }
                return launcherPanel as T;
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }

            return null;
        }
    }
}