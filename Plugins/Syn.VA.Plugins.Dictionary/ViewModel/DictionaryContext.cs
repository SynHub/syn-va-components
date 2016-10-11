using System;
using Syn.Utility;

namespace Syn.VA.Plugins.Dictionary.ViewModel
{
    public class DictionaryContext : ViewModelBase
    {
        private bool _useCommon;

        public DictionaryContext()
        {
            _useCommon = true;
        }


        public bool UseCommon
        {
            get { return _useCommon; }
            set
            {
                _useCommon = value;
                OnPropertyChanged("UseCommon");
                ToggleValueAction?.Invoke();
            }
        }

        public Action ToggleValueAction { get; set; }
    }
}
