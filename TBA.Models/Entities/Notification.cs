using System;
using System.Collections.Generic;
using TBA.Models.TBAModels;

namespace TBA.Models.Entities;

public partial class Notification : Entity
{
    public int NotificationId { get; set; }

    public int? UserId { get; set; }

    public int? CardId { get; set; }

    public string? Message { get; set; }

    public DateTime? NotifyAt { get; set; }

    public bool? IsRead { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual Card? Card { get; set; }

    public virtual User? User { get; set; }
    public string Type { get; set; }
    public int RelatedId { get; set; } 
    public string GroupName { get; set; }
}
