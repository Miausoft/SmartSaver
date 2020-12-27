using System.Linq;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SmartSaver.Domain.Repositories
{
    public class AccountRepository : IAccountRepoository
    {
        private readonly ApplicationDbContext _context;

        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Account GetAccountById(string id)
        {
            return _context.Users
                .Include(u => u.Account)
                .First(u => u.Id.ToString().Equals(id))
                .Account;
        }

        public bool IsAccountValid(Account account)
        {
            return account.Goal > 0;
        }
    }
}
