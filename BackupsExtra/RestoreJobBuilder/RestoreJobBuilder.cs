using System;
using Backups.Tools.Extensions;
using BackupsExtra.FileRestorers;
using BackupsExtra.FileRestorers.Configurations;
using BackupsExtra.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BackupsExtra.RestoreJobBuilder
{
    public class RestoreJobBuilder : ISetFileRestorerJobBuilder, IFinalRestoreJobBuilder
    {
        private readonly ServiceCollection _serviceCollection;

        public RestoreJobBuilder()
        {
            _serviceCollection = new ServiceCollection();
        }

        public ISetFileRestorerJobBuilder LoadJobConfiguration(ConfigurationManager manager, string configurationFilePath)
        {
            manager.LoadJobServices(configurationFilePath, _serviceCollection);
            return this;
        }

        IFinalRestoreJobBuilder ISetFileRestorerJobBuilder.SetFileRestorer<T>()
        {
            _serviceCollection.Remove<IFileRestorer>();
            _serviceCollection.AddScoped<IFileRestorer, T>();
            return this;
        }

        IFinalRestoreJobBuilder ISetFileRestorerJobBuilder.SetFileRestorer<T, TConfiguration>(
            TConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
            _serviceCollection.Remove<IFileRestorer>();
            _serviceCollection.AddScoped<IFileRestorer, T>();
            _serviceCollection.Remove<TConfiguration>();
            _serviceCollection.AddSingleton(configuration);
            return this;
        }

        public RestoreJob Build()
        {
            _serviceCollection.AddSingleton<RestoreJob>();
            return _serviceCollection.BuildServiceProvider().GetService<RestoreJob>();
        }
    }
}