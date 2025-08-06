using Microsoft.AspNetCore.Mvc;
using TBA.Models.Entities;
using TBA.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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

      public async Task<IActionResult> Create()
        {
            var boards = await _boardMemberService.GetAllBoardsAsync();
            var users = await _boardMemberService.GetAllUsersAsync();

            ViewBag.BoardId = new SelectList(boards, "BoardId", "Name");
            ViewBag.UserId = new SelectList(users, "UserId", "Email");

            ViewData["Title"] = "Create Board Member";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BoardMember model)
        {
            if (string.IsNullOrEmpty(model.Role))
            {
                model.Role = "member";
            }

            if (ModelState.IsValid)
            {
                var exists = await _boardMemberService.GetBoardMemberAsync(model.BoardId, model.UserId);
                if (exists != null)
                {
                    ModelState.AddModelError("", "Este usuario ya es miembro de este board.");
                }
                else
                {
                    var success = await _boardMemberService.SaveBoardMemberAsync(model);
                    if (success)
                    {
                        TempData["Success"] = "Board Member creado exitosamente.";
                        return RedirectToAction(nameof(Index));
                    }

                    ModelState.AddModelError("", "No se pudo guardar el miembro del board.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Formulario inválido. Verifica los datos.");

                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine("Validation error: " + error.ErrorMessage);
                }
            }

            var boards = await _boardMemberService.GetAllBoardsAsync();
            var users = await _boardMemberService.GetAllUsersAsync();

            ViewBag.BoardId = new SelectList(boards, "BoardId", "Name", model.BoardId);
            ViewBag.UserId = new SelectList(users, "UserId", "Email", model.UserId);
            ViewData["Title"] = "Create Board Member";

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int boardId, int userId)
        {
            var boardMember = await _boardMemberService.GetBoardMemberAsync(boardId, userId);

            if (boardMember == null)
                return NotFound();

            return View(boardMember);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BoardMember model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _boardMemberService.SaveBoardMemberAsync(model);
            if (!success)
                return BadRequest("No se pudo guardar el miembro del board.");

            return RedirectToAction(nameof(Index));
        }



        /// GET: BoardMembers/Delete?boardId=1&userId=2
        public async Task<IActionResult> Delete(int boardId, int userId)
        {
            var member = await _boardMemberService.GetBoardMemberWithDetailsAsync(boardId, userId);
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


        public async Task<IActionResult> Details(int boardId, int userId)
        {
            var boardMember = await _boardMemberService.GetBoardMemberWithDetailsAsync(boardId, userId);

            if (boardMember == null)
            {
                return NotFound();
            }

            return View(boardMember);
        }
    }
}
