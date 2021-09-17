using Terminal.Gui;

namespace Shops.UI.Components
{
    public interface ISubMenuItem
    {
        PanelMenuItem ParentItem { get; }
    }
}