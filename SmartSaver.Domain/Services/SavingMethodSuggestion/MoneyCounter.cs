using SmartSaver.Domain.Services.TransactionsCounterService;
using SmartSaver.EntityFrameworkCore.Models;
using System;

namespace SmartSaver.Domain.Services.SavingMethodSuggestion
{
    public static class MoneyCounter
    {
        /// <summary>
        /// a dynamic number to represent amount of money account have to save current month
        /// </summary>
        public static decimal AmountToSaveAMonth(decimal goal, DateTime goalStartDate, DateTime goalEndDate)
        {
            return (goal / DateCounter.GoalTimeInDays(goalStartDate, goalEndDate)) * DateCounter.DaysUntilMonthEnd(goalStartDate, goalEndDate);
        }

        /// <summary>
        /// avarage account saved everyday
        /// </summary>
        public static decimal Average(decimal daysPassed, decimal savedSum)
        {
            return savedSum / (daysPassed + 1);
        }

        /// <summary>
        /// a dynamic number which shows account's balance he can spend without any hesitation because goal will be reached in time
        /// zero represents that account can't have any more spendings if he wants to reach his goal in time. There will be some suggestion for an user in the future
        /// </summary>
        public static decimal AmountLeftToSpend(AccountDto acc)
        {
            return TransactionsCounter.SavedSum(acc.Transactions, acc.GoalStartDate, acc.GoalEndDate)
                - (AmountToSaveAMonth(acc.Goal, acc.GoalStartDate, acc.GoalEndDate) * Math.Ceiling(DateCounter.MonthsPassed(acc.GoalStartDate)));
        }
    }
}
