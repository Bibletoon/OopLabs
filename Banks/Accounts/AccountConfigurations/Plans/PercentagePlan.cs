using System;
using System.Collections.Generic;
using System.Linq;

namespace Banks.Accounts.AccountConfigurations.Plans
{
    public class PercentagePlan
    {
        private List<PercentageConfiguration> _percentageConfiguration;
        private Guid _id = Guid.NewGuid();

        internal PercentagePlan(List<PercentageConfiguration> percentageConfiguration)
        {
            _percentageConfiguration = percentageConfiguration;
        }

        private PercentagePlan()
        {
        }

        public decimal GetPercents(decimal sum)
        {
            return _percentageConfiguration.FindLast(x => x.Sum < sum).Percents;
        }

        internal void ChangeConfiguration(PercentagePlan plan)
        {
            _percentageConfiguration = plan._percentageConfiguration.ToList();
        }
    }
}