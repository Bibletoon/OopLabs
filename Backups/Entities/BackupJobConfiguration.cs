using System.Collections.Generic;
using Backups.Models;

namespace Backups.Entities
{
    public class BackupJobConfiguration
    {
        public string Name { get; init; }
        public string FileReader { get; init; }
        public string FileArchiver { get; init; }
        public string StorageAlgorithm { get; init; }
        public string Storage { get; init; }
        public IReadOnlyList<JobObject> JobObjects { get; init; }
        public IReadOnlyList<RestorePointInfo> RestorePoints { get; init; }
    }
}