using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Backups.Entities;
using BackupsExtra.Tools;

namespace BackupsExtra.FileRestorers
{
    public class OriginalPlaceFileRestorer : IFileRestorer
    {
        public void RestoreFiles(List<JobObjectInfo> objects)
        {
            if (objects.Any(o => Directory.Exists(Path.GetDirectoryName(o.JobObject.Path))))
                throw new RestoreException("Can't restore some files because their directory doesn't exists");
            foreach (var objectInfo in objects)
            {
                using var file = File.OpenWrite(objectInfo.JobObject.Path);
                objectInfo.Package.Content.Seek(0, SeekOrigin.Begin);
                objectInfo.Package.Content.CopyTo(file);
            }
        }
    }
}