using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.Domain.Managers
{
    public interface ISuggestionRepo
    {
        void AddSuggestion(Suggestion suggestion);
        void RemoveSuggestion(int suggestionId);
        void GetSuggestionById(int suggestionId);
        void GetSuggestionsByProblemCode(string problemCode);
        void GetSuggestionBySolutionCode(string solutionCode);
    }
}