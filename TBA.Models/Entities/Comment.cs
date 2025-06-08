using System;
using System.Collections.Generic;
using TBA.Models.TBAModels;

namespace TBA.Models.Entities;

public partial class Comment : Entity
{
    public int CommentId { get; set; }

    public int? CardId { get; set; }

    public int? UserId { get; set; }

    public string? CommentText { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Card? Card { get; set; }

    public virtual User? User { get; set; }
}
