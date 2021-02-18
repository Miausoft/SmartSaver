using SmartSaver.EntityFrameworkCore.Models;
using System.Threading.Tasks;

namespace SmartSaver.Domain.Services.AuthenticationServices
{
    public enum RegistrationResult
    {
        Success,
        UserAlreadyExist,
        BadPasswordFormat,
        Failure
    }

    public interface IAuthentication
    {
        User Login(string email, string password);
        Task<RegistrationResult> RegisterAsync(User user);
        public Task<bool> VerifyEmailAsync(User user);
        public Task SignInAsync<T>(T userId);
    }
}
