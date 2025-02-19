using System.Security.Claims;

namespace CleanArchitectureUtility.Endpoints.WebApi.Extensions;

public static class ClaimExtensions
{
    public static string GetClaim(this ClaimsPrincipal userClaimsPrincipal, string claimType)
        => userClaimsPrincipal.Claims?.FirstOrDefault((Claim x) => x.Type == claimType)?.Value;
}