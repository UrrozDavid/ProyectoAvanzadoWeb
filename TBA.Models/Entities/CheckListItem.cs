using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBA.Models.TBAModels;



namespace TBA.Models.Entities
{
    public partial class ChecklistItem : Entity
    {
        public int ChecklistItemId { get; set; }
        public int CardId { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsDone { get; set; }
        public int Position { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual Card Card { get; set; } = null!;
    }
}
