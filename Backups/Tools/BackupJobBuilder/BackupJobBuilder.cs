using System;
using System.Collections.Generic;
using Backups.Core.FileArchivers;
using Backups.Core.FileReaders;
using Backups.Core.RestorePoints.RestorePointsCleaners;
using Backups.Core.RestorePoints.RestorePointsLimters;
using Backups.Domain.ConfigProviders;
using Backups.Domain.FileHandlers;
using Backups.Domain.FileReaders;
using Backups.Domain.Models;
using Backups.Domain.RestorePoitnts.RestorePointsCleaners;
using Backups.Domain.RestorePoitnts.RestorePointsLimters;
using Backups.Domain.StorageAlgorithms;
using Backups.Domain.Storages;
using Backups.Tools.Exceptions;
using Backups.Tools.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backups.Tools.BackupJobBuilder
{
    public class BackupJobBuilder
        : ISetNameOrLoadJobBuilder,
          ISetStorageAlgorithmJobBuilder,
          ISetStorageJobBuilder,
          ISetCleanerJobBuilder,
          IFinalJobBuilder
    {
        private readonly ServiceCollection _serviceCollection;
        private string _name;

        public BackupJobBuilder(IConfiguration configuration)
        {
            _serviceCollection = new ServiceCollection();
            _serviceCollection.AddTransient(provider => configuration);
            _serviceCollection.AddTransient<IRestorePointsCleaner, EmptyRestorePointsCleaner>();
            _serviceCollection.AddTransient<IRestorePointsLimiter, EmptyRestorePointsLimiter>();
            _serviceCollection.AddTransient<IFileArchiver, ZipFileArchiver>();
            _serviceCollection.AddTransient<IFileReader, LocalFileReader>();
        }

        public ISetNameOrLoadJobBuilder SetConfigProvider<T>()
            where T : class, IConfigProvider
        {
            _serviceCollection.AddSingleton<IConfigProvider, T>();
            return this;
        }

        List<BackupJob> ISetNameOrLoadJobBuilder.LoadJobs()
        {
            IConfigProvider configProvider = _serviceCollection.BuildServiceProvider().GetService<IConfigProvider>();
            if (configProvider is null)
                throw BackupJobException.ServiceIsMissing(nameof(IConfigProvider));
            return configProvider.LoadJobs();
        }

        ISetStorageAlgorithmJobBuilder ISetNameOrLoadJobBuilder.SetName(string name)
        {
            ArgumentNullException.ThrowIfNull(name, nameof(name));
            _name = name;
            return this;
        }

        ISetStorageJobBuilder ISetStorageAlgorithmJobBuilder.SetStorageAlgorithm<T>()
        {
            _serviceCollection.AddScoped<IStorageAlgorithm, T>();
            return this;
        }

        IFinalJobBuilder ISetStorageJobBuilder.SetStorage<T>()
        {
            _serviceCollection.AddSingleton<IStorage, T>();
            return this;
        }

        ISetCleanerJobBuilder IFinalJobBuilder.SetPointsLimiter<T>()
        {
            _serviceCollection.Remove<IRestorePointsLimiter>();
            _serviceCollection.AddScoped<IRestorePointsLimiter, T>();
            return this;
        }

        IFinalJobBuilder IFinalJobBuilder.SetFileReader<T>()
        {
            _serviceCollection.Remove<IFileReader>();
            _serviceCollection.AddTransient<IFileReader, T>();
            return this;
        }

        IFinalJobBuilder ISetCleanerJobBuilder.SetRestorePointsCleaner<T>()
        {
            _serviceCollection.Remove<IRestorePointsCleaner>();
            _serviceCollection.AddScoped<IRestorePointsCleaner, T>();
            return this;
        }

        BackupJob IFinalJobBuilder.Build()
        {
            return new BackupJob(_name, _serviceCollection.BuildServiceProvider());
        }
    }
}