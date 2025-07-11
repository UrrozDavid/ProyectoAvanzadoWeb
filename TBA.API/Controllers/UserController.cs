using Microsoft.AspNetCore.Mvc;
using TBA.Business;
using TBA.Models.DTOs;
using TBA.Models.Entities;

namespace TBA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class UserController(IBusinessUser businessUser) : Controller
    {

        [HttpGet(Name = "GetUsers")]
        public async Task<IEnumerable<User>> GetUsers()
        {
            return await businessUser.GetAllUsersAsync();
        }

        [HttpGet("{id}")]
        public async Task<User> GetById(int id)
        {
            return await businessUser.GetUserAsync(id);
        }

        [HttpPost]
        public async Task<bool> Save([FromBody] IEnumerable<User> users)
        {
            foreach (var item in users)
            {
                await businessUser.SaveUserAsync(item);
            }
            return true;
        }

        [HttpPut("{id}")]
        public async Task<bool> Update(int id, [FromBody] User user)
        {
            user.UserId = id;
            return await businessUser.SaveUserAsync(user);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id)
        {
            var user = await businessUser.GetUserAsync(id);
            if (user == null) return false;

            return await businessUser.DeleteUserAsync(user);
        }

        [HttpPost("auth")]
        public async Task<IActionResult> Authenticate([FromBody] LoginDTO loginDTO)
        {
            var user = await businessUser.AuthenticateAsync(loginDTO.Email, loginDTO.Password);

            if (user == null) return Unauthorized();
            return Ok(user);

        }

        [HttpPut("password")]
        public async Task<IActionResult> UpdatePassword([FromBody] SetNewPasswordDTO dto)
        {
            var user = await businessUser.GetUserByEmail(dto.Email);

            if (user == null) return NotFound("Email not found.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            var success = await businessUser.SaveUserAsync(user);

            return success ? NoContent() : StatusCode(500, "Could not update password.");
        }
    }
}
