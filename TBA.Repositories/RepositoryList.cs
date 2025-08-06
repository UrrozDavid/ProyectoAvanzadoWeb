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
    public interface IRepositoryList
    {
        Task<bool> UpsertAsync(List entity, bool isUpdating);

        Task<bool> CreateAsync(List entity);

        Task<bool> DeleteAsync(List entity);

        Task<IEnumerable<List>> ReadAsync();

        Task<List> FindAsync(int id);

        Task<bool> UpdateAsync(List entity);

        Task<bool> UpdateManyAsync(IEnumerable<List> entities);

        Task<bool> ExistsAsync(List entity);
        Task<bool> CheckBeforeSavingAsync(List entity);

    }
    public class RepositoryList : RepositoryBase<List>, IRepositoryList
    {
        public RepositoryList(TrelloDbContext context) : base(context)
        {
        }

        public async Task<bool> CheckBeforeSavingAsync(List entity)
        {
            var exists = await ExistsAsync(entity);
            if (exists)
            {
                // algo más
            }

            return await UpsertAsync(entity, exists);
        }
        public async Task<IEnumerable<List>> ReadAsync()
        {
            return await DbContext.Lists.ToListAsync();
        }

    }
}
