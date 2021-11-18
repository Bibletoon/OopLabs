using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Entities;
using Backups.Models;

namespace Backups.StorageAlgorithms
{
    public class SplitStorageAlgorithm : IStorageAlgorithm
    {
        public List<PackagesGroup> ProceedFiles(List<Package> packages)
        {
            ArgumentNullException.ThrowIfNull(packages, nameof(packages));

            return packages.Select(p => new PackagesGroup(p)).ToList();
        }
    }
}