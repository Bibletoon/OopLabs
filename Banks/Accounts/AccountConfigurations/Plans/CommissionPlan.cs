using System;

namespace Banks.Accounts.AccountConfigurations.Plans
{
    public class CommissionPlan
    {
        private Guid _id = Guid.NewGuid();

        public CommissionPlan(decimal commission)
        {
            Commission = commission;
        }

        private CommissionPlan()
        {
        }

        public decimal Commission { get; internal set; }
    }
}