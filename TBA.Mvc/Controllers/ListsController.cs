using Microsoft.AspNetCore.Mvc;
using TBA.Models.Entities;
using TBA.Services;
using TBA.Mvc.Models; // Aquí podrías tener tu ListViewModel si usas

namespace TBA.Mvc.Controllers
{
    public class ListsController : Controller
    {
        private readonly ListService _listService;

        public ListsController(ListService listService)
        {
            _listService = listService;
        }

        // GET: Lists
        public async Task<IActionResult> Index()
        {
            var lists = await _listService.GetAllListsAsync();
            return View(lists);
        }

        // GET: Lists/Create
        public IActionResult Create()
        {
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
            var list = await _listService.GetListByIdAsync(id);
            if (list == null) return NotFound();

            return View(list);
        }

        // POST: Lists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _listService.DeleteListAsync(id);
            if (!success) return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}
