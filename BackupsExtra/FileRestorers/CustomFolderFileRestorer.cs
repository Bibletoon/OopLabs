using System;
using System.Collections.Generic;
using System.IO;
using Backups.Entities;
using BackupsExtra.FileRestorers.Configurations;

namespace BackupsExtra.FileRestorers
{
    public class CustomFolderFileRestorer : IFileRestorer
    {
        private readonly CustomFolderFileRestorerConfig _config;

        public CustomFolderFileRestorer(CustomFolderFileRestorerConfig config)
        {
            ArgumentNullException.ThrowIfNull(config, nameof(config));
            _config = config;
        }

        public void RestoreFiles(List<JobObjectInfo> objects)
        {
            if (!Directory.Exists(_config.Folder))
                Directory.CreateDirectory(_config.Folder);

            foreach (var objectInfo in objects)
            {
                using var file =
                    File.OpenWrite(Path.Combine(_config.Folder, Path.GetFileName(objectInfo.JobObject.Path)));

                objectInfo.Package.Content.Seek(0, SeekOrigin.Begin);
                objectInfo.Package.Content.CopyTo(file);
            }
        }
    }
}