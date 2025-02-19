using System.Security.Claims;
using CleanArchitectureUtility.Extensions.Abstractions.UsersManagements;
using CleanArchitectureUtility.Extensions.UsersManagement.Extensions;
using CleanArchitectureUtility.Extensions.UsersManagement.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CleanArchitectureUtility.Extensions.UsersManagement.Services;

public class WebUserInfoService : IUserInfoService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManagementOptions _configuration;

    public WebUserInfoService(IHttpContextAccessor httpContextAccessor, IOptions<UserManagementOptions> configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration.Value;
    }

    public string GetUserAgent() => _httpContextAccessor?.HttpContext?.Request?.Headers["User-Agent"] ?? _configuration.DefaultUserAgent;
    public string GetUserIp() => _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? _configuration.DefaultUserIp;
    public string GetUsername() => _httpContextAccessor.HttpContext?.User?.GetClaim(ClaimTypes.Name) ?? _configuration.DefaultUsername;
    public string GetFirstName() => _httpContextAccessor.HttpContext?.User?.GetClaim(ClaimTypes.GivenName) ?? _configuration.DefaultFirstName;
    public string GetLastName() => _httpContextAccessor.HttpContext?.User?.GetClaim(ClaimTypes.Surname) ?? _configuration.DefaultLastName;
    public bool IsCurrentUser(Guid userId) => UserId() == userId;
    public string? GetClaim(string claimType) => _httpContextAccessor.HttpContext?.User?.GetClaim(claimType);
    public Guid UserIdOrDefault() => UserIdOrDefault(_configuration.DefaultUserId);
    public Guid UserIdOrDefault(Guid defaultValue)
    {
        var userId = UserId();
        return userId == Guid.Empty ? defaultValue : userId;
    }
    public Guid UserId()
    {
        var userId = _httpContextAccessor?.HttpContext?.User?.GetClaim(ClaimTypes.NameIdentifier);
        return string.IsNullOrWhiteSpace(userId) ? Guid.Empty : Guid.Parse(userId);
    }
}