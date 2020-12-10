using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.EntityFrameworkCore.Repositories
{
    public interface IAccountRepo
    {
        /// <summary>
        /// Gets single account object for given username.
        /// </summary>
        /// <param name="username">Username of a user</param>
        /// <returns>Account object</returns>
        Account GetAccountByUsername(string username);

        /// <summary>
        /// Checks if account already had filled his information with
        /// saving goal, start, end dates.
        /// </summary>
        /// <param name="account">Reference to an account object</param>
        /// <returns>Boolean</returns>
        bool IsAccountValid(Account account);
    }
}