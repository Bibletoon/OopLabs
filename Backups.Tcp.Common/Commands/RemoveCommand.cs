namespace Backups.TcpServer.Common.Commands
{
    public class RemoveCommand
    {
        public RemoveCommand(string folderName)
        {
            FolderName = folderName;
        }

        public string FolderName { get; }
    }
}