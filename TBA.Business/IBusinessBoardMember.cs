using TBA.Models.Entities;

internal interface IBusinessBoardMember
{
    Task<IEnumerable<BoardMember>> GetAllBoardMembersAsync();
    Task<bool> SaveBoardMemberAsync(BoardMember boardMember);
    Task<bool> DeleteBoardMemberAsync(BoardMember boardMember);
    Task<BoardMember> GetBoardMemberAsync(int id);
}