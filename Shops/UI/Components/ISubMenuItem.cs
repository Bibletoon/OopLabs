using Terminal.Gui;

namespace Shops.UI.Components
{
    public interface ISubMenuItem
    {
        public PanelMenuItem ParentItem { get; }
    }
}