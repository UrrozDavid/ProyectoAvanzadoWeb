using TBA.Models.Entities;

namespace TBA.Business
{
    public interface IBusinessList
    {
        Task<IEnumerable<List>> GetAllListsAsync();
        Task<List?> GetListAsync(int id);
        Task<bool> SaveListAsync(List list);
        Task<List?> GetListWithBoardAsync(int listId);
        Task<bool> DeleteListAsync(int listId);
    }
}
