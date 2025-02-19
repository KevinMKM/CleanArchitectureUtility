namespace CleanArchitectureUtility.Extensions.Abstractions.Loggers;

public interface IScopeInformation
{
    Dictionary<string, string> HostScopeInfo { get; }
    Dictionary<string, string> RequestScopeInfo { get; }
}