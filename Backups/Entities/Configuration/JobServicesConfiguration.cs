using System.Collections.Generic;

namespace Backups.Entities.Configuration
{
    public class JobServicesConfiguration
    {
        public JobServicesConfiguration()
        {
        }

        public JobServicesConfiguration(IReadOnlyList<ServiceConfigurationInfo> servicesConfigurations, IReadOnlyList<ServiceInfo> services)
        {
            ServicesConfigurations = servicesConfigurations;
            Services = services;
        }

        public IReadOnlyList<ServiceConfigurationInfo> ServicesConfigurations { get; init; }
        public IReadOnlyList<ServiceInfo> Services { get; init; }
    }
}