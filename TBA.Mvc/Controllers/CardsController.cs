using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TBA.Models.Entities;
using TBA.Repositories;
using TBA.Services;

namespace TBA.Mvc.Controllers
{
    public class CardsController : Controller
    {
        private readonly ICardService _cardService;
        private readonly IRepositoryList _repositoryList;

        public CardsController(ICardService cardService, IRepositoryList repositoryList)
        {
            _cardService = cardService;
            _repositoryList = repositoryList;
        }

        // GET: Cards
        public async Task<IActionResult> Index()
        {
            
            var cards = await _cardService.GetAllCardsWithIncludesAsync();
            return View(cards);
        }

        // GET: Cards/Create
        public async Task<IActionResult> Create(int boardId)
        {
            await LoadListsAsync(boardId);
            ViewBag.BoardId = boardId;
            return View();
        }

        private async Task LoadListsAsync(int boardId, int? selectedListId = null)
        {
            var lists = await _repositoryList.ReadAsync();
            var filteredLists = lists.Where(l => l.BoardId == boardId).ToList();

            // TEMP: Verifica en consola
            Console.WriteLine($"BoardId recibido: {boardId}");
            Console.WriteLine($"Total listas encontradas para el board: {filteredLists.Count}");

            foreach (var list in filteredLists)
                Console.WriteLine($"List: {list.Name} (BoardId: {list.BoardId})");

            ViewBag.ListId = new SelectList(filteredLists, "ListId", "Name", selectedListId);
        }

        // POST: Cards/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Card card, int boardId)
        {
            if (ModelState.IsValid)
            {
                card.CreatedAt = DateTime.Now;
                await _cardService.SaveCardAsync(card); // ✅ Aquí
                return RedirectToAction(nameof(Index));
            }

            await LoadListsAsync(boardId, card.ListId); // Recarga listas si hay error
            ViewBag.BoardId = boardId;
            return View(card);
        }



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

        public async Task<IActionResult> Details(int id)
        {
            var card = await _cardService.GetCardByIdAsync(id);
            if (card == null)
                return NotFound();

            return View(card);
        }

        private async Task LoadListsAsync(int? selectedId = null)
        {
            var lists = await _repositoryList.ReadAsync();
            ViewBag.ListId = new SelectList(lists, "ListId", "Name", selectedId);
        }
    }
}
