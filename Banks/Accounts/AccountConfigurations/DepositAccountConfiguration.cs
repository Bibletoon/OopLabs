using System;
using Banks.Accounts.AccountConfigurations.Plans;
using Banks.Tools;

namespace Banks.Accounts.AccountConfigurations
{
    public class DepositAccountConfiguration
    {
        private Guid _id = Guid.NewGuid();

        public DepositAccountConfiguration(PercentagePlan percentagePlan, TimeLimitPlan timeLimitPlan)
        {
            ArgumentNullException.ThrowIfNull(percentagePlan, nameof(percentagePlan));
            ArgumentNullException.ThrowIfNull(timeLimitPlan, nameof(timeLimitPlan));
            PercentagePlan = percentagePlan;
            TimeLimitPlan = timeLimitPlan;
        }

        private DepositAccountConfiguration()
        {
        }

        public PercentagePlan PercentagePlan { get; internal set; }
        public TimeLimitPlan TimeLimitPlan { get; internal set; }
    }
}