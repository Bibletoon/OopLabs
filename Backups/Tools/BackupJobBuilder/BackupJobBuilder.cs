using System;
using Backups.Core.FileArchivers;
using Backups.Core.FileReaders;
using Backups.Domain.FileHandlers;
using Backups.Domain.FileReaders;
using Backups.Domain.Models;
using Backups.Domain.StorageAlgorithms;
using Backups.Domain.Storages;
using Backups.Tools.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backups.Tools.BackupJobBuilder
{
    public class BackupJobBuilder
        : ISetNameJobBuilder,
          ISetStorageJobBuilder,
          ISetStorageAlgorithmJobBuilder,
          IFinalJobBuilder
    {
        private readonly ServiceCollection _serviceCollection;
        private string _name;

        public BackupJobBuilder(IConfiguration configuration)
        {
            _serviceCollection = new ServiceCollection();
            _serviceCollection.AddTransient(provider => configuration);
            _serviceCollection.AddTransient<IFileArchiver, ZipFileArchiver>();
        }

        public ISetNameJobBuilder SetFileReader<T>()
            where T : class, IFileReader
        {
            _serviceCollection.AddTransient<IFileReader, T>();
            return this;
        }

        ISetStorageAlgorithmJobBuilder ISetNameJobBuilder.SetName(string name)
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

        IFinalJobBuilder IFinalJobBuilder.SetFileReader<T>()
        {
            _serviceCollection.Remove<IFileReader>();
            _serviceCollection.AddTransient<IFileReader, T>();
            return this;
        }

        BackupJob IFinalJobBuilder.Build()
        {
            return new BackupJob(_name, _serviceCollection.BuildServiceProvider());
        }
    }
}