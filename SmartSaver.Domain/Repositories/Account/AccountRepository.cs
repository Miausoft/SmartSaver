using System.Linq;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace SmartSaver.Domain.Repositories
{
    public class AccountRepository : IAccountRepoository
    {
        private readonly ApplicationDbContext _context;

        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Account GetById<T>(T accId)
        {
            return _context.Users
                .Include(u => u.Account)
                .First(u => u.Id.ToString().Equals(accId.ToString()))
                .Account;
        }

        public Task<int> Save()
        {
            return _context.SaveChangesAsync();
        }

        public bool IsValid(Account account)
        {
            return account.Goal > 0;
        }
    }
}
