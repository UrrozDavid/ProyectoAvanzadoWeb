using Microsoft.AspNetCore.Mvc;
using TBA.Business;
using TBA.Models.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class BoardController(IBusinessBoard businessBoard) : ControllerBase
    {
        [HttpGet(Name = "GetBoards")]
        public async Task<IEnumerable<Board>> GetBoards()
        {
            return await businessBoard.GetAllBoardsAsync();
        }

        [HttpGet("{id}")]
        public async Task<Board> GetById(int id)
        {
            var board = await businessBoard.GetBoardAsync(id);
            return board;
        }


        [HttpPost]
        public async Task<bool> Save([FromBody] IEnumerable<Board> boards)
        {
            foreach (var item in boards)
            {
                await businessBoard.SaveBoardAsync(item);
            }
            return true;
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(Board board)
        {
            return await businessBoard.DeleteBoardAsync(board);
        }
    }
}
