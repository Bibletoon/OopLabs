using System;
using System.IO;
using Backups.Entities.Configuration;
using Backups.Models;
using Backups.Tools;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BackupsExtra
{
    public class ConfigurationManager
    {
        private static readonly Lazy<ConfigurationManager> _instance =
            new Lazy<ConfigurationManager>(() => new ConfigurationManager());

        private ConfigurationManager()
        {
        }

        public static ConfigurationManager Instance => _instance.Value;

        public void Save(JobConfiguration configuration, string filename)
        {
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
            ArgumentNullException.ThrowIfNull(filename, nameof(filename));
            var stringConfiguration = JsonConvert.SerializeObject(configuration);
            File.WriteAllText(filename, stringConfiguration);
        }

        public BackupJob LoadBackupJob(TypeLocator typeLocator, string filename)
        {
            ArgumentNullException.ThrowIfNull(typeLocator, nameof(typeLocator));
            if (!File.Exists(filename))
                throw new FileNotFoundException("No such configuration file");
            var stringConfiguration = File.ReadAllText(filename);
            var configuration = JsonConvert.DeserializeObject<JobConfiguration>(stringConfiguration);

            if (configuration is null)
                throw new Exception("Invalid configuration file");
            var serviceCollection = new ServiceCollection();

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
            serviceCollection.AddSingleton<BackupJob>();
            return serviceCollection.BuildServiceProvider().GetService<BackupJob>();
        }
    }
}