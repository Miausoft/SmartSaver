using SmartSaver.EntityFrameworkCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSaver.Domain.Repositories
{
    public interface IAccountRepoository
    {
        Task<Account> CreateAsync(Account account);
        IEnumerable<Account> GetById<T>(T userId);
        public Task<int> Save();
        bool IsValid(Account account);
    }
}