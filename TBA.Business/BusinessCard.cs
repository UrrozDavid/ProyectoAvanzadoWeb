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
        public async Task<List<TaskViewModel>> GetTaskViewModelsAsync()
        {
            var cards = await repositoryCard.GetCardsWithIncludesAsync();

            return cards.Select(c => new TaskViewModel
            {
                CardId = c.CardId,
                Title = c.Title,
                Description = c.Description,
                DueDate = c.DueDate,
                AssignedUserName = c.Users.FirstOrDefault()?.Username,
                AssignedUserAvatarUrl = "/assets/images/users/avatar-2.jpg",
                CommentsCount = c.Comments.Count,
                ChecklistDone = 0,
                ChecklistTotal = 0,
                Priority = c.Labels.FirstOrDefault()?.Name
            }).ToList();
        }
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await repositoryCard.GetUserByUsernameAsync(username);
        }

    }
}

