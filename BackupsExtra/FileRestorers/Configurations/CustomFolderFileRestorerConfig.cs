using System;

namespace BackupsExtra.FileRestorers.Configurations
{
    public class CustomFolderFileRestorerConfig
    {
        public CustomFolderFileRestorerConfig(string folder)
        {
            if (string.IsNullOrEmpty(folder))
                throw new ArgumentException("Invalid folder name");
            Folder = folder;
        }

        public string Folder { get; init; }
    }
}