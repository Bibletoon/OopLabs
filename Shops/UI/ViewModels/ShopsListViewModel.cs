using System;
using System.Collections.Generic;
using System.Linq;
using Shops.Models;
using Shops.Services;
using Shops.UI.Commands;
using Shops.UI.Commands.CommandArguments;
using Shops.UI.Queries;
using Shops.UI.Views;
using Terminal.Gui;

namespace Shops.UI.ViewModels
{
    public class ShopsListViewModel : ViewModel
    {
        private ShopsListView _view;
        private ShopManager _shopManager;

        public ShopsListViewModel(ShopManager shopManager)
        {
            _shopManager = shopManager;

            ChangeProductPriceCommand = new BaseParametrizedCommand<ChangeProductPriceArguments>(ChangeProductPrice);

            GetAllShopsQuery = new BaseQuery<List<Shop>>(() => _shopManager.GetAllShops().ToList());

            _view = new ShopsListView(this);
        }

        public IParameterizedCommand<ChangeProductPriceArguments> ChangeProductPriceCommand { get; }
        public IQuery<List<Shop>> GetAllShopsQuery { get; }

        public override void Dispose()
        {
            _view.Dispose();
        }

        protected override void Init(Toplevel top)
        {
            _view.Init(top);
        }

        private CommandResult ChangeProductPrice(ChangeProductPriceArguments args)
        {
            try
            {
                args.Shop.SetProductPrice(args.Product, args.NewPrice);
                return CommandResult.Success();
            }
            catch (Exception e)
            {
                return CommandResult.Fail(e.Message);
            }
        }
    }
}