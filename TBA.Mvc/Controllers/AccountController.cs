using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TBA.Architecture.Providers;
using TBA.Models.DTOs;
using TBA.Mvc.Models;
using TBA.Services;


namespace TBA.Mvc.Controllers
{
    public class AccountController(IUserService userService, EmailProvider emailProvider) : Controller
    {

        private readonly IUserService _userService = userService;
        private readonly EmailProvider _emailProvider = emailProvider;

        #region Login
        // GET: Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await userService.AuthenticateAsync(model.Email, model.Password);

                if (user == null)
                {
                    ModelState.AddModelError("", "Usermane or password incorrect(s)");
                    return View(model);
                }

                var claims = new List<Claim>
                {
                  new Claim(ClaimTypes.Name, user.Username),
                  new Claim(ClaimTypes.Email, user.Email)
                };

                var identity = new ClaimsIdentity(claims, "login");
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(principal);

                TempData["User"] = user.Username;

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }
        }
        #endregion

        #region Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await userService.RegisterAsync(model);

            if (!result.success)
            {
                ModelState.AddModelError("", result.ErrorMessage ?? "An unexpected error occurred");
                return View(model);
            }

            return RedirectToAction("Login");
        }
        #endregion

        #region ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Checks if email exists in database
            if (!await userService.ExistsEmail(model.Email))
            {
                ModelState.AddModelError("", "This email does not exist.");
                return View(model);
            }

            // Generate a temporary password
            var tempPassword = userService.GenerateTemporaryPassword();
            var success = await userService.UpdatePasswordAsync(model.Email, tempPassword);

            if (!success)
            {
                ModelState.AddModelError("", "Coud not generate a temporary password");
                return View(model);
            }

            // Preparing the email
            string subject = "Temporary Password";
            string body = $"<p>Hello,<br/><br/>Here is your temporary password: <strong>{tempPassword}</strong></p>";

            // Sending email
            bool isSent = await _emailProvider.SendEmailAsync(model.Email, subject, body);

            if (!isSent)
            {
                ModelState.AddModelError("", "Faild to send email. Try again later.");
                return View(model);
            }

            TempData["Toast"] = "A temporary password has been sent to your email";
            TempData["RedirectTo"] = Url.Action("ResetPassword", new { email = model.Email });

            return RedirectToAction("ForgotPassword");
        }


        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            return View(new ResetPasswordViewModel { Email = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "New password and confirmation do not match");
                return View(model);
            }

            var user = await userService.GetUserByEmail(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View(model);
            }

            // Checks if temporary password == current hashed password in Database
            bool tempPasswordMatches = BCrypt.Net.BCrypt.Verify(model.TempPassword, user.PasswordHash);

            if (!tempPasswordMatches)
            {
                ModelState.AddModelError("", "Invalid password(s)");
                return View(model);
            }

            // Update password
            var success = await userService.UpdatePasswordAsync(model.Email, model.NewPassword);

            if (!success)
            {
                ModelState.AddModelError("", "Could not update password");
                return View(model);
            }

            TempData["Toast"] = "Password has been updated successfully";
            TempData["RedirectTo"] = Url.Action("Login");
            return RedirectToAction("ResetPassword", new { email = model.Email });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        #endregion
    }
}
