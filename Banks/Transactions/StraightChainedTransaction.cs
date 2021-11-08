using System;
using System.Transactions;
using Banks.Accounts;
using Banks.Commands;
using Banks.Commands.CommandType;
using Banks.Tools.Exceptions;
using TransactionException = Banks.Tools.Exceptions.TransactionException;

namespace Banks.Transactions
{
    public class StraightChainedTransaction : ChainedTransaction
    {
        private BalanceOperationCommand _command;
        private BankAccount _account;

        public StraightChainedTransaction(
            DateTime creationDateTime,
            BalanceOperationCommand command,
            BankAccount account,
            ChainedTransaction chainedTransaction = null)
            : base(creationDateTime, chainedTransaction)
        {
            _command = command;
            _account = account;
        }

        private StraightChainedTransaction()
        {
        }

        internal override decimal Calculate(decimal amount) => _command.Calculate(amount);

        protected override void ApplyThis()
        {
            try
            {
                _command.Execute(_account);
                Status = TransactionStatus.Completed;
            }
            catch (Exception e)
            {
                Message = e.Message;
                Status = TransactionStatus.Failed;
            }

            _account.SaveTransactionToHistory(this);
        }

        protected override void RevertThis()
        {
            if (Status is TransactionStatus.Created or TransactionStatus.Canceled)
            {
                Status = TransactionStatus.Canceled;
                return;
            }

            if (Status != TransactionStatus.Completed)
                throw new TransactionException("Can't revert not completed command");

            try
            {
                _command.Cancel(_account);
                Status = TransactionStatus.Canceled;
            }
            catch (Exception e)
            {
                throw new TransactionException("Can't revert command", e);
            }
        }
    }
}