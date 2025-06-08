using TBA.Models.Entities;
using TBA.Repositories;
using TBA.Core.Extensions;


namespace TBA.Business
{
    public interface IBusinessBoardMember
    {
        Task<IEnumerable<BoardMember>> GetAllBoardMembersAsync();
        Task<bool> SaveBoardMemberAsync(BoardMember boardMember);
        Task<bool> DeleteBoardMemberAsync(BoardMember boardMember);
        Task<BoardMember> GetBoardMemberAsync(int id);
    }

    public class BusinessBoardMember(IRepositoryBoardMember repositoryBoardMember) : IBusinessBoardMember
    {
        public async Task<IEnumerable<BoardMember>> GetAllBoardMembersAsync()
        {
            return await repositoryBoardMember.ReadAsync();
        }

        public async Task<bool> SaveBoardMemberAsync(BoardMember boardMember)
        {
            var newBoardMember = ""; //Identity
            boardMember.AddAudit(newBoardMember);
            boardMember.AddLogging(boardMember.BoardId<= 0 ? Models.Enums.LoggingType.Create: Models.Enums.LoggingType.Update);
            var exists = await repositoryBoardMember.ExistsAsync(boardMember);
            return await repositoryBoardMember.UpsertAsync(boardMember, exists);
        }

        public async Task<bool> DeleteBoardMemberAsync(BoardMember boardMember)
        {
            return await repositoryBoardMember.DeleteAsync(boardMember);
        }

        public async Task<BoardMember> GetBoardMemberAsync(int id)
        {
            return await repositoryBoardMember.FindAsync(id);
        }

        public Task<IEnumerable<BoardMember>> GetAllBoardMembers()
        {
            throw new NotImplementedException();
        }
    }
}

