using Banks.Accounts;
using Banks.Commands.CommandType;

namespace Banks.Commands
{
    public class WithdrawCommand : BalanceDecreaseCommand
    {
        public WithdrawCommand(decimal amount)
            : base(amount)
        {
        }

        public override void Execute(BankAccount account)
        {
            account.DecreaseBalance(Amount);
        }

        public override void Cancel(BankAccount account)
        {
            account.IncreaseBalance(Amount);
        }

        public override decimal Calculate(decimal amount) => amount - Amount;
    }
}