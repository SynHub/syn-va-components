using System.Collections.ObjectModel;
using Syn.Utility;

namespace Syn.VA.Plugins.TicTacToe.ViewModel
{
    class TicTacToeContext : ViewModelBase
    {
        private ObservableCollection<string> _playerTypes;
        private ObservableCollection<string> _difficultyTypes;

        public TicTacToeContext()
        {
            PlayerTypes = new ObservableCollection<string>();
            DifficultyTypes = new ObservableCollection<string>();
        }

        public ObservableCollection<string> PlayerTypes
        {
            get { return _playerTypes; }
            set
            {
                _playerTypes = value;
                OnPropertyChanged("PlayerTypes");
            }
        }

        public ObservableCollection<string> DifficultyTypes
        {
            get { return _difficultyTypes; }
            set
            {
                _difficultyTypes = value;
                OnPropertyChanged("DifficultyTypes");
            }
        }
    }
}
