using System.Security.AccessControl;
using Backups.Models;

namespace Backups.Entities
{
    public class JobObjectInfo
    {
        public JobObjectInfo(JobObject jobObject, Package package)
        {
            JobObject = jobObject;
            Package = package;
        }

        public JobObject JobObject { get; }
        public Package Package { get; }
    }
}