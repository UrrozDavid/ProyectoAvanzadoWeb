using TBA.Models.DTOs;
using TBA.Models.Entities;

internal interface IBusinessCard
{
    Task<IEnumerable<Card>> GetAllCardsAsync();
    Task<bool> SaveCardAsync(Card card);
    Task<bool> DeleteCardAsync(Card card);
    Task<Card> GetCardAsync(int id);
    Task<IEnumerable<Card>> GetAllCards();
    Task<List<TaskViewModel>> GetTaskViewModelsAsync();
    Task<User?> GetUserByUsernameAsync(string username);
    Task<bool> UpdateCardStatusAsync(int cardId, int newListId);
}