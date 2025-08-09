using System.ComponentModel.DataAnnotations;

namespace TBA.Mvc.Models
{
    public class BoardCreateViewModel
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        public string? Description { get; set; }

        public List<ListInput> Lists { get; set; } = new();

        public string? MemberEmail { get; set; }
    }

    public class ListInput
    {
        [Required] public string Name { get; set; } = "";
        public int? Position { get; set; }
    }
}
