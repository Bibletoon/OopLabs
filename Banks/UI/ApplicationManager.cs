using System;
using Banks.Accounts.AccountConfigurations;
using Banks.Accounts.AccountConfigurations.Plans;
using Banks.Banks;
using Banks.Builders;
using Banks.Clients;
using Banks.Data;
using Banks.Tools;
using Banks.UI.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Terminal.Gui;

namespace Banks.UI
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

            InitializeViewModels(serviceCollection);
            InitializeLogicServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();
            _navigationViewManager = new NavigationViewManager(_serviceProvider);
        }

        public void Run()
        {
            MockData();

            Application.Init();
            Application.Top.ColorScheme = Colors.Base;

            _navigationViewManager.OpenPage(StartPage);

            Application.Run();
        }

        private void MockData()
        {
            var bank = _serviceProvider.GetRequiredService<Bank>();
            var centralBank = _serviceProvider.GetRequiredService<CentralBank>();

            new DataFaker(centralBank, bank).CreateData();
        }

        private void InitializeViewModels(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<MenuViewModel>();
            serviceCollection.AddTransient<TimeTravelViewModel>();
            serviceCollection.AddTransient<PayFeesViewModel>();
            serviceCollection.AddTransient<AccountsViewModel>();
            serviceCollection.AddTransient<TransactionsViewModel>();
            serviceCollection.AddTransient<CreateClientViewModel>();
            serviceCollection.AddTransient<AddAccountViewModel>();
        }

        private void InitializeLogicServices(IServiceCollection serviceCollection)
        {
            var dtp = new DateTimeProvider();
            var options = new DbContextOptionsBuilder<BanksDbContext>().UseSqlite("Filename=banks.db").Options;
            var db = new BanksDbContext(options, dtp);
            serviceCollection.AddSingleton<IDateTimeProvider>(dtp);
            var cb = new CentralBank(db);
            var bank = InitializeBank(cb);
            serviceCollection.AddSingleton(cb);
            serviceCollection.AddSingleton(bank);
            serviceCollection.AddSingleton(new ClientService(db));
        }

        private Bank InitializeBank(CentralBank centralBank)
        {
            var config = new BankConfiguration(
                    new DepositAccountConfiguration(
                        new PercentagePlanBuilder(10).Build(),
                        new TimeLimitPlan(TimeSpan.FromDays(2))),
                    new DebitAccountConfiguration(
                        new PercentagePlanBuilder(10).SetPercentageForMoreThan(1000, 20)
                                                     .SetPercentageForMoreThan(2000, 40).Build()),
                    new CreditAccountConfiguration(
                        new CommissionPlan(300),
                        new LimitPlan(5000)),
                    new UnconfirmedClientLimits(
                        1000));

            return centralBank.CreateBank(config);
        }
    }
}