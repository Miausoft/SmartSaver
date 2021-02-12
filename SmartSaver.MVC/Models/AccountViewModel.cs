using System;
using System.ComponentModel.DataAnnotations;
using SmartSaver.Domain.CustomAttributes;

namespace SmartSaver.MVC.Models
{
    public class AccountViewModel
    {
        [Required]
        [MaxLength(100)]
        [RegularExpression("^[a-zA-Z][a-zA-Z0-9]*$", ErrorMessage = "Name may only contain alphanumeric characters.")]
        [Display(Name = "Goal Name")]
        public string Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Invalid input")]
        [Display(Name = "Amount")]
        public decimal Goal { get; set; }

        [Required]
        [GreaterThanToday(ErrorMessage = "The date must be greater than today")]
        [Display(Name = "Goal Day")]
        public DateTime GoalEndDate { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Invalid input")]
        [Display(Name = "Revenue")]
        public double Revenue { get; set; }
    }
}
