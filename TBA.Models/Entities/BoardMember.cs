using System;
using System.Collections.Generic;
using TBA.Models.TBAModels;

namespace TBA.Models.Entities;

public partial class BoardMember : Entity
{
    public int BoardId { get; set; }

    public int UserId { get; set; }

    public string? Role { get; set; }

    public virtual Board Board { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
