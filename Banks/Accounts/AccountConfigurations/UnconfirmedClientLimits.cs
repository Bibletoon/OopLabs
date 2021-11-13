using System;

namespace Banks.Accounts.AccountConfigurations
{
    public class UnconfirmedClientLimits
    {
        private Guid _id = Guid.NewGuid();

        public UnconfirmedClientLimits(decimal withdrawLimit)
        {
            WithdrawLimit = withdrawLimit;
        }

        private UnconfirmedClientLimits()
        {
        }

        public decimal WithdrawLimit { get; internal set; }
    }
}