using System;

namespace BackupsExtra.Loggers.LoggerConfigurations
{
    public class FileLoggerConfiguration
    {
        public FileLoggerConfiguration(string filename, bool dateTimeLabel)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("Filename must be provided");
            Filename = filename;
            DateTimeLabel = dateTimeLabel;
        }

        public string Filename { get; }
        public bool DateTimeLabel { get; }
    }
}