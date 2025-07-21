using Microsoft.AspNetCore.Mvc;
using TBA.Models.DTOs;
using TBA.Models.Entities;
using TBA.Services;

namespace TBA.Mvc.Controllers
{
    public class TasksController (ICardService _cardService) : Controller
    {

        public async Task<IActionResult> Index()
        {
            var tasks = await _cardService.GetTasksAsync();

            var username = TempData["User"]?.ToString();

            TempData.Keep("User");

            
            ViewBag.Username = username;

            return View(tasks); 
        }
        [HttpGet]
        public IActionResult Create()
        {
            TempData.Keep("User");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Card model)
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
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Error.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int cardId, int newListId)
        {
            var updated = await _cardService.UpdateCardListAsync(cardId, newListId);
            if (updated)
                return RedirectToAction("Index");

            return BadRequest("No se pudo actualizar el estado.");
        }

    }
}
