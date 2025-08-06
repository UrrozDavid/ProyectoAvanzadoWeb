using TBA.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TBA.Data.Models;

namespace TBA.Repositories
{
    public interface IRepositoryBoardMember
    {
        Task<bool> UpsertAsync(BoardMember entity, bool isUpdating);
        Task<bool> CreateAsync(BoardMember entity);
        Task<bool> DeleteAsync(BoardMember entity);
        Task<IEnumerable<BoardMember>> ReadAsync();
        Task<BoardMember> FindAsync(int boardId, int userId);
        Task<bool> UpdateAsync(BoardMember entity);
        Task<bool> UpdateManyAsync(IEnumerable<BoardMember> entities);
        Task<bool> ExistsAsync(BoardMember entity);
        Task<bool> CheckBeforeSavingAsync(BoardMember entity);
        Task<IEnumerable<BoardMember>> ReadWithIncludesAsync();
        Task<BoardMember?> GetWithIncludesAsync(int boardId, int userId);
    }

    public class RepositoryBoardMember : RepositoryBase<BoardMember>, IRepositoryBoardMember
    {
        public RepositoryBoardMember(TrelloDbContext context) : base(context)
        {
        }
        public async Task<bool> CreateAsync(BoardMember entity)
        {
            await DbSet.AddAsync(entity);
            return await DbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(BoardMember entity)
        {
            DbSet.Remove(entity);
            return await DbContext.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<BoardMember>> ReadAsync()
        {
            return await DbSet.ToListAsync();
        }

        public async Task<BoardMember> FindAsync(int boardId, int userId)
        {
            return await DbContext.BoardMembers
                .FirstOrDefaultAsync(bm => bm.BoardId == boardId && bm.UserId == userId);
        }

        public async Task<bool> UpdateAsync(BoardMember entity)
        {
            DbSet.Update(entity);
            return await DbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateManyAsync(IEnumerable<BoardMember> entities)
        {
            DbSet.UpdateRange(entities);
            return await DbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(BoardMember entity)
        {
            return await DbSet.AnyAsync(x =>
                x.BoardId == entity.BoardId &&
                x.UserId == entity.UserId);
        }

        public async Task<bool> CheckBeforeSavingAsync(BoardMember entity)
        {
            var exists = await ExistsAsync(entity);
            return await UpsertAsync(entity, exists);
        }

        public async Task<bool> UpsertAsync(BoardMember entity, bool isUpdating)
        {
            if (isUpdating)
            {
                DbSet.Update(entity);
            }
            else
            {
                await DbSet.AddAsync(entity);
            }

            return await DbContext.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<BoardMember>> ReadWithIncludesAsync()
        {
            return await DbContext.BoardMembers
                .Include(bm => bm.Board)
                .Include(bm => bm.User)
                .ToListAsync();
        }
        public async Task<BoardMember?> GetWithIncludesAsync(int boardId, int userId)
        {
            return await DbContext.BoardMembers
                .Include(bm => bm.Board)
                .Include(bm => bm.User)
                .FirstOrDefaultAsync(bm => bm.BoardId == boardId && bm.UserId == userId);
        }

    }
}

