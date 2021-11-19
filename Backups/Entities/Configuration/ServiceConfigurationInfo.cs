using System;

namespace Backups.Entities.Configuration
{
    public class ServiceConfigurationInfo
    {
        public ServiceConfigurationInfo(string type, object configurationObject)
        {
            Type = type;
            ConfigurationObject = configurationObject;
        }

        public ServiceConfigurationInfo(object configurationObject)
        {
            ArgumentNullException.ThrowIfNull(configurationObject, nameof(configurationObject));
            Type = configurationObject.GetType().FullName;
            ConfigurationObject = configurationObject;
        }

        private ServiceConfigurationInfo()
        {
        }

        public string Type { get; init; }
        public object ConfigurationObject { get; init; }
    }
}