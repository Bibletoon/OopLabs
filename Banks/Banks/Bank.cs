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
        private BankConfiguration _configuration;
        private List<FinalWrapper> _accounts = new List<FinalWrapper>();
        private List<Client> _subscribedClients = new List<Client>();
        private IDateTimeProvider _dateTimeProvider;
        private CentralBank _centralBank;

        internal Bank(
            BanksDbContext dbContext,
            BankConfiguration configuration)
        {
            _dateTimeProvider = dbContext.GetDateTimeProvider();
            _centralBank = dbContext.GetCentralBank();
            _configuration = configuration;
        }

        private Bank(BanksDbContext dbContext)
        {
            _dateTimeProvider = dbContext.GetDateTimeProvider();
            _centralBank = dbContext.GetCentralBank();
        }

        public Guid Id { get; private init; } = Guid.NewGuid();

        public List<FinalWrapper> GetAccounts() => _accounts.ToList();

        public BankAccount FindAccount(Guid id)
        {
            return _accounts.Find(a => a.GetId() == id);
        }

        public Transaction FindTransaction(Guid id)
        {
            return _accounts.SelectMany(a => a.GetTransactionsHistory()).FirstOrDefault(t => t.Id == id);
        }

        internal FinalWrapper CreateDepositAccount(Client client, decimal startBalance)
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
            return account;
        }

        internal FinalWrapper CreateDebitAccount(Client client, decimal startBalance)
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
            return account;
        }

        internal FinalWrapper CreateCreditAccount(Client client, decimal startBalance)
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
            return account;
        }

        internal Transaction WithdrawMoney(BankAccount account, decimal amount)
        {
            var transaction = new BasicTransaction(_dateTimeProvider.Now(), new WithdrawCommand(amount), account);
            transaction.Apply();
            return transaction;
        }

        internal Transaction TransferMoney(BankAccount fromAccount, BankAccount toAccount, decimal amount)
        {
            var currentDateTime = _dateTimeProvider.Now();
            var withdrawTransaction = new BasicChainedTransaction(currentDateTime, new WithdrawCommand(amount), fromAccount);
            var addTransaction = new StraightChainedTransaction(currentDateTime, new DepositCommand(amount), toAccount, withdrawTransaction);
            withdrawTransaction.Apply();
            return withdrawTransaction;
        }

        internal Transaction DepositMoney(BankAccount account, decimal amount)
        {
            var transaction = new BasicTransaction(_dateTimeProvider.Now(), new DepositCommand(amount), account);
            transaction.Apply();
            return transaction;
        }

        internal void RevertTransaction(Transaction transaction)
        {
            transaction.Revert();
        }

        internal void Subscribe(Client client)
        {
            ArgumentNullException.ThrowIfNull(client, nameof(client));
            if (!_subscribedClients.Contains(client))
                _subscribedClients.Add(client);
        }

        internal void ChangeDepositPercentage(PercentagePlan newPlan)
        {
            ArgumentNullException.ThrowIfNull(newPlan, nameof(newPlan));
            _configuration.DepositAccountConfiguration.PercentagePlan.ChangeConfiguration(newPlan);
            NotifyUsers(NotificationType.Deposit, "Deposit percentage was changed");
        }

        internal void ChangeDebitPercentage(PercentagePlan newPlan)
        {
            ArgumentNullException.ThrowIfNull(newPlan, nameof(newPlan));
            _configuration.DebitAccountConfiguration.PercentagePlan.ChangeConfiguration(newPlan);
            NotifyUsers(NotificationType.Debit, "Debit percentage was changed");
        }

        internal void ChangeCreditLimit(decimal limit)
        {
            _configuration.CreditAccountConfiguration.LimitPlan.Limit = limit;
            NotifyUsers(NotificationType.Credit, "Credit limit was changed");
        }

        internal void ChangeUnconfirmedClientLimit(decimal newLimit)
        {
            _configuration.UnconfirmedClientLimits.WithdrawLimit = newLimit;
            NotifyUsers(NotificationType.All, "Limits for unconfirmed clients was changed");
        }

        internal void PayFees()
        {
            foreach (var account in _accounts)
            {
                var transaction = new BasicTransaction(_dateTimeProvider.Now(), new PayFeesCommand(), account);
                transaction.Apply();
            }
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