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
    public interface IRepositoryComment
    {
        Task<bool> UpsertAsync(Comment entity, bool isUpdating);

        Task<bool> CreateAsync(Comment entity);

        Task<bool> DeleteAsync(Comment entity);

        Task<IEnumerable<Comment>> ReadAsync();

        Task<Comment> FindAsync(int id);

        Task<bool> UpdateAsync(Comment entity);

        Task<bool> UpdateManyAsync(IEnumerable<Comment> entities);

        Task<bool> ExistsAsync(Comment entity);
        Task<bool> CheckBeforeSavingAsync(Comment entity);

    }
    public class RepositoryComment : RepositoryBase<Comment>, IRepositoryComment
    {
        public RepositoryComment(TrelloDbContext context) : base(context)
        {
        }
        public async Task<bool> CheckBeforeSavingAsync(Comment entity)
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
