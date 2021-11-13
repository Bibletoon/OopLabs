using Banks.UI.Views;
using Terminal.Gui;

namespace Banks.UI.ViewModels
{
    public class MenuViewModel : ViewModel
    {
        private readonly MenuView _view;

        public MenuViewModel()
        {
            _view = new MenuView(this);
        }

        public override void Dispose()
        {
            _view?.Dispose();
        }

        protected override void Init(Toplevel top)
        {
            _view.Init(top);
        }
    }
}