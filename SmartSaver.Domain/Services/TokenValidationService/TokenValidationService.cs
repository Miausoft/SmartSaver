using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace SmartSaver.Domain.TokenValidation
{
    public class TokenValidationService : ITokenValidationService
    {
        private readonly IConfiguration _configuration;
        public TokenValidationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(string userId)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId)
                }),
                Expires = DateTime.UtcNow.AddDays(double.Parse(_configuration["Token:DaysToExpiration"])),
                Issuer = _configuration["Token:MyIssuer"],
                Audience = _configuration["Token:MyAudience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Token:MySecret"])), SecurityAlgorithms.HmacSha256Signature),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string GetClaim(string token, string claimType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            if (!securityToken.Claims.Any(claim => claim.Type == claimType))
            {
                return String.Empty;
            }

            return securityToken.Claims.FirstOrDefault(claim => claim.Type == claimType).Value;
        }

        public bool ValidateToken(string token)
        {
            try
            {
                new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["Token:MyIssuer"],
                    ValidAudience = _configuration["Token:MyAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Token:MySecret"]))
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
