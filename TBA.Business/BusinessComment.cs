using TBA.Models.Entities;
using TBA.Repositories;
using TBA.Core.Extensions;


namespace TBA.Business
{
    public interface IBusinessComment
    {
        Task<IEnumerable<Comment>> GetAllCommentsAsync();
        Task<bool> SaveCommentAsync(Comment comment);
        Task<bool> DeleteCommentAsync(Comment comment);
        Task<Comment> GetCommentAsync(int id);
    }

    public class BusinessComment(IRepositoryComment repositoryComment) : IBusinessComment
    {
        public async Task<IEnumerable<Comment>> GetAllCommentsAsync()
        {
            return await repositoryComment.ReadAsync();
        }

        public async Task<bool> SaveCommentAsync(Comment comment)
        {
            var newComment = ""; //Identity
            comment.AddAudit(newComment);
            comment.AddLogging(comment.CommentId <= 0 ? Models.Enums.LoggingType.Create: Models.Enums.LoggingType.Update);
            var exists = await repositoryComment.ExistsAsync(comment);
            return await repositoryComment.UpsertAsync(comment, exists);
        }

        public async Task<bool> DeleteCommentAsync(Comment comment)
        {
            return await repositoryComment.DeleteAsync(comment);
        }

        public async Task<Comment> GetCommentAsync(int id)
        {
            return await repositoryComment.FindAsync(id);
        }

        public Task<IEnumerable<Label>> GetAllComments()
        {
            throw new NotImplementedException();
        }
    }
}

