using System;
using System.Collections.Generic;
using Banks.Accounts.AccountConfigurations.Plans;

namespace Banks.Builders
{
    public class PercentagePlanBuilder
    {
        private readonly List<PercentageConfiguration> _percentageConfiguration;
        private decimal _previousSum;

        public PercentagePlanBuilder(decimal basePercent)
        {
            _percentageConfiguration = new List<PercentageConfiguration>();
            _previousSum = 0;
            _percentageConfiguration.Add(new PercentageConfiguration
            {
                Sum = 0,
                Percents = basePercent,
            });
        }

        public PercentagePlanBuilder SetPercentageForMoreThan(decimal sum, decimal percents)
        {
            if (sum <= _previousSum)
                throw new ArgumentOutOfRangeException(nameof(sum));

            if (percents <= 0)
                throw new ArgumentOutOfRangeException(nameof(percents));

            _percentageConfiguration.Add(new PercentageConfiguration
            {
                Sum = sum,
                Percents = percents,
            });
            _previousSum = sum;
            return this;
        }

        public PercentagePlan Build()
        {
            return new PercentagePlan(_percentageConfiguration);
        }
    }
}