namespace CleanArchitectureUtility.Extensions.Abstractions.UsersManagements;

public class FakeUserInfoService : IUserInfoService
{
    private readonly Guid _defaultUserId;

    public FakeUserInfoService() : this(Guid.Empty)
    {
    }

    public FakeUserInfoService(Guid defaultUserId)
    {
        _defaultUserId = defaultUserId;
    }

    public string? GetClaim(string claimType) => claimType;
    public string GetFirstName() => "FirstName";
    public string GetLastName() => "LastName";
    public string GetUserAgent() => "UserAgent";
    public string GetUserIp() => "0.0.0.0";
    public string GetUsername() => "Username";
    public bool HasAccess(string access) => true;
    public bool IsCurrentUser(Guid userId) => true;
    public Guid UserId() => Guid.Empty;
    public Guid UserIdOrDefault() => _defaultUserId;
    public Guid UserIdOrDefault(Guid defaultValue) => defaultValue;
}