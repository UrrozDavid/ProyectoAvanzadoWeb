using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace TBA.API.Hubs
{
    public class NotificationHub : Hub
    {
        // Método para enviar notificación a un usuario específico
        public async Task SendNotification(int userId, string message)
        {
            await Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);
        }
    }
}
