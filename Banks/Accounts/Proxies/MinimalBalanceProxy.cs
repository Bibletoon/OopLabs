using System;
using Banks.Accounts.AccountConfigurations.Plans;
using Banks.Commands;
using Banks.Commands.CommandType;
using Banks.Tools.Exceptions;

namespace Banks.Accounts.Proxies
{
    public class MinimalBalanceProxy : AccountWrapperBase
    {
        private LimitPlan _minimalAmount;

        public MinimalBalanceProxy(BankAccount account, LimitPlan minimalAmount)
            : base(account)
        {
            _minimalAmount = minimalAmount;
        }

        private MinimalBalanceProxy()
        {
        }

        internal override void Proceed(Command command)
        {
            if (command is BalanceDecreaseCommand brc
                && !CanWithdrawMoney(brc))
                throw new AccountLimitException("Can't take more than minimal balance limit");
            base.Proceed(command);
        }

        private bool CanWithdrawMoney(BalanceDecreaseCommand brc) => GetBalance() - brc.Amount + _minimalAmount.Limit >= 0;
    }
}