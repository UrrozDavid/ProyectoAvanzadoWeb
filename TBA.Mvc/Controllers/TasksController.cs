using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using TBA.Models.DTOs;
using TBA.Models.Entities;
using TBA.Services;

namespace TBA.Mvc.Controllers
{
  
    public class TasksController(ICardService _cardService) : Controller
    {
        #region Index
        public async Task<IActionResult> Index(int boardId)
        {
            TempData.Keep("User");

            var tasks = await _cardService.GetTasksAsync();
            var filtered = tasks
                .Where(t => t.BoardId == boardId)
                .ToList();

            ViewBag.Username = TempData["User"]?.ToString();
            ViewBag.BoardId = boardId;

            return View(filtered);
        }

        #endregion

        #region Create
        [HttpGet]
        public IActionResult Create()
        {
            TempData.Keep("User");
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Card model, int boardId)
        {
            TempData.Keep("User");

            var username = TempData["User"]?.ToString();

            var dto = new CardCreateDto
            {
                Title = model.Title,
                Description = model.Description,
                DueDate = model.DueDate,
                Username = username,
                ListId = 1
            };

            var success = await _cardService.SaveCardFromDtoAsync(dto);

            if (success)
                return RedirectToAction("Index", new { boardId });

            ModelState.AddModelError("", "Error.");
            return View(model);
        }
        #endregion

        #region Update
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int cardId, int newListId)
        {
            var boardId = await _cardService.UpdateCardListAsync(cardId, newListId);
            if (boardId.HasValue)
            {
                return RedirectToAction("Index", new { boardId = boardId.Value });
            }

            return BadRequest("No se pudo actualizar el estado.");
        }
        #endregion
    }
}
