using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace WPFTagControl
{
    /// <remarks>
    ///     Based on work of adabyron
    ///     http://stackoverflow.com/questions/15167809/how-can-i-create-a-tagging-control-similar-to-evernote-in-wpf
    /// </remarks>
    [TemplatePart(Name = "PART_CreateTagButton", Type = typeof (Button))]
    public class TagControl : ListBox
    {
        public static readonly DependencyProperty SelectedTagsProperty = DependencyProperty.Register("SelectedTags",
            typeof (IList<string>), typeof (TagControl), new FrameworkPropertyMetadata(null, OnSelectedTagsChanged));


        public static readonly DependencyProperty SuggestedTagsProperty = DependencyProperty.Register("SuggestedTags",
            typeof (IList<string>), typeof (TagControl), new PropertyMetadata(null));

        public static readonly DependencyProperty AddNewTagTextProperty = DependencyProperty.Register("AddNewTagText",
            typeof (string), typeof (TagControl), new PropertyMetadata(null));

        private static readonly DependencyPropertyKey IsEditingPropertyKey =
            DependencyProperty.RegisterReadOnly("IsEditing", typeof (bool), typeof (TagControl),
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsEditingProperty = IsEditingPropertyKey.DependencyProperty;

        static TagControl()
        {
            // lookless control, get default style from generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof (TagControl),
                new FrameworkPropertyMetadata(typeof (TagControl)));
        }

        public TagControl()
        {
            TagAdded += (s, e) => RaiseTagsChanged();
            TagAdded += (s, e) => UpdateSelectedTagsOnAdd(e);
            TagRemoved += (s, e) => RaiseTagsChanged();
            TagRemoved += (s, e) => UpdateSelectedTagsOnRemove(e);

            SuggestedTags = new List<string>();
        }

        public IList<string> SelectedTags
        {
            get { return (IList<string>) GetValue(SelectedTagsProperty); }
            set { SetValue(SelectedTagsProperty, value); }
        }

        public List<string> PossibleSuggestedTags
        {
            get
            {
                if (!ReferenceEquals(ItemsSource, null) && ((IList<TagItem>) ItemsSource).Any())
                {
                    var tokenizedTagItems = (IList<TagItem>) ItemsSource;
                    var typedTags = tokenizedTagItems.Select(item => item.Text);
                    return SuggestedTags?
                        .Except(typedTags)
                        .ToList();
                }
                return SuggestedTags;
            }
        }

        public List<string> SuggestedTags
        {
            get { return (List<string>) GetValue(SuggestedTagsProperty); }
            set { SetValue(SuggestedTagsProperty, value); }
        }

        public string AddNewTagText
        {
            get { return (string) GetValue(AddNewTagTextProperty); }
            set { SetValue(AddNewTagTextProperty, value); }
        }

        // IsEditing, readonly
        public bool IsEditing
        {
            get { return (bool) GetValue(IsEditingProperty); }
            internal set { SetValue(IsEditingPropertyKey, value); }
        }


        private static void OnSelectedTagsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (TagControl) d;
            c.ItemsSource = ((IList<string>) e.NewValue).Select(i => new TagItem(i)).ToList();
        }

        private void UpdateSelectedTagsOnRemove(TagEventArgs e)
        {
            if (e.Item == null)
                return;
            if (!string.IsNullOrEmpty(e.Item.Text)) // Remove if delete button was clicked
                SelectedTags.Remove(e.Item.Text);
            else  // Remove if backspace was used to delete tag (TagItem Text was changed to empty and was then removed)
            {
                var source = (IList<TagItem>) ItemsSource;
                SelectedTags.Where(i => source.All(s => !s.Text.Equals(i)))
                    .ToList()
                    .ForEach(r => SelectedTags.Remove(r));
            }
        }

        private void UpdateSelectedTagsOnAdd(TagEventArgs e)
        {
            var source = (IList<TagItem>) ItemsSource;
            if (source.Count == SelectedTags.Count) //Update SelectedTags list if user edits tags
                SelectedTags.Where(i => source.All(s => !s.Text.Equals(i) || i.Equals(e.Item.Text)))
                    .ToList()
                    .ForEach(r => SelectedTags.Remove(r));
            SelectedTags.Add(e.Item.Text);
        }

        public event EventHandler<TagEventArgs> TagClick;
        public event EventHandler<TagEventArgs> TagAdded;
        public event EventHandler<TagEventArgs> TagRemoved;
        public event EventHandler<TagsChangedEventArgs> TagsChanged;


        public override void OnApplyTemplate()
        {
            ApplyTemplate();
        }

        public void ApplyTemplate(TagItem appliedTag = null)
        {
            var createBtn = GetTemplateChild("PART_CreateTagButton") as Button;
            if (createBtn != null)
            {
                createBtn.Click -= createBtn_Click;
                createBtn.Click += createBtn_Click;
            }

            var tagIcon = GetTemplateChild("PART_TagIcon") as Path;
            if (tagIcon != null)
            {
                tagIcon.MouseUp -= createBtn_Click;
                tagIcon.MouseUp += createBtn_Click;
            }

            base.OnApplyTemplate();

            if (appliedTag != null)
                RaiseTagAdded(appliedTag);
        }

        void createBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectedItem = InitializeNewTag();
        }

        internal TagItem InitializeNewTag(bool suppressEditing = false)
        {
            var newItem = new TagItem {IsEditing = !suppressEditing};
            AddTag(newItem);
            IsEditing = !suppressEditing;
            return newItem;
        }


        internal void AddTag(TagItem tag)
        {
            if (ItemsSource == null)
                ItemsSource = new List<TagItem>();

            ((IList) ItemsSource).Add(tag); // assume IList for convenience
            Items.Refresh();
        }

        internal void RemoveTag(TagItem tag, bool cancelEvent = false)
        {
            if (ItemsSource != null)
            {
                ((IList) ItemsSource).Remove(tag); // assume IList for convenience
                Items.Refresh();

                if (TagRemoved != null && !cancelEvent)
                    TagRemoved(this, new TagEventArgs(tag));
            }
        }

        internal void RaiseTagClick(TagItem tag)
        {
            TagClick?.Invoke(this, new TagEventArgs(tag));
        }

        internal void RaiseTagsChanged()
        {
            var tokenizedTagItems = (IList<TagItem>) ItemsSource;
            TagsChanged?.Invoke(this, new TagsChangedEventArgs(tokenizedTagItems));
        }

        internal void RaiseTagAdded(TagItem tag)
        {
            TagAdded?.Invoke(this, new TagEventArgs(tag));
        }

        internal void RaiseTagDoubleClick(TagItem tag)
        {
            tag.IsEditing = true;
        }
    }
}