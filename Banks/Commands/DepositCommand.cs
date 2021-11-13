using Banks.Accounts;
using Banks.Commands.CommandType;

namespace Banks.Commands
{
    public class DepositCommand : BalanceIncreaseCommand
    {
        public DepositCommand(decimal amount)
            : base(amount)
        {
        }

        public override void Execute(BankAccount account)
        {
            account.IncreaseBalance(Amount);
        }

        public override void Cancel(BankAccount account)
        {
            account.DecreaseBalance(Amount);
        }

        public override decimal Calculate(decimal amount) => amount + Amount;
    }
}