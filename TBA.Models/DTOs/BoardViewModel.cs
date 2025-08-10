using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBA.Models.Entities;

namespace TBA.Models.DTOs
{
    public class BoardViewViewModel
    {
        public Board Board { get; set; }
        public List<ListWithCardsViewModel> Lists { get; set; }
    }
}
