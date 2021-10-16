using System;
using System.Collections.Generic;

namespace Backups.Domain.Models
{
    public class RestorePointInfo
    {
        private List<JobObject> _objects;

        public RestorePointInfo(DateTime creationDate, List<JobObject> objects)
        {
            ArgumentNullException.ThrowIfNull(creationDate, nameof(creationDate));
            ArgumentNullException.ThrowIfNull(objects, nameof(objects));
            _objects = objects;
            CreationDate = creationDate;
        }

        public DateTime CreationDate { get; }
        public IReadOnlyList<JobObject> Objects => _objects.AsReadOnly();
    }
}