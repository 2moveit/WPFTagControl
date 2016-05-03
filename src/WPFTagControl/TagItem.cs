using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WPFTagControl
{
    /// <remarks>Based on work of adabyron http://stackoverflow.com/questions/15167809/how-can-i-create-a-tagging-control-similar-to-evernote-in-wpf</remarks>
    [DebuggerDisplay("{this.Text}")]
    [TemplatePart(Name = "PART_InputBox", Type = typeof (AutoCompleteBox))]
    [TemplatePart(Name = "PART_DeleteTagButton", Type = typeof (Button))]
    [TemplatePart(Name = "PART_TagButton", Type = typeof (Button))]
    public class TagItem : Control
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof (string),
            typeof (TagItem), new PropertyMetadata(null));

        private static readonly DependencyPropertyKey IsEditingPropertyKey =
            DependencyProperty.RegisterReadOnly("IsEditing", typeof (bool), typeof (TagItem),
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsEditingProperty = IsEditingPropertyKey.DependencyProperty;

        static TagItem()
        {
            // lookless control, get default style from generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof (TagItem), new FrameworkPropertyMetadata(typeof (TagItem)));
        }

        public TagItem()
        {
        }

        public TagItem(string text)
            : this()
        {
            Text = text;
        }

        // Text
        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // IsEditing, readonly
        public bool IsEditing
        {
            get { return (bool) GetValue(IsEditingProperty); }
            internal set { SetValue(IsEditingPropertyKey, value); }
        }


        public override void OnApplyTemplate()
        {
            var inputBox = GetTemplateChild("PART_InputBox") as AutoCompleteBox;
            if (inputBox != null)
            {
                inputBox.LostFocus += inputBox_LostFocus;
                inputBox.GotFocus += inputBox_GotFocus;
                inputBox.Loaded += inputBox_Loaded;
            }

            var btn = GetTemplateChild("PART_TagButton") as Button;
            if (btn != null)
            {
                btn.Loaded += (s, e) =>
                {
                    var b = s as Button;
                    var btnDelete = b.Template.FindName("PART_DeleteTagButton", b) as Button;
                        // will only be found once button is loaded
                    if (btnDelete != null)
                    {
                        btnDelete.Click -= btnDelete_Click; // make sure the handler is applied just once
                        btnDelete.Click += btnDelete_Click;
                    }
                };

                btn.Click += (s, e) =>
                {
                    var parent = GetParent();
                    parent?.RaiseTagClick(this);
                };
                btn.MouseDoubleClick += (s, e) =>
                {
                  //  valueBeforeEditing = this.Text;
                    var parent = GetParent();
                    parent?.RaiseTagDoubleClick(this);
                };
            }

            base.OnApplyTemplate();
        }


        private string valueBeforeEditing = "";


        void inputBox_GotFocus(object sender, RoutedEventArgs e)
        {
            valueBeforeEditing = this.Text;
        }

        void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = FindUpVisualTree<TagItem>(sender as FrameworkElement);
            var parent = GetParent();
            if (item != null)
                parent?.RemoveTag(item);
            e.Handled = true; // bubbling would raise the tag click event
        }

        static bool isDuplicate(TagControl tagControl, string compareTo)
        {
            var duplicateCount = (from TagItem item in (IList) tagControl.ItemsSource
                where item.Text.ToLower() == compareTo.ToLower()
                select item).Count();
            if (duplicateCount > 1)
                return true;

            return false;
        }

        /// <summary>
        ///     When an AutoCompleteBox is created, set the focus to the textbox.
        ///     Wire PreviewKeyDown event to handle Escape/Enter keys
        /// </summary>
        /// <remarks>AutoCompleteBox.Focus() is broken: http://stackoverflow.com/questions/3572299/autocompletebox-focus-in-wpf</remarks>
        void inputBox_Loaded(object sender, RoutedEventArgs e)
        {
            var acb = sender as AutoCompleteBox;
            if (acb != null)
            {
                var tb = acb.Template.FindName("Text", acb) as TextBox;
                tb?.Focus();

                // PreviewKeyDown, because KeyDown does not bubble up for Enter
                acb.PreviewKeyDown += (s, e1) =>
                {
                    var parent = GetParent();
                    if (parent != null)
                    {
                        switch (e1.Key)
                        {
                            case Key.Tab:
                            case Key.Enter: // accept tag
                                if (!string.IsNullOrWhiteSpace(Text))
                                {
                                    if (isDuplicate(parent, Text))
                                        break;
                                    parent.ApplyTemplate(this);
                                    parent.SelectedItem = parent.InitializeNewTag(); //creates another tag
                                }
                                else
                                    parent.Focus();
                                break;
                            case Key.Escape: // reject tag
                                isEscapeClicked = true;
                                parent.Focus();
                                //parent.RemoveTag(this, true); // do not raise RemoveTag event
                                break;
                            case Key.Back:
                                if (string.IsNullOrWhiteSpace(Text))
                                {
                                    inputBox_LostFocus(this, new RoutedEventArgs());
                                    var previousTagIndex = ((IList) parent.ItemsSource).Count - 1;
                                    if (previousTagIndex < 0) break;

                                    var previousTag = ((IList) parent.ItemsSource)[previousTagIndex] as TagItem;
                                    previousTag.Focus();
                                    previousTag.IsEditing = true;
                                }
                                break;
                        }
                    }
                };
            }
        }

        private bool isEscapeClicked = false;

        /// <summary>
        ///     Set IsEditing to false when the AutoCompleteBox loses keyboard focus.
        ///     This will change the template, displaying the tag as a button.
        /// </summary>
        void inputBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var parent = GetParent();
            if (!string.IsNullOrWhiteSpace(Text))
            {
                if (parent != null)
                {
                    if (isDuplicate(parent, Text) && valueBeforeEditing == "")
                        parent.RemoveTag(this, true); // do not raise RemoveTag event
                    else if (isDuplicate(parent, Text) && valueBeforeEditing != "")
                        Text = valueBeforeEditing;
                    else if(valueBeforeEditing != Text)
                        parent.RaiseTagEdited(this);
                }
                if (!(sender as AutoCompleteBox).IsDropDownOpen)
                {
                    IsEditing = false;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(valueBeforeEditing))
                    this.Text = valueBeforeEditing;
                if(!isEscapeClicked && !string.IsNullOrEmpty(this.Text))
                    parent?.RemoveTag(this);
                else if(string.IsNullOrEmpty(this.Text))
                    parent?.RemoveTag(this, true);
                isEscapeClicked = false;
            }

            if (parent != null)
            {
                parent.IsEditing = false;
            }
        }

        private TagControl GetParent()
        {
            return FindUpVisualTree<TagControl>(this);
        }

        /// <summary>
        ///     Walks up the visual tree to find object of type T, starting from initial object
        ///     http://www.codeproject.com/Tips/75816/Walk-up-the-Visual-Tree
        /// </summary>
        private static T FindUpVisualTree<T>(DependencyObject initial) where T : DependencyObject
        {
            var current = initial;
            while (current != null && current.GetType() != typeof (T))
            {
                current = VisualTreeHelper.GetParent(current);
            }
            return current as T;
        }
    }
}