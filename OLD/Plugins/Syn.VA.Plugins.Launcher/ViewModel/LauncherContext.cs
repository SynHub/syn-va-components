using System;
using System.Collections.ObjectModel;
using Prism.Commands;
using Syn.Utility;
using Syn.VA.Plugins.Launcher.Model;

namespace Syn.VA.Plugins.Launcher.ViewModel
{
    public class LauncherContext : ViewModelBase
    {
        #region Fields
        private FileModel _selectedFile;
        private FolderModel _selectedFolder;
        private WebsiteModel _selectedSearchEngine;
        private WebsiteModel _selectedWebsite;
        #endregion

        #region Collection
        public ObservableCollection<FolderModel> FolderList { get; set; }
        public ObservableCollection<FileModel> FileList { get; set; }
        public ObservableCollection<WebsiteModel> SearchEngineList { get; set; }
        public ObservableCollection<WebsiteModel> WebsiteList { get; set; } 
        #endregion

        public LauncherContext()
        {
            FolderList = new ObservableCollection<FolderModel>();
            FileList = new ObservableCollection<FileModel>();
            SearchEngineList = new ObservableCollection<WebsiteModel>();
            WebsiteList = new ObservableCollection<WebsiteModel>();

            AddFileCommand = new DelegateCommand(() =>
            {
                AddFileAction?.Invoke();
            });

            var va = VirtualAssistant.Instance;

            RemoveFileCommand = new DelegateCommand(() =>
            {
                try
                {
                    va.SettingsManager[Constants.ApplicationList].Remove(SelectedFile.Name);
                    FileList.Remove(SelectedFile);
                }
                catch (Exception exception)
                {
                    VirtualAssistant.Instance.Logger.Error(exception);
                }
            });

            AddFolderCommand = new DelegateCommand(() =>
            {
                AddFolderAction?.Invoke();
            });

            RemoveFolderCommand = new DelegateCommand(() =>
            {
                try
                {
                    va.SettingsManager[Constants.DirectoryList].Remove(SelectedFolder.Name);
                    FolderList.Remove(SelectedFolder);
                }
                catch (Exception exception)
                {
                    VirtualAssistant.Instance.Logger.Error(exception);
                }
            });

            AddSearchEngineCommand = new DelegateCommand(() =>
            {
                AddSearchEngineAction?.Invoke();
            });

            RemoveSearchEngineCommand = new DelegateCommand(() =>
            {
                try
                {
                    va.SettingsManager[Constants.SearchEngineList].Remove(SelectedSearchEngine.Name);
                    SearchEngineList.Remove(SelectedSearchEngine);
                }
                catch (Exception exception)
                {
                    VirtualAssistant.Instance.Logger.Error(exception);
                }
            });

            AddWebsiteCommand = new DelegateCommand(() =>
            {
                AddWebsiteAction?.Invoke();
            });

            RemoveWebsiteCommand = new DelegateCommand(() =>
            {
                try
                {
                    va.SettingsManager[Constants.WebsiteList].Remove(SelectedWebsite.Name);
                    WebsiteList.Remove(SelectedWebsite);
                }
                catch (Exception exception)
                {
                    VirtualAssistant.Instance.Logger.Error(exception);
                }
            });
        }

        #region Selected Item

        public FileModel SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                _selectedFile = value;
                OnPropertyChanged("SelectedFile");
            }
        }

        public FolderModel SelectedFolder
        {
            get { return _selectedFolder; }
            set
            {
                _selectedFolder = value;
                OnPropertyChanged("SelectedFolder");
            }
        }

        public WebsiteModel SelectedSearchEngine
        {
            get { return _selectedSearchEngine; }
            set
            {
                _selectedSearchEngine = value;
                OnPropertyChanged("SelectedSearchEngine");
            }
        }

        public WebsiteModel SelectedWebsite
        {
            get { return _selectedWebsite; }
            set
            {
                _selectedWebsite = value;
                OnPropertyChanged("SelectedWebsite");
            }
        }

        #endregion

        #region Commands
        public Action AddFileAction;
        public DelegateCommand AddFileCommand { get; private set; }

        public DelegateCommand RemoveFileCommand { get; private set; }

        public Action AddFolderAction;
        public DelegateCommand AddFolderCommand { get; private set; }

        public DelegateCommand RemoveFolderCommand { get; private set; }

        public Action AddSearchEngineAction;
        public DelegateCommand AddSearchEngineCommand { get; private set; }

        public DelegateCommand RemoveSearchEngineCommand { get; private set; }

        public Action AddWebsiteAction;
        public DelegateCommand AddWebsiteCommand { get; private set; }

        public DelegateCommand RemoveWebsiteCommand { get; private set; }

        #endregion
    }
}