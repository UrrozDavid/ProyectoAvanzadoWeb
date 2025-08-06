using TBA.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBA.Repositories;
using Microsoft.EntityFrameworkCore;
using TBA.Data.Models;

namespace TBA.Repositories
{
    public interface IRepositoryBoard
    {
        Task<bool> UpsertAsync(Board entity, bool isUpdating);

        Task<bool> CreateAsync(Board entity);

        Task<bool> DeleteAsync(Board entity);

        Task<IEnumerable<Board>> ReadAsync();

        Task<Board> FindAsync(int id);

        Task<bool> UpdateAsync(Board entity);

        Task<bool> UpdateManyAsync(IEnumerable<Board> entities);

        Task<bool> ExistsAsync(Board entity);
        Task<bool> CheckBeforeSavingAsync(Board entity);

    }
    public class RepositoryBoard : RepositoryBase<Board>, IRepositoryBoard
    {
        public RepositoryBoard(TrelloDbContext context) : base(context)
        {
        }
        public async Task<bool> CheckBeforeSavingAsync(Board entity)
        {
            var exists = await ExistsAsync(entity);
            if (exists)
            {
                // algo más
            }

            return await UpsertAsync(entity, exists);
        }
    }
}
