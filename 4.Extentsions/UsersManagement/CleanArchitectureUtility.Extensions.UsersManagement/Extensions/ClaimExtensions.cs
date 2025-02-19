using System.Security.Claims;

namespace CleanArchitectureUtility.Extensions.UsersManagement.Extensions;

public static class ClaimExtensions
{
    public static string GetClaim(this ClaimsPrincipal userClaimsPrincipal, string claimType)
        => userClaimsPrincipal.Claims.FirstOrDefault(x => x.Type == claimType)?.Value;
}