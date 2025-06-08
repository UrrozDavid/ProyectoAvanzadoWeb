using TBA.Models.Entities;

internal interface IBusinessAttachment
{
    Task<IEnumerable<Attachment>> GetAllAttachmentsAsync();
    Task<bool> SaveAttachmentAsync(Attachment attachment);
    Task<bool> DeleteAttachmentAsync(Attachment attachment);
    Task<Attachment> GetAttachmentAsync(int id);
}