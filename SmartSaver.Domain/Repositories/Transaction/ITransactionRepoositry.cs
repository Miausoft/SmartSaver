using SmartSaver.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSaver.Domain.Repositories
{
    public enum CreateTransactionResponse
    {
        Success,
        BadAmountError,
        GeneralError
    }

    public interface ITransactionRepoositry
    {
        /// <summary>
        /// Method that returns all account transactions from spending category.
        /// </summary>
        /// <param name="accId"></param>
        /// <returns>Spendings</returns>
        List<Transaction> GetThisMonthAccountSpendings(int accId);

        /// <summary>
        /// Method that returns balance history (how balance changed) for an account.
        /// </summary>
        /// <param name="accId"></param>
        /// <returns>List of balance changes</returns>
        List<Balance> GetBalanceHistory(int accId);

        /// <summary>
        /// All transactions an account made.
        /// </summary>
        /// <param name="accId">Account id</param>
        /// <returns>List of transactions</returns>
        List<Transaction> GetByAccountId(int accId);
        IEnumerable<Transaction> GetByAccountForDateRange(int accId, DateTime startData, DateTime endDate);

        Transaction GetById(int transactionId);
        Task<int> DeleteByIdAsync(int transactionId);
        Task<int> CreateTransaction(Transaction transaction);
    }
}