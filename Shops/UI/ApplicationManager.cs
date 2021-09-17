using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Shops.Models;
using Shops.Services;
using Shops.UI.ViewModels;
using Shops.UI.Views;
using Terminal.Gui;
using View = Terminal.Gui.View;

namespace Shops.UI
{
    public class ApplicationManager
    {
        private static readonly Type StartPage = typeof(MenuViewModel);
        private readonly NavigationViewManager _navigationViewManager;
        private readonly ServiceProvider _serviceProvider;

        public ApplicationManager()
        {
            if (!StartPage.IsSubclassOf(typeof(ViewModel)))
                throw new ApplicationException("Start page must be ViewModel");

            var serviceCollection = new ServiceCollection();

            InitializeViews(serviceCollection);
            InitializeLogicServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();
            _navigationViewManager = new NavigationViewManager(_serviceProvider);
        }

        public void Run()
        {
            // Метод добавлен для того, чтобы легче тестировать UI
            MockData();

            Application.Init();
            Application.Top.ColorScheme = Colors.Base;

            _navigationViewManager.OpenPage(StartPage);

            Application.Run();
        }

        private void MockData()
        {
            ShopManager shopManager = _serviceProvider.GetService<ShopManager>()
                                      ?? throw new ApplicationException("Can't launch application");

            UserManager userManager = _serviceProvider.GetService<UserManager>()
                                      ?? throw new ApplicationException("Can't launch application");

            var shop1 = new Shop("Shop 1");
            var shop2 = new Shop("Shop 2");
            var shop3 = new Shop("Shop 3");
            shopManager.RegisterShop(shop1);
            shopManager.RegisterShop(shop2);
            shopManager.RegisterShop(shop3);

            var product1 = shopManager.RegisterProduct("Product 1");
            var product2 = shopManager.RegisterProduct("Product 2");
            var product3 = shopManager.RegisterProduct("Product 3");
            var product4 = shopManager.RegisterProduct("Product 4");
            var product5 = shopManager.RegisterProduct("Product 5");
            var product6 = shopManager.RegisterProduct("Product 6");
            var product7 = shopManager.RegisterProduct("Product 7");
            var product8 = shopManager.RegisterProduct("Product 8");
            var product9 = shopManager.RegisterProduct("Product 9");
            var product10 = shopManager.RegisterProduct("Product 10");
            var product11 = shopManager.RegisterProduct("Product 11");
            var product12 = shopManager.RegisterProduct("Product 12");
            var product13 = shopManager.RegisterProduct("Product 13");
            var product14 = shopManager.RegisterProduct("Product 14");
            var product15 = shopManager.RegisterProduct("Product 15");
            var product16 = shopManager.RegisterProduct("Product 16");
            var product17 = shopManager.RegisterProduct("Product 17");
            var product18 = shopManager.RegisterProduct("Product 18");
            var product19 = shopManager.RegisterProduct("Product 19");

            shop1.AddLots(new List<Lot?>()
            {
                product1.ToLot(10, 100),
                product2.ToLot(5, 20),
                product3.ToLot(7, 50),
                product4.ToLot(7, 50),
                product5.ToLot(7, 50),
                product6.ToLot(7, 50),
                product7.ToLot(7, 50),
                product8.ToLot(7, 50),
                product9.ToLot(7, 50),
                product10.ToLot(7, 50),
                product11.ToLot(7, 50),
                product12.ToLot(7, 50),
                product13.ToLot(7, 50),
                product14.ToLot(7, 50),
                product15.ToLot(7, 50),
                product16.ToLot(7, 50),
                product17.ToLot(7, 50),
                product18.ToLot(7, 50),
                product19.ToLot(7, 50),
            });

            shop2.AddLots(new List<Lot?>()
            {
                product1.ToLot(1, 30),
                product3.ToLot(500, 20),
                product4.ToLot(700, 50),
            });

            var user1 = new User("Person 1", 1000);
            var user2 = new User("Person 2", 525);
            var user3 = new User("Person 3", 10);
            var user4 = new User("Person 4", 777);

            userManager.RegisterUser(user1);
            userManager.RegisterUser(user2);
            userManager.RegisterUser(user3);
            userManager.RegisterUser(user4);
        }

        private void InitializeViews(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<MenuViewModel>();
            serviceCollection.AddTransient<ShopsListViewModel>();
            serviceCollection.AddTransient<SupplyProductsViewModel>();
            serviceCollection.AddTransient<AddShopViewModel>();
            serviceCollection.AddTransient<AddProductViewModel>();
            serviceCollection.AddTransient<AddPersonViewModel>();
            serviceCollection.AddTransient<BuyProductsViewModel>();
            serviceCollection.AddTransient<FindBestShopViewModel>();
        }

        private void InitializeLogicServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ShopManager>();
            serviceCollection.AddSingleton<UserManager>();
        }
    }
}