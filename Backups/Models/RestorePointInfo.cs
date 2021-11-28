using System;
using System.Collections.Generic;

namespace Backups.Models
{
    public class RestorePointInfo
    {
        private readonly List<JobObject> _objects;

        public RestorePointInfo(DateTime creationDate, string jobName, string name, List<JobObject> objects)
        {
            ArgumentNullException.ThrowIfNull(creationDate, nameof(creationDate));
            ArgumentNullException.ThrowIfNull(objects, nameof(objects));
            ArgumentNullException.ThrowIfNull(jobName, nameof(jobName));
            ArgumentNullException.ThrowIfNull(name, nameof(name));
            _objects = objects;
            JobName = jobName;
            Name = name;
            CreationDate = creationDate;
        }

        public DateTime CreationDate { get; }
        public string JobName { get; }
        public string Name { get; }
        public IReadOnlyList<JobObject> Objects => _objects.AsReadOnly();
    }
}