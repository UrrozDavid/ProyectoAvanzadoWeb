using Microsoft.AspNetCore.Mvc;
using TBA.Business;
using TBA.Models.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UserController(IBusinessUser businessUser) : ControllerBase
    {
        [HttpGet(Name = "GetUsers")]
        public async Task<IEnumerable<User>> GetUsers()
        {
            return await businessUser.GetAllUsersAsync();
        }

        [HttpGet("{id}")]
        public async Task<User> GetById(int id)
        {
            var user = await businessUser.GetUserAsync(id);
            return user;
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

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task<bool> Delete(User user)
        {
            return await businessUser.DeleteUserAsync(user);
        }
    }
}
