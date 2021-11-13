using System;

namespace Banks.Transactions
{
    public abstract class Transaction
    {
        protected Transaction(DateTime transactionDateTime)
        {
            TransactionDateTime = transactionDateTime;
        }

        protected Transaction()
        {
        }

        public DateTime TransactionDateTime { get; internal set; }
        public Guid Id { get; internal set; } = Guid.NewGuid();
        public TransactionStatus Status { get; internal set; }
        public string Message { get; set; } = string.Empty;
        internal abstract void Revert();
        internal abstract void Apply();
        internal abstract decimal Calculate(decimal amount);
    }
}