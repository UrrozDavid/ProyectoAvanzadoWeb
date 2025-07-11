using Microsoft.AspNetCore.Mvc;
using TBA.Models.Entities;
using TBA.Mvc.Models;
using TBA.Services;

namespace TBA.Mvc.Controllers
{
    public class UserController(IUserService userService) : Controller
    {
        
        // GET: User
        public async Task<IActionResult> Index()
        {
            var users = await userService.GetAllAsync();
            return View(users);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Username = model.Username!,
                    Email = model.Email!,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash!),
                };

                var success = await userService.CreateAsync(user);
                if (success)
                    return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            var model = new UserViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                PasswordHash = user.PasswordHash
            };

            return View(model);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserId = model.UserId,
                    Username = model.Username!,
                    Email = model.Email!,
                    PasswordHash = model.PasswordHash!
                };

                var success = await userService.UpdateAsync(user);
                if (success)
                    return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var user = await userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            var model = new UserViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                PasswordHash = user.PasswordHash
            };

            return View(model);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await userService.DeleteAsync(id);
            if (!success) return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}
