using CleanArchitectureUtility.Core.Abstractions.UsersManagement;
using Serilog.Core;
using Serilog.Events;

namespace CleanArchitectureUtility.Utilities.SerilogRegistration;

public class UserInfoEnricher : ILogEventEnricher
{
    private readonly IUserInfoService _userInfoService;

    public UserInfoEnricher(IUserInfoService userInfoService) => _userInfoService = userInfoService;

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory factory)
    {
        var userName = _userInfoService.GetUsername();
        if (string.IsNullOrEmpty(userName))
            userName = "Unknown";

        var userId = _userInfoService.UserIdOrDefault();
        var userIp = _userInfoService.GetUserIp();
        var clientId = _userInfoService.GetClaim("client_id");
        if (string.IsNullOrEmpty(clientId))
            clientId = "Unknown";

        var userNameProperty = factory.CreateProperty("UserName", userName);
        var userIdProperty = factory.CreateProperty("UserId", userId);
        var userIpProperty = factory.CreateProperty("UserIp", userIp);
        var clientIdProperty = factory.CreateProperty("ClientId", clientId);

        logEvent.AddPropertyIfAbsent(userNameProperty);
        logEvent.AddPropertyIfAbsent(userIdProperty);
        logEvent.AddPropertyIfAbsent(userIpProperty);
        logEvent.AddPropertyIfAbsent(clientIdProperty);
    }
}