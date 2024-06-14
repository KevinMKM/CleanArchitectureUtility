using System.Security.Claims;
using CleanArchitectureUtility.Utilities.Authentication.Options;

namespace CleanArchitectureUtility.Utilities.Authentication.Extensions;

public static class ClaimsPrincipalX
{
    public static bool HasSubClaim(this ClaimsPrincipal? principal, string userIdentifierClaimType)
        => principal?.Claims.Any(c => c.Type.Equals(userIdentifierClaimType)) ?? false;

    public static ClaimsIdentity CreateClaimsIdentity(this ClaimsPrincipal? principal, List<Claim> claims)
        => new(principal?.Claims?.ToList().GetNotExist(claims),
            principal?.Identities.FirstOrDefault()?.AuthenticationType,
            principal?.Identities.FirstOrDefault()?.NameClaimType,
            principal?.Identities.FirstOrDefault()?.RoleClaimType);

    public static ClaimsPrincipal? ClonePrincipalWithConvertedClaims(this ClaimsPrincipal? principal, ProviderOption provider)
    {
        if (principal is null)
            return null;

        var clone = principal.Clone();
        var claims = provider.ClaimMapper(clone.Claims.ToList());
        var authenticationType = clone.Identities.First().AuthenticationType;
        var nameType = clone.Identities.First().NameClaimType;
        var roleType = clone.Identities.First().RoleClaimType;
        var claimsIdentity = new ClaimsIdentity(claims, authenticationType, nameType, roleType);
        return new ClaimsPrincipal(claimsIdentity);
    }

    private static List<Claim> ClaimMapper(this ProviderOption provider, List<Claim> currentClaims)
    {
        var newClaims = new List<Claim>();
        foreach (var currentClaim in currentClaims)
        {
            var mapRule = provider.UserClaimTypeMapRules.FirstOrDefault(c => c.Source.Equals(currentClaim.Type));
            if (mapRule is null)
                newClaims.Add(currentClaim);
            else
            {
                var mappedClaim = new Claim(mapRule.Destination,
                    currentClaim.Value,
                    currentClaim.ValueType,
                    currentClaim.Issuer,
                    currentClaim.OriginalIssuer,
                    currentClaim.Subject);

                newClaims.Add(mappedClaim);
                if (mapRule.RemoveSource is false)
                    newClaims.Add(currentClaim);
            }
        }

        return newClaims;
    }

    private static List<Claim> GetNotExist(this List<Claim> current, List<Claim> target)
        => target.Where(tc => !current.Any(cc => cc.Type.Equals(tc.Type) && cc.Value.Equals(tc.Value))).ToList();
}