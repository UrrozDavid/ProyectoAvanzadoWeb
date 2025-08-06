using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TBA.Models.Entities;
using TBA.Services;

namespace TBA.Mvc.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly NotificationService _notificationService;
        private readonly IUserService _userService;
        private readonly ICardService _cardService;

        public NotificationsController(
            NotificationService notificationService,
            IUserService userService,
            ICardService cardService)
        {
            _notificationService = notificationService;
            _userService = userService;
            _cardService = cardService;
        }

        // GET: Notifications
        public async Task<IActionResult> Index()
        {
            var notifications = (await _notificationService.GetAllNotificationsAsync()).ToList();
            foreach (var n in notifications)
            {
                if (n.UserId != null)
                    n.User = await _userService.GetByIdAsync(n.UserId.Value);
                if (n.CardId != null)
                    n.Card = await _cardService.GetCardByIdAsync(n.CardId.Value);
            }

            return View(notifications);
        }

        // GET: Notifications/Create
        public async Task<IActionResult> Create()
        {
            await PopulateSelectLists();

            return View(new Notification());
        }

        // POST: Notifications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Notification model)
        {
            if (ModelState.IsValid)
            {
                var success = await _notificationService.SaveNotificationAsync(model);
                if (success)
                    return RedirectToAction(nameof(Index));
            }

            await PopulateSelectLists();
            return View(model);
        }

        // GET: Notifications/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null)
                return NotFound();

            await PopulateSelectLists(notification);
            return View(notification);
        }

        // POST: Notifications/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Notification model)
        {
            if (ModelState.IsValid)
            {
                var success = await _notificationService.UpdateNotificationAsync(model);
                if (success)
                    return RedirectToAction(nameof(Index));
            }

            await PopulateSelectLists(model);
            return View(model);
        }

        // GET: Notifications/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null)
                return NotFound();

            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _notificationService.DeleteNotificationAsync(id);
            if (!success)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }
        // GET: Notifications/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null)
                return NotFound();

            if (notification.UserId != null)
                notification.User = await _userService.GetByIdAsync(notification.UserId.Value);
            if (notification.CardId != null)
                notification.Card = await _cardService.GetCardByIdAsync(notification.CardId.Value);

            return View(notification);
        }

        // Helpers
        private async Task PopulateSelectLists(Notification notification = null)
        {
            var users = await _userService.GetAllAsync() ?? new List<User>();
            var cards = await _cardService.GetAllCardsAsync() ?? new List<Card>();
            ViewBag.Users = users;
            ViewBag.Cards = cards;
        }
    }
}
