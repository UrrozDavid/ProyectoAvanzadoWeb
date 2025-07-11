using TBA.Models.Entities;
using APW.Architecture;
using PAW.Architecture.Providers;
using System.Text.Json;
using TBA.Models.DTOs;
using Azure;

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

    public class UserService(IRestProvider restProvider) : IUserService
    {
        // CRUD
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
            var json = JsonSerializer.Serialize(new List<User> { user });
            var response = await restProvider.PostAsync($"https://localhost:7084/api/user/", json);
            return response.ToLower().Contains("true") || response.ToLower().Contains("ok");
        }

        public async Task<bool> UpdateAsync(User user)
        {
            var json = JsonSerializer.Serialize(user);
            var response = await restProvider.PutAsync($"https://localhost:7084/api/user/{user.UserId.ToString()}", "", json);
            return response.ToLower().Contains("true") || response.ToLower().Contains("ok");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await restProvider.DeleteAsync($"https://localhost:7084/api/user", id.ToString());
            return response.ToLower().Contains("true") || response.ToLower().Contains("ok");
        }

        // Authentication
        public async Task<User?> GetUserByEmail(string email)
        {
            var users = await GetAllAsync();
            return users.FirstOrDefault(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }
        public async Task<User> AuthenticateAsync(string email, string password)
        {
            var loginDTO = new LoginDTO
            {
                Email = email,
                Password = password
            };

            var json = JsonSerializer.Serialize(loginDTO);

            try
            {
                var response = await restProvider.PostAsync($"https://localhost:7084/api/user/auth", json);
                return await JsonProvider.DeserializeAsync<User>(response);
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("401"))
            {
                return null;
            }
            catch (ApplicationException ex) when (ex.Message.Contains("401"))
            {
                throw;
            }
        }

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
            // check if username exists in database
            if (await ExistsUsername(registerDTO.Username)) return (false, "This username is already taken. ");
            // check if email exists in database
            if (await ExistsEmail(registerDTO.Email)) return (false, "This email is already taken.");

            // prepare the User object
            var user = new User
            {
                Username = registerDTO.Username,
                Email = registerDTO.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password)
            };

            // send object to Create to send request to API
            var result = await CreateAsync(user);

            return result ? (true, null) : (false, "Something went wrong. Try again later.");
        }

        // Returns a temporary password to reset the current password.
        public string GenerateTemporaryPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<bool> UpdatePasswordAsync(string email, string newPassword)
        {
            var dto = new SetNewPasswordDTO
            {
                Email = email,
                NewPassword = newPassword
            };

            var json = JsonSerializer.Serialize(dto);
            var response = await restProvider.PutAsync($"https://localhost:7084/api/user/password", "", json);
            return response.ToLower().Contains("true") || response.ToLower().Contains("ok") || response == "";

        }
    }
}