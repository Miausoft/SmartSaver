using Microsoft.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;
using System.Linq;

namespace SmartSaver.Domain.Repositories
{
    public class EmailVerificationRepository : IEmailVerificationRepository
    {
        private readonly ApplicationDbContext _db;

        public EmailVerificationRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool IsVerified(string userId)
        {
            if(_db.EmailVerifications.FirstOrDefault(u => u.UserId.ToString().Equals(userId)) == null)
            {
                return true;
            }

            return false;
        }

        public string GetUserToken(string userId)
        {
            return _db.EmailVerifications.Include(a => a.User).Where(a => a.UserId.ToString().Equals(userId)).Select(a => a.Token).FirstOrDefault();
        }

        public EmailVerificationDto Create(EmailVerificationDto emailVerification)
        {
            _db.EmailVerifications.Add(emailVerification);
            _db.SaveChanges();

            return emailVerification;
        }

        public void Delete(string userId)
        {
            var dataToDelete = _db.EmailVerifications.FirstOrDefault(u => u.UserId.ToString() == userId);
            _db.EmailVerifications.Remove(dataToDelete);
            _db.SaveChanges();
        }
    }
}
