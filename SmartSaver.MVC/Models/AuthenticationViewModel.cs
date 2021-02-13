using System.ComponentModel.DataAnnotations;

namespace SmartSaver.MVC.Models
{
    public class AuthenticationViewModel
    {
        [Required]
        [MinLength(5, ErrorMessage = "Username must be at least 5 characters long")]
        [RegularExpression("^[a-zA-Z][a-zA-Z0-9]*$", ErrorMessage = "Username may only contain alphanumeric characters.")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required, DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The field is required"), DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "Password must be at least 5 characters long")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]+).+$", ErrorMessage = "Password must contain a number, lowercase and capital letter.")]
        [Display(Name = "New Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
