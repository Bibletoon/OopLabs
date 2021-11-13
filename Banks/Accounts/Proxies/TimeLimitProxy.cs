using System;
using Banks.Accounts.AccountConfigurations.Plans;
using Banks.Commands;
using Banks.Commands.CommandType;
using Banks.Data;
using Banks.Tools;
using Banks.Tools.Exceptions;

namespace Banks.Accounts.Proxies
{
    public class TimeLimitProxy : AccountWrapperBase
    {
        private DateTime _creationDateTime;
        private TimeLimitPlan _limitPlan;
        private IDateTimeProvider _dateTimeProvider;

        public TimeLimitProxy(BankAccount account, IDateTimeProvider dateTimeProvider, TimeLimitPlan timeLimitPlan)
            : base(account)
        {
            _dateTimeProvider = dateTimeProvider;
            _creationDateTime = _dateTimeProvider.Now();
            _limitPlan = timeLimitPlan;
        }

        private TimeLimitProxy(BanksDbContext dbContext)
        {
            _dateTimeProvider = dbContext.GetDateTimeProvider();
        }

        internal override void Proceed(Command command)
        {
            if (command is BalanceDecreaseCommand
                && !TimeLimitPassed())
                throw new AccountLimitException("Time limit not yet passed");
            base.Proceed(command);
        }

        private bool TimeLimitPassed() => _creationDateTime.Add(_limitPlan.TimeLimit) < _dateTimeProvider.Now();
    }
}