using System;

namespace Banks.UI.Commands
{
    public class BaseCommand : ICommand
    {
        private readonly Func<CommandResult> _func;

        public BaseCommand(Func<CommandResult> action)
        {
            _func = action;
        }

        public CommandResult Execute()
        {
            return _func();
        }
    }
}