
using TBA.Models.Entities;
using TBA.Business;

namespace TBA.Services
{
    public class UserService
    {
        private readonly IBusinessUser _businessUser;

        public UserService(IBusinessUser businessUser)
        {
            _businessUser = businessUser;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
            => await _businessUser.GetAllUsersAsync();

        public async Task<User?> GetUserByIdAsync(int id)
            => await _businessUser.GetUserAsync(id);

        public async Task<bool> SaveUserAsync(User user)
            => await _businessUser.SaveUserAsync(user);

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _businessUser.GetUserAsync(id);
            if (user == null) return false;
            return await _businessUser.DeleteUserAsync(user);
        }
    }
}