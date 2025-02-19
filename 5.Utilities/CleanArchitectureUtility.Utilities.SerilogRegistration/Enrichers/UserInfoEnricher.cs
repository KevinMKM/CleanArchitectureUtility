using CleanArchitectureUtility.Extensions.Abstractions.UsersManagements;
using Serilog.Core;
using Serilog.Events;

namespace CleanArchitectureUtility.Utilities.SerilogRegistration.Enrichers;

public class UserInfoEnricher : ILogEventEnricher
{
    private readonly IUserInfoService _userInfoService;

    public UserInfoEnricher(IUserInfoService userInfoService)
    {
        _userInfoService = userInfoService;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory factory)
    {
        string UserId;
        string UserIp;

        var userName = _userInfoService.GetUsername();
        if (string.IsNullOrEmpty(userName))
            userName = "Unknown";

        UserId = _userInfoService.UserIdOrDefault().ToString();
        UserIp = _userInfoService.GetUserIp();
        var clientId = _userInfoService.GetClaim("client_id");
        if (string.IsNullOrEmpty(clientId))
            clientId = "Unknown";

        var userNameProperty = factory.CreateProperty("UserName", userName);
        var userIdProperty = factory.CreateProperty("UserId", UserId);
        var userIpProperty = factory.CreateProperty("UserIp", UserIp);
        var clientIdProperty = factory.CreateProperty("ClientId", clientId);
        logEvent.AddPropertyIfAbsent(userNameProperty);
        logEvent.AddPropertyIfAbsent(userIdProperty);
        logEvent.AddPropertyIfAbsent(userIpProperty);
        logEvent.AddPropertyIfAbsent(clientIdProperty);
    }
}