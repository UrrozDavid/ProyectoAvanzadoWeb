using Microsoft.AspNetCore.Mvc;
using TBA.Business;
using TBA.Models.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AttachmentController(IBusinessAttachment businessAttachment) : ControllerBase
    {
        [HttpGet(Name = "GetAttachments")]
        public async Task<IEnumerable<Attachment>> GetBoards()
        {
            return await businessAttachment.GetAllAttachmentsAsync();
        }

        [HttpGet("{id}")]
        public async Task<Attachment> GetById(int id)
        {
            var attachment = await businessAttachment.GetAttachmentAsync(id);
            return attachment;
        }


        [HttpPost]
        public async Task<bool> Save([FromBody] IEnumerable<Attachment> attachments)
        {
            foreach (var item in attachments)
            {
                await businessAttachment.SaveAttachmentAsync(item);
            }
            return true;
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(Attachment attachment)
        {
            return await businessAttachment.DeleteAttachmentAsync(attachment);
        }
    }
}
