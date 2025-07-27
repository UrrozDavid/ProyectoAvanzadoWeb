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



    }

    public class BusinessCard(IRepositoryCard repositoryCard) : IBusinessCard
    {
        public async Task<IEnumerable<Card>> GetAllCardsAsync()
        {
            return await repositoryCard.ReadAsync();
        }

        public async Task<bool> SaveCardAsync(Card card)
        {
            try
            {
                bool isUpdate = card.CardId > 0;
                var currentUser = "system";

                card.AddAudit(currentUser);
                card.AddLogging(isUpdate ? Models.Enums.LoggingType.Update : Models.Enums.LoggingType.Create);

                if (isUpdate)
                {
                    var existing = await repositoryCard.FindAsync(card.CardId);
                    if (existing == null) return false;

                    // Actualiza los campos editables
                    existing.Title = card.Title;
                    existing.Description = card.Description;
                    existing.CreatedAt = card.CreatedAt;
                    existing.DueDate = card.DueDate;
                    existing.ListId = card.ListId;

                    return await repositoryCard.UpdateAsync(existing);
                }
                else
                {
                    return await repositoryCard.CreateAsync(card);
                }
            }
            catch (Exception ex)
            {
                // Puedes agregar logging si tienes un logger configurado
                return false;
            }
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
                CommentsCount = c.Comments?.Count ?? 0,
                ChecklistDone = 0,
                ChecklistTotal = 0,
                Priority = c.Labels.FirstOrDefault()?.Name,
                ListName = c.List?.Name ?? "UNKNOWN"
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
            return await repositoryCard.UpdateCardAsync(card); 
        }
        public async Task<IEnumerable<Card>> GetAllCardsWithIncludesAsync()
        {
            return await repositoryCard.GetCardsWithIncludesAsync();
        }


    }
}

