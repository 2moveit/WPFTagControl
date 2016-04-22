using System;
using System.Collections.Generic;

namespace WPFTagControl
{
    public class TagsChangedEventArgs : EventArgs
    {
        public List<TagItem> Items { get; set; }

        public TagsChangedEventArgs(List<TagItem> items)
        {
            this.Items = items;
        }
    }
}