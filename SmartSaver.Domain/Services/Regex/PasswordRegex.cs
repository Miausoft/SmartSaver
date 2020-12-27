
namespace SmartSaver.Domain.Services.Regex
{
    public class PasswordRegex : IPasswordRegex
    {
        public bool Match(string password)
        {
            bool isValid = true;

            var hasNumber = new System.Text.RegularExpressions.Regex(@"[0-9]+");
            var hasUpperChar = new System.Text.RegularExpressions.Regex(@"[A-Z]+");
            var hasMinimum5Chars = new System.Text.RegularExpressions.Regex(@".{5,}");

            if (!(hasNumber.IsMatch(password)
                  && hasUpperChar.IsMatch(password)
                  && hasMinimum5Chars.IsMatch(password)))
                isValid = false;

            return isValid;
        }
    }
}
