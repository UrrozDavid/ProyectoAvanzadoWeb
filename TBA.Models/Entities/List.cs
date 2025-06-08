using System;
using System.Collections.Generic;
using TBA.Models.TBAModels;

namespace TBA.Models.Entities;

public partial class List : Entity
{
    public int ListId { get; set; }

    public string Name { get; set; } = null!;

    public int? Position { get; set; }

    public int? BoardId { get; set; }

    public virtual Board? Board { get; set; }

    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();
}
