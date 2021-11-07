using System;

namespace Banks.Transactions
{
    public abstract class ChainedTransaction : Transaction
    {
        public ChainedTransaction(DateTime creationDateTime, ChainedTransaction parentTransaction = null)
            : base(creationDateTime)
        {
            Status = TransactionStatus.Created;
            ParentTransaction = parentTransaction;
            parentTransaction?.SetChainedTransaction(this);
        }

        protected ChainedTransaction()
        {
        }

        public ChainedTransaction ParentTransaction { get; private set; }
        public ChainedTransaction ChildTransaction { get; private set; }

        internal override void Revert()
        {
            RevertThis();
            ChildTransaction?.RevertFromParent();
            ParentTransaction?.RevertFromChild();
        }

        internal override void Apply()
        {
            if (Status != TransactionStatus.Created)
                throw new Exception("Transaction already applied");

            ApplyThis();

            if (Status == TransactionStatus.Failed)
            {
                ChildTransaction?.RevertFromParent();
                ParentTransaction?.RevertFromChild();
                return;
            }

            ParentTransaction?.ApplyFromChild();
            ChildTransaction?.ApplyFromParent();
        }

        protected void SetChainedTransaction(ChainedTransaction transaction)
        {
            if (ChildTransaction is not null)
                throw new Exception("ChainedTransaction already exists");

            ChildTransaction = transaction;
        }

        protected void RevertFromChild()
        {
            RevertThis();
            ParentTransaction?.RevertFromChild();
        }

        protected void RevertFromParent()
        {
            RevertThis();
            ChildTransaction?.RevertFromParent();
        }

        protected void ApplyFromChild()
        {
            ApplyThis();

            if (Status == TransactionStatus.Failed)
            {
                ChildTransaction?.RevertFromParent();
                return;
            }

            ParentTransaction?.ApplyFromChild();
        }

        protected void ApplyFromParent()
        {
            ApplyThis();

            if (Status == TransactionStatus.Failed)
            {
                ParentTransaction?.RevertFromChild();
                return;
            }

            ChildTransaction?.ApplyFromChild();
        }

        protected abstract void ApplyThis();

        protected abstract void RevertThis();
    }
}