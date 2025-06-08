using TBA.Models.Entities;

internal interface IBusinessBoard
{
    Task<IEnumerable<Board>> GetAllBoardsAsync();
    Task<bool> SaveBoardAsync(Board board);
    Task<bool> DeleteBoardAsync(Board board);
    Task<Board> GetBoardAsync(int id);
}