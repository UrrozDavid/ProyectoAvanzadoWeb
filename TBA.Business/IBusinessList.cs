using TBA.Models.Entities;

internal interface IBusinessList
{
    Task<IEnumerable<List>> GetAllListsAsync();
    Task<bool> SaveListAsync(List list);
    Task<bool> DeleteListAsync(List list);
    Task<List> GetListAsync(int id);
}