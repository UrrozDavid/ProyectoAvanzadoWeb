using TBA.Models.Entities;
using Microsoft.EntityFrameworkCore;
using TBA.Data.Models;

namespace TBA.Repositories
{
    public interface IRepositoryBoard
    {
        Task<Board> FindAsync(int id);
        Task<IEnumerable<Board>> ReadAsync();
        Task<bool> UpdateAsync(Board entity);
        Task<bool> CreateAsync(Board entity);
        Task<bool> DeleteAsync(Board entity);
        Task<bool> UpsertAsync(Board entity, bool isUpdating);
        Task<bool> UpdateManyAsync(IEnumerable<Board> entities);
        Task<bool> ExistsAsync(Board entity);
        Task<bool> CheckBeforeSavingAsync(Board entity);

        Task<int> SaveBoardAsync(
            int creatorUserId,
            string name,
            string? description,
            IEnumerable<(string Name, int Position)> lists,
            IEnumerable<int> memberUserIds);

        Task<List<Board>> GetBoardsForUserAsync(int userId);

        Task<Board?> GetBoardWithIncludesAsync(int id);
        void AddList(List list);
        void UpdateList(List list);
        void DeleteList(List list);
        void AddBoardMember(BoardMember bm);
        Task<bool> SaveChangesAsync();
        Task<List<User>> FindUsersByEmailsAsync(IEnumerable<string> emails);

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
    
        // Creates Board + Lists + BoardMembers
        public async Task<int> SaveBoardAsync(
            int creatorUserId,
            string name,
            string? description,
            IEnumerable<(string Name, int Position)> lists,
            IEnumerable<int> memberUserIds)
        {
            await using var tx = await DbContext.Database.BeginTransactionAsync();

            // Board
            var board = new Board
            {
                Name = name,
                Description = description,
                CreatedBy = creatorUserId,
                CreatedAt = DateTime.Now,
            };
            DbContext.Boards.Add(board);
            await DbContext.SaveChangesAsync();

            // Lists 
            foreach (var item in lists)
            {
                DbContext.Lists.Add(new List
                {
                    Name = item.Name,
                    Position = item.Position,
                    BoardId = board.BoardId
                });
            }
            await DbContext.SaveChangesAsync();

            // Members 
            var unique = memberUserIds.Distinct().ToList();
            if (!unique.Contains(creatorUserId)) unique.Add(creatorUserId);

            foreach (var item in unique)
            {
                DbContext.BoardMembers.Add(new BoardMember
                {
                    BoardId = board.BoardId,
                    UserId = item,
                    Role = item == creatorUserId ? "Owner" : "Member"
                });
            }
            await DbContext.SaveChangesAsync();

            await tx.CommitAsync();
            return board.BoardId;
        }


        // Devuelve las Board donde userId sea quien la creo o sea parte de BoardMembers
        public async Task<List<Board>> GetBoardsForUserAsync(int userId)
            => await DbContext.Boards
                    .AsNoTracking()
                    .Where(b => b.CreatedBy == userId 
                                || DbContext.BoardMembers.Any(bm => bm.BoardId == b.BoardId && bm.UserId == userId))
                    .OrderByDescending(bm => bm.CreatedAt)
                    .ToListAsync();

        public async Task<Board?> GetBoardWithIncludesAsync(int boardId)
            => await DbContext.Boards
                .Include(b => b.Lists)
                .Include(b => b.BoardMembers).ThenInclude(bm => bm.User)
                .FirstOrDefaultAsync(b => b.BoardId == boardId);

        public void AddList(List list) => DbContext.Lists.Add(list);
        public void UpdateList(List list) => DbContext.Lists.Update(list);
        public void DeleteList(List list) => DbContext.Lists.Remove(list);
        public void AddBoardMember(BoardMember boardMember) => DbContext.BoardMembers.Add(boardMember);

        public async Task<List<User>> FindUsersByEmailsAsync(IEnumerable<string> emails)
        {
            var set = emails?.Select(e => e.Trim().ToLower()).ToHashSet() ?? new HashSet<string>();
            return await DbContext.Users
                .Where(u => set.Contains(u.Email.ToLower()))
                .ToListAsync();
        }

        public async Task<bool> SaveChangesAsync()
            => (await DbContext.SaveChangesAsync()) > 0;
    }
}
