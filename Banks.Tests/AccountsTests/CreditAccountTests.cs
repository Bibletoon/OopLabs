using Banks.Accounts;
using Banks.Clients;
using Banks.Transactions;
using NUnit.Framework;

namespace Banks.Tests
{
    public class CreditAccountTests : CommonAccountsTests
    {
        protected override BankAccount CreateAccount(decimal startSum, Client client) => Bank.CreateCreditAccount(client, startSum);
        
        [Test]
        public void WithdrawMoreThanLimit_TransactionFailed()
        {
            var client = ClientService.CreateClientBuilder("name", "surname").SetAddress("Address").SetPassportNumber(1000).Build();
            var account = CreateAccount(10, client);
            var transaction = Bank.WithdrawMoney(account.GetId(), 200);
            Assert.AreEqual(transaction.Status, TransactionStatus.Failed);
        }

        [Test]
        public void UseAccountWithNegativeBalance_CommissionAssessed()
        {
            var client = ClientService.CreateClientBuilder("name", "surname").SetAddress("Address").SetPassportNumber(1000).Build();
            var account = CreateAccount(-10, client);
            DateTimeProvider.CurrentDateTime = DateTimeProvider.CurrentDateTime.AddMonths(5);
            CentralBank.NotifyAboutFeePayment();
            Assert.AreEqual(-60, account.GetBalance());
        }
    }
}