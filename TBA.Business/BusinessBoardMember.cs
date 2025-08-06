using System.Collections.Generic;
using System.Threading.Tasks;
using TBA.Models.Entities;
using TBA.Repositories;

namespace TBA.Business
{
    public interface IBusinessBoardMember
    {
        Task<IEnumerable<BoardMember>> GetAllBoardMembersWithIncludesAsync();
        Task<BoardMember?> GetBoardMemberAsync(int boardId, int userId);
        Task<bool> SaveBoardMemberAsync(BoardMember boardMember);
        Task<bool> DeleteBoardMemberAsync(BoardMember boardMember);
        Task<IEnumerable<Board>> GetAllBoardsAsync();
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<BoardMember?> GetBoardMemberWithIncludesAsync(int boardId, int userId);

    }

    public class BusinessBoardMember : IBusinessBoardMember
    {
        private readonly IRepositoryBoardMember _repositoryBoardMember;
        private readonly IRepositoryBoard _repositoryBoard;
        private readonly IRepositoryUser _repositoryUser;

        public BusinessBoardMember(
            IRepositoryBoardMember repositoryBoardMember,
            IRepositoryBoard repositoryBoard,
            IRepositoryUser repositoryUser)
        {
            _repositoryBoardMember = repositoryBoardMember;
            _repositoryBoard = repositoryBoard;
            _repositoryUser = repositoryUser;
        }

        public async Task<IEnumerable<BoardMember>> GetAllBoardMembersWithIncludesAsync()
        {
            return await _repositoryBoardMember.ReadWithIncludesAsync();
        }

        public async Task<BoardMember?> GetBoardMemberAsync(int boardId, int userId)
        {
            return await _repositoryBoardMember.FindAsync(boardId, userId);
        }

        public async Task<bool> SaveBoardMemberAsync(BoardMember boardMember)
        {
            var exists = await _repositoryBoardMember.ExistsAsync(boardMember);

            if (exists)
            {
                var existing = await _repositoryBoardMember.FindAsync(boardMember.BoardId, boardMember.UserId);
                if (existing == null) return false;

                existing.Role = boardMember.Role;
                return await _repositoryBoardMember.UpdateAsync(existing);
            }
            else
            {
                return await _repositoryBoardMember.CreateAsync(boardMember);
            }
        }

        public async Task<bool> DeleteBoardMemberAsync(BoardMember boardMember)
        {
            return await _repositoryBoardMember.DeleteAsync(boardMember);
        }

        public async Task<IEnumerable<Board>> GetAllBoardsAsync()
        {
            return await _repositoryBoard.ReadAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _repositoryUser.ReadAsync();
        }
        public async Task<BoardMember?> GetBoardMemberWithIncludesAsync(int boardId, int userId)
        {
            return await _repositoryBoardMember
                .GetWithIncludesAsync(boardId, userId);
        }

    }
}
