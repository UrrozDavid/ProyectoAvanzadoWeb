using System;
using System.Collections.Generic;
using TBA.Models.TBAModels;

namespace TBA.Models.Entities;

public partial class Attachment : Entity
{
    public int AttachmentId { get; set; }

    public int? CardId { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public DateTime? UploadedAt { get; set; }

    public virtual Card? Card { get; set; }
}
