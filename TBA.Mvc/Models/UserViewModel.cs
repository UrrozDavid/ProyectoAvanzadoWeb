using System.ComponentModel.DataAnnotations;

namespace TBA.Mvc.Models
{
    public class UserViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(100)]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string? PasswordHash { get; set; }
        public string DisplayPassword => "********";

    }
}
