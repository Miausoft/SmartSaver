using System;
using SmartSaver.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SmartSaver.Domain.Managers 
{
    public interface ITransactionManager 
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