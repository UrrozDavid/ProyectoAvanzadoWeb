using TBA.Models.Entities;
using TBA.Repositories;
using TBA.Core.Extensions;


namespace TBA.Business
{
    public interface IBusinessBoard
    {
        Task<IEnumerable<Board>> GetAllBoardsAsync();
        Task<bool> SaveBoardAsync(Board board);
        Task<bool> DeleteBoardAsync(Board board);
        Task<Board> GetBoardAsync(int id);
    }

    public class BusinessBoard(IRepositoryBoard repositoryBoard) : IBusinessBoard
    {
        public async Task<IEnumerable<Board>> GetAllBoardsAsync()
        {
            return await repositoryBoard.ReadAsync();
        }

        public async Task<bool> SaveBoardAsync(Board board)
        {
            var newBoard = ""; //Identity
            board.AddAudit(newBoard);
            board.AddLogging(board.BoardId<= 0 ? Models.Enums.LoggingType.Create: Models.Enums.LoggingType.Update);
            var exists = await repositoryBoard.ExistsAsync(board);
            return await repositoryBoard.UpsertAsync(board, exists);
        }

        public async Task<bool> DeleteBoardAsync(Board board)
        {
            return await repositoryBoard.DeleteAsync(board);
        }

        public async Task<Board> GetBoardAsync(int id)
        {
            return await repositoryBoard.FindAsync(id);
        }

        public Task<IEnumerable<Board>> GetAllBoards()
        {
            throw new NotImplementedException();
        }
    }
}

