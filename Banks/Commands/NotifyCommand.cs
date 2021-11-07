using Banks.Accounts;
using Banks.Accounts.Decorators;
using Banks.Enums;

namespace Banks.Commands
{
    public class NotifyCommand : Command
    {
        public NotifyCommand(NotificationType notificationType, string message)
        {
            NotificationType = notificationType;
            Message = message;
        }

        public NotificationType NotificationType { get; private init; }
        public string Message { get; private init; }

        public override void Execute(BankAccount account)
        {
            account.GetClient().Notify(Message);
        }
    }
}