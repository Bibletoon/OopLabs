using BackupsExtra.FileRestorers;
using BackupsExtra.FileRestorers.Configurations;

namespace BackupsExtra.RestoreJobBuilder
{
    public interface ISetFileRestorerJobBuilder
    {
        IFinalRestoreJobBuilder SetFileRestorer<T>()
            where T : class, IFileRestorer;

        IFinalRestoreJobBuilder SetFileRestorer<T, TConfiguration>(TConfiguration configuration)
            where T : class, IFileRestorer
            where TConfiguration : class;
    }
}