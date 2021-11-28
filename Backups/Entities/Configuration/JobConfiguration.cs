using System.Collections.Generic;
using Backups.Models;

namespace Backups.Entities.Configuration
{
    public class JobConfiguration
    {
        public JobConfiguration(string name, JobServicesConfiguration servicesConfiguration, IReadOnlyList<RestorePointInfo> restorePoints, IReadOnlyList<JobObject> jobObjects)
        {
            Name = name;
            ServicesConfiguration = servicesConfiguration;
            RestorePoints = restorePoints;
            JobObjects = jobObjects;
        }

        public string Name { get; set; }
        public JobServicesConfiguration ServicesConfiguration { get; init; }
        public IReadOnlyList<RestorePointInfo> RestorePoints { get; init; }
        public IReadOnlyList<JobObject> JobObjects { get; init; }
    }
}