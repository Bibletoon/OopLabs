using System;
using System.Transactions;
using Banks.Accounts;
using Banks.Commands;
using Banks.Commands.CommandType;

namespace Banks.Transactions
{
    public class BasicTransaction : Transaction
    {
        private BalanceOperationCommand _command;
        private BankAccount _account;

        public BasicTransaction(DateTime transactionDateTime, BalanceOperationCommand command, BankAccount account)
            : base(transactionDateTime)
        {
            Status = TransactionStatus.Created;
            _command = command;
            _account = account;
        }

        private BasicTransaction()
        {
        }

        internal override void Revert()
        {
            if (Status != TransactionStatus.Completed)
                throw new TransactionException("Transaction can't be reverted");

            try
            {
                _command.Cancel(_account);
                Status = TransactionStatus.Canceled;
            }
            catch (Exception e)
            {
                throw new TransactionException("Transaction can't be canceled", e);
            }
        }

        internal override void Apply()
        {
            if (Status != TransactionStatus.Created)
                throw new TransactionException("Transaction can't be run");
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

        internal override decimal Calculate(decimal amount) => _command.Calculate(amount);
    }
}