using System.Collections.Generic;
using System.Threading.Tasks;
using TBA.Models.Entities;
using TBA.Business;

namespace TBA.Services
{
    public class NotificationService
    {
        private readonly IBusinessNotification _businessNotification;

        public NotificationService(IBusinessNotification businessNotification)
        {
            _businessNotification = businessNotification;
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
            => await _businessNotification.GetAllNotificationsAsync();

        public async Task<Notification?> GetNotificationByIdAsync(int id)
            => await _businessNotification.GetNotificationAsync(id);

        public async Task<bool> SaveNotificationAsync(Notification notification)
            => await _businessNotification.SaveNotificationAsync(notification);

        public async Task<bool> DeleteNotificationAsync(int id)
        {
            var notification = await _businessNotification.GetNotificationAsync(id);
            if (notification == null) return false;
            return await _businessNotification.DeleteNotificationAsync(notification);
        }

        public async Task<bool> UpdateNotificationAsync(Notification notification)
            => await _businessNotification.UpdateNotificationAsync(notification);
    }
}