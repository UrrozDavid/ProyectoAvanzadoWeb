using System;
using System.Collections.Generic;
using TBA.Models.Entities;

namespace TBA.Models.DTOs
{
    public class BoardViewViewModel
    {
        public int CardId { get; set; }
        public string CardTitle { get; set; }
        public string CardDescription { get; set; }

        // Si quieres un solo color
        public string LabelColor { get; set; }

        // O si quieres varios colores por tarjeta
        public List<string> LabelColors { get; set; }

        public List<Label> AllLabels { get; set; }
        public List<ListEditDTO> Lists { get; set; }

        public BoardViewViewModel()
        {
            LabelColors = new List<string>();
            AllLabels = new List<Label>();
            Lists = new List<ListEditDTO>();
        }
    }
}
