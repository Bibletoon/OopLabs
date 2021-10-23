using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Entities;
using Backups.Models;

namespace Backups.StorageAlgorithms
{
    public class SplitStorageAlgorithm : IStorageAlgorithm
    {
        public List<JobsGroup> ProceedFiles(List<JobObject> jobObjects)
        {
            ArgumentNullException.ThrowIfNull(jobObjects, nameof(jobObjects));

            return jobObjects.Select(jo => new JobsGroup(jo)).ToList();
        }
    }
}