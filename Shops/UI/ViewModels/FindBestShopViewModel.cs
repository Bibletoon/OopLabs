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
    public class FindBestShopViewModel : ViewModel
    {
        private readonly FindBestShopView _view;
        private readonly ShopManager _shopManager;
        private readonly UserManager _userManager;

        public FindBestShopViewModel(ShopManager shopManager, UserManager userManager)
        {
            _shopManager = shopManager;
            _userManager = userManager;

            GetAllProductsQuery = new BaseQuery<List<Product>>(() => _shopManager.GetAllProducts().ToList());

            FindBestShopQuery = new BaseParametrizedQuery<Shop, List<ProductOrder>>(args =>
                                        _shopManager.FindShopWithBestOffer(args));

            _view = new FindBestShopView(this);
        }

        public IQuery<List<Product>> GetAllProductsQuery { get; }
        public IParameterizedQuery<Shop, List<ProductOrder>> FindBestShopQuery { get; }

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