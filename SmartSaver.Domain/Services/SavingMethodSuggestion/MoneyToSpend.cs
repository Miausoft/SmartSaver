using SmartSaver.EntityFrameworkCore.Models;
using System;

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
            return acc.GoalEndDate > DateTime.Now ? acc.GoalEndDate.Subtract(DateTime.Now).Days / (365.25 / 12) : 0;
        }

        private static double TimeDays(Account acc)
        {
            return acc.GoalEndDate > DateTime.Now ? (acc.GoalEndDate - DateTime.Now).TotalDays : 0;
        }

        /*
         * we will need to return DateTime in the future but default values for some cases are needed so string is chosen for this time
         */
        public static string EstimatedTime(Account acc)
        {
            double savedSum = TransactionsCounter.TransactionsCounter.SavedSum(acc.Transactions, acc.GoalStartDate, acc.GoalEndDate);
            Console.WriteLine("Per taupymo laikotarpi nuo " + acc.GoalStartDate.ToShortDateString() + " iki " + acc.GoalEndDate.ToShortDateString() + " sutaupe: " + savedSum);

            if (savedSum < 0) return "Per taupymo laikotarpį kol kas nesutaupėte jokios pinigų sumos";
            else if (savedSum >= acc.Goal && DateTime.Today < acc.GoalEndDate) return "Sugebėjote sutaupyti anksčiau nei numatėte!";
            else if (savedSum >= acc.Goal && DateTime.Today > acc.GoalEndDate) return "Sugebėjote sutaupyti, tačiau vėliau nei numatėte!";
            else if (Average(DaysPassed(acc), savedSum) == 1) return acc.Goal + " sutaupysite iki " + acc.GoalEndDate.ToShortDateString();
            else return acc.Goal + " sutaupysite iki " + DateTime.Now.AddDays(Math.Ceiling((acc.Goal - savedSum) / Average(DaysPassed(acc), savedSum))).ToShortDateString();
        }

        private static double DaysPassed(Account acc)
        {
            return (DateTime.Now - acc.GoalStartDate).TotalDays;
        }

        private static double Average(double daysPassed, double savedSum)
        {
            return savedSum / (daysPassed + 1);
        }
    }
}
