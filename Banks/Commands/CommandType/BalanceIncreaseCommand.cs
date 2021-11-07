namespace Banks.Commands.CommandType
{
    public abstract class BalanceIncreaseCommand : BalanceOperationCommand
    {
        protected BalanceIncreaseCommand(decimal amount)
        {
            Amount = amount;
        }

        public decimal Amount { get; private set; }
    }
}