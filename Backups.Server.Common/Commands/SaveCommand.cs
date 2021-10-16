namespace Backups.TcpServer.Common.Commands
{
    public class SaveCommand
    {
        public SaveCommand(string backupPath, int count, string archiveFormat)
        {
            BackupPath = backupPath;
            Count = count;
            ArchiveFormat = archiveFormat;
        }

        public string BackupPath { get; }
        public int Count { get; }
        public string ArchiveFormat { get; }
    }
}