namespace SmartSaver.Domain.Services.Regex
{
    public class PasswordRegex : IPasswordRegex
    {
        public bool Match(string password)
        {
            var hasNumber = new System.Text.RegularExpressions.Regex(@"[0-9]+");
            var hasUpperChar = new System.Text.RegularExpressions.Regex(@"[A-Z]+");
            var hasLowerChar = new System.Text.RegularExpressions.Regex(@"[a-z]+");
            var hasMinimum5Chars = password.Length >= 5;

            return hasNumber.IsMatch(password)
                && hasUpperChar.IsMatch(password)
                && hasLowerChar.IsMatch(password)
                && hasMinimum5Chars;
        }
    }
}
