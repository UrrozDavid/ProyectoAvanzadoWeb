using Microsoft.AspNetCore.Mvc;
using TBA.Models.Entities;
using TBA.Services;

namespace TBA.Mvc.Controllers
{
    public class CardsController : Controller
    {
        private readonly CardService _cardService;

        public CardsController(CardService cardService)
        {
            _cardService = cardService;
        }

        // GET: Cards
        public async Task<IActionResult> Index()
        {
            var cards = await _cardService.GetAllCardsAsync();
            return View(cards);
        }

        // GET: Cards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cards/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Card model)
        {
            if (ModelState.IsValid)
            {
                var success = await _cardService.SaveCardAsync(model);
                if (success)
                    return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Cards/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var card = await _cardService.GetCardByIdAsync(id);
            if (card == null) return NotFound();

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

            // Si Card tiene relaciones que debas cargar manualmente, hazlo aquí.
            // Por ejemplo:
            // card.Board = await repositoryBoard.FindAsync(card.BoardId);

            return View(card);
        }
    }
}
