namespace CleanArchitectureUtility.Core.Contract.ApplicationServices.Common;

public interface IApplicationServiceResult
{
    IEnumerable<string> Messages { get; }
    ApplicationServiceStatus Status { get; set; }
}