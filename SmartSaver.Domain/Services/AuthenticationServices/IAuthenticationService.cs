using System;
using System.Collections.Generic;
using System.Text;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.Domain.Services.AuthenticationServices
{
    public enum RegistrationResult
    {
        Success,
        UserAlreadyExist,
        BadPasswordFormat
    }

    public interface IBasicAuthenticationService
    {
        Account Login(string userAttribute, string password);
        RegistrationResult Register(User user);
    }

    public interface IAuthenticationService : IBasicAuthenticationService, IHash { }
}
