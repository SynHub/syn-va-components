using System;
using Prism.Commands;
using Syn.Utility;

namespace Syn.VA.Plugins.Windows.ViewModel
{
    class WindowsContext : ViewModelBase
    {
        #region Fields
        private string _screenshotDirectory;
        private string _wallpaperDirectory;
        #endregion

        public WindowsContext()
        {
            BrowseScreenshotCommand = new DelegateCommand(BrowseScreenshot);
            BrowseWallpaperCommand = new DelegateCommand(BrowseWallpaper);
        }

        public string ScreenshotDirectory
        {
            get { return _screenshotDirectory; }
            set
            {
                _screenshotDirectory = value;
                OnPropertyChanged("ScreenshotDirectory");
            }
        }

        public string WallpaperDirectory
        {
            get { return _wallpaperDirectory; }
            set
            {
                _wallpaperDirectory = value;
                OnPropertyChanged("WallpaperDirectory");
            }
        }

        public Action BrowseWallpaperAction;
        public Action BrowseScreenshotAction;

        public void BrowseWallpaper()
        {
            BrowseWallpaperAction?.Invoke();
        }

        public DelegateCommand BrowseWallpaperCommand { get; set; }

        public void BrowseScreenshot()
        {
            BrowseScreenshotAction?.Invoke();
        }

        public DelegateCommand BrowseScreenshotCommand { get; set; }
    }
}
