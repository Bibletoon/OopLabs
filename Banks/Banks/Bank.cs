using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Accounts;
using Banks.Accounts.AccountConfigurations.Plans;
using Banks.Builders;
using Banks.Clients;
using Banks.Commands;
using Banks.Data;
using Banks.Enums;
using Banks.Tools;
using Banks.Transactions;

namespace Banks.Banks
{
    public class Bank
    {
        private readonly BanksDbContext _dbContext;
        private Guid _id = Guid.NewGuid();
        private BankConfiguration _configuration;
        private List<FinalWrapper> _accounts = new List<FinalWrapper>();
        private List<Client> _subscribedClients = new List<Client>();
        private IDateTimeProvider _dateTimeProvider;
        private CentralBank _centralBank;

        internal Bank(
            BanksDbContext dbContext,
            BankConfiguration configuration)
        {
            _dbContext = dbContext;
            _dateTimeProvider = dbContext.GetDateTimeProvider();
            _centralBank = dbContext.GetCentralBank();
            _configuration = configuration;
        }

        private Bank(BanksDbContext dbContext)
        {
            _dbContext = dbContext;
            _dateTimeProvider = dbContext.GetDateTimeProvider();
            _centralBank = dbContext.GetCentralBank();
        }

        public List<BankAccount> GetAccounts() => _accounts.Cast<BankAccount>().ToList();

        public BankAccount FindAccount(Guid id)
        {
            return _accounts.Find(a => a.GetId() == id);
        }

        public BankAccount CreateDepositAccount(Client client, decimal startBalance)
        {
            ArgumentNullException.ThrowIfNull(client, nameof(client));
            var account = new BankAccountBuilder(startBalance, client)
                          .AddNotificationHandler(NotificationType.All, NotificationType.Deposit)
                          .AddPercentage(_dateTimeProvider, _configuration.DepositAccountConfiguration.PercentagePlan)
                          .SetMinimalBalance(new LimitPlan(0))
                          .AddLimitsForUnconfirmedClient(_configuration.UnconfirmedClientLimits)
                          .AddTimeLimit(_dateTimeProvider, _configuration.DepositAccountConfiguration.TimeLimitPlan)
                          .Build();
            _accounts.Add(account);
            _dbContext.Accounts.Add(account);
            _dbContext.Banks.Update(this);
            _dbContext.SaveChanges();
            return account;
        }

        public BankAccount CreateDebitAccount(Client client, decimal startBalance)
        {
            ArgumentNullException.ThrowIfNull(client, nameof(client));
            var account = new BankAccountBuilder(startBalance, client)
                          .AddNotificationHandler(NotificationType.All, NotificationType.Debit)
                          .AddPercentage(_dateTimeProvider, _configuration.DebitAccountConfiguration.PercentagePlan)
                          .SetMinimalBalance(new LimitPlan(0))
                          .AddLimitsForUnconfirmedClient(
                              _configuration.UnconfirmedClientLimits)
                          .Build();
            _accounts.Add(account);
            _dbContext.Accounts.Add(account);
            _dbContext.Banks.Update(this);
            _dbContext.SaveChanges();
            return account;
        }

        public BankAccount CreateCreditAccount(Client client, decimal startBalance)
        {
            ArgumentNullException.ThrowIfNull(client, nameof(client));
            var account = new BankAccountBuilder(startBalance, client)
                          .AddNotificationHandler(NotificationType.All, NotificationType.Credit)
                          .AddCommission(_configuration.CreditAccountConfiguration.CommissionPlan, _dateTimeProvider)
                          .SetMinimalBalance(_configuration.CreditAccountConfiguration.LimitPlan)
                          .AddLimitsForUnconfirmedClient(
                              _configuration.UnconfirmedClientLimits)
                          .Build();
            _accounts.Add(account);
            _dbContext.Accounts.Add(account);
            _dbContext.Banks.Update(this);
            _dbContext.SaveChanges();
            return account;
        }

        public Transaction WithdrawMoney(Guid accountId, decimal amount)
        {
            var account = _accounts.Find(a => a.GetId() == accountId)
                          ?? throw new Exception("Wrong account id");
            var transaction = new BasicTransaction(_dateTimeProvider.Now(), new WithdrawCommand(amount), account);
            transaction.Apply();
            _dbContext.Transactions.Add(transaction);
            _dbContext.Update(account);
            _dbContext.SaveChanges();
            return transaction;
        }

        public Transaction TransferMoney(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var fromAccount = FindAccount(fromAccountId)
                          ?? throw new Exception("Account not found");
            var toAccount = _centralBank.FindAccount(toAccountId)
                          ?? throw new Exception("Account not found");

            var currentDateTime = _dateTimeProvider.Now();
            var withdrawTransaction = new BasicChainedTransaction(currentDateTime, new WithdrawCommand(amount), fromAccount);
            var addTransaction = new StraightChainedTransaction(currentDateTime, new DepositCommand(amount), toAccount, withdrawTransaction);
            withdrawTransaction.Apply();
            _dbContext.Transactions.Add(withdrawTransaction);
            _dbContext.Transactions.Add(addTransaction);
            _dbContext.SaveChanges();
            return withdrawTransaction;
        }

        public Transaction DepositMoney(Guid accountId, decimal amount)
        {
            var account = FindAccount(accountId)
                          ?? throw new Exception("Account not found");

            var transaction = new BasicTransaction(_dateTimeProvider.Now(), new DepositCommand(amount), account);
            transaction.Apply();
            _dbContext.Transactions.Add(transaction);
            _dbContext.Update(account);
            _dbContext.SaveChanges();
            return transaction;
        }

        public void RevertTransaction(Guid transactionId)
        {
           var transaction = _accounts.SelectMany(a => a.GetTransactionsHistory()).FirstOrDefault(t => t.Id == transactionId);

           if (transaction is null)
               throw new Exception("Can't find transaction with such id");

           transaction.Revert();
           _dbContext.Transactions.Update(transaction);
           _dbContext.SaveChanges();
        }

        public void Subscribe(Client client)
        {
            ArgumentNullException.ThrowIfNull(client, nameof(client));
            if (!_subscribedClients.Contains(client))
                _subscribedClients.Add(client);
        }

        public void ChangeDepositPercentage(PercentagePlan newPlan)
        {
            ArgumentNullException.ThrowIfNull(newPlan, nameof(newPlan));
            _configuration.DepositAccountConfiguration.PercentagePlan.ChangeConfiguration(newPlan);
            _dbContext.BankConfigurations.Update(_configuration);
            _dbContext.SaveChanges();
            NotifyUsers(NotificationType.Deposit, "Deposit percentage was changed");
        }

        public void ChangeDebitPercentage(PercentagePlan newPlan)
        {
            ArgumentNullException.ThrowIfNull(newPlan, nameof(newPlan));
            _configuration.DebitAccountConfiguration.PercentagePlan.ChangeConfiguration(newPlan);
            _dbContext.BankConfigurations.Update(_configuration);
            _dbContext.SaveChanges();
            NotifyUsers(NotificationType.Debit, "Debit percentage was changed");
        }

        public void ChangeCreditLimit(decimal limit)
        {
            _configuration.CreditAccountConfiguration.LimitPlan.Limit = limit;
            _dbContext.BankConfigurations.Update(_configuration);
            _dbContext.SaveChanges();
            NotifyUsers(NotificationType.Credit, "Credit limit was changed");
        }

        public void ChangeUnconfirmedClientLimit(decimal newLimit)
        {
            _configuration.UnconfirmedClientLimits.WithdrawLimit = newLimit;
            _dbContext.BankConfigurations.Update(_configuration);
            _dbContext.SaveChanges();
            NotifyUsers(NotificationType.All, "Limits for unconfirmed clients was changed");
        }

        internal void PayFees()
        {
            foreach (var account in _accounts)
            {
                var transaction = new BasicTransaction(_dateTimeProvider.Now(), new PayFeesCommand(), account);
                transaction.Apply();
                _dbContext.Transactions.Add(transaction);
                _dbContext.Update(account);
            }

            _dbContext.SaveChanges();
        }

        private void NotifyUsers(NotificationType notificationType, string message)
        {
            _accounts.Where(
                acc => _subscribedClients.Contains(acc.GetClient())).Distinct().ToList().ForEach(
                n => n.Proceed(new NotifyCommand(
                                                 notificationType,
                                                 $"Notification of type {notificationType.ToString()}")));
        }
    }
}