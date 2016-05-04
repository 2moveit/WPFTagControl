using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using WPFTagControl;

namespace TestUI
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private List<string> suggestedTags;
        private ObservableCollection<string> selectedTags;

        public MainWindowViewModel()
        {
            SuggestedTags = new List<string> {"Tag 1", "Tag 2", "Tag 3"};
            SelectedTags = new ObservableCollection<string>{ "TestTag"};
            SelectedTags.CollectionChanged += (sender, args) => 
            Debug.WriteLine("SelectedTagsCollectionChanged: " + SelectedTags.Aggregate("", (s,i)=> $"{s} {i}"));
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


        public ObservableCollection<string> SelectedTags
        {
            get { return selectedTags; }
            set
            {
                if (selectedTags == value) return;
                selectedTags = value;
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

        public void SelectedTagsChanged(IList<TagItem> items)
        {
            Debug.WriteLine("VM.TagItemsChanged: " + items.Aggregate("", (s, item) => $"{s} {item.Text}"));
            Debug.WriteLine("VM.SelectedTagsChanged: " + SelectedTags.Aggregate("", (s, item) => $"{s} {item}"));
        }

        public void SetTagsFromViewModel()
        {
            SelectedTags = new ObservableCollection<string>(new List<string> { "Test from VM"}); //Workaround: SelectedTags.Add("Tag") does not work
        }
    }
}