using Backups.Tools.Extensions;
using BackupsExtra.FileRestorers;
using BackupsExtra.FileRestorers.Configurations;
using BackupsExtra.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BackupsExtra.RestoreJobBuilder
{
    public class RestoreJobBuilder : ISetFileRestorerJobBilder, IFinalRestoreJobBuilder
    {
        private ServiceCollection _serviceCollection;

        public RestoreJobBuilder()
        {
            _serviceCollection = new ServiceCollection();
        }

        public ISetFileRestorerJobBilder LoadJobConfiguration(ConfigurationManager manager, TypeLocator typeLocator, string configurationFilePath)
        {
            manager.LoadJobServices(typeLocator, configurationFilePath, _serviceCollection);
            return this;
        }

        IFinalRestoreJobBuilder ISetFileRestorerJobBilder.SetFileRestorer<T>(
            CustomFolderFileRestorerConfig customFolderFileRestorerConfig)
        {
            _serviceCollection.Remove<IFileRestorer>();
            _serviceCollection.AddScoped<IFileRestorer, T>();
            return this;
        }

        IFinalRestoreJobBuilder ISetFileRestorerJobBilder.SetFileRestorer<T, TConfiguration>(
            TConfiguration configuration)
        {
            _serviceCollection.Remove<IFileRestorer>();
            _serviceCollection.AddScoped<IFileRestorer, T>();
            _serviceCollection.Remove<TConfiguration>();
            _serviceCollection.AddSingleton<TConfiguration>(configuration);
            return this;
        }

        public RestoreJob Build()
        {
            _serviceCollection.AddSingleton<RestoreJob>();
            return _serviceCollection.BuildServiceProvider().GetService<RestoreJob>();
        }
    }
}