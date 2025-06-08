using TBA.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBA.Repositories;

namespace TBA.Repositories
{
    public interface IRepositoryUser
    {
        Task<bool> UpsertAsync(User entity, bool isUpdating);

        Task<bool> CreateAsync(User entity);

        Task<bool> DeleteAsync(User entity);

        Task<IEnumerable<User>> ReadAsync();

        Task<User> FindAsync(int id);

        Task<bool> UpdateAsync(User entity);

        Task<bool> UpdateManyAsync(IEnumerable<User> entities);

        Task<bool> ExistsAsync(User entity);
        Task<bool> CheckBeforeSavingAsync(User entity);

    }
    public class RepositoryUser : RepositoryBase<User>, IRepositoryUser
    {
        public async Task<bool> CheckBeforeSavingAsync(User entity)
        {
            var exists = await ExistsAsync(entity);
            if (exists)
            {
                // algo más
            }

            return await UpsertAsync(entity, exists);
        }
    }
}
