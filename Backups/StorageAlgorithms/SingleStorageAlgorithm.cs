using System;
using System.Collections.Generic;
using Backups.Entities;
using Backups.Models;

namespace Backups.StorageAlgorithms
{
    public class SingleStorageAlgorithm : IStorageAlgorithm
    {
        public List<PackagesGroup> ProceedFiles(List<Package> packages)
        {
            ArgumentNullException.ThrowIfNull(packages, nameof(packages));

            return new List<PackagesGroup>() { new PackagesGroup(packages) };
        }
    }
}