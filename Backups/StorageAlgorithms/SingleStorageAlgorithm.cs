using System;
using System.Collections.Generic;
using Backups.Entities;
using Backups.Models;

namespace Backups.StorageAlgorithms
{
    public class SingleStorageAlgorithm : IStorageAlgorithm
    {
        public List<JobsGroup> ProceedFiles(List<JobObject> jobObjects)
        {
            ArgumentNullException.ThrowIfNull(jobObjects, nameof(jobObjects));

            return new List<JobsGroup>() { new JobsGroup(jobObjects) };
        }
    }
}