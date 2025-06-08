using Microsoft.AspNetCore.Mvc;
using TBA.Business;
using TBA.Models.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class NotificationController(IBusinessNotification businessNotification) : ControllerBase
    {
        [HttpGet(Name = "GetNotificationss")]
        public async Task<IEnumerable<Notification>> GetNotifications()
        {
            return await businessNotification.GetAllNotificationsAsync();
        }

        [HttpGet("{id}")]
        public async Task<Notification> GetById(int id)
        {
            var notification = await businessNotification.GetNotificationAsync(id);
            return notification;
        }


        [HttpPost]
        public async Task<bool> Save([FromBody] IEnumerable<Notification> notifications)
        {
            foreach (var item in notifications)
            {
                await businessNotification.SaveNotificationAsync(item);
            }
            return true;
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task<bool> Delete(Notification notification)
        {
            return await businessNotification.DeleteNotificationAsync(notification);
        }
    }
}
