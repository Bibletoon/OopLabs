using System;
using System.Collections.Generic;
using Banks.Banks;
using Banks.Clients;
using Banks.Commands;
using Banks.Transactions;

namespace Banks.Accounts
{
    public abstract class BankAccount
    {
        protected Guid Id { get; set; } = Guid.NewGuid();
        public abstract decimal GetBalance();
        public abstract Guid GetId();
        public abstract Client GetClient();
        public abstract List<Transaction> GetTransactionsHistory();
        internal abstract void Proceed(Command command);
        internal abstract void IncreaseBalance(decimal amount);
        internal abstract void DecreaseBalance(decimal amount);
        internal abstract void SaveTransactionToHistory(Transaction transaction);
    }
}