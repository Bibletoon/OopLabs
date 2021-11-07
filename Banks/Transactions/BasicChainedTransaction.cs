using System;
using Banks.Accounts;
using Banks.Commands;
using Banks.Commands.CommandType;

namespace Banks.Transactions
{
    public class BasicChainedTransaction : ChainedTransaction
    {
        private BalanceOperationCommand _command;
        private BankAccount _account;

        public BasicChainedTransaction(
            DateTime creationDateTime,
            BalanceOperationCommand command,
            BankAccount account,
            ChainedTransaction chainedTransaction = null)
            : base(creationDateTime, chainedTransaction)
        {
            _command = command;
            _account = account;
        }

        private BasicChainedTransaction()
        {
        }

        internal override decimal Calculate(decimal amount) => _command.Calculate(amount);

        protected override void ApplyThis()
        {
            try
            {
                _account.Proceed(_command);
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
                throw new Exception("Can't revert not completed command");

            try
            {
                _command.Cancel(_account);
                Status = TransactionStatus.Canceled;
            }
            catch (Exception e)
            {
                throw new Exception("Can't revert command", e);
            }
        }
    }
}