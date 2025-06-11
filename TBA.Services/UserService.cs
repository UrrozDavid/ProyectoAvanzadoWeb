using TBA.Models.Entities;
using APW.Architecture;
using PAW.Architecture.Providers;
using System.Text.Json;

namespace TBA.Services
{
    public interface IUserService
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<bool> CreateAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
    }

    public class UserService(IRestProvider restProvider) : IUserService
    {

        public async Task<List<User>> GetAllAsync()
        {
            var result = await restProvider.GetAsync($"https://localhost:7084/api/user", null);
            var users = await JsonProvider.DeserializeAsync<List<User>>(result) ?? new();
            return users;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            var result = await restProvider.GetAsync($"https://localhost:7084/api/user", id.ToString());
            return await JsonProvider.DeserializeAsync<User>(result);
        }

        public async Task<bool> CreateAsync(User user)
        {
            var json = JsonSerializer.Serialize(user);
            var response = await restProvider.PostAsync($"https://localhost:7084/api/user/", json);
            return response.ToLower().Contains("true") || response.ToLower().Contains("ok");
        }

        public async Task<bool> UpdateAsync(User user)
        {
            var json = JsonSerializer.Serialize(user);
            var response = await restProvider.PutAsync($"https://localhost:7084/api/user/", user.UserId.ToString(), json);
            return response.ToLower().Contains("true") || response.ToLower().Contains("ok");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await restProvider.DeleteAsync($"https://localhost:7084/api/user", id.ToString());
            return response.ToLower().Contains("true") || response.ToLower().Contains("ok");
        }
    }
}
