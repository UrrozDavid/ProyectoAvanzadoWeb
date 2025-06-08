using TBA.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBA.Repositories;

namespace TBA.Repositories
{
    public interface IRepositoryAttachment
    {
        Task<bool> UpsertAsync(Attachment entity, bool isUpdating);

        Task<bool> CreateAsync(Attachment entity);

        Task<bool> DeleteAsync(Attachment entity);

        Task<IEnumerable<Attachment>> ReadAsync();

        Task<Attachment> FindAsync(int id);

        Task<bool> UpdateAsync(Attachment entity);

        Task<bool> UpdateManyAsync(IEnumerable<Attachment> entities);

        Task<bool> ExistsAsync(Attachment entity);
        Task<bool> CheckBeforeSavingAsync(Attachment entity);

    }
    public class RepositoryAttachment : RepositoryBase<Attachment>, IRepositoryAttachment
    {
        public async Task<bool> CheckBeforeSavingAsync(Attachment entity)
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
