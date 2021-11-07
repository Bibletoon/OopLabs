using System;
using Banks.Accounts.AccountConfigurations;
using Banks.Commands;
using Banks.Commands.CommandType;
using Banks.Tools.Exceptions;

namespace Banks.Accounts.Proxies
{
    public class UnconfirmedClientProxy : AccountWrapperBase
    {
        private UnconfirmedClientLimits _limit;

        public UnconfirmedClientProxy(BankAccount account, UnconfirmedClientLimits limit)
            : base(account)
        {
            _limit = limit;
        }

        private UnconfirmedClientProxy()
        {
        }

        internal override void Proceed(Command command)
        {
            if (!GetClient().IsConfirmed
                && command is BalanceDecreaseCommand brc
                && !CanWithdrawMoney(brc))
                throw new AccountLimitException("Can't withdraw more than limit");
            base.Proceed(command);
        }

        private bool CanWithdrawMoney(BalanceDecreaseCommand brc) => brc.Amount <= _limit.WithdrawLimit;
    }
}