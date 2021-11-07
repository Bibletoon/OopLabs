using System;
using Banks.Accounts.AccountConfigurations.Plans;

namespace Banks.Accounts.AccountConfigurations
{
    public class CreditAccountConfiguration
    {
        private Guid _id = Guid.NewGuid();

        public CreditAccountConfiguration(CommissionPlan commissionPlan, LimitPlan limitPlan)
        {
            ArgumentNullException.ThrowIfNull(commissionPlan, nameof(commissionPlan));
            ArgumentNullException.ThrowIfNull(limitPlan, nameof(limitPlan));
            CommissionPlan = commissionPlan;
            LimitPlan = limitPlan;
        }

        private CreditAccountConfiguration()
        {
        }

        public CommissionPlan CommissionPlan { get; internal set; }
        public LimitPlan LimitPlan { get; internal set; }
    }
}