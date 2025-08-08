using Microsoft.AspNetCore.Mvc;
using TBA.Models.DTOs;
using TBA.Models.Entities;
using TBA.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using TBA.Business; // Para IBusinessComment

namespace TBA.Mvc.Controllers
{
    public class TasksController : Controller
    {
        private readonly ICardService _cardService;
        private readonly ListService _listService;
        private readonly BoardService _boardService;
        private readonly IBusinessComment _businessComment;
        private readonly IUserService _userService;  // <-- Agrega esto
                                                     // Unifica ambos constructores en uno solo para evitar conflictos y asegurar que todos los servicios estén disponibles
        public TasksController(
            ICardService cardService,
            ListService listService,
            BoardService boardService,
            IBusinessComment businessComment,
            IUserService userService)  // <-- Inyecta aquí también
        {
            _cardService = cardService;
            _listService = listService;
            _boardService = boardService;
            _businessComment = businessComment;
            _userService = userService;
        }

        #region Index

        public async Task<IActionResult> Index(int boardId)
        {
            TempData.Keep("User");

            var allTasks = await _cardService.GetTasksAsync();
            var boardTasks = allTasks.Where(t => t.BoardId == boardId).ToList();

            ViewBag.Username = TempData["User"]?.ToString();
            return View(boardTasks);
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

        #region MoveCard

        // Mueve la tarjeta a otra lista (NO BORRAR)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveCard([FromBody] CardStatusUpdateDto model)
        {
            Console.WriteLine("CardId: " + model?.CardId);
            Console.WriteLine("NewListId: " + model?.NewListId);

            if (model == null || model.CardId <= 0 || model.NewListId <= 0)
                return BadRequest("Datos inválidos");

            var card = await _cardService.GetCardByIdAsync(model.CardId);
            if (card == null) return NotFound();

            card.ListId = model.NewListId;
            var success = await _cardService.SaveCardAsync(card);

            if (success)
                return Ok();

            return StatusCode(500, "No se pudo mover la tarjeta");
        }

        #endregion

        #region Create

        // GET: Muestra la vista de creación de tarjeta
        [HttpGet]
        public IActionResult Create()
        {
            // Aquí puedes preparar datos para la vista si lo necesitas
            return View();
        }

        // POST: Crea una tarjeta y la mueve a una lista específica
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] CardStatusUpdateDto model)
        {
            Console.WriteLine("CardId: " + model?.CardId);
            Console.WriteLine("NewListId: " + model?.NewListId);

            if (model == null || model.CardId <= 0 || model.NewListId <= 0)
                return BadRequest("Datos inválidos");

            var card = await _cardService.GetCardByIdAsync(model.CardId);
            if (card == null) return NotFound();

            card.ListId = model.NewListId;
            var success = await _cardService.SaveCardAsync(card);

            if (success)
                return Ok();

            return StatusCode(500, "No se pudo crear/mover la tarjeta");
        }
        #endregion

        #region Comments

        [HttpGet]
        public async Task<IActionResult> GetComments(int cardId)
        {
            var comments = await _businessComment.GetAllCommentsAsync();
            var users = await _userService.GetAllAsync();  // Traer todos los usuarios

            var filtered = comments
                .Where(c => c.CardId == cardId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new
                {
                    c.CommentId,
                    c.CardId,
                    c.CommentText,
                    CreatedAt = c.CreatedAt.HasValue ? c.CreatedAt.Value.ToString("o") : null,
                    CreatedBy = users.FirstOrDefault(u => u.UserId == c.UserId)?.Username ?? "Anon"
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

            // Busca el usuario completo para obtener el UserId
            var user = await _userService.GetByUsernameAsync(username);
            if (user == null)
                return Unauthorized();

            model.UserId = user.UserId;          // asigna UserId al comentario
            model.CreatedAt = DateTime.Now;
            model.CreatedBy = username;

            var success = await _businessComment.SaveCommentAsync(model);

            if (success)
                return Ok();

            return StatusCode(500, "No se pudo guardar el comentario");
        }

        #endregion
    }

    public class BoardViewViewModel
    {
        public required Board Board { get; set; }
        public required List<ListWithCardsViewModel> Lists { get; set; }
    }

    public class ListWithCardsViewModel
    {
        public required List List { get; set; }
        public required List<TaskViewModel> Cards { get; set; }
    }
}

