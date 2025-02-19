namespace CleanArchitectureUtility.Extensions.Abstractions.UsersManagements;

public interface IUserInfoService
{
    string GetUserAgent();
    string GetUserIp();
    Guid UserId();
    string GetFirstName();
    string GetLastName();
    string GetUsername();
    string? GetClaim(string claimType);
    bool IsCurrentUser(Guid userId);
    Guid UserIdOrDefault();
    Guid UserIdOrDefault(Guid defaultValue);
}