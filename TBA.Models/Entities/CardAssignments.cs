using System;
using System.Collections.Generic;
using TBA.Models.TBAModels;

namespace TBA.Models.Entities;


public partial class CardAssignment : Entity
{
    public int CardId { get; set; }
    public int UserId { get; set; }

    public Card Card { get; set; } = null!;
    public User User { get; set; } = null!;
}
