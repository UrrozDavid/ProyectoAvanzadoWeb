using TBA.Models.Entities;
using TBA.Repositories;
using TBA.Core.Extensions;
using TBA.Models.DTOs;


namespace TBA.Business
{
    public interface IBusinessCard
    {
        Task<IEnumerable<Card>> GetAllCardsAsync();
        Task<bool> SaveCardAsync(Card card);
        Task<bool> DeleteCardAsync(Card card);
        Task<Card> GetCardAsync(int id);
        Task<IEnumerable<Card>> GetAllCards();
        Task<List<TaskViewModel>> GetTaskViewModelsAsync();
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> UpdateCardStatusAsync(int cardId, int newListId);

        Task<IEnumerable<Card>> GetAllCardsWithIncludesAsync();
        Task<Card?> GetCardWithBoardInfoAsync(int cardId);

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
            card.AddLogging(card.CardId <= 0 ? Models.Enums.LoggingType.Create : Models.Enums.LoggingType.Update);

            var exists = await repositoryCard.ExistsAsync(card);
            return await repositoryCard.UpsertAsync(card, exists);
        }

        public async Task<bool> DeleteCardAsync(Card card)
        {
            // Eliminar relaciones primero
            await repositoryCard.RemoveCardRelationsAsync(card.CardId); // 👇 lo implementamos abajo

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
        public async Task<List<TaskViewModel>> GetTaskViewModelsAsync()
        {
            var cards = await repositoryCard.GetCardsWithIncludesAsync();

            return cards.Select(c =>
            {
                var board = c.List?.Board;
                var boardMembers = board?.BoardMembers?.Select(bm => bm.User?.Username ?? "Unknown").ToList() ?? new List<string>();

                return new TaskViewModel
                {
                    CardId = c.CardId,
                    Title = c.Title,
                    Description = c.Description,
                    DueDate = c.DueDate,
                    AssignedUserName = c.Users.FirstOrDefault()?.Username,
                    AssignedUserAvatarUrl = "/assets/images/users/avatar-2.jpg",
                    CommentsCount = c.Comments?.Count ?? 0,
                    ChecklistDone = 0,
                    ChecklistTotal = 0,
                    Priority = c.Labels.FirstOrDefault()?.Name,
                    ListName = c.List?.Name ?? "UNKNOWN",
                    BoardId = board?.BoardId ?? 0,
                    BoardName = board?.Name ?? "Sin Board",
                    Members = boardMembers
                };
            }).ToList();
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await repositoryCard.GetUserByUsernameAsync(username);
        }
        public async Task<bool> UpdateCardStatusAsync(int cardId, int newListId)
        {
            var card = await repositoryCard.GetCardAsync(cardId);
            if (card == null) return false;

            card.ListId = newListId;
            var result = await repositoryCard.UpdateCardAsync(card);

            if (result)
            {
                
                card = await repositoryCard.GetCardWithListAndBoardAsync(cardId);
            }

            return result;
        }




        public async Task<IEnumerable<Card>> GetAllCardsWithIncludesAsync()
        {
            return await repositoryCard.GetCardsWithIncludesAsync();
        }
        public async Task<Card?> GetCardWithBoardInfoAsync(int cardId)
        {
            return await repositoryCard.GetCardWithListAndBoardAsync(cardId);
        }


    }
}