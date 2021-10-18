namespace Backups.TcpServer.Common
{
    public class CommandInfo
    {
        public CommandInfo(string commandName)
        {
            CommandName = commandName;
        }

        public string CommandName { get; }
    }
}