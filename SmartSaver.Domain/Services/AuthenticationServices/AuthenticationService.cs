using System;
using System.Collections.Generic;
using System.Text;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.Domain.Services.AuthenticationServices
{
    class AuthenticationService : IAuthenticationService
    {
        public Account Login(string userAttribute, string password)
        {
            throw new NotImplementedException();
        }

        public RegistrationResult Register(User user)
        {
            throw new NotImplementedException();
        }

        public string Hash(string password)
        {
            throw new NotImplementedException();
        }

        public bool Verify(string password, string hash)
        {
            throw new NotImplementedException();
        }
    }
}
