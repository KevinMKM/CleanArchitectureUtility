﻿using System.Security.Claims;

namespace CleanArchitectureUtility.Utilities.Auth.ApiAuthentication.Options;

public sealed class ProviderOption
{
    public bool Enabled { get; set; } = true;
    public bool IgnoreSSL { get; set; } = false;
    public string Scheme { get; set; } = default!;
    public string Authority { get; set; } = default!;
    public string? HttpClientFactoryName { get; set; }
    public TokenType TokenTypeSupport { get; set; } = TokenType.Jwt;
    public JwtTokenConfigOption JwtTokenConfig { get; set; } = new();
    public ReferenceTokenConfigOption ReferenceTokenConfig { get; set; } = new();
    public string UserIdentifierClaimType { get; set; } = ClaimTypes.NameIdentifier;
    public RegisterUserInfoClaimsOption RegisterUserInfoClaims { get; set; } = new();
    public List<UserClaimRuleOption> UserClaimRules { get; set; } = new();
    public List<UserClaimAddOnOption> UserClaimAddOns { get; set; } = new();
    public EndpointsPathOption? EndpointsPath { get; set; } = null;
    public int Priority { get; set; } = 1;
}