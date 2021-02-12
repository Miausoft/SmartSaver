using SmartSaver.Domain.Helpers;
using SmartSaver.Domain.Services.SavingSuggestions;
using SmartSaver.Domain.Services.Transactions;
using SmartSaver.EntityFrameworkCore.Models;
using System;

namespace SmartSaver.Domain.Services.SavingSuggestion
{
    public class Suggestions : ISuggestions
    {
        private readonly ITransactionsService _transactionsService;

        public Suggestions(ITransactionsService transactionsService)
        {
            _transactionsService = transactionsService;
        }

        public decimal AmountToSaveAMonth(Account acc)
        {
            return (acc.Goal / DateTimeHelper.DifferenceInDays(acc.GoalStartDate, acc.GoalEndDate) * DateTimeHelper.DaysUntilMonthEnd(DateTime.Now, acc.GoalStartDate, acc.GoalEndDate)) + AmountLeftToSave(acc);
        }

        public decimal Average(decimal daysPassed, decimal savedSum)
        {
            return savedSum / (daysPassed + 1);
        }

        public decimal FreeMoneyToSpend(Account acc)
        {
            return _transactionsService.SavedSum(acc.Transactions, acc.GoalStartDate, acc.GoalEndDate)
                - (AmountToSaveAMonth(acc) * Math.Ceiling(DateTimeHelper.DifferenceInMonths(acc.GoalStartDate, DateTime.Now)));
        }

        public decimal AmountLeftToSave(Account acc)
        {
            var i = 1;
            var firstDayOfPreviousMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var leftToSpend = 0m;

            while (firstDayOfPreviousMonth.Year != acc.GoalStartDate.Year && firstDayOfPreviousMonth.Month != acc.GoalStartDate.Month)
            {
                firstDayOfPreviousMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-i);
                var lastDayOfPreviousMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-i+1).AddDays(-1);
                var savedPreviousMonth = _transactionsService.SavedSum(acc.Transactions, firstDayOfPreviousMonth, lastDayOfPreviousMonth);
                var amountToSavePreviousMonth = (acc.Goal / DateTimeHelper.DifferenceInDays(acc.GoalStartDate, acc.GoalEndDate)) * DateTimeHelper.DaysUntilMonthEnd(firstDayOfPreviousMonth, acc.GoalStartDate, acc.GoalEndDate);

                leftToSpend += amountToSavePreviousMonth - savedPreviousMonth;
                i++;
            }

            return leftToSpend;
        }

        public DateTime EstimatedTime(Account acc)
        {
            decimal savedSum = _transactionsService.SavedSum(acc.Transactions, acc.GoalStartDate, acc.GoalEndDate);

            if (savedSum < 0) return DateTime.MinValue;
            if (savedSum >= acc.Goal && DateTime.Today < acc.GoalEndDate) return DateTime.Today;
            else if (Average(Math.Floor(DateTimeHelper.DifferenceInDays(acc.GoalStartDate, DateTime.Now)), savedSum) == 1) return acc.GoalEndDate.Date;
            else if (Average(Math.Floor(DateTimeHelper.DifferenceInDays(acc.GoalStartDate, DateTime.Now)), savedSum) == 0) return DateTime.MinValue;
            else return DateTime.Now.AddDays((double)Math.Ceiling((acc.Goal - savedSum) / Average(DateTimeHelper.DifferenceInDays(acc.GoalStartDate, DateTime.Now), savedSum)));
        }
    }
}
