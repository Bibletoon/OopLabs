using System;

namespace Banks.Accounts.AccountConfigurations.Plans
{
    public class LimitPlan
    {
        private Guid _id = Guid.NewGuid();

        public LimitPlan(decimal limit)
        {
            Limit = limit;
        }

        private LimitPlan()
        {
        }

        public decimal Limit { get; internal set; }
    }
}