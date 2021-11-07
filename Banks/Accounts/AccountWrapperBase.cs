using System;
using System.Collections.Generic;
using Banks.Banks;
using Banks.Clients;
using Banks.Commands;
using Banks.Transactions;

namespace Banks.Accounts
{
    public abstract class AccountWrapperBase : BankAccount
    {
        private BankAccount _account;

        protected AccountWrapperBase()
        {
        }

        protected AccountWrapperBase(BankAccount account)
        {
            _account = account;
        }

        public override Guid GetId() => _account.GetId();

        public override decimal GetBalance() => _account.GetBalance();

        public override Client GetClient() => _account.GetClient();

        public override List<Transaction> GetTransactionsHistory() => _account.GetTransactionsHistory();

        internal override void Proceed(Command command)
        {
            _account.Proceed(command);
        }

        internal sealed override void IncreaseBalance(decimal amount)
        {
            _account.IncreaseBalance(amount);
        }

        internal sealed override void DecreaseBalance(decimal amount)
        {
            _account.DecreaseBalance(amount);
        }

        internal override void SaveTransactionToHistory(Transaction transaction)
        {
            _account.SaveTransactionToHistory(transaction);
        }
    }
}