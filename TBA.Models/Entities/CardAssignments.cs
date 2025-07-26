using System;
using System.Collections.Generic;
using TBA.Models.TBAModels;

namespace TBA.Models.Entities;


public partial class CardAssignments : Entity
{
    public int CardId { get; set; }
    public int UserId { get; set; }


}
