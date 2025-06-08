
using TBA.Models.Entities;
using TBA.Business;

namespace TBA.Services
{
    public class AttachmentService
    {
        private readonly IBusinessAttachment _businessAttachment;

        public AttachmentService(IBusinessAttachment businessAttachment)
        {
            _businessAttachment = businessAttachment;
        }

        public async Task<IEnumerable<Attachment>> GetAllAttachmentsAsync()
            => await _businessAttachment.GetAllAttachmentsAsync();

        public async Task<Attachment?> GetAttachmentByIdAsync(int id)
            => await _businessAttachment.GetAttachmentAsync(id);

        public async Task<bool> SaveAttachmentAsync(Attachment attachment)
            => await _businessAttachment.SaveAttachmentAsync(attachment);

        public async Task<bool> DeleteAttachmentAsync(int id)
        {
            var attachment = await _businessAttachment.GetAttachmentAsync(id);
            if (attachment == null) return false;
            return await _businessAttachment.DeleteAttachmentAsync(attachment);
        }
    }
}