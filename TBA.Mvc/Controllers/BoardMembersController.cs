using Microsoft.AspNetCore.Mvc;
using TBA.Models.Entities;
using TBA.Services;

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
            var boardMembers = await _boardMemberService.GetAllBoardMembersAsync();
            return View(boardMembers);
        }

        // GET: BoardMembers/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var boardMember = await _boardMemberService.GetBoardMemberAsync(id);
            if (boardMember == null)
                return NotFound();

            return View(boardMember);
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

        // GET: BoardMembers/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var boardMember = await _boardMemberService.GetBoardMemberAsync(id);
            if (boardMember == null)
                return NotFound();

            return View(boardMember);
        }

        // POST: BoardMembers/Edit/5
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

        // GET: BoardMembers/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var boardMember = await _boardMemberService.GetBoardMemberAsync(id);
            if (boardMember == null)
                return NotFound();

            return View(boardMember);
        }

        // POST: BoardMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _boardMemberService.DeleteBoardMemberAsync(id);
            if (!success)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}