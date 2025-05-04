using Data.Entities;
using Microsoft.AspNetCore.SignalR;

namespace AlphaPortal.Hubs;

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (Context.User?.IsInRole("Administrator") ?? false)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            Console.WriteLine($"User {Context.UserIdentifier} added to Admins group.");
        }
        else
        {
            Console.WriteLine($"User {Context.UserIdentifier} is not an admin.");
        }

        await base.OnConnectedAsync();
    }
    public async Task SendNotification(NotificationEntity notification)
    {
        if (notification.NotificationTargetGroupId == 1)
        {
            await Clients.All.SendAsync("ReceiveNotification", notification);
        }
        else if (notification.NotificationTargetGroupId == 2)
        {
            await Clients.Group("Admins").SendAsync("AdminReceiveNotification", notification);
        }
    }
}
