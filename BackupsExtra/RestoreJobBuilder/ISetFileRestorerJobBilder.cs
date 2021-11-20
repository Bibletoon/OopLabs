using BackupsExtra.FileRestorers;
using BackupsExtra.FileRestorers.Configurations;

namespace BackupsExtra.RestoreJobBuilder
{
    public interface ISetFileRestorerJobBilder
    {
        IFinalRestoreJobBuilder SetFileRestorer<T>(CustomFolderFileRestorerConfig customFolderFileRestorerConfig)
            where T : class, IFileRestorer;

        IFinalRestoreJobBuilder SetFileRestorer<T, TConfiguration>(TConfiguration configuration)
            where T : class, IFileRestorer
            where TConfiguration : class;
    }
}