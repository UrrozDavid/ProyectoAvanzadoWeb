using Microsoft.AspNetCore.Mvc;
using TBA.Business;
using TBA.Models.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class BoardMemberController(IBusinessBoardMember businessBoardMember) : ControllerBase
    {
        [HttpGet(Name = "GetBoardMembers")]
        public async Task<IEnumerable<BoardMember>> GetBoardMembers()
        {
            return await businessBoardMember.GetAllBoardMembersAsync();
        }

        [HttpGet("{id}")]
        public async Task<BoardMember> GetById(int id)
        {
            var boardMember = await businessBoardMember.GetBoardMemberAsync(id);
            return boardMember;
        }


        [HttpPost]
        public async Task<bool> Save([FromBody] IEnumerable<BoardMember> boardMembers)
        {
            foreach (var item in boardMembers)
            {
                await businessBoardMember.SaveBoardMemberAsync(item);
            }
            return true;
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(BoardMember boardMember)
        {
            return await businessBoardMember.DeleteBoardMemberAsync(boardMember);
        }
    }
}
