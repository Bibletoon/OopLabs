using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Banks;
using Banks.Clients;
using Banks.Commands;
using Banks.Transactions;

namespace Banks.Accounts
{
    public sealed class BaseAccount : BankAccount
    {
        private decimal _balance;
        private Client _client;
        private List<Transaction> _transactionsHistory = new List<Transaction>();

        public BaseAccount(decimal balance, Client client)
        {
            _balance = balance;
            _client = client;
        }

        private BaseAccount()
        {
        }

        public override decimal GetBalance() => _balance;

        public override Guid GetId() => Id;

        public override Client GetClient() => _client;

        public override List<Transaction> GetTransactionsHistory()
            => _transactionsHistory.OrderBy(t => t.TransactionDateTime).ToList();

        internal override void Proceed(Command command)
        {
            command.Execute(this);
        }

        internal override void IncreaseBalance(decimal amount)
        {
            _balance += amount;
        }

        internal override void DecreaseBalance(decimal amount)
        {
            _balance -= amount;
        }

        internal override void SaveTransactionToHistory(Transaction transaction)
        {
            _transactionsHistory.Add(transaction);
        }
    }
}