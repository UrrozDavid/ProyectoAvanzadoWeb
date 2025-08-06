using Microsoft.AspNetCore.Mvc;
using TBA.Business;
using TBA.Models.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ListController(IBusinessList businessList) : ControllerBase
    {
        [HttpGet(Name = "GetLists")]
        public async Task<IEnumerable<List>> GetLists()
        {
            return await businessList.GetAllListsAsync();
        }

        [HttpGet("{id}")]
        public async Task<List> GetById(int id)
        {
            var list = await businessList.GetListAsync(id);
            return list;
        }


        [HttpPost]
        public async Task<bool> Save([FromBody] IEnumerable<List> lists)
        {
            foreach (var item in lists)
            {
                await businessList.SaveListAsync(item);
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
        public async Task<bool> Delete(int id)
        {
            return await businessList.DeleteListAsync(id);
        }
    }
}
