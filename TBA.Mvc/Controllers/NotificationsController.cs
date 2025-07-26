using Microsoft.AspNetCore.Mvc;
using TBA.Models.Entities;
using TBA.Services;

namespace TBA.Mvc.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly NotificationService _notificationService;

        public NotificationsController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: Notifications
        public async Task<IActionResult> Index()
        {
            var notifications = await _notificationService.GetAllNotificationsAsync();
            return View(notifications);
        }

        // GET: Notifications/Create
        public IActionResult Create()
        {
            return View();
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
            return View(model);
        }

        // GET: Notifications/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null) return NotFound();

            return View(notification);
        }

        // POST: Notifications/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Notification model)
        {
            if (ModelState.IsValid)
            {
                var success = await _notificationService.SaveNotificationAsync(model);
                if (success)
                    return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Notifications/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null) return NotFound();

            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _notificationService.DeleteNotificationAsync(id);
            if (!success) return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}
