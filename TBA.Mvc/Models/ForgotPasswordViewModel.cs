using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TBA.Mvc.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
