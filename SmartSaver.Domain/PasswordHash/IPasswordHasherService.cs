using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSaver.Domain.PasswordHash
{
    public interface IPasswordHasherService : IPasswordHash, IPasswordVerify { }

    public interface IPasswordVerify
    {
        /// <summary>
        /// Confirms if given password is equal to given passwordHash.
        /// </summary>
        /// <param name="password">Users password</param>
        /// <param name="passwordHash">Users password hash.</param>
        /// <returns>Boolean</returns>
        bool Verify(string password, string passwordHash);
    }

    public interface IPasswordHash
    {
        /// <summary>
        /// Hashes password and returns it.
        /// </summary>
        /// <param name="password">User's password</param>
        /// <returns>Hashed password</returns>
        string Hash(string password);
    }
}
