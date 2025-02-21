using Duende.IdentityModel.Client;

namespace CleanArchitectureUtility.Utilities.SoftwarePartDetector.Authentications;

public interface ISoftwarePartAuthentication
{
    Task<TokenResponse> LoginAsync();
}