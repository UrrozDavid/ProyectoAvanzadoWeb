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

    }
}
