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

        public BackupJobBuilder()
        {
            _serviceCollection = new ServiceCollection();
            _serviceCollection.AddScoped<IFileArchiver, ZipFileArchiver>();
        }

        public ISetNameJobBuilder SetFileReader<T>()
            where T : class, IFileReader
        {
            _serviceCollection.Remove<IFileReader>();
            _serviceCollection.AddScoped<IFileReader, T>();
            return this;
        }

        ISetStorageAlgorithmJobBuilder ISetNameJobBuilder.SetName(string name)
        {
            _serviceCollection.Remove<string>();
            _serviceCollection.AddSingleton<string>(name);
            return this;
        }

        ISetStorageJobBuilder ISetStorageAlgorithmJobBuilder.SetStorageAlgorithm<T>()
        {
            _serviceCollection.Remove<IStorageAlgorithm>();
            _serviceCollection.AddScoped<IStorageAlgorithm, T>();
            return this;
        }

        ISetLoggerJobBuilder ISetStorageJobBuilder.SetStorage<T>()
        {
            _serviceCollection.Remove<IStorage>();
            _serviceCollection.AddScoped<IStorage, T>();
            return this;
        }

        ISetLoggerJobBuilder ISetStorageJobBuilder.SetStorage<T, TConfig>(TConfig config)
            where TConfig : class
        {
            _serviceCollection.Remove<IStorage>();
            _serviceCollection.AddScoped<IStorage, T>();
            _serviceCollection.Remove<TConfig>();
            _serviceCollection.AddSingleton(config);
            return this;
        }

        ISetDateTimeProviderJobBuilder ISetLoggerJobBuilder.SetLogger<T>()
        {
            _serviceCollection.Remove<ILogger>();
            _serviceCollection.AddScoped<ILogger, T>();
            return this;
        }

        ISetDateTimeProviderJobBuilder ISetLoggerJobBuilder.SetLogger<T, TConfiguration>(TConfiguration configuration)
            where TConfiguration : class
        {
            _serviceCollection.Remove<ILogger>();
            _serviceCollection.AddScoped<ILogger, T>();
            _serviceCollection.Remove<TConfiguration>();
            _serviceCollection.AddSingleton(configuration);
            return this;
        }

        ISetRestorePointsLimiterJobBuilder ISetDateTimeProviderJobBuilder.SetDateTimeProvider<T>()
        {
            _serviceCollection.Remove<IDateTimeProvider>();
            _serviceCollection.AddScoped<IDateTimeProvider, T>();
            return this;
        }

        ISetRestorePointsCleanerJobBuilder ISetRestorePointsLimiterJobBuilder.SetRestorePointsCleaner<T>()
        {
            _serviceCollection.Remove<IRestorePointsLimiter>();
            _serviceCollection.AddScoped<IRestorePointsLimiter, T>();
            return this;
        }

        ISetRestorePointsCleanerJobBuilder ISetRestorePointsLimiterJobBuilder.SetRestorePointsCleaner<T, TConfig>(TConfig config)
            where TConfig : class
        {
            _serviceCollection.Remove<IRestorePointsLimiter>();
            _serviceCollection.AddScoped<IRestorePointsLimiter, T>();
            _serviceCollection.Remove<TConfig>();
            _serviceCollection.AddSingleton(config);
            return this;
        }

        IFinalJobBuilder ISetRestorePointsCleanerJobBuilder.SetRestorePointsCleaner<T>()
        {
            _serviceCollection.Remove<IRestorePointsCleaner>();
            _serviceCollection.AddScoped<IRestorePointsCleaner, T>();
            return this;
        }

        IFinalJobBuilder IFinalJobBuilder.SetFileArchiver<T>()
        {
            _serviceCollection.Remove<IFileArchiver>();
            _serviceCollection.AddScoped<IFileArchiver, T>();
            return this;
        }

        BackupJob IFinalJobBuilder.Build()
        {
            _serviceCollection.AddSingleton<BackupJob>();
            return _serviceCollection.BuildServiceProvider().GetService<BackupJob>();
        }
    }
}