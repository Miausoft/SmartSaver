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

    public interface IAuthenticationService
    {
        User Login(string email, string password);
        RegistrationResult Register(User user);
        public bool VerifyEmail(User user);
        public Task SignInAsync<T>(T userId);
    }
}
