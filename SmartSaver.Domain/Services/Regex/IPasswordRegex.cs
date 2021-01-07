
namespace SmartSaver.Domain.Services.Regex
{
    public interface IPasswordRegex
    {
        /// <summary>
        /// Checks if given password is great format.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        bool Match(string password);
    }
}
