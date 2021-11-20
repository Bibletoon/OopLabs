using System.Collections.Generic;
using Backups.Entities;
using Backups.Entities.Configuration;
using Backups.FileHandlers;
using Backups.FileReaders;
using Backups.Models;
using Backups.RestorePointsCleaners;
using Backups.RestorePointsLimiters;
using Backups.StorageAlgorithms;
using Backups.Storages;
using Backups.Tools.Extensions;
using Backups.Tools.Logger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Backups.Tools.BackupJobBuilder
{
    public class BackupJobBuilder
        : ISetNameJobBuilder,
          ISetStorageJobBuilder,
          ISetStorageAlgorithmJobBuilder,
          ISetLoggerJobBuilder,
          ISetDateTimeProviderJobBuilder,
          ISetRestorePointsLimiterJobBuilder,
          ISetRestorePointsCleanerJobBuilder,
          IFinalJobBuilder
    {
        private ServiceCollection _serviceCollection;
        private List<ServiceInfo> _typedServices;
        private List<ServiceConfigurationInfo> _serviceConfigurations;
        private string _name;

        public BackupJobBuilder()
        {
            _serviceCollection = new ServiceCollection();
            _typedServices = new List<ServiceInfo>();
            _serviceConfigurations = new List<ServiceConfigurationInfo>();
            _serviceCollection.AddScoped<IFileArchiver, ZipFileArchiver>();
            SaveServiceToJobConfig<IFileArchiver, ZipFileArchiver>();
        }

        public ISetNameJobBuilder SetFileReader<T>()
            where T : class, IFileReader
        {
            _serviceCollection.Remove<IFileReader>();
            RemoveServiceFromJobConfig<IFileReader>();
            _serviceCollection.AddScoped<IFileReader, T>();
            SaveServiceToJobConfig<IFileReader, T>();
            return this;
        }

        ISetStorageAlgorithmJobBuilder ISetNameJobBuilder.SetName(string name)
        {
            _name = name;
            return this;
        }

        ISetStorageJobBuilder ISetStorageAlgorithmJobBuilder.SetStorageAlgorithm<T>()
        {
            _serviceCollection.Remove<IStorageAlgorithm>();
            RemoveServiceFromJobConfig<IStorageAlgorithm>();
            _serviceCollection.AddScoped<IStorageAlgorithm, T>();
            SaveServiceToJobConfig<IStorageAlgorithm, T>();
            return this;
        }

        ISetLoggerJobBuilder ISetStorageJobBuilder.SetStorage<T>()
        {
            _serviceCollection.Remove<IStorage>();
            RemoveServiceFromJobConfig<IStorage>();
            _serviceCollection.AddScoped<IStorage, T>();
            SaveServiceToJobConfig<IStorage, T>();
            return this;
        }

        ISetLoggerJobBuilder ISetStorageJobBuilder.SetStorage<T, TConfig>(TConfig config)
            where TConfig : class
        {
            _serviceCollection.Remove<IStorage>();
            RemoveServiceFromJobConfig<IStorage>();
            _serviceCollection.AddScoped<IStorage, T>();
            SaveServiceToJobConfig<IStorage, T>();
            _serviceCollection.Remove<TConfig>();
            RemoveServiceConfigFromJobConfig<TConfig>();
            _serviceCollection.AddSingleton(config);
            SaveServiceConfigToJobConfig(config);
            return this;
        }

        ISetDateTimeProviderJobBuilder ISetLoggerJobBuilder.SetLogger<T>()
        {
            _serviceCollection.Remove<ILogger>();
            RemoveServiceFromJobConfig<ILogger>();
            _serviceCollection.AddScoped<ILogger, T>();
            SaveServiceToJobConfig<ILogger, T>();
            return this;
        }

        ISetDateTimeProviderJobBuilder ISetLoggerJobBuilder.SetLogger<T, TConfiguration>(TConfiguration configuration)
            where TConfiguration : class
        {
            _serviceCollection.Remove<ILogger>();
            RemoveServiceFromJobConfig<ILogger>();
            _serviceCollection.AddScoped<ILogger, T>();
            SaveServiceToJobConfig<ILogger, T>();
            _serviceCollection.Remove<TConfiguration>();
            RemoveServiceConfigFromJobConfig<TConfiguration>();
            _serviceCollection.AddSingleton(configuration);
            SaveServiceConfigToJobConfig(configuration);
            return this;
        }

        ISetRestorePointsLimiterJobBuilder ISetDateTimeProviderJobBuilder.SetDateTimeProvider<T>()
        {
            _serviceCollection.Remove<IDateTimeProvider>();
            RemoveServiceFromJobConfig<IDateTimeProvider>();
            _serviceCollection.AddScoped<IDateTimeProvider, T>();
            SaveServiceToJobConfig<IDateTimeProvider, T>();
            return this;
        }

        ISetRestorePointsCleanerJobBuilder ISetRestorePointsLimiterJobBuilder.SetRestorePointsLimiter<T>()
        {
            _serviceCollection.Remove<IRestorePointsLimiter>();
            RemoveServiceFromJobConfig<IRestorePointsLimiter>();
            _serviceCollection.AddScoped<IRestorePointsLimiter, T>();
            SaveServiceToJobConfig<IRestorePointsLimiter, T>();
            return this;
        }

        ISetRestorePointsCleanerJobBuilder ISetRestorePointsLimiterJobBuilder.SetRestorePointsLimiter<T, TConfig>(TConfig config)
            where TConfig : class
        {
            _serviceCollection.Remove<IRestorePointsLimiter>();
            RemoveServiceFromJobConfig<IRestorePointsLimiter>();
            _serviceCollection.AddScoped<IRestorePointsLimiter, T>();
            SaveServiceToJobConfig<IRestorePointsLimiter, T>();
            _serviceCollection.Remove<TConfig>();
            RemoveServiceConfigFromJobConfig<TConfig>();
            _serviceCollection.AddSingleton(config);
            SaveServiceConfigToJobConfig(config);
            return this;
        }

        IFinalJobBuilder ISetRestorePointsCleanerJobBuilder.SetRestorePointsCleaner<T>()
        {
            _serviceCollection.Remove<IRestorePointsCleaner>();
            RemoveServiceFromJobConfig<IRestorePointsCleaner>();
            _serviceCollection.AddScoped<IRestorePointsCleaner, T>();
            SaveServiceToJobConfig<IRestorePointsCleaner, T>();
            return this;
        }

        IFinalJobBuilder IFinalJobBuilder.SetFileArchiver<T>()
        {
            _serviceCollection.Remove<IFileArchiver>();
            RemoveServiceFromJobConfig<IFileArchiver>();
            _serviceCollection.AddScoped<IFileArchiver, T>();
            SaveServiceToJobConfig<IFileArchiver, T>();
            return this;
        }

        BackupJob IFinalJobBuilder.Build()
        {
            _serviceCollection.AddSingleton(new JobConfiguration(
                                                _name,
                                                new JobServicesConfiguration(_serviceConfigurations, _typedServices),
                                                new List<RestorePointInfo>(),
                                                new List<JobObject>()));
            _serviceCollection.AddSingleton<BackupJob>();
            return _serviceCollection.BuildServiceProvider().GetService<BackupJob>();
        }

        private void SaveServiceToJobConfig<T, TImplementation>()
        {
            _typedServices.Add(new ServiceInfo()
            {
                Type = typeof(T).FullName,
                ImplementationType = typeof(TImplementation).FullName,
            });
        }

        private void SaveServiceConfigToJobConfig<T>(T config)
        {
            _serviceConfigurations.Add(new ServiceConfigurationInfo(config));
        }

        private void RemoveServiceFromJobConfig<T>()
        {
            _typedServices.RemoveAll(s => s.Type == typeof(T).FullName);
        }

        private void RemoveServiceConfigFromJobConfig<T>()
        {
            _serviceConfigurations.RemoveAll(s => s.Type == typeof(T).FullName);
        }
    }
}