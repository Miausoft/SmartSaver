using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.Domain.Repositories
{
    public interface IEmailVerificationRepository
    {
        public bool IsVerified<T>(T userId);
        public string GetUserToken<T>(T userId);
        public EmailVerification Create(EmailVerification emailVerification);
        public void Delete<T>(T userId);
    }
}
