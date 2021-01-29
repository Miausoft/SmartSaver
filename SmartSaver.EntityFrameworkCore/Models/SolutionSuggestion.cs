using System.ComponentModel.DataAnnotations;

namespace SmartSaver.EntityFrameworkCore.Models
{
    public class SolutionSuggestion
    {
        [Key]
        public int Id { get; set; }
        public string SolutionText { get; set; }
    }
}
