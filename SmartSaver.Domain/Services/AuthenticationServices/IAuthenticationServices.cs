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
        InvalidUserObject
    }

    public interface IAuthenticationServices
    {
        User Login(string username, string password);
        RegistrationResult Register(string username, string password, string number);

    }
}
