using System;

namespace Banks.UI.Commands
{
    public class BaseParametrizedCommand<TArgument> : IParameterizedCommand<TArgument>
    {
        private readonly Func<TArgument, CommandResult> _func;

        public BaseParametrizedCommand(Func<TArgument, CommandResult> func)
        {
            _func = func;
        }

        public CommandResult Execute(TArgument argument)
        {
            return _func(argument);
        }
    }
}