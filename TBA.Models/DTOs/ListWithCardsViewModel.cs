using TBA.Models.Entities;

namespace TBA.Models.DTOs
{
    public class ListWithCardsViewModel
    {
        public required Models.Entities.List List { get; set; }
        public required List<Card> Cards { get; set; }
    }
}