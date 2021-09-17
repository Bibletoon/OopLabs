using System;
using Shops.Models;
using Shops.Services;
using Shops.UI.Commands;
using Shops.UI.Views;
using Terminal.Gui;

namespace Shops.UI.ViewModels
{
    public class AddShopViewModel : ViewModel
    {
        private readonly AddShopView _view;
        private ShopManager _shopManager;

        public AddShopViewModel(ShopManager shopManager)
        {
            _shopManager = shopManager;

            AddShopCommand = new BaseParametrizedCommand<string?>(AddShop);

            _view = new AddShopView(this);
        }

        public IParameterizedCommand<string?> AddShopCommand { get; }

        public override void Dispose()
        {
            _view.Dispose();
        }

        protected override void Init(Toplevel top)
        {
            _view.Init(top);
        }

        private CommandResult AddShop(string? name)
        {
            try
            {
                var shop = new Shop(name);
                _shopManager.RegisterShop(shop);
                return CommandResult.Success();
            }
            catch (Exception e)
            {
                return CommandResult.Fail(e.Message);
            }
        }
    }
}