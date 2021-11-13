using Banks.Accounts;
using Banks.Commands.CommandType;

namespace Banks.Commands
{
    public class PayFeesCommand : BalanceOperationCommand
    {
        public decimal Fees { get; internal set; } = 0;

        public override void Execute(BankAccount account)
        {
            account.IncreaseBalance(Fees);
        }

        public override void Cancel(BankAccount account)
        {
            account.DecreaseBalance(Fees);
        }

        public override decimal Calculate(decimal amount) => amount + Fees;
    }
}