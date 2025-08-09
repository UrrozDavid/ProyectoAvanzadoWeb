using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBA.Models.DTOs
{
    public class ListEditDTO
    {
        public int? ListId { get; set; }
        public string Name { get; set; } = "";
        public int Position { get; set; }
        public bool Remove { get; set; } 
    }
}
