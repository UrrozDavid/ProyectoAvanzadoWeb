using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TBA.Models.TBAModels;

namespace TBA.Models.Entities;

public partial class Board : Entity
{
    public int BoardId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<BoardMember> BoardMembers { get; set; } = new List<BoardMember>();

    public virtual ICollection<List> Lists { get; set; } = new List<List>();
}
