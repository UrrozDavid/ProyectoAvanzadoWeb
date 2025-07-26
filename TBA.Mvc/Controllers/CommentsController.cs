using Microsoft.AspNetCore.Mvc;
using TBA.Models.Entities;
using TBA.Services;

namespace TBA.Mvc.Controllers
{
    public class CommentsController : Controller
    {
        private readonly CommentService _commentService;

        public CommentsController(CommentService commentService)
        {
            _commentService = commentService;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var comments = await _commentService.GetAllCommentsAsync();
            return View(comments);
        }

        // GET: Comments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Comment model)
        {
            if (ModelState.IsValid)
            {
                var success = await _commentService.SaveCommentAsync(model);
                if (success)
                    return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null) return NotFound();

            return View(comment);
        }

        // POST: Comments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Comment model)
        {
            if (ModelState.IsValid)
            {
                var success = await _commentService.SaveCommentAsync(model);
                if (success)
                    return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null) return NotFound();

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _commentService.DeleteCommentAsync(id);
            if (!success) return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}
