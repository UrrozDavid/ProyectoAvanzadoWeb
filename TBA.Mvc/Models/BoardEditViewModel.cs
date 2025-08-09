using System.ComponentModel.DataAnnotations;
using TBA.Models.DTOs;

namespace TBA.Mvc.Models
{
    public class BoardEditViewModel
    {
        public int BoardId { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }

        public List<ListEditDTO> Lists { get; set; } = new();
        public List<MemberDTO> CurrentMembers { get; set; } = new();

        public string? MemberEmailsToAdd { get; set; }
    }
}
