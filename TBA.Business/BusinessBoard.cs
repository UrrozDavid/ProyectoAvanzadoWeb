using TBA.Models.Entities;
using TBA.Repositories;
using TBA.Core.Extensions;
using TBA.Models.DTOs;
using Microsoft.EntityFrameworkCore;


namespace TBA.Business
{
    public interface IBusinessBoard
    {
        Task<IEnumerable<Board>> GetAllBoardsAsync();
        Task<bool> SaveBoardDetailsAsync(Board board);
        Task<bool> DeleteBoardAsync(Board board);
        Task<Board> GetBoardAsync(int id);

        Task<int> SaveBoardAsync(
            int creatorUserId,
            string name,
            string? description,
            IEnumerable<(string Name, int Position)> lists,
            IEnumerable<int> memberUserIds
        );
        Task<List<Board>> GetBoardsForUserAsync(int userId);

        Task<bool> UpdateBoardAsync(
            int boardId,
            string name,
            string? description,
            List<ListEditDTO> listsToUpsert,
            List<int> listIdsToDelete,
            List<string> memberEmailsToAdd
        );
    }

    public class BusinessBoard(IRepositoryBoard repositoryBoard) : IBusinessBoard
    {
        public async Task<IEnumerable<Board>> GetAllBoardsAsync()
        {
            return await repositoryBoard.ReadAsync();
        }


        public async Task<bool> SaveBoardDetailsAsync(Board board)
        {
            try
            {
                bool isUpdate = board.BoardId > 0;
                var currentUser = "system";

                board.AddAudit(currentUser);
                board.AddLogging(isUpdate ? Models.Enums.LoggingType.Update : Models.Enums.LoggingType.Create);

                if (isUpdate)
                {
                    var existing = await repositoryBoard.FindAsync(board.BoardId);
                    if (existing == null) return false;

                    existing.Name = board.Name;
                    existing.Description = board.Description;
                    existing.CreatedBy = board.CreatedBy;
                    existing.CreatedAt = board.CreatedAt;

                    return await repositoryBoard.UpdateAsync(existing);
                }
                else
                {
                    return await repositoryBoard.CreateAsync(board);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteBoardAsync(Board board)
        {
            return await repositoryBoard.DeleteAsync(board);
        }

        public async Task<Board> GetBoardAsync(int id)
        {
            return await repositoryBoard.FindAsync(id);
        }

        public Task<IEnumerable<Board>> GetAllBoards()
        {
            throw new NotImplementedException();
        }

        
        public async Task<int> SaveBoardAsync(int creatorUserId,
            string name,
            string? description,
            IEnumerable<(string Name, int Position)> lists,
            IEnumerable<int> memberUserIds)
       => await repositoryBoard.SaveBoardAsync(creatorUserId, name, description, lists, memberUserIds);

        public Task<List<Board>> GetBoardsForUserAsync(int userId) => repositoryBoard.GetBoardsForUserAsync(userId);

        public async Task<bool> UpdateBoardAsync(
         int boardId,
         string name,
         string? description,
         List<ListEditDTO> upserts,
         List<int> deletes,
         List<string> newEmails)
        {
            var board = await repositoryBoard.GetBoardWithIncludesAsync(boardId);
            if (board is null) return false;

            board.Name = name;
            board.Description = description;

            // DELETE lists
            foreach (var id in deletes.Distinct())
            {
                var toRemove = board.Lists?.FirstOrDefault(l => l.ListId == id);
                if (toRemove != null) repositoryBoard.DeleteList(toRemove);
            }

            // UPSERT lists (create/update)
            foreach (var dto in upserts)
            {
                if (dto.ListId == null)
                {
                    repositoryBoard.AddList(new List
                    {
                        BoardId = boardId,
                        Name = dto.Name,
                        Position = dto.Position
                    });
                }
                else
                {
                    var existing = board.Lists!.First(l => l.ListId == dto.ListId.Value);
                    existing.Name = dto.Name;
                    existing.Position = dto.Position;
                    repositoryBoard.UpdateList(existing);
                }
            }

            // ADD members by email
            if (newEmails.Count > 0)
            {
                var users = await repositoryBoard.FindUsersByEmailsAsync(newEmails);
                foreach (var u in users)
                {
                    var exists = board.BoardMembers!.Any(bm => bm.UserId == u.UserId);
                    if (!exists)
                    {
                        repositoryBoard.AddBoardMember(new BoardMember
                        {
                            BoardId = boardId,
                            UserId = u.UserId,
                            Role = "member"
                        });
                    }
                }
            }

            return await repositoryBoard.SaveChangesAsync();
        }

    }
}

