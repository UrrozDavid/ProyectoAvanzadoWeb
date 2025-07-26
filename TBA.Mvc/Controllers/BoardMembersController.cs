using Microsoft.AspNetCore.Mvc;
using TBA.Models.Entities;
using TBA.Services;
using System.Threading.Tasks;

namespace TBA.Mvc.Controllers
{
    public class BoardMembersController : Controller
    {
        private readonly BoardMemberService _boardMemberService;

        public BoardMembersController(BoardMemberService boardMemberService)
        {
            _boardMemberService = boardMemberService;
        }

        // GET: BoardMembers
        public async Task<IActionResult> Index()
        {
            var members = await _boardMemberService.GetAllBoardMembersAsync();
            return View(members);
        }

        // GET: BoardMembers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BoardMembers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BoardMember model)
        {
            if (ModelState.IsValid)
            {
                var success = await _boardMemberService.SaveBoardMemberAsync(model);
                if (success)
                    return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: BoardMembers/Edit?boardId=1&userId=2
        public async Task<IActionResult> Edit(int boardId, int userId)
        {
            var member = await _boardMemberService.GetBoardMemberAsync(boardId, userId);
            if (member == null) return NotFound();

            return View(member);
        }

        // POST: BoardMembers/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BoardMember model)
        {
            if (ModelState.IsValid)
            {
                var success = await _boardMemberService.SaveBoardMemberAsync(model);
                if (success)
                    return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: BoardMembers/Delete?boardId=1&userId=2
        public async Task<IActionResult> Delete(int boardId, int userId)
        {
            var member = await _boardMemberService.GetBoardMemberAsync(boardId, userId);
            if (member == null) return NotFound();

            return View(member);
        }

        // POST: BoardMembers/DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int boardId, int userId)
        {
            var success = await _boardMemberService.DeleteBoardMemberAsync(boardId, userId);
            if (!success) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // GET: BoardMembers/Details?boardId=1&userId=2
        public async Task<IActionResult> Details(int boardId, int userId)
        {
            var member = await _boardMemberService.GetBoardMemberAsync(boardId, userId);
            if (member == null)
                return NotFound();

            // Cargar propiedades relacionadas si es necesario
            // member.Board = await repositoryBoard.FindAsync(member.BoardId);
            // member.User = await repositoryUser.FindAsync(member.UserId);

            return View(member);
        }
    }
}
