using System;
using Banks.Accounts;
using Banks.Banks;
using Banks.Clients;
using Banks.Tools.Exceptions;
using Banks.Transactions;
using NUnit.Framework;

namespace Banks.Tests
{
    [TestFixture]
    public class DepositAccountTests : CommonAccountsTests 
    {
        protected override BankAccount CreateAccount(decimal startSum, Client client)
            => CentralBank.CreateDepositAccount(Bank.Id, client, startSum);

        [Test]
        public void WithdrawMoneyBeforeTimeLimitEnded_TransactionFailed()
        {
            var client = ClientService.CreateClientBuilder("name", "surname").Build();
            var account = CreateAccount(1000, client);

            var transaction = CentralBank.WithdrawMoney(Bank.Id, account.GetId(), 1);
            Assert.AreEqual(transaction.Status, TransactionStatus.Failed);
        }

        [Test]
        [TestCase(1000, 10)]
        public void WithdrawMoneyAfterLimitEndedThenRevert_MoneyWithdrawnThenReturned(decimal startSum, decimal withdrawAmount)
        {
            var client = ClientService.CreateClientBuilder("name", "surname").Build();
            var account = CreateAccount(startSum, client);
            DateTimeProvider.CurrentDateTime = DateTimeProvider.CurrentDateTime.AddDays(5);
            var transaction = CentralBank.WithdrawMoney(Bank.Id, account.GetId(), withdrawAmount);
            Assert.AreEqual(startSum-withdrawAmount, account.GetBalance());
            CentralBank.RevertTransaction(Bank.Id, transaction.Id);
            Assert.AreEqual(startSum, account.GetBalance());
        }
        
        [Test]
        public void WithdrawMoreThanAvailable_TransactionFailed()
        {
            var client = ClientService.CreateClientBuilder("name", "surname").Build();
            var account = CreateAccount(10, client);
            DateTimeProvider.CurrentDateTime = DateTimeProvider.CurrentDateTime.AddDays(5);
            var transaction = CentralBank.WithdrawMoney(Bank.Id, account.GetId(), 20);
            Assert.AreEqual(transaction.Status, TransactionStatus.Failed);
        }

        [Test]
        public override void WithdrawMoreThanUnconfirmedClientLimit_TransactionFailed()
        {
            var client = ClientService.CreateClientBuilder("name", "surname").Build();
            var account = CreateAccount(1000, client);
            DateTimeProvider.CurrentDateTime = DateTimeProvider.CurrentDateTime.AddDays(5);
            var transaction = CentralBank.WithdrawMoney(Bank.Id, account.GetId(), 100);
            Assert.AreEqual(transaction.Status, TransactionStatus.Failed);
        }

        [Test]
        [TestCase(500, 10)]
        public override void TransferMoneyThenRevertTransactions_MoneyDecreasedThenReturned(decimal startSum, decimal transferSum)
        {
            var client = ClientService.CreateClientBuilder("name", "surname").Build();
            var firstAccount = CreateAccount(startSum, client);
            var secondAccount = CreateAccount(startSum, client);
            DateTimeProvider.CurrentDateTime = DateTimeProvider.CurrentDateTime.AddDays(5);
            var transaction = CentralBank.TransferMoney(Bank.Id, firstAccount.GetId(), secondAccount.GetId(), transferSum);
            Assert.AreEqual(startSum-transferSum, firstAccount.GetBalance());
            Assert.AreEqual(startSum+transferSum, secondAccount.GetBalance());
            CentralBank.RevertTransaction(Bank.Id, transaction.Id);
            Assert.AreEqual(startSum, firstAccount.GetBalance());
            Assert.AreEqual(startSum, secondAccount.GetBalance());
        }

        [Test]
        [TestCase(10000,200)]
        [TestCase(20000,800)]
        [TestCase(30000,1800)]
        [TestCase(40000,2400)]
        public void AccurePercentage_PercentsPayedCorrectly(decimal startBalance, decimal percents)
        {
            DateTimeProvider.CurrentDateTime = new DateTime(2021, 10, 30);
            var client = ClientService.CreateClientBuilder("name", "surname").Build();
            var account = CreateAccount(startBalance, client);
            DateTimeProvider.CurrentDateTime = DateTimeProvider.CurrentDateTime.AddDays(3);
            CentralBank.NotifyAboutFeePayment();
            Assert.AreEqual(startBalance+percents, account.GetBalance());
        }

        [Test]
        public void AccurePercentageAfterTransactions_PercentsPayedCorrectly()
        {
            DateTimeProvider.CurrentDateTime = new DateTime(2021, 10, 29);
            var client = ClientService.CreateClientBuilder("name", "surname").SetAddress("Address").SetPassportNumber(1000).Build();
            var account = CreateAccount(10000, client);
            DateTimeProvider.CurrentDateTime = DateTimeProvider.CurrentDateTime.AddDays(1);
            CentralBank.DepositMoney(Bank.Id, account.GetId(), 1000);
            DateTimeProvider.CurrentDateTime = DateTimeProvider.CurrentDateTime.AddDays(1);
            CentralBank.WithdrawMoney(Bank.Id, account.GetId(), 2000);
            DateTimeProvider.CurrentDateTime = DateTimeProvider.CurrentDateTime.AddDays(2);
            CentralBank.NotifyAboutFeePayment();
            Assert.AreEqual(9300, account.GetBalance());
        }
    }
}