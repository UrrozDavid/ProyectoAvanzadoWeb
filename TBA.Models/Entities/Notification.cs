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

    public virtual Card? Card { get; set; }

    public virtual User? User { get; set; }
}
