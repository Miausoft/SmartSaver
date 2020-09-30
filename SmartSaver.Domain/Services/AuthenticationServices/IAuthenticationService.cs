using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
        Account Login(string userAttribute, string password);
        RegistrationResult Register(User user);

    }
}
