using System;

namespace Shops.UI.Commands
{
    public class CommandResult
    {
        public CommandResult(bool isSuccess, string message = "")
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public bool IsSuccess { get; }
        public string Message { get; }

        public static CommandResult Success() => new CommandResult(true);

        public static CommandResult Fail(string message) => new CommandResult(false, message);
    }
}