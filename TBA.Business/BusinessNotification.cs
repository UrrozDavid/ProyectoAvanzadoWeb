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
    }

    public class BusinessNotification(IRepositoryNotification repositoryNotification) : IBusinessNotification
    {
        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
        {
            return await repositoryNotification.ReadAsync();
        }

        public async Task<bool> SaveNotificationAsync(Notification notification)
        {
            var newNotification = ""; //Identity
            notification.AddAudit(newNotification);
            notification.AddLogging(notification.UserId <= 0 ? Models.Enums.LoggingType.Create: Models.Enums.LoggingType.Update);
            var exists = await repositoryNotification.ExistsAsync(notification);
            return await repositoryNotification.UpsertAsync(notification, exists);
        }

        public async Task<bool> DeleteNotificationAsync(Notification notification)
        {
            return await repositoryNotification.DeleteAsync(notification);
        }

        public async Task<Notification> GetNotificationAsync(int id)
        {
            return await repositoryNotification.FindAsync(id);
        }

        public Task<IEnumerable<Notification>> GetAllNotifications()
        {
            throw new NotImplementedException();
        }
    }
}

