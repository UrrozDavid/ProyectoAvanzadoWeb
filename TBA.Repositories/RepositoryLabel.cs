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
    public interface IRepositoryLabel
    {
        Task<bool> UpsertAsync(Label entity, bool isUpdating);

        Task<bool> CreateAsync(Label entity);

        Task<bool> DeleteAsync(Label entity);

        Task<IEnumerable<Label>> ReadAsync();

        Task<Label> FindAsync(int id);

        Task<bool> UpdateAsync(Label entity);

        Task<bool> UpdateManyAsync(IEnumerable<Label> entities);

        Task<bool> ExistsAsync(Label entity);
        Task<bool> CheckBeforeSavingAsync(Label entity);

    }
    public class RepositoryLabel : RepositoryBase<Label>, IRepositoryLabel
    {
        public RepositoryLabel(TrelloDbContext context) : base(context)
        {
        }
        public async Task<bool> CheckBeforeSavingAsync(Label entity)
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
