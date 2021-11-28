using System.Collections.Generic;
using System.Linq;
using Backups.Models;

namespace Backups.Entities
{
    public class PackagesGroup
    {
        private readonly List<Package> _packages;

        public PackagesGroup(List<Package> packages)
        {
            _packages = packages;
        }

        public PackagesGroup(Package package)
        {
            _packages = new List<Package>() { package };
        }

        public List<Package> Packages => _packages.ToList();
    }
}