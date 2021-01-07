using SmartSaver.EntityFrameworkCore.Models;
using System.Threading.Tasks;

namespace SmartSaver.Domain.Repositories
{
    public interface IAccountRepoository
    {
        Account GetById<T>(T accId);
        public Task<int> Save();
        bool IsValid(Account account);
    }
}