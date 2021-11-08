using System;
using Banks.Accounts;
using Banks.Clients;
using Banks.Transactions;
using NUnit.Framework;

namespace Banks.Tests
{
    [TestFixture]
    public class DebitAccountTests : CommonAccountsTests
    {
        protected override BankAccount CreateAccount(decimal startSum, Client client) => CentralBank.CreateDebitAccount(Bank.Id, client, startSum);
        
        [Test]
        public void WithdrawMoreThanAvailable_TransactionFailed()
        {
            var client = ClientService.CreateClientBuilder("name", "surname").Build();
            var account = CreateAccount(10, client);
            var transaction = CentralBank.WithdrawMoney(Bank.Id, account.GetId(), 20);
            Assert.AreEqual(transaction.Status, TransactionStatus.Failed);
        }
        
        [Test]
        [TestCase(10000,200)]
        public void AccurePercentage_PercentsPayedCorrectly(decimal startBalance, decimal percents)
        {
            DateTimeProvider.CurrentDateTime = new DateTime(2021, 10, 30);
            var client = ClientService.CreateClientBuilder("name", "surname").Build();
            var account = CreateAccount(startBalance, client);
            DateTimeProvider.CurrentDateTime = DateTimeProvider.CurrentDateTime.AddDays(3);
            CentralBank.NotifyAboutFeePayment();
            Assert.AreEqual(startBalance+percents, account.GetBalance());
        }
    }
}