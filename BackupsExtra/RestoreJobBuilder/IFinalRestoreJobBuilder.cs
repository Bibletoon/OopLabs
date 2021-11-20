using BackupsExtra.Models;

namespace BackupsExtra.RestoreJobBuilder
{
    public interface IFinalRestoreJobBuilder
    {
        RestoreJob Build();
    }
}