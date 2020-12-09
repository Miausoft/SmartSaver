using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSaver.Domain.TokenValidation
{
    public interface ITokenValidation
    {
        public string GenerateToken(string userId);
        public bool ValidateToken(string token);
        public string GetClaim(string token, string claimType);
    }
}
