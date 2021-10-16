using System;
using System.Collections.Generic;
using Backups.Domain.ConfigProviders;
using Backups.Domain.RestorePoitnts.RestorePointsCleaners;
using Backups.Domain.RestorePoitnts.RestorePointsLimters;
using Backups.Domain.Storages;
using Backups.Tools.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Backups.Domain.Models
{
    public class BackupJob
    {
        private readonly List<JobObject> _jobObjects;
        private readonly List<RestorePointInfo> _restorePoitnts;
        private readonly ServiceProvider _serviceProvider;
        private string _name;

        internal BackupJob(string name, ServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(name, nameof(name));
            ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));
            _jobObjects = new List<JobObject>();
            _restorePoitnts = new List<RestorePointInfo>();
            _name = name;
            _serviceProvider = serviceProvider;
        }

        public void AddJobObject(JobObject jobObject)
        {
            if (_jobObjects.Contains(jobObject))
                throw new BackupJobException("Such job object already added.");
            _jobObjects.Add(jobObject);
        }

        public void RemoveJobObject(JobObject jobObject)
        {
            _jobObjects.Remove(jobObject);
        }

        public void Run()
        {
            IStorage storage = _serviceProvider.GetService<IStorage>();
            if (storage is null)
                throw BackupJobException.ServiceIsMissing(nameof(IStorage));

            IRestorePointsLimiter limiter = _serviceProvider.GetService<IRestorePointsLimiter>();
            if (limiter is null)
                throw BackupJobException.ServiceIsMissing(nameof(IRestorePointsCleaner));

            List<RestorePointInfo> pointsToClear = limiter.GetPointsToClear(_restorePoitnts, _jobObjects);

            if (pointsToClear.Count != 0)
            {
                IRestorePointsCleaner cleaner = _serviceProvider.GetService<IRestorePointsCleaner>();
                if (cleaner is null)
                    throw BackupJobException.ServiceIsMissing(typeof(IRestorePointsCleaner).FullName);

                storage.CleanRestorePoints(pointsToClear, cleaner);
            }

            RestorePointInfo info = storage.CreateBackup(_name, _jobObjects);
            _restorePoitnts.Add(info);
            IConfigProvider configProvider = _serviceProvider.GetService<IConfigProvider>();
            if (configProvider is null)
                throw BackupJobException.ServiceIsMissing(nameof(IConfigProvider));
            configProvider.SaveJob(this);
        }
    }
}