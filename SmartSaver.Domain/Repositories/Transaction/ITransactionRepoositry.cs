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
        IEnumerable<Transaction> GetByAccountId<T>(T accId);
        IEnumerable<Transaction> GetByAccountForDateRange<T>(T accId, DateTime startData, DateTime endDate);
        Transaction GetById<T>(T transactionId);
        Task<int> DeleteById<T>(T transactionId);
        Task<int> Create(Transaction transaction);
    }
}