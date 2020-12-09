namespace SmartSaver.EntityFrameworkCore.Models
{
    public class Suggestion
    {
        public int Id { get; set; }
        public string ProblemCode { get; set; }
        public string SolutionCode { get; set; }
        public string SuggestionText { get; set; }
    }
}