using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Shops.Models;
using Shops.Services;
using Shops.Tools;
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

            new DataFaker(shopManager, userManager).CreateData();
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