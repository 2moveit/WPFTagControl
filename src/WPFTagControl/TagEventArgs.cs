using System;

namespace WPFTagControl
{
    public class TagEventArgs : EventArgs
    {
        public TagItem Item { get; set; }

        public TagEventArgs(TagItem item)
        {
            this.Item = item;
        }
    }
}