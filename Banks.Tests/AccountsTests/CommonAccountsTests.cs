using System;
using Banks.Accounts;
using Banks.Accounts.AccountConfigurations;
using Banks.Accounts.AccountConfigurations.Plans;
using Banks.Banks;
using Banks.Builders;
using Banks.Clients;
using Banks.Data;
using Banks.Transactions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Banks.Tests
{
    [TestFixture]
    public abstract class CommonAccountsTests : IDisposable
    {
        protected TestDateTimeProvider DateTimeProvider;
        protected BanksDbContext DbContext;
        protected CentralBank CentralBank;
        protected Bank Bank;
        protected ClientService ClientService;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var options = new DbContextOptionsBuilder<BanksDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            DateTimeProvider = new TestDateTimeProvider();
            DbContext = new BanksDbContext(options, DateTimeProvider);
            CentralBank = new CentralBank(DbContext);

            var configuration = new BankConfiguration(
                new DepositAccountConfiguration(
                    new PercentagePlanBuilder(365).SetPercentageForMoreThan(10000,  730).SetPercentageForMoreThan(20000, 1095).Build(),
                    new TimeLimitPlan(TimeSpan.FromHours(4))
                ),
                new DebitAccountConfiguration(
                    new PercentagePlanBuilder(365).Build()
                ),
                new CreditAccountConfiguration(new CommissionPlan(10), new LimitPlan(100)),
                new UnconfirmedClientLimits(50)
            );
            Bank = CentralBank.CreateBank(configuration);
            ClientService = new ClientService(DbContext);
        }

        protected abstract BankAccount CreateAccount(decimal startSum, Client client);

        [Test]
        [TestCase(500, 10)]
        public virtual void DepositMoneyToAccountThenRevertTransaction_MoneyAddedThenReturned(decimal amount, decimal depositAmount)
        {
            var client = ClientService.CreateClientBuilder("name", "surname").Build();
            var account = CreateAccount(amount, client);
            var transaction = Bank.DepositMoney(account.GetId(), depositAmount);
            Assert.AreEqual(amount + depositAmount, account.GetBalance());

            Bank.RevertTransaction(transaction.Id);

            Assert.AreEqual(amount, account.GetBalance());
        }

        [Test]
        [TestCase(1000, 2)]
        public virtual void TransferMoneyThenRevertTransactions_MoneyDecreasedThenReturned(decimal startSum, decimal transferSum)
        {
            var client = ClientService.CreateClientBuilder("name", "surname").Build();
            var firstAccount = CreateAccount(startSum, client);
            var secondAccount = CreateAccount(startSum, client);
            var transaction = Bank.TransferMoney(firstAccount.GetId(), secondAccount.GetId(), transferSum);
            Assert.AreEqual(startSum-transferSum, firstAccount.GetBalance());
            Assert.AreEqual(startSum+transferSum, secondAccount.GetBalance());
            Bank.RevertTransaction(transaction.Id);
            Assert.AreEqual(startSum, firstAccount.GetBalance());
            Assert.AreEqual(startSum, secondAccount.GetBalance());
        }

        [Test]
        public virtual void WithdrawMoreThanUnconfirmedClientLimit_TransactionFailed()
        {
            var client = ClientService.CreateClientBuilder("name", "surname").Build();
            var account = CreateAccount(1000, client);
            var transaction = Bank.WithdrawMoney(account.GetId(), 100);
            Assert.AreEqual(transaction.Status, TransactionStatus.Failed);
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }
    }
}