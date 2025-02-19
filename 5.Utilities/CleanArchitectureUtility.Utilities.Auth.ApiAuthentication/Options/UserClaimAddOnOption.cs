namespace CleanArchitectureUtility.Utilities.Auth.ApiAuthentication.Options;

public class UserClaimAddOnOption
{
    public string Type { get; set; } = default!;
    public string Value { get; set; } = default!;
    public string? ValueType { get; set; }
    public string? Issuer { get; set; }
    public string? OriginalIssuer { get; set; }
}