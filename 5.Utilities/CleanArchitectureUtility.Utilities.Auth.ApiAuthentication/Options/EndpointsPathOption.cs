namespace CleanArchitectureUtility.Utilities.Auth.ApiAuthentication.Options;

public class EndpointsPathOption
{
    public string AuthorizeEndpoint { get; set; } = "/connect/authorize";
    public string TokenEndpoint { get; set; } = "/connect/token"!;
    public string UserInfoEndpoint { get; set; } = "/connect/userInfo"!;
    public string IntrospectionEndpoint { get; set; } = "/connect/introspect"!;
}