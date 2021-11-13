using System;

namespace Banks.Accounts.AccountConfigurations.Plans
{
    public class TimeLimitPlan
    {
        private Guid _id = Guid.NewGuid();

        public TimeLimitPlan(TimeSpan timeLimit)
        {
            TimeLimit = timeLimit;
        }

        private TimeLimitPlan()
        {
        }

        public TimeSpan TimeLimit { get; set; }
    }
}