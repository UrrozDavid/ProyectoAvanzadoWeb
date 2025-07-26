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
            try
            {
                bool isUpdate = board.BoardId > 0;
                var currentUser = "system";

                board.AddAudit(currentUser);
                board.AddLogging(isUpdate ? Models.Enums.LoggingType.Update : Models.Enums.LoggingType.Create);

                if (isUpdate)
                {
                    var existing = await repositoryBoard.FindAsync(board.BoardId);
                    if (existing == null) return false;

                    existing.Name = board.Name;
                    existing.Description = board.Description;
                    existing.CreatedBy = board.CreatedBy;
                    existing.CreatedAt = board.CreatedAt;

                    return await repositoryBoard.UpdateAsync(existing);
                }
                else
                {
                    return await repositoryBoard.CreateAsync(board);
                }
            }
            catch (Exception ex)
            {
                // Aquí puedes agregar logging del error si tienes un logger configurado
                return false;
            }
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

