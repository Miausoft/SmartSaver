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
        IEnumerable<Transaction> GetByAccountId(int accId);
        IEnumerable<Transaction> GetByAccountForDateRange(int accId, DateTime startData, DateTime endDate);
        Transaction GetById(int transactionId);
        Task<int> DeleteById(int transactionId);
        Task<int> CreateTransaction(Transaction transaction);
    }
}