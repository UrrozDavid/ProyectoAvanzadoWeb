using Microsoft.AspNetCore.Mvc;
using TBA.Business;
using TBA.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TBA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoardMemberController : ControllerBase
    {
        private readonly IBusinessBoardMember _businessBoardMember;

        public BoardMemberController(IBusinessBoardMember businessBoardMember)
        {
            _businessBoardMember = businessBoardMember;
        }

        // GET: api/BoardMember
        [HttpGet]
        public async Task<IEnumerable<BoardMember>> GetBoardMembers()
        {
            return await _businessBoardMember.GetAllBoardMembersWithIncludesAsync();
        }

        // GET: api/BoardMember/{boardId}/{userId}
        [HttpGet("{boardId:int}/{userId:int}")]
        public async Task<ActionResult<BoardMember>> GetById(int boardId, int userId)
        {
            var boardMember = await _businessBoardMember.GetBoardMemberAsync(boardId, userId);
            if (boardMember == null)
                return NotFound();

            return Ok(boardMember);
        }

        // POST: api/BoardMember
        [HttpPost]
        public async Task<ActionResult> Save([FromBody] IEnumerable<BoardMember> boardMembers)
        {
            if (boardMembers == null)
                return BadRequest();

            foreach (var item in boardMembers)
            {
                var result = await _businessBoardMember.SaveBoardMemberAsync(item);
                if (!result)
                    return StatusCode(500, "Error saving one or more BoardMembers.");
            }

            return Ok();
        }

        // DELETE: api/BoardMember/{boardId}/{userId}
        [HttpDelete("{boardId:int}/{userId:int}")]
        public async Task<ActionResult> Delete(int boardId, int userId)
        {
            var boardMember = await _businessBoardMember.GetBoardMemberAsync(boardId, userId);
            if (boardMember == null)
                return NotFound();

            var result = await _businessBoardMember.DeleteBoardMemberAsync(boardMember);
            if (!result)
                return StatusCode(500, "Error deleting the BoardMember.");

            return NoContent();
        }
    }
}
