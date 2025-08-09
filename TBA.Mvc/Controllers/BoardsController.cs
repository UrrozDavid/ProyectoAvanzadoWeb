using Microsoft.AspNetCore.Mvc;
using TBA.Business;
using TBA.Models.Entities;
using TBA.Mvc.Models;
using TBA.Services;
using TBA.Models.DTOs;

namespace TBA.Mvc.Controllers
{
    public class BoardsController : Controller
    {
        private readonly BoardService _boardService;
        private readonly IUserService _userService;
        private readonly IBusinessBoard _businessBoard;


        public BoardsController(BoardService boardService, IUserService userService, IBusinessBoard businessBoard)
        {
            _boardService = boardService;
            _userService = userService;
            _businessBoard = businessBoard;
        }

        #region Index
        // GET: Boards
        public async Task<IActionResult> Index()
        {
            var username = User?.Identity?.Name ?? (TempData["User"] as string);
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var me = await _userService.GetByUsernameAsync(username);
            if (me is null) return Unauthorized();

            var boards = await _boardService.GetBoardsForUserAsync(me.UserId);
            return View(boards);
        }
        #endregion

        #region Create
        // GET: Boards/Create
        public IActionResult Create() => View(new BoardCreateViewModel());


        // POST: Boards/Create
        // Creates a Board with Lists and Members
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BoardCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Obtenemos usuario actual
            var username = User?.Identity?.Name ?? (TempData["User"] as string);
            if (string.IsNullOrWhiteSpace(username))
            {
                ModelState.AddModelError("", "No authenticated user.");
                return View(model);
            }

            var me = await _userService.GetByUsernameAsync(username);

            // Normalizar y Validar List
            var lists = (model.Lists ?? new List<ListInput>())
                .Where(list => !string.IsNullOrWhiteSpace(list.Name))
                .Select((list, i) => (list.Name.Trim(), Position: list.Position ?? (i + 1)))
                .ToList();

            if (lists.Count == 0)
            {
                ModelState.AddModelError("", "Add at least one list");
                return View(model);
            }

            // BoardMember por email de usuario
            var emails = (model.MemberEmail ?? "")
                .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(e => e.ToLower())
                .Distinct()
                .ToHashSet();

            var allUsers = await _userService.GetAllAsync();
            var memberIds = allUsers
                .Where(u => emails.Contains(u.Email.ToLower()))
                .Select(u => u.UserId)
                .ToList();

            // Creación
            var boardId = await _businessBoard.SaveBoardAsync(
                creatorUserId: me.UserId,
                name: model.Name.Trim(),
                description: model.Description?.Trim(),
                lists: lists,
                memberUserIds: memberIds
            );
            

            return RedirectToAction("Index", "Tasks", new {boardId});

        }
        #endregion

        #region Edit
        // GET: Boards/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var board = await _boardService.GetBoardByIdAsync(id);
            if (board == null) return NotFound();

            var vm = new BoardEditViewModel
            {
                BoardId = board.BoardId,
                Name = board.Name,
                Description = board.Description,

                Lists = board.Lists?
                    .OrderBy(l => l.Position)
                    .Select(l => new ListEditDTO
                    {
                        ListId = l.ListId,
                        Name = l.Name,
                        Position = l.Position ?? 0
                    }).ToList() ?? new(),

                CurrentMembers = board.BoardMembers?
                    .Where(bm => bm.User != null)
                    .Select(bm => new MemberDTO
                    {
                        UserId = bm.UserId,
                        Username = bm.User!.Username,
                        Email = bm.User!.Email
                    }).ToList() ?? new()
            };

            return View(vm);
        }

        // POST: Boards/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BoardEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var lists = (model.Lists ?? new())
                .Where(x => !x.Remove && !string.IsNullOrWhiteSpace(x.Name))
                .Select((x, i) => new ListEditDTO
                {
                    ListId = x.ListId,
                    Name = x.Name.Trim(),
                    Position = x.Position > 0 ? x.Position : i + 1
                })
                .ToList();

            var deletes = model.Lists?
                .Where(l => l.Remove && l.ListId.HasValue)
                .Select(l => l.ListId!.Value)
                .ToList() ?? new List<int>();

            var emails = (model.MemberEmailsToAdd ?? "")
                .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(e => e.ToLower())
                .Distinct()
                .ToList();

            var ok = await _businessBoard.UpdateBoardAsync(
                model.BoardId,
                model.Name.Trim(),
                model.Description?.Trim(),
                lists,
                deletes,
                emails
            );

            if (!ok)
            {
                ModelState.AddModelError("", "No se pudieron guardar los cambios del tablero.");
                return View(model);
            }

            return RedirectToAction("Index", "Tasks", new { boardId = model.BoardId });
        }
        #endregion

        #region Delete
        // GET: Boards/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var board = await _boardService.GetBoardByIdAsync(id);
            if (board == null) return NotFound();

            return View(board);
        }

        // POST: Boards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _boardService.DeleteBoardAsync(id);
            if (!success) return NotFound();

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region View Details
        // GET: Boards/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var board = await _boardService.GetBoardByIdAsync(id);
            if (board == null)
                return NotFound();

            return View(board);
        }
        #endregion
    }
}
