using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Accounts;
using Banks.Accounts.AccountConfigurations.Plans;
using Banks.Builders;
using Banks.Clients;
using Banks.Data;
using Banks.Enums;
using Banks.Tools.Exceptions;
using Banks.Transactions;

namespace Banks.Banks
{
    public class CentralBank
    {
        private BanksDbContext _dbContext;
        private List<Bank> _banks;

        public CentralBank(BanksDbContext dbContext)
        {
            ArgumentNullException.ThrowIfNull(dbContext);
            _dbContext = dbContext;
            _dbContext.SetCentralBank(this);
            _dbContext.LoadData();
            _banks = _dbContext.Banks.ToList();
        }

        public Bank CreateBank(BankConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            var bank = new Bank(_dbContext, configuration);
            _banks.Add(bank);
            _dbContext.Banks.Add(bank);
            _dbContext.SaveChanges();
            return bank;
        }

        public BankAccount CreateDepositAccount(Guid bankId, Client client, decimal startBalance)
        {
            ArgumentNullException.ThrowIfNull(client, nameof(client));
            ArgumentNullException.ThrowIfNull(bankId, nameof(bankId));
            var bank = _dbContext.Banks.Find(bankId);

            if (bank is null)
            {
                throw new NotFoundException("Bank not found");
            }

            var account = bank.CreateDepositAccount(client, startBalance);
            _dbContext.Accounts.Add(account);
            _dbContext.SaveChanges();
            return account;
        }

        public BankAccount CreateDebitAccount(Guid bankId, Client client, decimal startBalance)
        {
            ArgumentNullException.ThrowIfNull(client, nameof(client));
            ArgumentNullException.ThrowIfNull(bankId, nameof(bankId));
            var bank = _dbContext.Banks.Find(bankId);

            if (bank is null)
            {
                throw new NotFoundException("Bank not found");
            }

            var account = bank.CreateDebitAccount(client, startBalance);
            _dbContext.Accounts.Add(account);
            _dbContext.SaveChanges();
            return account;
        }

        public BankAccount CreateCreditAccount(Guid bankId, Client client, decimal startBalance)
        {
            ArgumentNullException.ThrowIfNull(client, nameof(client));
            ArgumentNullException.ThrowIfNull(bankId, nameof(bankId));
            var bank = _dbContext.Banks.Find(bankId);

            if (bank is null)
            {
                throw new NotFoundException("Bank not found");
            }

            var account = bank.CreateCreditAccount(client, startBalance);
            _dbContext.Accounts.Add(account);
            _dbContext.SaveChanges();
            return account;
        }

        public Transaction WithdrawMoney(Guid bankId, Guid accountId, decimal amount)
        {
            ArgumentNullException.ThrowIfNull(bankId);
            ArgumentNullException.ThrowIfNull(accountId);
            var bank = _dbContext.Banks.Find(bankId);

            if (bank is null)
            {
                throw new NotFoundException("Bank not found");
            }

            var account = bank.FindAccount(accountId);

            if (account is null)
                throw new NotFoundException("Account not found");

            var transaction = bank.WithdrawMoney(account, amount);
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();
            return transaction;
        }

        public Transaction TransferMoney(Guid bankId, Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var bank = _dbContext.Banks.Find(bankId);

            if (bank is null)
            {
                throw new NotFoundException("Bank not found");
            }

            var fromAccount = bank.FindAccount(fromAccountId);

            if (fromAccount is null)
                throw new NotFoundException("From account not found");

            var toAccount = FindAccount(toAccountId);

            if (toAccount is null)
                throw new NotFoundException("To account not fount");

            var transaction = bank.TransferMoney(fromAccount, toAccount, amount);

            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();
            return transaction;
        }

        public Transaction DepositMoney(Guid bankId, Guid accountId, decimal amount)
        {
            ArgumentNullException.ThrowIfNull(bankId);
            ArgumentNullException.ThrowIfNull(accountId);
            var bank = _dbContext.Banks.Find(bankId);

            if (bank is null)
            {
                throw new NotFoundException("Bank not found");
            }

            var account = bank.FindAccount(accountId);

            if (account is null)
                throw new NotFoundException("Account not found");

            var transaction = bank.DepositMoney(account, amount);
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();
            return transaction;
        }

        public void RevertTransaction(Guid bankId, Guid transactionId)
        {
            ArgumentNullException.ThrowIfNull(bankId);
            var bank = _dbContext.Banks.Find(bankId);

            if (bank is null)
            {
                throw new NotFoundException("Bank not found");
            }

            var transaction = bank.FindTransaction(transactionId);

            bank.RevertTransaction(transaction);
            _dbContext.Transactions.Update(transaction);
            _dbContext.SaveChanges();
        }

        public void ChangeDepositPercentage(Guid bankId, PercentagePlan newPlan)
        {
            ArgumentNullException.ThrowIfNull(newPlan, nameof(newPlan));
            var bank = _dbContext.Banks.Find(bankId);

            if (bank is null)
            {
                throw new NotFoundException("Bank not found");
            }

            bank.ChangeDepositPercentage(newPlan);
            _dbContext.Banks.Update(bank);
            _dbContext.SaveChanges();
        }

        public void ChangeDebitPercentage(Guid bankId, PercentagePlan newPlan)
        {
            ArgumentNullException.ThrowIfNull(newPlan, nameof(newPlan));
            var bank = _dbContext.Banks.Find(bankId);

            if (bank is null)
            {
                throw new NotFoundException("Bank not found");
            }

            bank.ChangeDebitPercentage(newPlan);
            _dbContext.Banks.Update(bank);
            _dbContext.SaveChanges();
        }

        public void ChangeCreditLimit(Guid bankId, decimal limit)
        {
            var bank = _dbContext.Banks.Find(bankId);

            if (bank is null)
            {
                throw new NotFoundException("Bank not found");
            }

            bank.ChangeCreditLimit(limit);
            _dbContext.Banks.Update(bank);
            _dbContext.SaveChanges();
        }

        public void ChangeUnconfirmedClientLimit(Guid bankId, decimal newLimit)
        {
            var bank = _dbContext.Banks.Find(bankId);

            if (bank is null)
            {
                throw new NotFoundException("Bank not found");
            }

            bank.ChangeUnconfirmedClientLimit(newLimit);
            _dbContext.Banks.Update(bank);
            _dbContext.SaveChanges();
        }

        public void NotifyAboutFeePayment()
        {
            foreach (var bank in _banks)
            {
                bank.PayFees();
                bank.GetAccounts().ForEach(a =>
                                           {
                                               _dbContext.Accounts.Update(a);
                                               _dbContext.SaveChanges();
                                           });
            }
        }

        internal BankAccount FindAccount(Guid id)
        {
            return _dbContext.Banks.ToList().Select(bank => bank.FindAccount(id)).FirstOrDefault(account => account is not null);
        }
    }
}