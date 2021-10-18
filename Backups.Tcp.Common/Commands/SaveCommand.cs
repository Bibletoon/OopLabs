namespace Backups.TcpServer.Common.Commands
{
    public class SaveCommand
    {
        public SaveCommand(string backupPath, int count)
        {
            BackupPath = backupPath;
            Count = count;
        }

        public string BackupPath { get; }
        public int Count { get; }
    }
}