using TBA.Models.Entities;
using TBA.Repositories;
using TBA.Core.Extensions;


namespace TBA.Business
{
    public interface IBusinessAttachment
    {
        Task<IEnumerable<Attachment>> GetAllAttachmentsAsync();
        Task<bool> SaveAttachmentAsync(Attachment attachment);
        Task<bool> DeleteAttachmentAsync(Attachment attachment);
        Task<Attachment> GetAttachmentAsync(int id);
    }

    public class BusinessAttachment(IRepositoryAttachment repositoryAttachment) : IBusinessAttachment
    {
        public async Task<IEnumerable<Attachment>> GetAllAttachmentsAsync()
        {
            return await repositoryAttachment.ReadAsync();
        }

        public async Task<bool> SaveAttachmentAsync(Attachment attachment)
        {
            var newBoard = ""; //Identity
            attachment.AddAudit(newBoard);
            attachment.AddLogging(attachment.AttachmentId<= 0 ? Models.Enums.LoggingType.Create: Models.Enums.LoggingType.Update);
            var exists = await repositoryAttachment.ExistsAsync(attachment);
            return await repositoryAttachment.UpsertAsync(attachment, exists);
        }

        public async Task<bool> DeleteAttachmentAsync(Attachment attachment)
        {
            return await repositoryAttachment.DeleteAsync(attachment);
        }

        public async Task<Attachment> GetAttachmentAsync(int id)
        {
            return await repositoryAttachment.FindAsync(id);
        }

        public Task<IEnumerable<Attachment>> GetAllAttachments()
        {
            throw new NotImplementedException();
        }
    }
}

