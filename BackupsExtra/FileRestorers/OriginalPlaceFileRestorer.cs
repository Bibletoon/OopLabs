using System.Collections.Generic;
using System.IO;
using Backups.Entities;

namespace BackupsExtra.FileRestorers
{
    public class OriginalPlaceFileRestorer : IFileRestorer
    {
        public void RestoreFiles(List<JobObjectInfo> objects)
        {
            foreach (var objectInfo in objects)
            {
                using var file = File.OpenWrite(objectInfo.JobObject.Path);
                objectInfo.Package.Content.Seek(0, SeekOrigin.Begin);
                objectInfo.Package.Content.CopyTo(file);
            }
        }
    }
}