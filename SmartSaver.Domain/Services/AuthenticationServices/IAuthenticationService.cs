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

    public interface IAuthenticationService
    {
        UserDto Login(string userAttribute, string password);
        RegistrationResult Register(UserDto user);
    }
}
