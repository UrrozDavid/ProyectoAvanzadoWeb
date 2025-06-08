using TBA.Models.Entities;

internal interface IBusinessUser
{
    Task<bool> DeleteUserAsync(User user);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User> GetUserAsync(int id);
    Task<bool> SaveUserAsync(User user);
}