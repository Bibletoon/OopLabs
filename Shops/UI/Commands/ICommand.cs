using System;

namespace Shops.UI.Commands
{
    public interface ICommand
    {
        CommandResult Execute();
    }
}