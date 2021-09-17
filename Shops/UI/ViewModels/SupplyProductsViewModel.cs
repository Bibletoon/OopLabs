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
    public class SupplyProductsViewModel : ViewModel
    {
        private readonly SupplyProductsView _view;
        private readonly ShopManager _shopManager;

        public SupplyProductsViewModel(ShopManager shopManager)
        {
            _shopManager = shopManager;

            GetAllShopsQuery = new BaseQuery<List<Shop>>(() => _shopManager.GetAllShops().ToList());
            GetAllProductsQuery = new BaseQuery<List<Product>>(() => _shopManager.GetAllProducts().ToList());

            SupplyProductsCommand = new BaseParametrizedCommand<SupplyProductsCommandArguments>(SupplyProducts);

            _view = new SupplyProductsView(this);
        }

        public IQuery<List<Shop>> GetAllShopsQuery { get; }
        public IQuery<List<Product>> GetAllProductsQuery { get; }
        public IParameterizedCommand<SupplyProductsCommandArguments> SupplyProductsCommand { get; }

        public override void Dispose()
        {
            _view.Dispose();
        }

        protected override void Init(Toplevel top)
        {
            _view.Init(top);
        }

        private CommandResult SupplyProducts(SupplyProductsCommandArguments args)
        {
            try
            {
                args.Shop.AddLots(args.Lots.Cast<Lot?>().ToList());
                return CommandResult.Success();
            }
            catch (Exception e)
            {
                return CommandResult.Fail(e.Message);
            }
        }
    }
}