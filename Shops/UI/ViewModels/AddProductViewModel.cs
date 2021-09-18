using System;
using Shops.Services;
using Shops.UI.Commands;
using Shops.UI.Views;
using Terminal.Gui;

namespace Shops.UI.ViewModels
{
    public class AddProductViewModel : ViewModel
    {
        private readonly AddProductView _view;
        private ShopManager _shopManager;

        public AddProductViewModel(ShopManager shopManager)
        {
            _shopManager = shopManager;

            AddProductCommand = new BaseParametrizedCommand<string>(AddProduct);

            _view = new AddProductView(this);
        }

        public IParameterizedCommand<string> AddProductCommand { get; }

        public override void Dispose()
        {
            _view?.Dispose();
        }

        protected override void Init(Toplevel top)
        {
            _view.Init(top);
        }

        private CommandResult AddProduct(string name)
        {
            try
            {
                _shopManager.RegisterProduct(name);
                return new CommandResult(true);
            }
            catch (Exception e)
            {
                return new CommandResult(false, e.Message);
            }
        }
    }
}