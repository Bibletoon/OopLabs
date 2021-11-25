using System;

namespace Backups.Entities.Configuration
{
    public class ServiceConfigurationInfo
    {
        public ServiceConfigurationInfo(Type type, object configurationObject)
        {
            Type = type;
            ConfigurationObject = configurationObject;
        }

        public ServiceConfigurationInfo(object configurationObject)
        {
            ArgumentNullException.ThrowIfNull(configurationObject, nameof(configurationObject));
            Type = configurationObject.GetType();
            ConfigurationObject = configurationObject;
        }

        private ServiceConfigurationInfo()
        {
        }

        public Type Type { get; init; }
        public object ConfigurationObject { get; init; }
    }
}