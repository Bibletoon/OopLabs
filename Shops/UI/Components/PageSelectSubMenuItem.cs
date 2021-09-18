using System;

namespace Shops.UI.Components
{
    public class PageSelectSubMenuItem : PanelMenuItem, ISubMenuItem
    {
        public PageSelectSubMenuItem(string name, PanelMenuItem parentItem, Type page)
            : base(name)
        {
            ParentItem = parentItem;
            Page = page;
        }

        public PanelMenuItem ParentItem { get; }
        public Type Page { get; }
    }
}