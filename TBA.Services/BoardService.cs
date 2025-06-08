using System.Collections.Generic;
using System.Threading.Tasks;
using TBA.Models.Entities;
using TBA.Business;

namespace TBA.Services
{
    public class BoardService
    {
        private readonly IBusinessBoard _businessBoard;

        public BoardService(IBusinessBoard businessBoard)
        {
            _businessBoard = businessBoard;
        }

        public async Task<IEnumerable<Board>> GetAllBoardsAsync()
            => await _businessBoard.GetAllBoardsAsync();

        public async Task<Board?> GetBoardByIdAsync(int id)
            => await _businessBoard.GetBoardAsync(id);

        public async Task<bool> SaveBoardAsync(Board board)
            => await _businessBoard.SaveBoardAsync(board);

        public async Task<bool> DeleteBoardAsync(int id)
        {
            var board = await _businessBoard.GetBoardAsync(id);
            if (board == null) return false;
            return await _businessBoard.DeleteBoardAsync(board);
        }
    }
}