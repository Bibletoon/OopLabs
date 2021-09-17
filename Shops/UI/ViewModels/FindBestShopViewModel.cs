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

            // Note: Cast здесь (и в паре других мест) добавлен,
            // так как нельзя передать List<Class> как параметр в метод, принимающий List<Class?>,
            // а создать перегрузку, принимающую List<Class> тоже нельзя
            // (при передаче ругается, что типы разные, а при перегрузке ругается, что типы одинаковые )
            FindBestShopQuery = new BaseParametrizedQuery<Shop?, List<ProductOrder>>(args =>
                                        _shopManager.FindShopWithBestOffer(args.Cast<ProductOrder?>().ToList()));

            _view = new FindBestShopView(this);
        }

        public IQuery<List<Product>> GetAllProductsQuery { get; }
        public IParameterizedQuery<Shop?, List<ProductOrder>> FindBestShopQuery { get; }

        public override void Dispose()
        {
            _view.Dispose();
        }

        protected override void Init(Toplevel top)
        {
            _view.Init(top);
        }
    }
}