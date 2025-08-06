using TBA.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBA.Repositories;
using Microsoft.EntityFrameworkCore;
using TBA.Data.Models;

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
        public RepositoryNotification(TrelloDbContext context) : base(context)
        {
        }
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
