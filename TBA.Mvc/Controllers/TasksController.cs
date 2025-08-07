using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using TBA.Models.DTOs;
using TBA.Models.Entities;
using TBA.Services;
using TBA.Business; // Para IBusinessComment

namespace TBA.Mvc.Controllers
{
    public class TasksController : Controller
    {
        private readonly ICardService _cardService;
        private readonly IBusinessComment _businessComment;

        public TasksController(ICardService cardService, IBusinessComment businessComment)
        {
            _cardService = cardService;
            _businessComment = businessComment;
        }

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus([FromBody] CardStatusUpdateDto model)
        {
            if (model == null || model.CardId <= 0 || model.NewListId <= 0)
                return BadRequest("Datos inválidos");

            var boardId = await _cardService.UpdateCardListAsync(model.CardId, model.NewListId);

            if (boardId.HasValue)
                return Ok();

            return BadRequest("No se pudo actualizar el estado.");
        }
        #endregion

        #region Comments

        [HttpGet]
        public async Task<IActionResult> GetComments(int cardId)
        {
            var comments = await _businessComment.GetAllCommentsAsync();
            var filtered = comments
                .Where(c => c.CardId == cardId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new
                {
                    c.CommentId,
                    c.CardId,
                    c.CommentText,
                    CreatedAt = c.CreatedAt.HasValue ? c.CreatedAt.Value.ToString("o") : null, // formato ISO para JS
                    CreatedBy = c.CreatedBy ?? "Anon"
                })
                .ToList();

            return Json(filtered);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment([FromBody] Comment model)
        {
            if (string.IsNullOrWhiteSpace(model.CommentText))
                return BadRequest("Comentario vacío");

            var username = TempData["User"]?.ToString();
            TempData.Keep("User");

            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            model.CreatedAt = DateTime.Now;
            model.CreatedBy = username;

            var success = await _businessComment.SaveCommentAsync(model);

            if (success)
                return Ok();

            return StatusCode(500, "No se pudo guardar el comentario");
        }
        #endregion
    }
}
