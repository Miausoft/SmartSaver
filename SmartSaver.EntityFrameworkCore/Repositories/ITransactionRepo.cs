using SmartSaver.EntityFrameworkCore.Models;
using System.Collections.Generic;

namespace SmartSaver.EntityFrameworkCore.Repositories
{
    public interface ITransactionRepo
    {
        /// <summary>
        /// Method that returns all account transactions from spending category.
        /// </summary>
        /// <param name="accId"></param>
        /// <returns>Spendings</returns>
        List<Transaction> GetAccountSpendings(int accId);

        /// <summary>
        /// Method that returns balance history (how balance changed) for an account.
        /// </summary>
        /// <param name="accId"></param>
        /// <returns>List of balance changes</returns>
        List<Balance> GetBalanceHistory(int accId);
    }
}