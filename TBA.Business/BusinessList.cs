using TBA.Models.Entities;
using TBA.Repositories;
using TBA.Core.Extensions;


namespace TBA.Business
{
    public interface IBusinessList
    {
        Task<IEnumerable<List>> GetAllListsAsync();
        Task<bool> SaveListAsync(List list);
        Task<bool> DeleteListAsync(List list);
        Task<List> GetListAsync(int id);
    }

    public class BusinessList(IRepositoryList repositoryList) : IBusinessList
    {
        public async Task<IEnumerable<List>> GetAllListsAsync()
        {
            return await repositoryList.ReadAsync();
        }

        public async Task<bool> SaveListAsync(List list)
        {
            var newList = ""; //Identity
            list.AddAudit(newList);
            list.AddLogging(list.ListId <= 0 ? Models.Enums.LoggingType.Create: Models.Enums.LoggingType.Update);
            var exists = await repositoryList.ExistsAsync(list);
            return await repositoryList.UpsertAsync(list, exists);
        }

        public async Task<bool> DeleteListAsync(List user)
        {
            return await repositoryList.DeleteAsync(user);
        }

        public async Task<List> GetListAsync(int id)
        {
            return await repositoryList.FindAsync(id);
        }

        public Task<IEnumerable<List>> GetAllLists()
        {
            throw new NotImplementedException();
        }
    }
}

