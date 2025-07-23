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
            try
            {
                bool isUpdate = list.ListId > 0;
                var currentUser = "system";

                list.AddAudit(currentUser);
                list.AddLogging(isUpdate ? Models.Enums.LoggingType.Update : Models.Enums.LoggingType.Create);

                if (isUpdate)
                {
                    var existing = await repositoryList.FindAsync(list.ListId);
                    if (existing == null) return false;

                    existing.Name = list.Name;
                    existing.Position = list.Position;
                    existing.BoardId = list.BoardId;

                    return await repositoryList.UpdateAsync(existing);
                }
                else
                {

                    return await repositoryList.CreateAsync(list);
                }
            }
            catch (Exception ex)
            {

                return false;
            }
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

