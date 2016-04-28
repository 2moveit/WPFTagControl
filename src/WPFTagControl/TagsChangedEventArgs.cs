using System;
using System.Collections.Generic;

namespace WPFTagControl
{
    public class TagsChangedEventArgs : EventArgs
    {
        public IList<TagItem> Items { get; set; }

        public TagsChangedEventArgs(IList<TagItem> items)
        {
            this.Items = items;
        }
    }
}