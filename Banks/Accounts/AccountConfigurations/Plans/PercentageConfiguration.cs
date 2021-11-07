using System;

namespace Banks.Accounts.AccountConfigurations.Plans
{
    public class PercentageConfiguration
    {
        private Guid _id = Guid.NewGuid();
        public decimal Sum { get; internal set; }
        public decimal Percents { get; internal set; }
    }
}