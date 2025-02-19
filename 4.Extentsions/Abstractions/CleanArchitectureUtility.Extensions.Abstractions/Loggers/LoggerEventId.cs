namespace CleanArchitectureUtility.Extensions.Abstractions.Loggers;
public class LoggerEventId
{
    public const int PerformanceMeasurement = 1001;

    public const int DomainValidationException = 2001;

    public const int CommandValidation = 3001;
    public const int QueryValidation = 3002;
    public const int EventValidation = 3003;
}