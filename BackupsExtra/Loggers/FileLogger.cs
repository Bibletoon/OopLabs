using System;
using System.IO;
using Backups.Tools;
using Backups.Tools.Logger;
using BackupsExtra.Loggers.LoggerConfigurations;

namespace BackupsExtra.Loggers
{
    public class FileLogger : ILogger
    {
        private FileLoggerConfiguration _configuration;
        private IDateTimeProvider _dateTimeProvider;

        public FileLogger(FileLoggerConfiguration configuration, IDateTimeProvider dateTimeProvider)
        {
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
            ArgumentNullException.ThrowIfNull(dateTimeProvider, nameof(dateTimeProvider));
            _configuration = configuration;
            _dateTimeProvider = dateTimeProvider;
        }

        public void Log(string message)
        {
            string logEntry = _configuration.DateTimeLabel ? $"[{_dateTimeProvider.Now().ToShortDateString()}] {message}" : message;
            File.AppendAllText(_configuration.Filename, $"{logEntry}\n");
        }
    }
}