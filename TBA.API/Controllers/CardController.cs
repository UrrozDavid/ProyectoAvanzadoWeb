using Microsoft.AspNetCore.Mvc;
using TBA.Business;
using TBA.Models.DTOs;
using TBA.Models.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class CardController(IBusinessCard businessCard) : ControllerBase
    {
        [HttpGet(Name = "GetCards")]
        public async Task<IEnumerable<Card>> GetCards()
        {
            return await businessCard.GetAllCardsAsync();
        }

        [HttpGet("{id}")]
        public async Task<Card> GetById(int id)
        {
            var card = await businessCard.GetCardAsync(id);
            return card;
        }
        [HttpGet("tasks")]
        public async Task<IEnumerable<TaskViewModel>> GetTasks()
        {
            var pro = await businessCard.GetTaskViewModelsAsync();
            return pro;
        }


        [HttpPost]
        public async Task<bool> Save([FromBody] IEnumerable<Card> cards)
        {
            foreach (var item in cards)
            {
                await businessCard.SaveCardAsync(item);
            }
            return true;
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(Card card)
        {
            return await businessCard.DeleteCardAsync(card);
        }
        [HttpPost("create-dto")]
        public async Task<IActionResult> SaveFromDto([FromBody] CardCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
                return BadRequest("Username is required.");

          
            var user = await businessCard.GetUserByUsernameAsync(dto.Username);
            if (user == null)
                return NotFound("User not found.");

            var card = new Card
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                CreatedAt = DateTime.Now,
                ListId = 1,
                Users = new List<User> { user } 
            };

            var result = await businessCard.SaveCardAsync(card);

            return Ok(result);
        }


    }
}
