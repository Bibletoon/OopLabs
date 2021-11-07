using System;
using Banks.Accounts;
using Banks.Accounts.AccountConfigurations;
using Banks.Accounts.AccountConfigurations.Plans;
using Banks.Accounts.Decorators;
using Banks.Accounts.Proxies;
using Banks.Banks;
using Banks.Clients;
using Banks.Enums;
using Banks.Tools;

namespace Banks.Builders
{
    internal class BankAccountBuilder
    {
        private BankAccount _bankAccount;

        public BankAccountBuilder(decimal startBalance, Client client)
        {
            _bankAccount = new BaseAccount(startBalance, client);
        }

        public BankAccountBuilder AddNotificationHandler(params NotificationType[] notificationType)
        {
            _bankAccount = new NotificationHandlerDecorator(_bankAccount, notificationType);
            return this;
        }

        public BankAccountBuilder AddTimeLimit(IDateTimeProvider dateTimeProvider, TimeLimitPlan limit)
        {
            _bankAccount = new TimeLimitProxy(_bankAccount, dateTimeProvider, limit);
            return this;
        }

        public BankAccountBuilder SetMinimalBalance(LimitPlan limit)
        {
            _bankAccount = new MinimalBalanceProxy(_bankAccount, limit);
            return this;
        }

        public BankAccountBuilder AddPercentage(IDateTimeProvider dateTimeProvider, PercentagePlan plan)
        {
            _bankAccount = new PercentageDecorator(_bankAccount, plan, dateTimeProvider);
            return this;
        }

        public BankAccountBuilder AddCommission(CommissionPlan commission, IDateTimeProvider dateTimeProvider)
        {
            _bankAccount = new CommissionDecorator(_bankAccount, commission, dateTimeProvider);
            return this;
        }

        public BankAccountBuilder AddLimitsForUnconfirmedClient(UnconfirmedClientLimits limit)
        {
            _bankAccount = new UnconfirmedClientProxy(_bankAccount, limit);
            return this;
        }

        public FinalWrapper Build()
        {
            var b = new FinalWrapper(_bankAccount);
            return b;
        }
    }
}