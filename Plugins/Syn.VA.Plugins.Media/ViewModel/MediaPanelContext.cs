using System;
using Prism.Commands;
using Syn.Utility;

namespace Syn.VA.Plugins.Media.ViewModel
{
    public class MediaPanelContext : ViewModelBase
    {
        private string _musicDirectory;
        private string _videoDirectory;
        private string _musicLabel;
        private string _videoLabel;

        public MediaPanelContext()
        {
            MusicBrowseCommand = new DelegateCommand(() =>
            {
                MusicBrowseAction?.Invoke();
            });

            VideoBrowseCommand = new DelegateCommand(() =>
            {
                VideoBrowseAction?.Invoke();
            });
        }

        public string MusicLabel
        {
            get { return _musicLabel; }
            set { _musicLabel = value; OnPropertyChanged("MusicLabel"); }
        }

        public string VideoLabel
        {
            get { return _videoLabel; }
            set { _videoLabel = value; OnPropertyChanged("VideoLabel"); }
        }

        public string MusicDirectory
        {
            get { return _musicDirectory; }
            set
            {
                _musicDirectory = value; OnPropertyChanged("MusicDirectory");
            }
        }

        public string VideoDirectory
        {
            get { return _videoDirectory; }
            set { _videoDirectory = value; OnPropertyChanged("VideoDirectory"); }
        }

        public Action MusicBrowseAction;
        public DelegateCommand MusicBrowseCommand { get; set; }

        public Action VideoBrowseAction;
        public DelegateCommand VideoBrowseCommand { get; set; }
    }
}