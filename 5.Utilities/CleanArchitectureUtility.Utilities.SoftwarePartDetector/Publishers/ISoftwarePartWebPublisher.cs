using CleanArchitectureUtility.Utilities.SoftwarePartDetector.DataModel;

namespace CleanArchitectureUtility.Utilities.SoftwarePartDetector.Publishers;

public interface ISoftwarePartPublisher
{
    Task PublishAsync(SoftwarePart softwarePart);
}