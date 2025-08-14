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
        Task<bool> UpdateCommentAsync(Comment comment);


    }

    public class BusinessComment : IBusinessComment
    {
        private readonly IRepositoryComment _repositoryComment;
        private readonly IBusinessNotification _businessNotification;
        private readonly IRepositoryCard _repositoryCard;

        public BusinessComment(
        IRepositoryComment repositoryComment,
        IBusinessNotification businessNotification,
        IRepositoryCard repositoryCard)
        {
            _repositoryComment = repositoryComment;
            _businessNotification = businessNotification;
            _repositoryCard = repositoryCard;
        }

        public async Task<IEnumerable<Comment>> GetAllCommentsAsync()
        {
            return await _repositoryComment.ReadAsync();
        }

        public async Task<bool> SaveCommentAsync(Comment comment)
        {
            var newCommentUser = ""; // Identity del usuario que crea el comentario
            comment.AddAudit(newCommentUser);
            comment.AddLogging(comment.CommentId <= 0 ? Models.Enums.LoggingType.Create : Models.Enums.LoggingType.Update);

            // Guardar o actualizar el comentario
            var exists = await _repositoryComment.ExistsAsync(comment);
            var result = await _repositoryComment.UpsertAsync(comment, exists);

            if (result) // el comentario se guardó con éxito
            {
                // Verificar que CardId tenga valor
                if (comment.CardId.HasValue)
                {
                    // Traer la tarjeta con sus asignaciones
                    var card = await _repositoryCard.GetCardWithAssignmentsAsync(comment.CardId.Value);

                    if (card?.Assignments != null)
                    {
                        foreach (var assignment in card.Assignments)
                        {
                            // Crear notificación para cada usuario asignado
                            await _businessNotification.CreateNotificationAsync(new Notification
                            {
                                UserId = assignment.UserId,
                                CardId = card.CardId,
                                Message = $"Nuevo comentario en la tarjeta '{card.Title}': {comment.CommentText}",
                                Type = "CommentAdded",
                                RelatedId = comment.CommentId,
                                GroupName = "Comments",
                                IsRead = false,
                                NotifyAt = DateTime.UtcNow
                            });
                        }
                    }
                }
            }

            return result;
        }


        public async Task<bool> DeleteCommentAsync(Comment comment)
        {
            return await _repositoryComment.DeleteAsync(comment);
        }

        public async Task<Comment> GetCommentAsync(int id)
        {
            return await _repositoryComment.FindAsync(id);
        }

        public async Task<bool> UpdateCommentAsync(Comment comment)
        {
            var existingComment = await _repositoryComment.FindAsync(comment.CommentId);
            if (existingComment == null) return false;
            comment.AddAudit(existingComment.CreatedBy ?? string.Empty);
            comment.AddLogging(Models.Enums.LoggingType.Update);
            return await _repositoryComment.UpdateAsync(comment);
        }
    }
}

