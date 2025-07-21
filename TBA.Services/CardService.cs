
using TBA.Models.Entities;
using TBA.Business;
using TBA.Models.DTOs;
using APW.Architecture;
using PAW.Architecture.Providers;
using System.Collections.Generic;

namespace TBA.Services
{
    public interface ICardService
    {
        Task<IEnumerable<Card>> GetAllCardsAsync();
        Task<Card?> GetCardAsync(int id);
        Task<bool> SaveCardAsync(IEnumerable<Card> cards);
        Task<bool> DeleteCardAsync(int id);
        Task<List<TaskViewModel>> GetTasksAsync();
        Task<bool> SaveCardFromDtoAsync(CardCreateDto cardDto);

        Task<bool> UpdateCardListAsync(int cardId, int newListId);
    }

    public class CardService(IRestProvider restProvider) : ICardService
    {

        public async Task<IEnumerable<Card>> GetAllCardsAsync()
        {
            var results = await restProvider.GetAsync($"https://localhost:7084/api/card", null);
            var cards = await JsonProvider.DeserializeAsync<IEnumerable<Card>>(results);
            return cards;
        }

        public async Task<Card?> GetCardAsync(int id)
        {
            var result = await restProvider.GetAsync($"https://localhost:7084/api/card", "1");
            var card = await JsonProvider.DeserializeAsync<Card>(result);
            return card;
        }

        public async Task<bool> SaveCardAsync(IEnumerable<Card> cards)
        {
            var content = JsonProvider.Serialize(cards); 
            var result = await restProvider.PostAsync($"https://localhost:7084/api/card", content);
            return true;
        }

        public async Task<bool> DeleteCardAsync(int id)
        {
            var result = await restProvider.DeleteAsync($"https://localhost:7084/api/card", $"{id}");
            return true;
        }


        public async Task<List<TaskViewModel>> GetTasksAsync()
        {
            var result = await restProvider.GetAsync($"https://localhost:7084/api/card/tasks", null);
            var cards = await JsonProvider.DeserializeAsync<List<TaskViewModel>>(result);
            return cards;
        }
        public async Task<bool> SaveCardFromDtoAsync(CardCreateDto cardDto)
        {
            var content = JsonProvider.Serialize(cardDto);
            var result = await restProvider.PostAsync("https://localhost:7084/api/card/create-dto", content);
            return true;
        }
        public async Task<bool> UpdateCardListAsync(int cardId, int newListId)
        {
            var payload = new { CardId = cardId, NewListId = newListId };
            var content = JsonProvider.Serialize(payload);
            var response = await restProvider.PostWithResponseAsync("https://localhost:7084/api/card/update-status", content);
            return response.IsSuccessStatusCode;


        }


    }
}