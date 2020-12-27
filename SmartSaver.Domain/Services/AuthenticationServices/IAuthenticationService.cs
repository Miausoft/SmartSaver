using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.Domain.Services.AuthenticationServices
{
    public enum RegistrationResult
    {
        Success,
        UserAlreadyExist,
        BadPasswordFormat
    }

    public interface IAuthenticationService
    {
        User Login(string userAttribute, string password);
        RegistrationResult Register(User user);
    }
}
