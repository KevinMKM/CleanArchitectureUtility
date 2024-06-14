using System.Security.Claims;

namespace CleanArchitectureUtility.Utilities.UsersManagement;

public static class ClaimX
{
    public static string? GetClaim(this ClaimsPrincipal userClaimsPrincipal, string claimType)
        => userClaimsPrincipal.Claims.FirstOrDefault(x => x.Type == claimType)?.Value;
}