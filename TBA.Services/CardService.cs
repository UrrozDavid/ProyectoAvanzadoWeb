using System.Collections.Generic;
using System.Threading.Tasks;
using TBA.Models.Entities;
using TBA.Business;

namespace TBA.Services
{
    public class CardService
    {
        private readonly IBusinessCard _businessCard;

        public CardService(IBusinessCard businessCard)
        {
            _businessCard = businessCard;
        }

        public async Task<IEnumerable<Card>> GetAllCardsAsync()
            => await _businessCard.GetAllCardsAsync();

        public async Task<Card?> GetCardByIdAsync(int id)
            => await _businessCard.GetCardAsync(id);

        public async Task<bool> SaveCardAsync(Card card)
            => await _businessCard.SaveCardAsync(card);

        public async Task<bool> DeleteCardAsync(int id)
        {
            var card = await _businessCard.GetCardAsync(id);
            if (card == null) return false;
            return await _businessCard.DeleteCardAsync(card);
        }
    }
}
