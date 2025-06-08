using TBA.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBA.Repositories;

namespace TBA.Repositories
{
    public interface IRepositoryNotification
    {
        Task<bool> UpsertAsync(Notification entity, bool isUpdating);

        Task<bool> CreateAsync(Notification entity);

        Task<bool> DeleteAsync(Notification entity);

        Task<IEnumerable<Notification>> ReadAsync();

        Task<Notification> FindAsync(int id);

        Task<bool> UpdateAsync(Notification entity);

        Task<bool> UpdateManyAsync(IEnumerable<Notification> entities);

        Task<bool> ExistsAsync(Notification entity);
        Task<bool> CheckBeforeSavingAsync(Notification entity);

    }
    public class RepositoryNotification : RepositoryBase<Notification>, IRepositoryNotification
    {
        public async Task<bool> CheckBeforeSavingAsync(Notification entity)
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
