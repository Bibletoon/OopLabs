using System;

namespace Banks.Commands.CommandType
{
    public abstract class BalanceDecreaseCommand : BalanceOperationCommand
    {
        protected BalanceDecreaseCommand(decimal amount)
        {
            Amount = amount;
        }

        public decimal Amount { get; private init; }
    }
}