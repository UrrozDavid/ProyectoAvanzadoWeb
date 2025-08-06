using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TBA.Data.Models;
using TBA.Models.Entities;
using TBA.Mvc.Models; 
using TBA.Repositories;
using TBA.Services;

namespace TBA.Mvc.Controllers
{
    public class ListsController : Controller
    {
        private readonly ListService _listService;
        private readonly IRepositoryBoard repositoryBoard;
        private readonly TrelloDbContext _context;


        public ListsController(ListService listService, IRepositoryBoard boardRepository,TrelloDbContext context)
        {
            _listService = listService;
            this.repositoryBoard = boardRepository;
            _context = context;

        }

        public async Task<IActionResult> Index()
        {
            var lists = (await _listService.GetAllListsAsync()).ToList();
            foreach (var list in lists)
            {
                list.Board = list.BoardId.HasValue ? await repositoryBoard.FindAsync(list.BoardId.Value) : null;
            }

            return View(lists);
        }

        public async Task<IActionResult> Create()
        {
            var boards = await repositoryBoard.ReadAsync();
            ViewBag.BoardId = new SelectList(boards, "BoardId", "Name");

            return View();
        }

        // POST: Lists/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List model)
        {
            if (ModelState.IsValid)
            {
                var success = await _listService.SaveListAsync(model);
                if (success)
                    return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Lists/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var list = await _listService.GetListByIdAsync(id);
            if (list == null) return NotFound();
            var boards = await repositoryBoard.ReadAsync();
            ViewBag.BoardId = new SelectList(boards, "BoardId", "Name", list.BoardId);

            return View(list);
        }

        // POST: Lists/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(List model)
        {
            if (ModelState.IsValid)
            {
                var success = await _listService.SaveListAsync(model);
                if (success)
                    return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Lists/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var list = await _listService.GetListWithBoardAsync(id);

            if (list == null)
                return NotFound();

            return View(list);
        }

        // POST: Lists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Elimina primero las cards asociadas a la lista
            var cards = await _context.Cards.Where(c => c.ListId == id).ToListAsync();
            _context.Cards.RemoveRange(cards);

            // Ahora elimina la lista
            var list = await _context.Lists.FindAsync(id);
            if (list == null) return NotFound();

            _context.Lists.Remove(list);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Lists/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var list = await _listService.GetListByIdAsync(id);
            if (list == null)
                return NotFound();
            list.Board = list.BoardId.HasValue ? await repositoryBoard.FindAsync(list.BoardId.Value) : null;
            return View(list);
        }
    }
}
