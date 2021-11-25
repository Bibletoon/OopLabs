using System;
using System.IO;
using Backups.Entities.Configuration;
using Backups.Models;
using BackupsExtra.Tools;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BackupsExtra
{
    public class ConfigurationManager
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            TypeNameHandling = TypeNameHandling.All,
        };

        public void Save(JobConfiguration configuration, string filename)
        {
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
            ArgumentNullException.ThrowIfNull(filename, nameof(filename));
            var stringConfiguration = JsonConvert.SerializeObject(configuration, Settings);
            File.WriteAllText(filename, stringConfiguration);
        }

        public BackupJob LoadBackupJob(string filename)
        {
            var serviceCollection = new ServiceCollection();

            LoadJobServices(filename, serviceCollection);

            serviceCollection.AddSingleton<BackupJob>();
            return serviceCollection.BuildServiceProvider().GetService<BackupJob>();
        }

        internal void LoadJobServices(string filename, ServiceCollection serviceCollection)
        {
            ArgumentNullException.ThrowIfNull(serviceCollection, nameof(serviceCollection));
            if (!File.Exists(filename))
                throw new FileNotFoundException("No such configuration file");
            var stringConfiguration = File.ReadAllText(filename);
            JobConfiguration configuration;

            try
            {
                configuration = JsonConvert.DeserializeObject<JobConfiguration>(stringConfiguration, Settings);
            }
            catch (Exception e)
            {
                throw new JobConfigurationException("Invalid configuration file", e);
            }

            if (configuration is null)
                throw new JobConfigurationException("Invalid configuration file");

            foreach (var service in configuration.ServicesConfiguration.Services)
            {
                serviceCollection.AddScoped(service.Type, service.ImplementationType);
            }

            foreach (var serviceConfiguration in configuration.ServicesConfiguration.ServicesConfigurations)
            {
                serviceCollection.AddSingleton(serviceConfiguration.Type, serviceConfiguration.ConfigurationObject);
            }

            serviceCollection.AddSingleton(configuration);
        }
    }
}