using System.Collections.Generic;
using System.Threading.Tasks;
using TBA.Models.Entities;
using TBA.Business;
using APW.Architecture;
using PAW.Architecture.Providers;
using TBA.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using TBA.Data.Models;

namespace TBA.Services
{
    public interface ICardService
    {
        Task<IEnumerable<Card>> GetAllCardsAsync();
        Task<Card?> GetCardByIdAsync(int id);
        Task<bool> SaveCardAsync(Card card);
        Task<bool> DeleteCardAsync(int id);
        Task<IEnumerable<Card>> GetAllCardsWithIncludesAsync();
        Task<List<TaskViewModel>> GetTasksAsync();
        Task<bool> SaveCardFromDtoAsync(CardCreateDto cardDto);
        Task<int?> UpdateCardListAsync(int cardId, int newListId);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<int?> GetBoardIdFromCardAsync(int cardId);
        Task<bool> AssignUserAsync(int cardId, int userId);
        Task AssignLabelsAsync(int cardId, List<int> labelIds);
        Task<IEnumerable<BoardViewViewModel>> GetCards();
    }

    public class CardService: ICardService
    {
        private readonly IBusinessCard _businessCard;

        private readonly RestProvider _restProvider;
        private readonly TrelloDbContext _dbContext;

        public CardService(IBusinessCard businessCard, RestProvider restProvider, TrelloDbContext dbContext)
        {
            _businessCard = businessCard;
            _restProvider = restProvider;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<BoardViewViewModel>> GetCards()
        {
            return await _businessCard.GetAllCardsAsync();
        }
        public async Task<IEnumerable<Card>> GetAllCardsAsync()
            => await _businessCard.GetAllCards();

        public async Task<Card?> GetCardByIdAsync(int id)
        {
            return await _businessCard.GetCardWithBoardInfoAsync(id); 
        }

        public async Task<bool> SaveCardAsync(Card card)
            => await _businessCard.SaveCardAsync(card);

        public async Task<bool> DeleteCardAsync(int id)
        {
            var card = await _businessCard.GetCardAsync(id);
            if (card == null) return false;
            return await _businessCard.DeleteCardAsync(card);
        }
        public async Task<IEnumerable<Card>> GetAllCardsWithIncludesAsync()
            => await _businessCard.GetAllCardsWithIncludesAsync();

        public async Task<List<TaskViewModel>> GetTasksAsync() => await _businessCard.GetTasksViewAsync();
        public async Task<bool> SaveCardFromDtoAsync(CardCreateDto cardDto)
        {
            var content = JsonProvider.Serialize(cardDto);
            var result = await _restProvider.PostAsync("https://localhost:7084/api/card/create-dto", content);
            return true;
        }

        public async Task<int?> UpdateCardListAsync(int cardId, int newListId)
        {
            var ok = await _businessCard.UpdateCardStatusAsync(cardId, newListId);
            if (!ok) return null;

            var card = await _businessCard.GetCardWithBoardInfoAsync(cardId);
            return card?.List?.BoardId;
        }


        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _businessCard.GetUserByUsernameAsync(username);
        }
        public async Task<int?> GetBoardIdFromCardAsync(int cardId)
        {
            var card = await _businessCard.GetCardWithBoardInfoAsync(cardId);
            return card?.List?.BoardId;
        }

        public async Task<bool> AssignUserAsync(int cardId, int userId)
            => await _businessCard.AssignUserAsync(cardId, userId);

        public async Task AssignLabelsAsync(int cardId, List<int> labelIds)
        {
            var card = await _dbContext.Cards
                .Include(c => c.Labels)
                .FirstOrDefaultAsync(c => c.CardId == cardId);

            if (card == null)
                throw new Exception("Card no encontrada");

            // Limpiar labels anteriores
            card.Labels.Clear();

            // Agregar los nuevos labels
            var labels = await _dbContext.Labels
                .Where(l => labelIds.Contains(l.LabelId))
                .ToListAsync();

            foreach (var label in labels)
            {
                card.Labels.Add(label);
            }

            await _dbContext.SaveChangesAsync();
        }

        public Task<List<BoardViewViewModel>> GetBoardViewAsync()
        {
            return _businessCard.GetBoardViewAsync();
        }


    }
}
