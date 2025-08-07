using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TBA.Data.Models;
using TBA.Models.Entities;

namespace TBA.Repositories
{
    public interface IRepositoryChecklistItem
    {
        Task<bool> CreateAsync(ChecklistItem entity);
        Task<bool> UpdateAsync(ChecklistItem entity);
        Task<bool> DeleteAsync(ChecklistItem entity);
        Task<IEnumerable<ChecklistItem>> ReadAsync();
        Task<ChecklistItem?> FindAsync(int id);
        Task<bool> ExistsAsync(ChecklistItem entity);
        Task<IEnumerable<ChecklistItem>> ReadByCardIdAsync(int cardId);
    }

    public class RepositoryChecklistItem : IRepositoryChecklistItem
    {
        private readonly TrelloDbContext _context;

        public RepositoryChecklistItem(TrelloDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateAsync(ChecklistItem entity)
        {
            _context.ChecklistItems.Add(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(ChecklistItem entity)
        {
            _context.ChecklistItems.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(ChecklistItem entity)
        {
            _context.ChecklistItems.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<ChecklistItem>> ReadAsync()
        {
            return await _context.ChecklistItems.ToListAsync();
        }

        public async Task<ChecklistItem?> FindAsync(int id)
        {
            return await _context.ChecklistItems.FindAsync(id);
        }

        public async Task<bool> ExistsAsync(ChecklistItem entity)
        {
            return await _context.ChecklistItems.AnyAsync(x => x.ChecklistItemId == entity.ChecklistItemId);
        }

        public async Task<IEnumerable<ChecklistItem>> ReadByCardIdAsync(int cardId)
        {
            return await _context.ChecklistItems
                .Where(x => x.CardId == cardId)
                .ToListAsync();
        }
    }
}