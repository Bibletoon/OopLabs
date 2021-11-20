namespace BackupsExtra.FileRestorers.Configurations
{
    public class CustomFolderFileRestorerConfig
    {
        public CustomFolderFileRestorerConfig(string folder)
        {
            Folder = folder;
        }

        public string Folder { get; init; }
    }
}