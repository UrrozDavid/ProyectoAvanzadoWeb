using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TBA.API.Hubs;
using TBA.Business;
using TBA.Models.Entities;

namespace TBA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly IBusinessNotification _businessNotification;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationController(
            IBusinessNotification businessNotification,
            IHubContext<NotificationHub> hubContext)
        {
            _businessNotification = businessNotification;
            _hubContext = hubContext;
        }

        [HttpGet(Name = "GetNotifications")]
        public async Task<IEnumerable<Notification>> GetNotifications()
        {
            return await _businessNotification.GetAllNotificationsAsync();
        }

        [HttpGet("{id}")]
        public async Task<Notification> GetById(int id)
        {
            var notification = await _businessNotification.GetNotificationAsync(id);
            return notification;
        }

        [HttpPost]
        public async Task<bool> Save([FromBody] IEnumerable<Notification> notifications)
        {
            foreach (var item in notifications)
            {
                await _businessNotification.SaveNotificationAsync(item);
            }
            return true;
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(Notification notification)
        {
            return await _businessNotification.DeleteNotificationAsync(notification);
        }

        // *** NUEVO ENDPOINT para enviar notificaciones vía SignalR ***
        public class NotificationPayload
        {
            public int CardId { get; set; }
            public string Username { get; set; }
            public string Text { get; set; }
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationPayload payload)
        {
            if (payload == null || string.IsNullOrEmpty(payload.Text))
                return BadRequest("Invalid payload");

            var message = $"Nuevo comentario de {payload.Username} en la tarjeta {payload.CardId}: {payload.Text}";

            // Enviar notificación a todos los clientes conectados vía SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);

            return Ok();
        }
    }
}
