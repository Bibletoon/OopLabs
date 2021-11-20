using System;
using System.IO;
using Backups.Entities.Configuration;
using Backups.Models;
using Backups.Tools;
using BackupsExtra.Tools;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BackupsExtra
{
    public class ConfigurationManager
    {
        public void Save(JobConfiguration configuration, string filename)
        {
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
            ArgumentNullException.ThrowIfNull(filename, nameof(filename));
            var stringConfiguration = JsonConvert.SerializeObject(configuration);
            File.WriteAllText(filename, stringConfiguration);
        }

        public BackupJob LoadBackupJob(TypeLocator typeLocator, string filename)
        {
            var serviceCollection = new ServiceCollection();

            LoadJobServices(typeLocator, filename, serviceCollection);

            serviceCollection.AddSingleton<BackupJob>();
            return serviceCollection.BuildServiceProvider().GetService<BackupJob>();
        }

        internal void LoadJobServices(TypeLocator typeLocator, string filename, ServiceCollection serviceCollection)
        {
            ArgumentNullException.ThrowIfNull(typeLocator, nameof(typeLocator));
            ArgumentNullException.ThrowIfNull(serviceCollection, nameof(serviceCollection));
            if (!File.Exists(filename))
                throw new FileNotFoundException("No such configuration file");
            var stringConfiguration = File.ReadAllText(filename);
            JobConfiguration configuration;

            try
            {
                configuration = JsonConvert.DeserializeObject<JobConfiguration>(stringConfiguration);
            }
            catch (Exception e)
            {
                throw new JobConfigurationException("Invalid configuration file", e);
            }

            if (configuration is null)
                throw new JobConfigurationException("Invalid configuration file");

            foreach (var service in configuration.ServicesConfiguration.Services)
            {
                var type = typeLocator.GetType(service.Type);
                var implementationType = typeLocator.GetType(service.ImplementationType);
                serviceCollection.AddScoped(type, implementationType);
            }

            foreach (var serviceConfiguration in configuration.ServicesConfiguration.ServicesConfigurations)
            {
                var type = typeLocator.GetType(serviceConfiguration.Type);
                var deserializedConfig = serviceConfiguration.ConfigurationObject as JObject;
                serviceCollection.AddSingleton(type, deserializedConfig.ToObject(type));
            }

            serviceCollection.AddSingleton(configuration);
        }
    }
}