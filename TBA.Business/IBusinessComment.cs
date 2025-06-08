using TBA.Models.Entities;

internal interface IBusinessComment
{
    Task<IEnumerable<Comment>> GetAllCommentsAsync();
    Task<bool> SaveCommentAsync(Comment comment);
    Task<bool> DeleteCommentAsync(Comment comment);
    Task<Comment> GetCommentAsync(int id);
}