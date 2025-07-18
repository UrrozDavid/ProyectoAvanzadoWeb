using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TBA.Models.DTOs
{
    public class TaskViewModel
    {
        [JsonPropertyName("cardId")]
        public int CardId { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("dueDate")]
        public DateTime? DueDate { get; set; }

        [JsonPropertyName("assignedUserName")]
        public string? AssignedUserName { get; set; }

        [JsonPropertyName("assignedUserAvatarUrl")]
        public string? AssignedUserAvatarUrl { get; set; }

        [JsonPropertyName("commentsCount")]
        public int CommentsCount { get; set; }

        [JsonPropertyName("checklistDone")]
        public int ChecklistDone { get; set; }

        [JsonPropertyName("checklistTotal")]
        public int ChecklistTotal { get; set; }

        [JsonPropertyName("priority")]
        public string? Priority { get; set; }
    }
}
