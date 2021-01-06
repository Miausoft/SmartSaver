using SmartSaver.Domain.Services.TransactionsCounterService;
using SmartSaver.EntityFrameworkCore.Models;
using System;

namespace SmartSaver.Domain.Services.SavingMethodSuggestion
{
    public static class MoneyCounter
    {
        /// <summary>
        /// a dynamic number to represent amount of money account have to save current month.
        /// The method can return a negative number meaning that in previous months it was saved more than it was necessary.
        /// </summary>
        public static decimal AmountToSaveAMonth(Account acc)
        {
            return (acc.Goal / DateCounter.GoalTimeInDays(acc.GoalStartDate, acc.GoalEndDate) * DateCounter.DaysUntilMonthEnd(DateTime.Now, acc.GoalStartDate, acc.GoalEndDate)) + AmountLeftToSave(acc);
        }

        /// <summary>
        /// amount of money account saved everyday on average
        /// </summary>
        public static decimal Average(decimal daysPassed, decimal savedSum)
        {
            return savedSum / (daysPassed + 1);
        }

        /// <summary>
        /// A dynamic number which shows account's balance he can spend without any hesitation because goal will be reached in time.
        /// Zero represents that account can't have any more spendings if he wants to reach his goal in time.
        /// </summary>
        public static decimal FreeMoneyToSpend(Account acc)
        {
            return TransactionsCounter.SavedSum(acc.Transactions, acc.GoalStartDate, acc.GoalEndDate)
                - (AmountToSaveAMonth(acc) * Math.Ceiling(DateCounter.MonthsPassed(acc.GoalStartDate)));
        }

        /// <summary>
        /// A dynamic number to represent amount of money account left to save until now.
        /// The method does not consider today's date. It only counts previous months.
        /// The method will return a negative number if the account saved more than he had to.
        /// The method will return a positive number if the account saved less than he had to.
        /// </summary>
        public static decimal AmountLeftToSave(Account acc)
        {
            var i = 1;
            var firstDayOfPreviousMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var leftToSpend = 0m;

            while (firstDayOfPreviousMonth.Year != acc.GoalStartDate.Year && firstDayOfPreviousMonth.Month != acc.GoalStartDate.Month)
            {
                firstDayOfPreviousMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-i);
                var lastDayOfPreviousMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-i+1).AddDays(-1);
                var savedPreviousMonth = TransactionsCounter.SavedSum(acc.Transactions, firstDayOfPreviousMonth, lastDayOfPreviousMonth);
                var amountToSavePreviousMonth = (acc.Goal / DateCounter.GoalTimeInDays(acc.GoalStartDate, acc.GoalEndDate)) * DateCounter.DaysUntilMonthEnd(firstDayOfPreviousMonth, acc.GoalStartDate, acc.GoalEndDate);

                leftToSpend += amountToSavePreviousMonth - savedPreviousMonth;
                i++;
            }

            return leftToSpend;
        }

        public static DateTime EstimatedTime(Account acc)
        {
            decimal savedSum = TransactionsCounter.SavedSum(acc.Transactions, acc.GoalStartDate, acc.GoalEndDate);

            if (savedSum < 0) return DateTime.MinValue;
            if (savedSum >= acc.Goal && DateTime.Today < acc.GoalEndDate) return DateTime.Today;
            else if (MoneyCounter.Average(Math.Floor(DateCounter.DaysPassed(acc.GoalStartDate)), savedSum) == 1) return acc.GoalEndDate.Date;
            else if (MoneyCounter.Average(Math.Floor(DateCounter.DaysPassed(acc.GoalStartDate)), savedSum) == 0) return DateTime.MinValue;
            else return DateTime.Now.AddDays((double)Math.Ceiling((acc.Goal - savedSum) / MoneyCounter.Average(DateCounter.DaysPassed(acc.GoalStartDate), savedSum)));
        }
    }
}
