
using TBA.Models.Entities;
using TBA.Business;

namespace TBA.Services
{
    public class ListService
    {
        private readonly IBusinessList _businessList;

        public ListService(IBusinessList businessList)
        {
            _businessList = businessList;
        }

        public async Task<IEnumerable<List>> GetAllListsAsync()
            => await _businessList.GetAllListsAsync();

        public async Task<List?> GetListByIdAsync(int id)
            => await _businessList.GetListAsync(id);

        public async Task<bool> SaveListAsync(List list)
            => await _businessList.SaveListAsync(list);

        public async Task<bool> DeleteListAsync(int id)
        {
            var list = await _businessList.GetListAsync(id);
            if (list == null) return false;
            return await _businessList.DeleteListAsync(list);
        }
    }
}