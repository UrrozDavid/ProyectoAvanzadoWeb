// Archivo: TBA.Services/UserService.cs
using System.Text.Json;
using TBA.Business;
using TBA.Models.DTOs;
using TBA.Models.Entities;

namespace TBA.Services
{
    public interface IUserService
    {
        // CRUD
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<bool> CreateAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);

        // Authentication
        Task<User?> GetUserByEmail(string email);
        Task<User?> AuthenticateAsync(string email, string password);
        Task<bool> ExistsUsername(string username);
        Task<bool> ExistsEmail(string email);
        Task<(bool success, string? ErrorMessage)> RegisterAsync(RegisterDTO registerDTO);
        string GenerateTemporaryPassword();
        Task<bool> UpdatePasswordAsync(string email, string newPassword);
    }

    public class UserService : IUserService
    {
        private readonly IBusinessUser _business;

        public UserService(IBusinessUser business)
        {
            _business = business;
        }

        // ----------------- CRUD -----------------
        public async Task<List<User>> GetAllAsync()
            => (await _business.GetAllUsersAsync()).ToList();

        public async Task<User?> GetByIdAsync(int id)
            => await _business.GetUserAsync(id);

        public async Task<bool> CreateAsync(User user)
            => await _business.SaveUserAsync(user);

        public async Task<bool> UpdateAsync(User user)
            => await _business.SaveUserAsync(user);

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _business.GetUserAsync(id);
            if (user == null) return false;
            return await _business.DeleteUserAsync(user);
        }

        // --------------- Auth / Helpers ---------------
        public async Task<User?> GetUserByEmail(string email)
            => await _business.GetUserByEmail(email);

        public async Task<User?> AuthenticateAsync(string email, string password)
            => await _business.AuthenticateAsync(email, password);

        public async Task<bool> ExistsUsername(string username)
        {
            var users = await GetAllAsync();
            return users.Any(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> ExistsEmail(string email)
        {
            var users = await GetAllAsync();
            return users.Any(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<(bool success, string? ErrorMessage)> RegisterAsync(RegisterDTO registerDTO)
        {
            if (await ExistsUsername(registerDTO.Username)) return (false, "This username is already taken.");
            if (await ExistsEmail(registerDTO.Email)) return (false, "This email is already taken.");

            var user = new User
            {
                Username = registerDTO.Username,
                Email = registerDTO.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password)
            };

            var result = await _business.SaveUserAsync(user);
            return result ? (true, null) : (false, "Something went wrong. Try again later.");
        }

        public string GenerateTemporaryPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<bool> UpdatePasswordAsync(string email, string newPassword)
        {
            var user = await _business.GetUserByEmail(email);
            if (user == null) return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            return await _business.SaveUserAsync(user);
        }
    }
}
