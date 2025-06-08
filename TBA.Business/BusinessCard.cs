using TBA.Models.Entities;
using TBA.Repositories;
using TBA.Core.Extensions;


namespace TBA.Business
{
    public interface IBusinessCard
    {
        Task<IEnumerable<Card>> GetAllCardsAsync();
        Task<bool> SaveCardAsync(Card card);
        Task<bool> DeleteCardAsync(Card card);
        Task<Card> GetCardAsync(int id);
    }

    public class BusinessCard(IRepositoryCard repositoryCard) : IBusinessCard
    {
        public async Task<IEnumerable<Card>> GetAllCardsAsync()
        {
            return await repositoryCard.ReadAsync();
        }

        public async Task<bool> SaveCardAsync(Card card)
        {
            var newCard = ""; //Identity
            card.AddAudit(newCard);
            card.AddLogging(card.CardId<= 0 ? Models.Enums.LoggingType.Create: Models.Enums.LoggingType.Update);
            var exists = await repositoryCard.ExistsAsync(card);
            return await repositoryCard.UpsertAsync(card, exists);
        }

        public async Task<bool> DeleteCardAsync(Card card)
        {
            return await repositoryCard.DeleteAsync(card);
        }

        public async Task<Card> GetCardAsync(int id)
        {
            return await repositoryCard.FindAsync(id);
        }

        public Task<IEnumerable<Card>> GetAllCards()
        {
            throw new NotImplementedException();
        }
    }
}

