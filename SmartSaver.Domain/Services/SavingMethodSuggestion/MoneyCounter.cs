using SmartSaver.EntityFrameworkCore.Models;
using System;

namespace SmartSaver.Domain.Services.SavingMethodSuggestion
{
    public static class MoneyCounter
    {
        /*
         * need some better implementation of this method. 
         * Don't know how to work out with the numbers to represent amount of money user could spend for a month
         */
        /*        public static double AmountToSpendAMonth(Account acc)
                {
                    return Math.Round(TransactionsCounter.TransactionsCounter.TotalIncome(acc.Transactions, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now.AddDays(1)) - AmountToSaveAMonth(acc), 2);
                    return AmountLeftToSpend(acc) / TimeInMonths(acc);
                }*/

        /*
        * will be comparing this number with the amount of money he has to save a month (AmountToSaveAMonth())
        */
        public static double AmountSavedCurrentMonth(Account acc)
        {
            return TransactionsCounter.TransactionsCounter.SavedSum(acc.Transactions, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now.AddDays(1));
        }

        /*
        * will be comparing this number with the amount of money he has to save a month (AmountToSaveAMonth())
        */
        public static double AmountSpentCurrentMonth(Account acc)
        {
            return TransactionsCounter.TransactionsCounter.TotalExpense(acc.Transactions, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now.AddDays(1));
        }

        /*
         * a static number to represent amount of money account have to save every month
         */
        public static double AmountToSaveAMonth(Account acc)
        {
            return Math.Round(acc.Goal / TimeInMonths(acc), 2);
        }

        /*
        * a static number to represent a goal time in months
        */
        public static double TimeInMonths(Account acc)
        {
            return (acc.GoalEndDate.Subtract(acc.GoalStartDate)).Days / (365.25 / 12);
        }

        /*
        * a dynamic number which shows account's balance he can spend without any hesitation because goal will be reached in time
        * zero represents that account can't have any more spendings if he wants to reach his goal in time. There will be some suggestion for an user in the future
        */
        public static double AmountLeftToSpend(Account acc)
        {
            double sum = Math.Round(TransactionsCounter.TransactionsCounter.SavedSum(acc.Transactions, DateTime.MinValue, DateTime.MaxValue)
                - (AmountToSaveAMonth(acc) * Math.Ceiling(MonthsPassed(acc))), 2);
            return sum >= 0 ? sum : 0; 
        }

        /*
         * we will need to return DateTime in the future but default values for some cases are needed so string is chosen for this time
         */
        public static string EstimatedTime(Account acc)
        {
            double savedSum = TransactionsCounter.TransactionsCounter.SavedSum(acc.Transactions, acc.GoalStartDate, acc.GoalEndDate);

            if (savedSum < 0) return "Per taupymo laikotarpį kol kas nesutaupėte jokios pinigų sumos";
            else if (savedSum >= acc.Goal && DateTime.Today < acc.GoalEndDate) return "Sugebėjote sutaupyti anksčiau nei numatėte!";
            else if (savedSum >= acc.Goal && DateTime.Today > acc.GoalEndDate) return "Sugebėjote sutaupyti, tačiau vėliau nei numatėte!";
            else if (Average(DaysPassed(acc), savedSum) == 1) return acc.Goal + " sutaupysite iki " + acc.GoalEndDate.ToShortDateString();
            else return acc.Goal + " sutaupysite iki " + DateTime.Now.AddDays(Math.Ceiling((acc.Goal - savedSum) / Average(DaysPassed(acc), savedSum))).ToShortDateString();
        }

        /*
        * a dynamic number which represents how many days have passed since account started to save
        */
        private static double DaysPassed(Account acc)
        {
            return DateTime.Now.Subtract(acc.GoalStartDate).TotalDays;
        }

        /*
         * a dynamic number which represents how many months have passed since account started to save
         */
        private static double MonthsPassed(Account acc)
        {
            return DateTime.Now.Subtract(acc.GoalStartDate).Days / (365.25 / 12);
        }

        /*
        * avarage account saved everyday
        */
        private static double Average(double daysPassed, double savedSum)
        {
            return savedSum / (daysPassed + 1);
        }
    }
}
