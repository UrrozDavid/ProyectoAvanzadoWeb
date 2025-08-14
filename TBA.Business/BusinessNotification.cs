using TBA.Models.Entities;
using TBA.Repositories;
using TBA.Core.Extensions;


namespace TBA.Business
{
    public interface IBusinessNotification
    {
        Task<IEnumerable<Notification>> GetAllNotificationsAsync();
        Task<bool> SaveNotificationAsync(Notification notification);
        Task<bool> DeleteNotificationAsync(Notification notification);
        Task<Notification> GetNotificationAsync(int id);
        Task<bool> UpdateNotificationAsync(Notification notification);
        Task<bool> CreateNotificationAsync(Notification notification);
        Task<IEnumerable<Notification>> GetNotificationsForUserAsync(int userId);
    }

    public class BusinessNotification : IBusinessNotification
    {

        private readonly IRepositoryNotification _repositoryNotification;

        public BusinessNotification(IRepositoryNotification repositoryNotification)
        {
            _repositoryNotification = repositoryNotification;
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
        {
            return await _repositoryNotification.ReadAsync();
        }

        public async Task<bool> SaveNotificationAsync(Notification notification)
        {
            var newNotification = ""; //Identity
            notification.AddAudit(newNotification);
            notification.AddLogging(notification.UserId <= 0 ? Models.Enums.LoggingType.Create: Models.Enums.LoggingType.Update);
            var exists = await _repositoryNotification.ExistsAsync(notification);
            return await _repositoryNotification.UpsertAsync(notification, exists);
        }

        public async Task<bool> DeleteNotificationAsync(Notification notification)
        {
            return await _repositoryNotification.DeleteAsync(notification);
        }

        public async Task<Notification> GetNotificationAsync(int id)
        {
            return await _repositoryNotification.FindAsync(id);
        }

        public Task<IEnumerable<Notification>> GetAllNotifications()
        {
            throw new NotImplementedException();
        }
        public async Task<bool> UpdateNotificationAsync(Notification notification)
        {
            var existingNotification = await _repositoryNotification.FindAsync(notification.NotificationId);
            if (existingNotification == null) return false;
            notification.AddAudit(existingNotification.CreatedBy ?? string.Empty); 
            notification.AddLogging(Models.Enums.LoggingType.Update);
            return await _repositoryNotification.UpdateAsync(notification);
        }

        public async Task<bool> CreateNotificationAsync(Notification notification)
        {
            notification.IsRead ??= false;
            notification.NotifyAt ??= DateTime.UtcNow;

            var newNotification = ""; // Usuario actual si tienes forma de obtenerlo
            notification.AddAudit(newNotification);
            notification.AddLogging(Models.Enums.LoggingType.Create);

            return await _repositoryNotification.CreateAsync(notification);
        }
        public async Task<IEnumerable<Notification>> GetNotificationsForUserAsync(int userId)
        {
            var allNotifications = await _repositoryNotification.ReadAsync();
            return allNotifications.Where(n => n.UserId == userId && n.IsRead == false);
        }

    }
}


