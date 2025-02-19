using System.Security.Claims;
using CleanArchitectureUtility.Utilities.Auth.ApiAuthentication.Options;

namespace CleanArchitectureUtility.Utilities.Auth.ApiAuthentication.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static bool HasSubClaim(this ClaimsPrincipal? principal, string userIdentifierClaimType)
        => principal?.Claims.Any(c => c.Type.Equals(userIdentifierClaimType)) ?? false;

    public static ClaimsIdentity CreateClaimsIdentity(this ClaimsPrincipal? principal, List<Claim> claims)
    {
        var currentClaims = principal?.Claims?.ToList();
        var newClaims = GetNotExist(currentClaims, claims);
        var identity = principal?.Identities.FirstOrDefault();
        return new ClaimsIdentity(newClaims, identity?.AuthenticationType, identity?.NameClaimType, identity?.RoleClaimType);
    }

    public static ClaimsPrincipal? ClonePrincipalWithConvertedClaims(this ClaimsPrincipal? principal, ProviderOption provider)
    {
        if (provider.UserClaimRules.Any(rule => string.IsNullOrWhiteSpace(rule.Source) || string.IsNullOrWhiteSpace(rule.Destination)))
            throw new ArgumentNullException("Source or Destination cannot be null or white-space in UserClaimRule");

        if (provider.UserClaimAddOns.Any(addOn => string.IsNullOrWhiteSpace(addOn.Type) || string.IsNullOrWhiteSpace(addOn.Value)))
            throw new ArgumentNullException("Type or Value cannot be null or white-space in UserClaimAddon");

        if (principal == null)
            return null;

        var clone = principal.Clone();
        List<Claim> claims = new();
        claims.AddRange(provider.UserClaimRulesProcessor(clone.Claims.ToList()));
        claims.AddRange(provider.UserClaimAddOnsProcessor());
        var authenticationType = clone.Identities.FirstOrDefault()?.AuthenticationType;
        var nameType = clone.Identities.FirstOrDefault()?.NameClaimType;
        var roleType = clone.Identities.FirstOrDefault()?.RoleClaimType;
        ClaimsIdentity claimsIdentity = new(claims, authenticationType, nameType, roleType);
        return new ClaimsPrincipal(claimsIdentity);
    }

    private static List<Claim> UserClaimRulesProcessor(this ProviderOption provider, List<Claim> currentClaims)
    {
        List<Claim> newClaims = new();
        provider.UserClaimRules.ForEach(rule =>
        {
            var currentClaim = currentClaims.FirstOrDefault(claim => claim.Type.Equals(rule.Source));
            if (currentClaim == null)
                return;
            var mappedClaim = new Claim(rule.Destination, currentClaim.Value, currentClaim.ValueType, currentClaim.Issuer, currentClaim.OriginalIssuer, currentClaim.Subject);
            newClaims.Add(mappedClaim);
            if (!rule.RemoveSource)
                newClaims.Add(currentClaim);
        });

        return newClaims;
    }

    private static List<Claim> UserClaimAddOnsProcessor(this ProviderOption provider)
        => provider.UserClaimAddOns.Select(addOn => new Claim(addOn.Type,
            addOn.Value,
            addOn.ValueType,
            addOn.Issuer,
            addOn.OriginalIssuer)).ToList();

    private static List<Claim> GetNotExist(List<Claim> current, List<Claim> target)
        => target.Where(claim => !current.Any(currentClaim => currentClaim.Type.Equals(claim.Type) && currentClaim.Value.Equals(claim.Value))).ToList();
}