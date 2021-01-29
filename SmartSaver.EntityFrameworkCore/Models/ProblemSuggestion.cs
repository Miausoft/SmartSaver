using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSaver.EntityFrameworkCore.Models
{
    public class ProblemSuggestion
    {
        [Key]
        public int Id { get; set; }
        public string ProblemText { get; set; }

        [ForeignKey(nameof(SolutionId))]
        public SolutionSuggestion SolutionSuggestion { get; set; }
        public int SolutionId { get; set; }
    }
}
