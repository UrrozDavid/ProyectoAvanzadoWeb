using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TBA.Models.TBAModels;

namespace TBA.Models.Entities;

public partial class BoardMember : Entity
{
    [Required]
    public int BoardId { get; set; }

    [Required]
    public int UserId { get; set; }

    public string Role { get; set; } = "member";
    public virtual Board? Board { get; set; }
    public virtual User? User { get; set; }
}
