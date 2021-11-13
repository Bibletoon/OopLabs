using System;

namespace Banks.UI.Commands.CommandArguments
{
    public class SingleAccountBalanceChangeCommandArguments
    {
        public SingleAccountBalanceChangeCommandArguments(Guid accountId, decimal amount)
        {
            AccountId = accountId;
            Amount = amount;
        }

        public Guid AccountId { get; init; }
        public decimal Amount { get; init; }
    }
}