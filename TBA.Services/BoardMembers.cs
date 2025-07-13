using System.Collections.Generic;
using System.Threading.Tasks;
using TBA.Models.Entities;
using TBA.Business;

namespace TBA.Services
{
    public class BoardMemberService
    {
        private readonly IBusinessBoardMember _businessBoardMember;

        public BoardMemberService(IBusinessBoardMember businessBoardMember)
        {
            _businessBoardMember = businessBoardMember;
        }

        public async Task<IEnumerable<BoardMember>> GetAllBoardMembersAsync()
            => await _businessBoardMember.GetAllBoardMembersAsync();

        public async Task<BoardMember?> GetBoardMemberAsync(int id)
            => await _businessBoardMember.GetBoardMemberAsync(id);

        public async Task<bool> SaveBoardMemberAsync(BoardMember boardMember)
            => await _businessBoardMember.SaveBoardMemberAsync(boardMember);

        public async Task<bool> DeleteBoardMemberAsync(int id)
        {
            var board = await _businessBoardMember.GetBoardMemberAsync(id);
            if (board == null) return false;
            return await _businessBoardMember.DeleteBoardMemberAsync(board);
        }
    }
}