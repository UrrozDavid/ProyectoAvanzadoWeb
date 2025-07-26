using Microsoft.AspNetCore.Mvc;
using TBA.Models.Entities;
using TBA.Services;

namespace TBA.Mvc.Controllers
{
    public class BoardsController : Controller
    {
        private readonly BoardService _boardService;

        public BoardsController(BoardService boardService)
        {
            _boardService = boardService;
        }

        // GET: Boards
        public async Task<IActionResult> Index()
        {
            var boards = await _boardService.GetAllBoardsAsync();
            return View(boards);
        }

        // GET: Boards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Boards/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Board model)
        {
            if (ModelState.IsValid)
            {
                var success = await _boardService.SaveBoardAsync(model);
                if (success)
                    return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Boards/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var board = await _boardService.GetBoardByIdAsync(id);
            if (board == null) return NotFound();

            return View(board);
        }

        // POST: Boards/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Board model)
        {
            if (ModelState.IsValid)
            {
                var success = await _boardService.SaveBoardAsync(model);
                if (success)
                    return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

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

        // GET: Boards/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var board = await _boardService.GetBoardByIdAsync(id);
            if (board == null)
                return NotFound();

            // Si necesitas cargar propiedades relacionadas, por ejemplo, usuarios o miembros:
            // board.Members = await repositoryBoardMember.FindByBoardIdAsync(board.BoardId);

            return View(board);
        }
    }
}
