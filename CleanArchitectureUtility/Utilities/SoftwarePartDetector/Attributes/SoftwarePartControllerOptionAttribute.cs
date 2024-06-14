namespace CleanArchitectureUtility.Utilities.SoftwarePartDetector.Attributes;

public class SoftwarePartControllerOptionAttribute : Attribute
{
    public SoftwarePartControllerOptionAttribute(string? service , string? module = null)
    {
        if (string.IsNullOrWhiteSpace(service))
            throw new ArgumentNullException(nameof(service));

        Module = module;
        Service = service;
    }

    public string? Module { get; }

    public string? Service { get; }
}