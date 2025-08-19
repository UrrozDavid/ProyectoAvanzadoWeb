using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TBA.Models.DTOs
{
    public class CommentViewModel
    {
        public int CommentID { get; set; }
        public int CardID { get; set; }
        public int UserID { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string Username { get; set; } = string.Empty; 
    }
}
