namespace CleanArchitectureUtility.Core.Abstractions.Logger;
public class LogEventId
{
    public const int PerformanceMeasurement = 1001;

    public const int DomainValidationException = 2001;

    public const int CommandValidation = 3001;
    public const int QueryValidation = 3002;
    public const int EventValidation = 3003;
}