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
        [HttpGet]
        public async Task<IActionResult> Create(int boardId)
        {
            TempData.Keep("User");
            ViewBag.BoardId = boardId;

            var lists = await _repositoryList.ReadAsync();
            var filteredLists = lists.Where(l => l.BoardId == boardId);

            ViewBag.ListId = new SelectList(filteredLists, "ListId", "Name");
            return View();
        }



        // POST: Cards/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Card model, int boardId)
        {
            if (ModelState.IsValid)
            {
                // Busca usuario de sesión
                var username = TempData["User"]?.ToString();
                TempData.Keep("User");

                if (!string.IsNullOrWhiteSpace(username))
                {
                    // Obtiene el usuario desde el repositorio
                    var user = await _cardService.GetUserByUsernameAsync(username);
                    if (user != null)
                    {
                        model.Users = new List<User> { user }; 
                    }
                }

                var success = await _cardService.SaveCardAsync(model);
                if (success)
                    return RedirectToAction("Index", "Tasks", new { boardId = model.List?.BoardId ?? boardId });



            }

            await LoadListsAsync(model.ListId);
            return View(model);
        }

        // GET: Cards/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var card = await _cardService.GetCardByIdAsync(id);
            if (card == null) return NotFound();

            await LoadListsAsync(card.ListId);
            return View(card);
        }

        // POST: Cards/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Card model)
        {
            if (ModelState.IsValid)
            {
                var success = await _cardService.SaveCardAsync(model);
                if (success)
                    return RedirectToAction(nameof(Index));
            }

            await LoadListsAsync(model.ListId);
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
