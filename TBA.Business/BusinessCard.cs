using TBA.Models.Entities;
using TBA.Repositories;
using TBA.Core.Extensions;
using TBA.Models.DTOs;
using TBA.Data.Models;
using Microsoft.EntityFrameworkCore;




namespace TBA.Business
{
    public interface IBusinessCard
    {
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
        Task AssignLabelsAsync(int cardId, List<int> labelIds);
        Task<List<BoardViewViewModel>> GetBoardViewAsync();
        Task<List<BoardViewViewModel>> GetAllCardsAsync();


    }

    public class BusinessCard : IBusinessCard
    {
        private readonly IRepositoryCard repositoryCard;
        private readonly IRepositoryBase<Label> _labelRepository;
        private readonly IBusinessChecklistItem businessChecklist;
        private readonly TrelloDbContext _dbContext;

        public BusinessCard(
            IRepositoryCard cardRepository,
            IRepositoryBase<Label> labelRepository,
            IBusinessChecklistItem businessChecklist,
            TrelloDbContext dbContext)
        {
            repositoryCard = cardRepository;
            _labelRepository = labelRepository;
            businessChecklist = businessChecklist;
            _dbContext = dbContext;
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
            await repositoryCard.RemoveCardRelationsAsync(card.CardId);

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
                    ListPosition           = c.List?.Position

                };


                var items = await businessChecklist.GetItemsByCardIdAsync(c.CardId);
                vm.ChecklistTotal   = items.Count;
                vm.ChecklistDone    = items.Count(i => i.IsDone);
                vm.ChecklistItems   = items.Select(i => new ChecklistItemDto
                {
                    ChecklistItemId = i.ChecklistItemId,
                    CardId          = i.CardId,
                    Text            = i.Text,
                    IsDone          = i.IsDone,
                    Position        = i.Position
                }).ToList();

                viewModels.Add(vm);
            }

            return viewModels;
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

        public Task<bool> AssignUserAsync(int cardId, int userId)
            => repositoryCard.UpsertAssignmentAsync(cardId, userId);

        public async Task<List<TaskViewModel>> GetTasksViewAsync()
        {
            var cards = await repositoryCard.GetCardsWithIncludesAsync();

            var result = cards.Select( c=>
            {
                var assignedNames =
                (c.Assignments != null && c.Assignments.Any() && c.Assignments.Any(a => a.User != null))
                    ? c.Assignments.Where(a => a.User != null).Select(a => a.User!.Username)
                    : Enumerable.Empty<string>();

                var assignedUserName = assignedNames.FirstOrDefault();

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
                };
            })
            .ToList();
            
           return result;
        }

        public async Task AssignLabelsAsync(int cardId, List<int> labelIds)
        {
            var card = await _dbContext.Cards
        .Include(c => c.Labels)
        .FirstOrDefaultAsync(c => c.CardId == cardId);

            if (card == null)
                throw new Exception("Card not found");

            // Limpiamos etiquetas anteriores
            card.Labels.Clear();

            // Obtenemos nuevas etiquetas
            var labels = await _dbContext.Labels.ToListAsync();


            foreach (var label in labels)
            {
                card.Labels.Add(label);
            }

            await _dbContext.SaveChangesAsync();
        }
        public async Task<List<BoardViewViewModel>> GetBoardViewAsync()
        {
            var cards = await _dbContext.Cards
                .Include(c => c.Labels)
                .ToListAsync();

            return cards.Select(card => new BoardViewViewModel
            {
                CardId = card.CardId,
                CardTitle = card.Title,
                CardDescription = card.Description,
                LabelColors = card.Labels.Select(l => l.Color).ToList()
            }).ToList();
        }
        public async Task<List<BoardViewViewModel>> GetAllCardsAsync()
        {
            var cards = (await repositoryCard.ReadAsync()).Cast<Card>();

            return cards.Select(c =>
            {
                var vm = new BoardViewViewModel
                {
                    CardId = c.CardId,
                    CardTitle = c.Title,
                    CardDescription = c.Description,
                    LabelColors = c.Labels?
                                    .Where(l => !string.IsNullOrEmpty(l.Color))
                                    .Select(l => l.Color!)
                                    .ToList()
                                  ?? new List<string>()
                };
                return vm;
            }).ToList();
        }





    }
}