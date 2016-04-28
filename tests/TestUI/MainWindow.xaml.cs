using System.Windows;
using WPFTagControl;

namespace TestUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ctl_Tags.SuggestedTags.Add("Tag Code 1");
            ctl_Tags.SuggestedTags.Add("Tag Code 2");
            ctl_Tags.SuggestedTags.Add("Tag Code 3");
        }


        private void Ctl_Tags_TagAdded(object sender, TagEventArgs e)
        {
            LastAdded.Content = e.Item.Text;
        }

        private void Ctl_Tags_TagClick(object sender, TagEventArgs e)
        {
            LastClicked.Content = e.Item.Text;
        }


        private void Ctl_Tags_OnTagsChanged(object sender, TagsChangedEventArgs e)
        {
            SelectionChanged.Content = e.Items.Count;
        }


        private void Ctl_Tags_OnTagRemoved(object sender, TagEventArgs e)
        {
            Removed.Content = e.Item.Text;
        }
    }
}