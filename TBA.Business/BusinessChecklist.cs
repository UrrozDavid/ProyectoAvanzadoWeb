using TBA.Models.Entities;
using TBA.Repositories;

namespace TBA.Business
{
    public interface IBusinessChecklistItem
    {
        Task<List<ChecklistItem>> GetItemsByCardIdAsync(int cardId);
        Task<bool> AddItemAsync(ChecklistItem newItem);
        Task<bool> ToggleItemAsync(int checklistItemId, bool isDone);
        Task<bool> DeleteItemAsync(int checklistItemId);
        Task<bool> UpdateItemAsync(ChecklistItem item);
    }

    public class BusinessChecklistItem : IBusinessChecklistItem
    {
        private readonly IRepositoryChecklistItem _repository;

        public BusinessChecklistItem(IRepositoryChecklistItem repository)
        {
            _repository = repository;
        }

        public async Task<List<ChecklistItem>> GetItemsByCardIdAsync(int cardId)
        {
            var items = await _repository.ReadByCardIdAsync(cardId);
            return items.ToList();
        }

        public async Task<bool> AddItemAsync(ChecklistItem newItem)
        {
            newItem.CreatedAt = DateTime.UtcNow;
            return await _repository.CreateAsync(newItem);
        }

        public async Task<bool> ToggleItemAsync(int checklistItemId, bool isDone)
        {
            var item = await _repository.FindAsync(checklistItemId);
            if (item == null) return false;
            item.IsDone = isDone;
            return await _repository.UpdateAsync(item);
        }

        public async Task<bool> DeleteItemAsync(int checklistItemId)
        {
            var item = await _repository.FindAsync(checklistItemId);
            if (item == null) return false;
            return await _repository.DeleteAsync(item);
        }

        public async Task<bool> UpdateItemAsync(ChecklistItem item)
        {
            return await _repository.UpdateAsync(item);
        }
    }
}