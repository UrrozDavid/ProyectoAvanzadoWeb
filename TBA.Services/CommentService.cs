using System.Collections.Generic;
using System.Threading.Tasks;
using TBA.Models.Entities;
using TBA.Business;

namespace TBA.Services
{
    public class CommentService
    {
        private readonly IBusinessComment _businessComment;

        public CommentService(IBusinessComment businessComment)
        {
            _businessComment = businessComment;
        }

        public async Task<IEnumerable<Comment>> GetAllCommentsAsync()
            => await _businessComment.GetAllCommentsAsync();

        public async Task<Comment?> GetCommentByIdAsync(int id)
            => await _businessComment.GetCommentAsync(id);

        public async Task<bool> SaveCommentAsync(Comment comment)
            => await _businessComment.SaveCommentAsync(comment);

        public async Task<bool> DeleteCommentAsync(int id)
        {
            var comment = await _businessComment.GetCommentAsync(id);
            if (comment == null) return false;
            return await _businessComment.DeleteCommentAsync(comment);
        }

        public async Task<bool> UpdateCommentAsync(Comment comment)
            => await _businessComment.UpdateCommentAsync(comment);
    }
}