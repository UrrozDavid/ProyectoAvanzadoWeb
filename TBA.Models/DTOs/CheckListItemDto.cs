
using System.Text.Json.Serialization;

namespace TBA.Models.DTOs
{
    public class ChecklistItemDto
    {
        [JsonPropertyName("id")]
        public int ChecklistItemId { get; set; }

        [JsonPropertyName("cardId")]
        public int CardId { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("isDone")]
        public bool IsDone { get; set; }

        [JsonPropertyName("position")]
        public int Position { get; set; }
    }
}
