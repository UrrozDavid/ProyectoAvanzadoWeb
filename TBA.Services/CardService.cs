using System.Collections.Generic;
using System.Threading.Tasks;
using TBA.Models.Entities;
using TBA.Business;
using APW.Architecture;
using PAW.Architecture.Providers;
using TBA.Models.DTOs;

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
    }

    public class CardService: ICardService
    {
        private readonly IBusinessCard _businessCard;

        private readonly RestProvider _restProvider;

        public CardService(IBusinessCard businessCard, RestProvider restProvider)
        {
            _businessCard = businessCard;
            _restProvider = restProvider;
        }

        public async Task<IEnumerable<Card>> GetAllCardsAsync()
            => await _businessCard.GetAllCardsAsync();

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

        public async Task<List<TaskViewModel>> GetTasksAsync()
        {
            var result = await _restProvider.GetAsync($"https://localhost:7084/api/card/tasks", null);
            var cards = await JsonProvider.DeserializeAsync<List<TaskViewModel>>(result);
            return cards;
        }
        public async Task<bool> SaveCardFromDtoAsync(CardCreateDto cardDto)
        {
            var content = JsonProvider.Serialize(cardDto);
            var result = await _restProvider.PostAsync("https://localhost:7084/api/card/create-dto", content);
            return true;
        }

        public async Task<int?> UpdateCardListAsync(int cardId, int newListId)
        {
            var payload = new { CardId = cardId, NewListId = newListId };
            var content = JsonProvider.Serialize(payload);

            var response = await _restProvider.PostWithResponseAsync("https://localhost:7084/api/card/update-status", content);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();

           
            var boardId = System.Text.Json.JsonSerializer.Deserialize<int>(json); 
            return boardId;
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


    }
}
