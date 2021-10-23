using System.Collections.Generic;
using System.Linq;
using Backups.Models;

namespace Backups.Entities
{
    public class JobsGroup
    {
        private readonly List<JobObject> _jobs;

        public JobsGroup(List<JobObject> jobs)
        {
            _jobs = jobs;
        }

        public JobsGroup(JobObject job)
        {
            _jobs = new List<JobObject>() { job };
        }

        public List<JobObject> Jobs => _jobs.ToList();
    }
}