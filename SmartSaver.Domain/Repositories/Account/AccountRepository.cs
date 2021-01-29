using System.Linq;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using SmartSaver.Domain.CustomExceptions;

namespace SmartSaver.Domain.Repositories
{
    public class AccountRepository : IAccountRepoository
    {
        private readonly ApplicationDbContext _context;

        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Account> CreateAsync(Account account)
        {
            _context.Accounts.Add(account);

            if (await _context.SaveChangesAsync() == 0)
            {
                throw new InvalidModelException();
            }

            return _context.Accounts
                .First(a => a.UserId.ToString().Equals(account.UserId));
        }

        public IEnumerable<Account> GetById<T>(T userId)
        {
            return _context.Accounts
                .Where(a => a.UserId.ToString().Equals(userId.ToString()));
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
