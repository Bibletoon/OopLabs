using System;
using Banks.Accounts.AccountConfigurations.Plans;

namespace Banks.Accounts.AccountConfigurations
{
    public class DebitAccountConfiguration
    {
        private Guid _id = Guid.NewGuid();

        public DebitAccountConfiguration(PercentagePlan percentagePlan)
        {
            ArgumentNullException.ThrowIfNull(percentagePlan, nameof(percentagePlan));
            PercentagePlan = percentagePlan;
        }

        private DebitAccountConfiguration()
        {
        }

        public PercentagePlan PercentagePlan { get; internal set; }
    }
}