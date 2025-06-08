using Microsoft.AspNetCore.Mvc;
using TBA.Business;
using TBA.Models.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class CommentController(IBusinessComment businessComment) : ControllerBase
    {
        [HttpGet(Name = "GetComments")]
        public async Task<IEnumerable<Comment>> GetComments()
        {
            return await businessComment.GetAllCommentsAsync();
        }

        [HttpGet("{id}")]
        public async Task<Comment> GetById(int id)
        {
            var comment = await businessComment.GetCommentAsync(id);
            return comment;
        }


        [HttpPost]
        public async Task<bool> Save([FromBody] IEnumerable<Comment> comments)
        {
            foreach (var item in comments)
            {
                await businessComment.SaveCommentAsync(item);
            }
            return true;
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(Comment comment)
        {
            return await businessComment.DeleteCommentAsync(comment);
        }
    }
}
