using SmartSaver.EntityFrameworkCore.Models;


namespace SmartSaver.EntityFrameworkCore.Repositories
{
    public interface IEmailVerificationRepository
    {
        public bool IsVerified(string userId);
        public string GetUserToken(string userId);
        public EmailVerification Create(EmailVerification emailVerification);
        public void Delete(string userId);
    }
}
