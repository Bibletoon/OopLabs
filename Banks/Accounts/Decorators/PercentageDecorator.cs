using System;
using System.Linq;
using Banks.Accounts.AccountConfigurations.Plans;
using Banks.Commands;
using Banks.Data;
using Banks.Tools;
using Banks.Transactions;

namespace Banks.Accounts.Decorators
{
    public class PercentageDecorator : AccountWrapperBase
    {
        private PercentagePlan _percentagePlan;
        private decimal _startSum;
        private DateTime _lastPercentageDate;
        private decimal _lastBalance;
        private decimal _assessedPercents;
        private IDateTimeProvider _dateTimeProvider;

        public PercentageDecorator(BankAccount account, PercentagePlan plan, IDateTimeProvider dateTimeProvider)
            : base(account)
        {
            _percentagePlan = plan;
            _dateTimeProvider = dateTimeProvider;
            _startSum = account.GetBalance();
            _assessedPercents = 0;
            _lastPercentageDate = dateTimeProvider.Now();
            _lastBalance = account.GetBalance();
        }

        private PercentageDecorator(BanksDbContext dbContext)
        {
            _dateTimeProvider = dbContext.GetDateTimeProvider();
        }

        internal override void Proceed(Command command)
        {
            if (command is PayFeesCommand pfc)
                pfc.Fees = CalculatePercentage();
            base.Proceed(command);
        }

        private decimal CalculatePercentage()
        {
            var transactions = GetTransactionsHistory()
                               .SkipWhile(t => t.TransactionDateTime < _lastPercentageDate)
                               .Where(t => t.Status == TransactionStatus.Completed).ToList();

            var percentageDateTime = _lastPercentageDate;
            decimal percents = 0;

            while (percentageDateTime < _dateTimeProvider.Now())
            {
                var transactionsToApply = transactions.TakeWhile(t => t.TransactionDateTime <= percentageDateTime).ToList();

                foreach (var transaction in transactionsToApply)
                {
                    _lastBalance = transaction.Calculate(_lastBalance);
                    transactions.Remove(transaction);
                }

                if (percentageDateTime.Day == 1)
                {
                    percents += _assessedPercents;
                    _lastBalance += _assessedPercents;
                    _assessedPercents = 0;
                }

                _assessedPercents += _lastBalance * _percentagePlan.GetPercents(_startSum) / 100 / 365;

                percentageDateTime = percentageDateTime.AddDays(1);
            }

            _lastBalance -= percents;
            _lastPercentageDate = percentageDateTime;
            return percents;
        }
    }
}