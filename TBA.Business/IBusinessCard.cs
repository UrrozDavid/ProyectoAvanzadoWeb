using TBA.Models.Entities;

internal interface IBusinessCard
{
    Task<IEnumerable<Card>> GetAllCardsAsync();
    Task<bool> SaveCardAsync(Card card);
    Task<bool> DeleteCardAsync(Card card);
    Task<Card> GetCardAsync(int id);
}