using System;
using System.Linq;
using Banks.Accounts;
using Banks.Accounts.AccountConfigurations;
using Banks.Accounts.AccountConfigurations.Plans;
using Banks.Accounts.Decorators;
using Banks.Accounts.Proxies;
using Banks.Banks;
using Banks.Clients;
using Banks.Commands;
using Banks.Commands.CommandType;
using Banks.Enums;
using Banks.Tools;
using Banks.Transactions;
using Microsoft.EntityFrameworkCore;

namespace Banks.Data
{
    public class BanksDbContext : DbContext
    {
        private IDateTimeProvider _dateTimeProvider;
        private CentralBank _centralBank;

        public BanksDbContext(DbContextOptions<BanksDbContext> options, IDateTimeProvider dateTimeProvider)
            : base(options)
        {
            _dateTimeProvider = dateTimeProvider;
            Database.EnsureCreated();
        }

        public DbSet<Bank> Banks { get; set; }
        public DbSet<PercentagePlan> PercentagePlans { get; set; }
        public DbSet<AccountWrapperBase> Accounts { get; set; }
        public DbSet<BaseAccount> BaseAccounts { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Command> Commands { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<BankConfiguration> BankConfigurations { get; set; }
        public DbSet<DebitAccountConfiguration> DebitAccountConfigurations { get; set; }
        public DbSet<CreditAccountConfiguration> CreditAccountConfigurations { get; set; }
        public DbSet<DepositAccountConfiguration> DepositAccountConfigurations { get; set; }
        public DbSet<UnconfirmedClientLimits> UnconfirmedClientLimits { get; set; }

        public void LoadData()
        {
            Clients.Load();
            PercentagePlans.Include("_percentageConfiguration").Load();
            UnconfirmedClientLimits.Load();
            DepositAccountConfigurations
                .Include(x => x.PercentagePlan)
                .Include(x => x.TimeLimitPlan).Load();
            DebitAccountConfigurations.Include(x => x.PercentagePlan).Load();
            CreditAccountConfigurations
                .Include(x => x.CommissionPlan)
                .Include(x => x.LimitPlan).Load();
            BankConfigurations.Load();
            Commands.Load();
            Transactions.Load();
            BaseAccounts.Load();
            Accounts.Load();
            Commands.Load();
            Transactions.Load();
            Banks.Load();
        }

        internal IDateTimeProvider GetDateTimeProvider()
        {
            return _dateTimeProvider;
        }

        internal void SetCentralBank(CentralBank bank)
        {
            _centralBank = bank;
        }

        internal CentralBank GetCentralBank()
        {
            return _centralBank;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureAccountConfigurations(modelBuilder);
            ConfigureBankAccounts(modelBuilder);
            ConfigureCommands(modelBuilder);
            ConfigureBanks(modelBuilder);
            ConfigurePlans(modelBuilder);
            ConfigureTransactions(modelBuilder);
            modelBuilder.Entity<Client>().HasKey(k => k.Id);
        }

        protected void ConfigureAccountConfigurations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CreditAccountConfiguration>().HasKey("_id");
            modelBuilder.Entity<DebitAccountConfiguration>().HasKey("_id");
            modelBuilder.Entity<DepositAccountConfiguration>().HasKey("_id");
            modelBuilder.Entity<UnconfirmedClientLimits>().HasKey("_id");
        }

        protected void ConfigureBankAccounts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotificationHandlerDecorator>().Property<NotificationType[]>("_notificationTypes")
                        .HasConversion(
                            v => string.Join(',', v.Select(e => e.ToString("D")).ToArray()),
                            v => v.Split(new[] { ',' })
                                  .Select(e => Enum.Parse(typeof(NotificationType), e))
                                  .Cast<NotificationType>()
                                  .ToArray());

            modelBuilder.Entity<CommissionDecorator>().HasOne("_commission");
            modelBuilder.Entity<CommissionDecorator>().Property("_lastCommissionDate");
            modelBuilder.Entity<CommissionDecorator>().Property("_lastBalance");

            modelBuilder.Entity<PercentageDecorator>().HasOne("_percentagePlan");
            modelBuilder.Entity<PercentageDecorator>().Property("_startSum");
            modelBuilder.Entity<PercentageDecorator>().Property("_lastPercentageDate");
            modelBuilder.Entity<PercentageDecorator>().Property("_lastBalance");
            modelBuilder.Entity<PercentageDecorator>().Property("_assessedPercents");

            modelBuilder.Entity<MinimalBalanceProxy>().HasOne("_minimalAmount");

            modelBuilder.Entity<TimeLimitProxy>().Property("_creationDateTime");
            modelBuilder.Entity<TimeLimitProxy>().HasOne("_limitPlan");

            modelBuilder.Entity<UnconfirmedClientProxy>().HasOne("_limit");

            modelBuilder.Entity<AccountWrapperBase>().HasOne("_account");

            modelBuilder.Entity<FinalWrapper>();

            modelBuilder.Entity<BankAccount>().HasKey("Id");

            modelBuilder.Entity<BaseAccount>().Property("_balance");
            modelBuilder.Entity<BaseAccount>().HasOne("_client");
            modelBuilder.Entity<BaseAccount>().HasMany("_transactionsHistory");
        }

        protected void ConfigureCommands(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Command>().HasKey("Id");

            modelBuilder.Entity<BalanceIncreaseCommand>();
            modelBuilder.Entity<BalanceDecreaseCommand>();
            modelBuilder.Entity<DepositCommand>();
            modelBuilder.Entity<PayFeesCommand>();
            modelBuilder.Entity<WithdrawCommand>();
        }

        protected void ConfigureBanks(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BankConfiguration>().HasOne(c => c.DebitAccountConfiguration);
            modelBuilder.Entity<BankConfiguration>().HasOne(c => c.DepositAccountConfiguration);
            modelBuilder.Entity<BankConfiguration>().HasOne(c => c.CreditAccountConfiguration);
            modelBuilder.Entity<BankConfiguration>().HasOne(c => c.UnconfirmedClientLimits);
            modelBuilder.Entity<BankConfiguration>().HasKey("_id");
            modelBuilder.Entity<Bank>().HasOne("_configuration");
            modelBuilder.Entity<Bank>().HasMany("_accounts");
            modelBuilder.Entity<Bank>().HasMany("_subscribedClients");
            modelBuilder.Entity<Bank>().HasKey("_id");
        }

        protected void ConfigurePlans(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PercentagePlan>().HasKey("_id");
            modelBuilder.Entity<PercentagePlan>().HasMany("_percentageConfiguration");
            modelBuilder.Entity<PercentageConfiguration>().HasKey("_id");

            modelBuilder.Entity<CommissionPlan>().HasKey("_id");
            modelBuilder.Entity<LimitPlan>().HasKey("_id");
            modelBuilder.Entity<TimeLimitPlan>().HasKey("_id");
        }

        protected void ConfigureTransactions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>();
            modelBuilder.Entity<BasicTransaction>().HasOne("_command");
            modelBuilder.Entity<BasicTransaction>().HasOne("_account");
            modelBuilder.Entity<ChainedTransaction>().HasOne("ChildTransaction").WithOne("ParentTransaction");
            modelBuilder.Entity<BasicChainedTransaction>().HasOne("_command");
            modelBuilder.Entity<BasicChainedTransaction>().HasOne("_account");
            modelBuilder.Entity<StraightChainedTransaction>().HasOne("_command");
            modelBuilder.Entity<StraightChainedTransaction>().HasOne("_account");
        }
    }
}