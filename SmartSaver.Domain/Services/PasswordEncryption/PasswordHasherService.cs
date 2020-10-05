using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSaver.Domain.Services.PasswordEncryption
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private const int Cost = 11;

        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: Cost);
        }

        public bool Verify(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
