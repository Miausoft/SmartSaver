namespace SmartSaver.Domain.Services.AuthenticationServices.Jwt
{
    public interface ITokenAuthentication
    {
        public string GenerateToken<T>(T userId);
        public bool ValidateToken(string token);
        public string GetClaim(string token, string claimType);
    }
}
