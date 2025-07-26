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

        public async Task<BoardMember?> GetBoardMemberAsync(int boardId, int userId)
            => await _businessBoardMember.GetBoardMemberAsync(boardId, userId);

        public async Task<bool> SaveBoardMemberAsync(BoardMember boardMember)
            => await _businessBoardMember.SaveBoardMemberAsync(boardMember);

        // CORREGIDO: recibe ambos IDs para borrar
        public async Task<bool> DeleteBoardMemberAsync(int boardId, int userId)
        {
            var boardMember = await _businessBoardMember.GetBoardMemberAsync(boardId, userId);
            if (boardMember == null) return false;

            return await _businessBoardMember.DeleteBoardMemberAsync(boardMember);
        }
    }
}
