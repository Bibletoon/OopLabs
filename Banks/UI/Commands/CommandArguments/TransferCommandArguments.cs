using System;

namespace Banks.UI.Commands.CommandArguments
{
    public class TransferCommandArguments
    {
        public TransferCommandArguments(Guid accountFromId, Guid accountToId, decimal amount)
        {
            AccountFromId = accountFromId;
            AccountToId = accountToId;
            Amount = amount;
        }

        public Guid AccountFromId { get; init; }
        public Guid AccountToId { get; init; }
        public decimal Amount { get; init; }
    }
}