using System;
using System.Collections.Generic;
using Backups.Domain.Entities;
using Backups.Domain.Models;
using Backups.Domain.StorageAlgorithms;

namespace Backups.Core.StorageAlgorithms
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