using System;
using System.ComponentModel.DataAnnotations;
using SmartSaver.Domain.CustomAttributes;

namespace SmartSaver.MVC.Models
{
    public class AccountViewModel
    {
        [Required]
        [MaxLength(100)]
        [Display(Name = "Goal Name")]
        public string Name { get; set; }

        [Required]
        [Range(double.Epsilon, double.MaxValue, ErrorMessage = "Invalid input")]
        [Display(Name = "Amount")]
        public decimal Goal { get; set; }

        [Required]
        [GreaterThanToday(ErrorMessage = "Invalid input")]
        [Display(Name = "Goal Day")]
        public DateTime GoalEndDate { get; set; }

        [Required]
        [Range(double.Epsilon, double.MaxValue, ErrorMessage = "Invalid input")]
        [Display(Name = "Revenue")]
        public double Revenue { get; set; }
    }
}
