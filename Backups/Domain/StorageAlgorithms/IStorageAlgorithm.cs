using System.Collections.Generic;
using Backups.Domain.Entities;
using Backups.Domain.Models;

namespace Backups.Domain.StorageAlgorithms
{
    public interface IStorageAlgorithm
    {
        List<JobsGroup> ProceedFiles(List<JobObject> jobObjects);
    }
}