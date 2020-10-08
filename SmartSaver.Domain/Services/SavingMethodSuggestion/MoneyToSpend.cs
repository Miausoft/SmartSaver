using SmartSaver.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSaver.Domain.Services.SavingMethodSuggestion
{
    public static class MoneyToSpend
    {
        public static double AmountToSpendAMonth(Account acc)
        {
            return acc.Revenue - AmountToSaveAMonth(acc);
        }

        public static double AmountToSpendADay(Account acc)
        {
            return (acc.Revenue - AmountToSaveAMonth(acc)) / (acc.GoalEndDate - acc.GoalStartDate).TotalDays;
        }

        public static double AmountToSaveAMonth(Account acc)
        {
            return acc.Goal / TimeMonths(acc);
        }

        public static double AmountToSaveADay(Account acc)
        {
            return acc.Goal / TimeDays(acc);
        }

        private static double TimeMonths(Account acc)
        {
            return acc.GoalEndDate.Subtract(acc.GoalStartDate).Days / (365.25 / 12);
        }

        private static double TimeDays(Account acc)
        {
            return (acc.GoalEndDate - acc.GoalStartDate).TotalDays;
        }

        /*
         * to make this method work we have to know how much money an user saved already. This could be implemented in the future as an extra feature
         */
        //public static double EstimatedTime(Account acc)
        //{
        //    return Average(DaysPassed(acc), savedSum) == 1 ? DaysLeft(acc) : Math.Ceiling((acc.Goal - savedSum) / Average(DaysPassed(acc), savedSum));
        //}

        private static double DaysLeft(Account acc)
        {
            return acc.GoalEndDate.DayOfYear - DateTime.Now.DayOfYear;
        }

        private static double DaysPassed(Account acc)
        {
            return (DateTime.Now.Date - acc.GoalStartDate).TotalDays;
        }

        /*
         * to make this method work we have to know how much money an user saved already
         */
        //public static double Average(double daysPassed, double savedSum)
        //{
        //    return savedSum / (daysPassed + 1);
        //}
    }
}
