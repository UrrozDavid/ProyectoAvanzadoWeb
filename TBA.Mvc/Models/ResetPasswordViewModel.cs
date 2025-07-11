using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TBA.Mvc.Models
{
    public class ResetPasswordViewModel 
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string TempPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;
        
        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set;} = string.Empty;
    }
}
