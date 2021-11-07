using System.Linq;
using Banks.Commands;
using Banks.Enums;

namespace Banks.Accounts.Decorators
{
    public class NotificationHandlerDecorator : AccountWrapperBase
    {
        private NotificationType[] _notificationTypes;

        public NotificationHandlerDecorator(BankAccount account, params NotificationType[] notificationTypes)
            : base(account)
        {
            _notificationTypes = notificationTypes;
        }

        private NotificationHandlerDecorator()
        {
        }

        internal override void Proceed(Command command)
        {
            if (command is not NotifyCommand nc
                || _notificationTypes.Contains<NotificationType>(nc.NotificationType))
            {
                base.Proceed(command);
            }
        }
    }
}