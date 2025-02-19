namespace CleanArchitectureUtility.Utilities.Auth.ApiAuthentication.Options;

public sealed class ApiAuthenticationOption
{
    public bool Enabled { get; set; } = true;
    public List<ProviderOption> Providers { get; set; } = new List<ProviderOption>();
    public IReadOnlyList<ProviderOption> EnabledProviders => Providers.Where(c => c.Enabled).OrderBy(c => c.Priority).ToList();
    public ProviderOption? DefaultProvider => EnabledProviders.FirstOrDefault();
    public bool Active => Enabled && EnabledProviders.Any();
}