using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSaver.Domain.Services.AuthenticationServices
{
    public interface IHash
    {
        string Hash(string password);
        bool Verify(string password, string hash);
    }
}
