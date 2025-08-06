using TBA.Models.Entities;

internal interface IBusinessNotification
{
    Task<bool> DeleteNotificationAsync(Notification notification);
    Task<IEnumerable<User>> GetAllNotificationsAsync();
    Task<User> GetNotificationAsync(int id);
    Task<bool> SaveNotificationAsync(Notification notification);
   

}