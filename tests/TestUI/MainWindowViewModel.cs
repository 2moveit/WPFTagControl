using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestUI
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private List<string> suggestedTags;

        public MainWindowViewModel()
        {
            SuggestedTags = new List<string> {"Tag 1", "Tag 2", "Tag 3"};
        }

        public List<string> SuggestedTags
        {
            get { return suggestedTags; }
            set
            {
                if (suggestedTags == value) return;
                suggestedTags = value;
                OnPropertyChanged();
            }
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}