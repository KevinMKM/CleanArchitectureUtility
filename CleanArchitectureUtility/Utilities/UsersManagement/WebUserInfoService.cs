﻿using System.Security.Claims;
using CleanArchitectureUtility.Core.Abstractions.UsersManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CleanArchitectureUtility.Utilities.UsersManagement;

public class WebUserInfoService : IUserInfoService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManagementOptions _configuration;

    public WebUserInfoService(IHttpContextAccessor httpContextAccessor, IOptions<UserManagementOptions> configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration.Value;
    }

    public string GetUserAgent() => _httpContextAccessor?.HttpContext?.Request.Headers?.UserAgent.ToString() ?? _configuration.DefaultUserAgent;

    public string GetUserIp() => _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? _configuration.DefaultUserIp;

    public string UserId() => _httpContextAccessor?.HttpContext?.User?.GetClaim(ClaimTypes.NameIdentifier) ?? string.Empty;

    public string GetUsername() => _httpContextAccessor.HttpContext?.User?.GetClaim(ClaimTypes.Name) ?? _configuration.DefaultUsername;

    public string GetFirstName() => _httpContextAccessor.HttpContext?.User?.GetClaim(ClaimTypes.GivenName) ?? _configuration.DefaultFirstName;

    public string GetLastName() => _httpContextAccessor.HttpContext?.User?.GetClaim(ClaimTypes.Surname) ?? _configuration.DefaultLastName;

    public bool IsCurrentUser(string userId) => string.Equals(UserId().ToString(), userId, StringComparison.OrdinalIgnoreCase);

    public string? GetClaim(string claimType) => _httpContextAccessor.HttpContext?.User?.GetClaim(claimType);

    public string UserIdOrDefault() => UserIdOrDefault(_configuration.DefaultUserId);

    public string UserIdOrDefault(string defaultValue)
    {
        var userId = UserId();
        return string.IsNullOrEmpty(userId) ? defaultValue : userId;
    }
}