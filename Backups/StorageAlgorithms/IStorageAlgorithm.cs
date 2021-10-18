using System.Collections.Generic;
using Backups.Entities;
using Backups.Models;

namespace Backups.StorageAlgorithms
{
    public interface IStorageAlgorithm
    {
        List<JobsGroup> ProceedFiles(List<JobObject> jobObjects);
    }
}