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
        Task<List<Label>> GetByIdsAsync(IEnumerable<int> ids);
        
        Task<bool> DeleteByIdAsync(int id);

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
        public async Task<List<Label>> GetByIdsAsync(IEnumerable<int> ids)
        => await DbContext.Labels.Where(l => ids.Contains(l.LabelId)).ToListAsync();
        // public async Task<bool> DeleteHardAsync(Label entity)
        // {
        //await DbContext.Set<CardLabel>()
        //      .Where(cl => cl.LabelId == entity.LabelId)
        //    .ExecuteDeleteAsync();

        // DbContext.Labels.Remove(entity);
        //   return await DbContext.SaveChangesAsync() > 0;
        //}

        public async Task<bool> DeleteByIdAsync(int id)
        {
            
            var label = await DbContext.Labels
                .Include(l => l.Cards)
                .FirstOrDefaultAsync(l => l.LabelId == id);

            if (label == null) return false;

            
            label.Cards.Clear();

            
            DbContext.Labels.Remove(label);

            return await DbContext.SaveChangesAsync() > 0;
        }

    }
}
