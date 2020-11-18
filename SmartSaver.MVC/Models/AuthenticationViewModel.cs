using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SmartSaver.MVC.Models
{
    public class AuthenticationViewModel
    {
        [Required]
        [MinLength(5, ErrorMessage = "Username must be at least 5 characters long")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required, DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "Password must be at least 5 characters long")]
        [RegularExpression("^(?=.*[A-Z])(?=.*[0-9]+).+$", ErrorMessage = "Password must contain atleast one number and capital letter")]
        [Display(Name = "New Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
