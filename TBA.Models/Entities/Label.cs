using System;
using System.Collections.Generic;
using TBA.Models.TBAModels;

namespace TBA.Models.Entities;

public partial class Label : Entity
{
    public int LabelId { get; set; }

    public string? Name { get; set; }

    public string? Color { get; set; }

    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();
}
