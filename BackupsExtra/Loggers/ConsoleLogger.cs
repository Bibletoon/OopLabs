using System;
using Backups.Tools;
using Backups.Tools.Logger;
using BackupsExtra.Loggers.LoggerConfigurations;

namespace BackupsExtra.Loggers
{
    public class ConsoleLogger : ILogger
    {
        private readonly ConsoleLoggerConfiguration _configuration;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ConsoleLogger(ConsoleLoggerConfiguration configuration, IDateTimeProvider dateTimeProvider)
        {
            ArgumentNullException.ThrowIfNull(dateTimeProvider, nameof(dateTimeProvider));
            _configuration = configuration;
            _dateTimeProvider = dateTimeProvider;
        }

        public void Log(string message)
        {
            Console.WriteLine(_configuration.DateTimeLabel
                                  ? $"[{_dateTimeProvider.Now().ToShortDateString()}] {message}"
                                  : message);
        }
    }
}