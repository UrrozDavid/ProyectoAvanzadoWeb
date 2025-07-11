using TBA.Models.Entities;
using TBA.Repositories;
using TBA.Core.Extensions;
using BCrypt.Net;


namespace TBA.Business
{
    public interface IBusinessUser
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<bool> SaveUserAsync(User user);
        Task<bool> DeleteUserAsync(User user);
        Task<User> GetUserAsync(int id);
        Task<User> AuthenticateAsync(string email, string password);
        Task<User?> GetUserByEmail(string email);
    }

    public class BusinessUser(IRepositoryUser repositoryUser) : IBusinessUser
    {
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            // Business Rules
            // revisar que sea entre las 7 am y 7 pm
            // tener permisos para leer en el usuario
            return await repositoryUser.ReadAsync();
        }

        public async Task<bool> SaveUserAsync(User user)
        {
            var newUser = ""; //Identity
            user.AddAudit(newUser);
            user.AddLogging(user.UserId <= 0 ? Models.Enums.LoggingType.Create : Models.Enums.LoggingType.Update);
            var exists = await repositoryUser.ExistsAsync(user);
            return await repositoryUser.UpsertAsync(user, exists);
        }

        public async Task<bool> DeleteUserAsync(User user)
        {
            return await repositoryUser.DeleteAsync(user);
        }

        public async Task<User> GetUserAsync(int id)
        {
            return await repositoryUser.FindAsync(id);
        }

        public Task<IEnumerable<User>> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            var user = await repositoryUser.GetByEmailAsync(email);

            if (user == null)
                return null;

            // Checking the Hashed Password
            var isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            // if valid return user
            return isValid ? user : null;
        }

        public async Task<User?> GetUserByEmail(string email) { return await repositoryUser.GetByEmailAsync(email);}
    }

}

