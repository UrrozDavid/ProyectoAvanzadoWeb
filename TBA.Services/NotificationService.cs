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

        public async Task CreateNotificationAsync(int userId, int? cardId, string message, string type, int relatedId, string groupName)
        {
            var notification = new Notification
            {
                UserId = userId,
                CardId = cardId,
                Message = message,
                NotifyAt = DateTime.UtcNow,
                IsRead = false,
                Type = type,
                RelatedId = relatedId,
                GroupName = groupName
            };

            await _businessNotification.CreateNotificationAsync(notification);
        }

    }
}