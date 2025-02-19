namespace CleanArchitectureUtility.Extensions.UsersManagement.Options;

public sealed class UserManagementOptions
{
    public Guid DefaultUserId { get; set; } = Guid.Empty;
    public string DefaultUserAgent { get; set; } = "Unknown";
    public string DefaultUserIp { get; set; } = "0.0.0.0";
    public string DefaultUsername { get; set; } = "UnknownUserName";
    public string DefaultFirstName { get; set; } = "UnknownFirstName";
    public string DefaultLastName { get; set; } = "UnknownLastName";
}