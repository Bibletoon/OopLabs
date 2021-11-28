namespace Backups.TcpServer.Common.Commands
{
    public class ReadCommand
    {
        public ReadCommand(string folderName)
        {
            FolderName = folderName;
        }

        public string FolderName { get; }
    }
}