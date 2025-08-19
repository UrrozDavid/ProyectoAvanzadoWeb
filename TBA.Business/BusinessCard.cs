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
        Task<bool> AssignUserAsync(int cardId, int userId);
        Task<List<TaskViewModel>> GetTasksViewAsync();
    }

    public class BusinessCard : IBusinessCard
    {
        private readonly IRepositoryCard _repositoryCard;
        private readonly IBusinessNotification _businessNotification;

        public BusinessCard(
            IRepositoryCard repositoryCard,
            IBusinessNotification businessNotification)
        {
            _repositoryCard = repositoryCard;
            _businessNotification = businessNotification;
        }
        public async Task<IEnumerable<Card>> GetAllCardsAsync()
        {
            return await _repositoryCard.ReadAsync();
        }

        public async Task<bool> SaveCardAsync(Card card)
        {
            var newCard = ""; //Identity

            card.AddAudit(newCard);
            card.AddLogging(card.CardId <= 0 ? Models.Enums.LoggingType.Create : Models.Enums.LoggingType.Update);

            var exists = await _repositoryCard.ExistsAsync(card);
            return await _repositoryCard.UpsertAsync(card, exists);
        }

        public async Task<bool> DeleteCardAsync(Card card)
        {
            // Eliminar relaciones primero

            await _repositoryCard.RemoveCardRelationsAsync(card.CardId);


            return await _repositoryCard.DeleteAsync(card);
        }

        public async Task<Card> GetCardAsync(int id)
        {
            return await _repositoryCard.FindAsync(id);
        }

        public Task<IEnumerable<Card>> GetAllCards()
        {
            throw new NotImplementedException();
        }

        public async Task<List<TaskViewModel>> GetTaskViewModelsAsync()
        {
            var cards = await _repositoryCard.GetCardsWithIncludesAsync();

            var viewModels = new List<TaskViewModel>();

            foreach (var c in cards)
            {
                var board = c.List?.Board;
                var boardMembers = board?.BoardMembers
                                        .Select(bm => bm.User?.Username ?? "Unknown")
                                        .ToList()
                                    ?? new List<string>();

                var vm = new TaskViewModel
                {
                    CardId                 = c.CardId,
                    Title                  = c.Title,
                    Description            = c.Description,
                    DueDate                = c.DueDate,
                    AssignedUserName       = c.Users.FirstOrDefault()?.Username,
                    AssignedUserAvatarUrl  = "/assets/images/users/avatar-2.jpg",
                    CommentsCount          = c.Comments?.Count ?? 0,
                    Priority               = c.Labels.FirstOrDefault()?.Name,
                    ListName               = c.List?.Name ?? "UNKNOWN",
                    BoardId                = board?.BoardId ?? 0,
                    BoardName              = board?.Name ?? "Sin Board",
                    Members                = boardMembers,
                    ListId                 = c.ListId ?? 0,
                    ListPosition           = c.List?.Position,
                    IsActive = c.IsActive
                };

            }

            return viewModels;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _repositoryCard.GetUserByUsernameAsync(username);
        }
        public async Task<bool> UpdateCardStatusAsync(int cardId, int newListId)
        {
            var card = await _repositoryCard.GetCardAsync(cardId);
            if (card == null) return false;

            var oldListId = card.ListId;
            card.ListId = newListId;

            var result = await _repositoryCard.UpdateCardAsync(card);

            if (result && oldListId != newListId)
            {
                // Vuelve a traer la card con toda la info de board y asignaciones
                card = await _repositoryCard.GetCardWithListAndBoardAsync(cardId);

                if (card.Assignments != null)
                {
                    foreach (var assignment in card.Assignments)
                    {
                        await _businessNotification.CreateNotificationAsync(new Notification
                        {
                            UserId = assignment.UserId,
                            CardId = card.CardId,
                            Message = $"La tarjeta '{card.Title}' fue movida a otra lista.",
                            Type = "CardMoved",
                            RelatedId = card.CardId,
                            GroupName = "Cards",
                            IsRead = false,
                            NotifyAt = DateTime.UtcNow
                        });
                    }
                }
            }

            return result;
        }

        public async Task<IEnumerable<Card>> GetAllCardsWithIncludesAsync()
        {
            return await _repositoryCard.GetCardsWithIncludesAsync();
        }
        public async Task<Card?> GetCardWithBoardInfoAsync(int cardId)
        {
            return await _repositoryCard.GetCardWithListAndBoardAsync(cardId);
        }

        public Task<bool> AssignUserAsync(int cardId, int userId)
            => _repositoryCard.UpsertAssignmentAsync(cardId, userId);

        public async Task<List<TaskViewModel>> GetTasksViewAsync()
        {
            var cards = await _repositoryCard.GetCardsWithIncludesAsync();

            var result = cards.Select( c=>
            {
                var assignedNames =
                (c.Assignments != null && c.Assignments.Any() && c.Assignments.Any(a => a.User != null))
                    ? c.Assignments.Where(a => a.User != null).Select(a => a.User!.Username)
                    : Enumerable.Empty<string>();

                var assignedUserName = assignedNames.FirstOrDefault();
                var firstLabel = c.Labels?.FirstOrDefault();
                return new TaskViewModel
                {
                    CardId = c.CardId,
                    Title = c.Title,
                    Description = c.Description,
                    DueDate = c.DueDate,

                    ListId = c.ListId ?? 0,
                    ListName = c.List?.Name,
                    ListPosition = c.List?.Position,

                    BoardId = c.List?.BoardId ?? 0,
                    BoardName = c.List?.Board?.Name,

                    AssignedUserName = assignedUserName,
                    Members = assignedNames.Distinct().ToList(),

                    CommentsCount = c.Comments?.Count ?? 0,
                    ChecklistTotal = c.ChecklistItems?.Count ?? 0,
                    ChecklistDone = c.ChecklistItems?.Count(i => i.IsDone) ?? 0,
                    IsActive = c.IsActive,
                    LabelColor = firstLabel?.Color
                };
            })
            .ToList();
            
           return result;
        }
    }
}