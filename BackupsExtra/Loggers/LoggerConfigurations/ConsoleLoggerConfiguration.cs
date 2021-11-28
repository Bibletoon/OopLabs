namespace BackupsExtra.Loggers.LoggerConfigurations
{
    public class ConsoleLoggerConfiguration
    {
        public ConsoleLoggerConfiguration(bool dateTimeLabel)
        {
            DateTimeLabel = dateTimeLabel;
        }

        public bool DateTimeLabel { get; }
    }
}