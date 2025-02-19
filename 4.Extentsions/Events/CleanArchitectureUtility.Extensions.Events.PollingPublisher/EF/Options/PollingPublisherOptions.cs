namespace CleanArchitectureUtility.Extensions.Events.PollingPublisher.EF.Options
{
    public class PollingPublisherOptions
    {
        public int SendInterval { get; set; } = 1000;
        public int ExceptionInterval { get; set; } = 10000;
        public int SendCount { get; set; } = 100;
        public string ApplicationName { get; set; }
    }
}