using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSaver.Domain.Services.SavingMethodSuggestion
{
    public static class MoneyToSpend
    {
        /*public static double AmountToSpendAMonth(double revenue, double goal, DateTime startDate, DateTime endDate)
        {
            return revenue - AmountToSaveAMonth(goal, TimeMonths(startDate, endDate));
        }

        public static double AmountToSpendADay(double revenue, double goal, DateTime startDate, DateTime endDate)
        {
            return (revenue - AmountToSaveAMonth(goal, TimeMonths(startDate, endDate))) / (endDate - startDate).TotalDays;
        }

        public static double AmountToSaveAMonth(double goal, double timeMonths)
        {
            return goal / timeMonths;
        }

        public static double AmountToSaveADay(double goal, double timeDays)
        {
            return goal / timeDays;
        }

        public static double TimeMonths(DateTime startDate, DateTime endDate)
        {
            return endDate.Subtract(startDate).Days / (365.25 / 12);
        }
        public static double TimeDays(DateTime startDate, DateTime endDate)
        {
            return (endDate - startDate).TotalDays;
        }

        public static double EstimatedTime(double goal, double savedSum, DateTime startDate, DateTime endDate)
        {
            return Average(DaysPassed(endDate, startDate), savedSum) == 1 ? DaysLeft(endDate) : Math.Ceiling((goal - savedSum) / Average(DaysPassed(endDate, startDate), savedSum));
        }

        public static double DaysLeft(DateTime endDate)
        {
            return iki.DayOfYear - DateTime.Now.DayOfYear;
        }

        public static double DaysPassed(DateTime endDate, DateTime startDate)
        {
            return (DateTime.Now.Date - nuo).TotalDays;
        }

        public static double Average(double daysPassed, double savedSum)
        {
            return savedSum / (daysPassed + 1);
        }*/
    }
}
