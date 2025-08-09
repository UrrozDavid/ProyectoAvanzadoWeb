using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TBA.Mvc.Models
{
    public class CardCreateViewModel
    {
        public int BoardId { get; set; }

        // Campos de Card
        [Required, MaxLength(200)]
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }

        // List 
        [Required]
        public int ListId { get; set; }
        public IEnumerable<SelectListItem> Lists { get; set; } = [];

        // Assignee
        public int? AssigneeUserId { get; set; }
        public IEnumerable<SelectListItem> Members { get; set; } = [];
    }
}
