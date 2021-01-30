namespace SmartSaver.Domain.TokenValidation
{
    public interface ITokenValidationService
    {
        public string GenerateToken<T>(T userId);
        public bool ValidateToken(string token);
        public string GetClaim(string token, string claimType);
    }
}
