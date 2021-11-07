using Banks.Accounts;

namespace Banks.Commands.CommandType
{
    public abstract class BalanceOperationCommand : Command
    {
        public abstract void Cancel(BankAccount account);
        public abstract decimal Calculate(decimal amount);
    }
}