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
    public class BuyProductsViewModel : ViewModel
    {
        private readonly BuyProductsView _view;
        private readonly ShopManager _shopManager;
        private readonly UserManager _userManager;

        public BuyProductsViewModel(ShopManager shopManager, UserManager userManager)
        {
            _shopManager = shopManager;
            _userManager = userManager;

            GetAllShopsQuery = new BaseQuery<List<Shop>>(() => _shopManager.GetAllShops().ToList());
            GetAllProductsQuery = new BaseQuery<List<Product>>(() => _shopManager.GetAllProducts().ToList());
            GetAllUsersQuery = new BaseQuery<List<User>>(() => _userManager.GetAllUsers().ToList());

            BuyProductsCommand = new BaseParametrizedCommand<BuyProductsCommandArguments>(BuyProducts);

            _view = new BuyProductsView(this);
        }

        public IQuery<List<Shop>> GetAllShopsQuery { get; }
        public IQuery<List<Product>> GetAllProductsQuery { get; }
        public IQuery<List<User>> GetAllUsersQuery { get; }
        public IParameterizedCommand<BuyProductsCommandArguments> BuyProductsCommand { get; }

        public override void Dispose()
        {
            _view?.Dispose();
        }

        protected override void Init(Toplevel top)
        {
            _view.Init(top);
        }

        private CommandResult BuyProducts(BuyProductsCommandArguments args)
        {
            try
            {
                args.Shop.Buy(args.User, args.Orders);
                return CommandResult.Success();
            }
            catch (Exception e)
            {
                return CommandResult.Fail(e.Message);
            }
        }
    }
}