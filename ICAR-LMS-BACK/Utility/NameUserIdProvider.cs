using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

public class NameUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        // Match with claim you put in JWT
        return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
