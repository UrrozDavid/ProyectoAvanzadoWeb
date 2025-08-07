using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TBA.Data.Models;
using TBA.Models.Entities;

namespace TBA.Services
{
    public class ChecklistService : IChecklistService
    {
        private readonly TrelloDbContext _ctx;
        public ChecklistService(TrelloDbContext ctx) => _ctx = ctx;

        public async Task<List<ChecklistItem>> GetItemsByCardIdAsync(int cardId) =>
            await _ctx.ChecklistItems
                      .Where(i => i.CardId == cardId)
                      .OrderBy(i => i.Position)
                      .ToListAsync();

        public async Task<ChecklistItem> AddItemAsync(ChecklistItem newItem)
        {
            newItem.CreatedAt = DateTime.UtcNow;
            _ctx.ChecklistItems.Add(newItem);
            await _ctx.SaveChangesAsync();
            return newItem;
        }

        public async Task<bool> ToggleItemAsync(int checklistItemId, bool isDone)
        {
            var item = await _ctx.ChecklistItems.FindAsync(checklistItemId);
            if (item == null) return false;
            item.IsDone = isDone;
            await _ctx.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteItemAsync(int checklistItemId)
        {
            var item = await _ctx.ChecklistItems.FindAsync(checklistItemId);
            if (item == null) return false;
            _ctx.ChecklistItems.Remove(item);
            await _ctx.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePositionAsync(int checklistItemId, int newPosition)
        {
            var item = await _ctx.ChecklistItems.FindAsync(checklistItemId);
            if (item == null) return false;
            item.Position = newPosition;
            await _ctx.SaveChangesAsync();
            return true;
        }
    }

public interface IChecklistService
        {
            Task<List<ChecklistItem>> GetItemsByCardIdAsync(int cardId);
            Task<ChecklistItem> AddItemAsync(ChecklistItem newItem);
            Task<bool> ToggleItemAsync(int checklistItemId, bool isDone);
            Task<bool> DeleteItemAsync(int checklistItemId);
            Task<bool> UpdatePositionAsync(int checklistItemId, int newPosition);
        }
}

