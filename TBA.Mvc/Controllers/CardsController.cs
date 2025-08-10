using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TBA.Models.Entities;
using TBA.Mvc.Models;
using TBA.Repositories;
using TBA.Services;

namespace TBA.Mvc.Controllers
{
    public class CardsController : Controller
    {
        private readonly IRepositoryList _repositoryList;
        private readonly ICardService _cardService;
        private readonly ListService _listService;
        private readonly IBoardMemberService _boardMemberService;

        public CardsController(
            ICardService cardService, 
            IRepositoryList repositoryList, 
            ListService listService, 
            IBoardMemberService boardMemberService)
        {
            _repositoryList = repositoryList;
            _cardService = cardService;
            _listService = listService;
            _boardMemberService = boardMemberService;
        }

        #region Index
        // GET: Cards
        public async Task<IActionResult> Index()
        {
            
            var cards = await _cardService.GetAllCardsWithIncludesAsync();
            return View(cards);
        }
        #endregion

        #region Create
        // GET: Cards/Create
        [HttpGet]
        public async Task<IActionResult> Create(int boardId)
        {
            var viewModel = new CardCreateViewModel
            {
                BoardId = boardId,
                Lists = await LoadListsAsync(boardId),
                Members = await LoadMembersAsync(boardId),
            };

            ViewBag.BoardId = boardId;
            return View(viewModel);
        }

        // POST: Cards/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CardCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Lists = await LoadListsAsync(model.BoardId, model.ListId);
                model.Members = await LoadMembersAsync(model.BoardId, model.AssigneeUserId);

                return View(model);
            }

            var card = new Card
            {
                Title = model.Title,
                Description = model.Description,
                DueDate = model.DueDate,
                ListId = model.ListId,
                CreatedAt = DateTime.Now
            };
            
            var ok = await _cardService.SaveCardAsync(card);
            if (!ok)
            {
                ModelState.AddModelError("", "Could not create the card");
                model.Lists = await LoadListsAsync(model.BoardId, model.ListId);
                return View(model);
            }

            // Asignar una Card a un usuario
            if (model.AssigneeUserId.HasValue) 
                await _cardService.AssignUserAsync(card.CardId, model.AssigneeUserId.Value);

            return RedirectToAction("Index", "Tasks", new { boardId = model.BoardId });
        }

        private async Task<SelectList> LoadListsAsync(int boardId, int? selectedListId = null)
        {
            var filtered = (await _repositoryList.ReadAsync())
                .Where(list => list.BoardId == boardId)
                .OrderBy(list => list.Position)
                .Select(list => new SelectListItem { Value = list.ListId.ToString(), Text = list.Name})
                .ToList();

            return new SelectList(filtered, "Value", "Text", selectedListId);
        }

        private async Task<SelectList> LoadMembersAsync(int boardId, int? selectedUserId = null)
        {
            var users = await _boardMemberService.GetMembersByBoardAsync(boardId);
            var items = users.OrderBy(u => u.Username)
                             .Select(u => new SelectListItem { Value = u.UserId.ToString(), Text = u.Username })
                             .ToList();
            return new SelectList(items, "Value", "Text", selectedUserId);
        }

        #endregion

        #region Edit
        // GET: Cards/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var card = await _cardService.GetCardByIdAsync(id);
            if (card == null) return NotFound();

            var boardId = card.List?.BoardId ?? 0;
            await LoadListsAsync(boardId, card.ListId);

            return View(card);
        }

        // POST: Cards/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Card model)
        {
            if (!ModelState.IsValid)
            {
                await LoadListsAsync(model.List?.BoardId ?? 0, model.ListId);
                return View(model);
            }

            var existing = await _cardService.GetCardByIdAsync(model.CardId);
            if (existing == null)
            {
                ModelState.AddModelError("", "Card not found.");
                return View(model);
            }

            // Solo campos editables
            existing.Title = model.Title;
            existing.Description = model.Description;
            existing.DueDate = model.DueDate;
            existing.ListId = model.ListId;

            var success = await _cardService.SaveCardAsync(existing);
            if (success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Error updating the card.");
            await LoadListsAsync(model.List?.BoardId ?? 0, model.ListId);
            return View(model);
        }
        #endregion

        #region Delete
        // GET: Cards/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var card = await _cardService.GetCardByIdAsync(id);
            if (card == null) return NotFound();

            return View(card);
        }

        // POST: Cards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _cardService.DeleteCardAsync(id);
            if (!success) return NotFound();

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Details
        public async Task<IActionResult> Details(int id)
        {
            var card = await _cardService.GetCardByIdAsync(id);
            if (card == null)
                return NotFound();

            return View(card);
        }
        #endregion
    }
}
