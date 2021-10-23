using System;

namespace Backups.Models
{
    public class JobObject
    {
        public JobObject(string path)
        {
            ArgumentNullException.ThrowIfNull(path);
            Path = path;
        }

        public string Path { get; }

        public override bool Equals(object obj)
        {
            if (obj is not JobObject jobObject)
                return false;

            return Equals(jobObject);
        }

        public override int GetHashCode() => Path.GetHashCode();

        protected bool Equals(JobObject other) => Path.Equals(other.Path);
    }
}