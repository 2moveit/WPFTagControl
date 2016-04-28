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
            DataContext = new MainWindowViewModel();
          
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