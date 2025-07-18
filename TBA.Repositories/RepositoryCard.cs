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
    public interface IRepositoryCard
    {
        Task<bool> UpsertAsync(Card entity, bool isUpdating);

        Task<bool> CreateAsync(Card entity);

        Task<bool> DeleteAsync(Card entity);

        Task<IEnumerable<Card>> ReadAsync();

        Task<Card> FindAsync(int id);

        Task<bool> UpdateAsync(Card entity);

        Task<bool> UpdateManyAsync(IEnumerable<Card> entities);

        Task<bool> ExistsAsync(Card entity);
        Task<bool> CheckBeforeSavingAsync(Card entity);
        Task<List<Card>> GetCardsWithIncludesAsync();

    }
    public class RepositoryCard : RepositoryBase<Card>, IRepositoryCard
    {
        public async Task<bool> CheckBeforeSavingAsync(Card entity)
        {
            var exists = await ExistsAsync(entity);
            if (exists)
            {
                // algo más
            }

            return await UpsertAsync(entity, exists);
        }
        public async Task<List<Card>> GetCardsWithIncludesAsync()
        {
            return await DbContext.Cards
                .Include(c => c.Users)
                .Include(c => c.Comments)
                .Include(c => c.Labels)
                .Include(c => c.Attachments)
                .ToListAsync();
        }
    }
}
