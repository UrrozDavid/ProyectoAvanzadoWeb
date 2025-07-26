using TBA.Models.Entities;
using TBA.Repositories;
using TBA.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TBA.Business
{
    public interface IBusinessBoardMember
    {
        Task<IEnumerable<BoardMember>> GetAllBoardMembersAsync();
        Task<bool> SaveBoardMemberAsync(BoardMember boardMember);
        Task<bool> DeleteBoardMemberAsync(BoardMember boardMember);
        Task<BoardMember> GetBoardMemberAsync(int boardId, int userId);
    }

    public class BusinessBoardMember : IBusinessBoardMember
    {
        private readonly IRepositoryBoardMember repositoryBoardMember;

        public BusinessBoardMember(IRepositoryBoardMember repositoryBoardMember)
        {
            this.repositoryBoardMember = repositoryBoardMember;
        }

        public async Task<IEnumerable<BoardMember>> GetAllBoardMembersAsync()
        {
            return await repositoryBoardMember.ReadAsync();
        }

        public async Task<bool> SaveBoardMemberAsync(BoardMember boardMember)
        {
            try
            {
                bool isUpdate = boardMember.BoardId > 0 && boardMember.UserId > 0;
                var currentUser = "system";

                boardMember.AddAudit(currentUser);
                boardMember.AddLogging(isUpdate ? Models.Enums.LoggingType.Update : Models.Enums.LoggingType.Create);

                if (isUpdate)
                {
                    var existing = await repositoryBoardMember.FindAsync(boardMember.BoardId, boardMember.UserId);
                    if (existing == null) return false;

                    // Solo actualizamos lo que cambia
                    existing.Role = boardMember.Role;

                    return await repositoryBoardMember.UpdateAsync(existing);
                }
                else
                {
                    return await repositoryBoardMember.CreateAsync(boardMember);
                }
            }
            catch (Exception ex)
            {
                // Aquí puedes agregar logging del error si tienes un logger configurado
                return false;
            }
        }

        public async Task<bool> DeleteBoardMemberAsync(BoardMember boardMember)
        {
            return await repositoryBoardMember.DeleteAsync(boardMember);
        }

        public async Task<BoardMember> GetBoardMemberAsync(int boardId, int userId)
        {
            return await repositoryBoardMember.FindAsync(boardId, userId);
        }
    }
}
