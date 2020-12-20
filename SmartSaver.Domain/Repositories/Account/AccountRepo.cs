using System;
using System.Collections.Generic;
using System.Linq;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SmartSaver.Domain.Repositories
{
    public class AccountRepo : IAccountRepo
    {
        private ApplicationDbContext _context;

        public AccountRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public AccountDto GetAccountById(string id)
        {
            return _context.Users
                .Include(u => u.Account)
                .First(u => u.Id.ToString().Equals(id))
                .Account;
        }

        public bool IsAccountValid(AccountDto account)
        {
            return account.Goal > 0;
        }
    }
}
