using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TBA.Models.Entities;
using TBA.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TBA.Mvc.Controllers
{
    public class CommentsController : Controller
    {
        private readonly CommentService _commentService;
        private readonly IUserService _userService;
        private readonly ICardService _cardService;

        public CommentsController(
            CommentService commentService,
            IUserService userService,
            ICardService cardService)
        {
            _commentService = commentService;
            _userService = userService;
            _cardService = cardService;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {

            var comments = (await _commentService.GetAllCommentsAsync()).ToList();
            foreach (var cm in comments)
            {
                if (cm.UserId != null)
                    cm.User = await _userService.GetByIdAsync(cm.UserId.Value);

                if (cm.CardId != null)
                    cm.Card = await _cardService.GetCardByIdAsync(cm.CardId.Value);
            }

            return View(comments);
        }



        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null) return NotFound();


            if (comment.UserId != null)
                comment.User = await _userService.GetByIdAsync(comment.UserId.Value);
            if (comment.CardId != null)
                comment.Card = await _cardService.GetCardByIdAsync(comment.CardId.Value);

            return View(comment);
        }

        // GET: Comments/Create
        public async Task<IActionResult> Create()
        {
            await PopulateSelectLists();
            return View(new Comment());
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

            await PopulateSelectLists();
            return View(model);
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null) return NotFound();

            await PopulateSelectLists(comment);
            return View(comment);
        }

        // POST: Comments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Comment model)
        {
            if (ModelState.IsValid)
            {
                var success = await _commentService.UpdateCommentAsync(model);
                if (success)
                    return RedirectToAction(nameof(Index));
            }

            await PopulateSelectLists(model);
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

        // Helpers: 
        private async Task PopulateSelectLists(Comment comment = null)
        {
            var users = await _userService.GetAllAsync() ?? new List<User>();
            var cards = await _cardService.GetAllCardsAsync() ?? new List<Card>();

            ViewBag.Users = users;
            ViewBag.Cards = cards;
        }
    }
}