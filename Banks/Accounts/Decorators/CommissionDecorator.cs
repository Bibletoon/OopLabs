using System;
using System.Linq;
using Banks.Accounts.AccountConfigurations.Plans;
using Banks.Commands;
using Banks.Commands.CommandType;
using Banks.Data;
using Banks.Tools;
using Banks.Transactions;

namespace Banks.Accounts.Decorators
{
    public class CommissionDecorator : AccountWrapperBase
    {
        private CommissionPlan _commission;
        private DateTime _lastCommissionDate;
        private decimal _lastBalance;
        private IDateTimeProvider _dateTimeProvider;

        public CommissionDecorator(BankAccount account, CommissionPlan commission, IDateTimeProvider dateTimeProvider)
            : base(account)
        {
            _lastBalance = account.GetBalance();
            _commission = commission;
            _dateTimeProvider = dateTimeProvider;
            _lastCommissionDate = dateTimeProvider.Now();
        }

        private CommissionDecorator(BanksDbContext dbContext)
        {
            _dateTimeProvider = dbContext.GetDateTimeProvider();
        }

        internal override void Proceed(Command command)
        {
            if (command is PayFeesCommand pfs)
                pfs.Fees = CalculateCommission();
            base.Proceed(command);
        }

        private decimal CalculateCommission()
        {
            var transactions = GetTransactionsHistory()
                               .SkipWhile(t => t.TransactionDateTime < _lastCommissionDate)
                               .Where(t => t.Status == TransactionStatus.Completed).ToList();

            var percentageDateTime = _lastCommissionDate;
            decimal commission = 0;

            while (percentageDateTime < _dateTimeProvider.Now())
            {
                var transactionsToApply = transactions.TakeWhile(t => t.TransactionDateTime <= percentageDateTime).ToList();

                foreach (var transaction in transactionsToApply)
                {
                    _lastBalance = transaction.Calculate(_lastBalance);
                    transactions.Remove(transaction);
                }

                if (percentageDateTime.Day == 1 && _lastBalance < 0)
                {
                    commission -= _commission.Commission;
                    _lastBalance -= _commission.Commission;
                }

                percentageDateTime = percentageDateTime.AddDays(1);
            }

            _lastBalance -= commission;
            _lastCommissionDate = percentageDateTime;
            return commission;
        }
    }
}