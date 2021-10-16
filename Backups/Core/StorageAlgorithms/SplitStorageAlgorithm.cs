using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Domain.Entities;
using Backups.Domain.Models;
using Backups.Domain.StorageAlgorithms;

namespace Backups.Core.StorageAlgorithms
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