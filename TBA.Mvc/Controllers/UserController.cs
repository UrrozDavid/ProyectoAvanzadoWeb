using Microsoft.AspNetCore.Mvc;
using TBA.Models.Entities;
using TBA.Mvc.Models;
using TBA.Services;

namespace TBA.Mvc.Controllers
{
    public class UserController(IUserService userService) : Controller
    {

        // GET: User
        // GET: User
        public async Task<IActionResult> Index()
        {
            var users = await userService.GetAllAsync();

            var model = users.Select(u => new UserViewModel
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                PasswordHash = u.PasswordHash
            }).ToList();

            return View(model);
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
            if (!ModelState.IsValid)
                return View(model);

            
            var existingUser = await userService.GetByIdAsync(model.UserId);
            if (existingUser == null)
                return NotFound();

            existingUser.Username = model.Username!;
            existingUser.Email = model.Email!;

            
            if (!string.IsNullOrEmpty(model.PasswordHash))
            {
                existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash!);
            }

            var success = await userService.UpdateAsync(existingUser);
            if (success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Could not update user.");
            return View(model);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var user = await userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            var model = new UserViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                PasswordHash = user.PasswordHash
            };

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
