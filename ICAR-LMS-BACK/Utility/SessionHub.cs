using Microsoft.AspNetCore.SignalR;

public class SessionHub : Hub
{
    public override Task OnConnectedAsync()
    {
        var userIdStr = Context.UserIdentifier;
        if (int.TryParse(userIdStr, out int userId))
        {
            UserConnectionMapping.Add(userId, Context.ConnectionId);
        }
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        var userIdStr = Context.UserIdentifier;
        if (int.TryParse(userIdStr, out int userId))
        {
            UserConnectionMapping.Remove(userId, Context.ConnectionId);
        }
        return base.OnDisconnectedAsync(exception);
    }
}