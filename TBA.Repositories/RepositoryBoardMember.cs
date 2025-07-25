﻿using TBA.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBA.Repositories;
using Microsoft.EntityFrameworkCore;

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

    }
    public class RepositoryBoardMember : RepositoryBase<BoardMember>, IRepositoryBoardMember
    {
        public async Task<bool> CheckBeforeSavingAsync(BoardMember entity)
        {
            var exists = await ExistsAsync(entity);
            if (exists)
            {
                // algo más
            }

            return await UpsertAsync(entity, exists);
        }
        public async Task<BoardMember> FindAsync(int boardId, int userId)
        {
            return await DbContext.BoardMembers
                .FirstOrDefaultAsync(bm => bm.BoardId == boardId && bm.UserId == userId);
        }
    }
}
