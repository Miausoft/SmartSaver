using SmartSaver.EntityFrameworkCore.Models;
using System;
using SmartSaver.Domain.Services.TransactionsCounterService;

namespace SmartSaver.Domain.Services.SavingMethodSuggestion
{
    public static class MoneyCounter
    {
        /// <summary>
        /// a static number to represent amount of money account have to save every month
        /// </summary>
        public static decimal AmountToSaveAMonth(Account acc)
        {
            return acc.Goal / TimeInMonths(acc);
        }

        /// <summary>
        /// a dynamic number which shows account's balance he can spend without any hesitation because goal will be reached in time
        /// zero represents that account can't have any more spendings if he wants to reach his goal in time. There will be some suggestion for an user in the future
        /// </summary>
        public static decimal AmountLeftToSpend(Account acc)
        {
            decimal sum = TransactionsCounter.SavedSum(acc.Transactions, DateTime.MinValue, DateTime.MaxValue)
                - (AmountToSaveAMonth(acc) * Math.Ceiling(MonthsPassed(acc)));
            return sum >= 0 ? sum : -1;
        }

        /// <summary>
        /// we will need to return DateTime in the future but default values for some cases are needed so string is chosen for this time
        /// </summary>
        public static string EstimatedTime(Account acc)
        {
            decimal savedSum = TransactionsCounter.SavedSum(acc.Transactions, acc.GoalStartDate, acc.GoalEndDate);

            if (savedSum < 0) return "Per taupymo laikotarpį kol kas nesutaupėte jokios pinigų sumos";
            else if (savedSum >= acc.Goal && DateTime.Today < acc.GoalEndDate) return "Sugebėjote sutaupyti anksčiau nei numatėte!";
            else if (savedSum >= acc.Goal && DateTime.Today > acc.GoalEndDate) return "Sugebėjote sutaupyti, tačiau vėliau nei numatėte!";
            else if (Average(DaysPassed(acc), savedSum) == 1) return acc.Goal + " sutaupysite iki " + acc.GoalEndDate.ToShortDateString();
            else if (Average(DaysPassed(acc), savedSum) == 0) return "Numatytas laikas iki taupymo pabaigos: N/A";
            else return acc.Goal.ToString("C") + " sutaupysite iki\n" + DateTime.Now.AddDays((double)Math.Ceiling((acc.Goal - savedSum) / Average(DaysPassed(acc), savedSum))).ToShortDateString();
        }

        /// <summary>
        /// shows how many days left until goal
        /// </summary>
        public static int DaysLeft(Account acc)
        {
            return acc.GoalEndDate.Subtract(DateTime.Now).Days;
        }

        /// <summary>
        /// a static number to represent a goal time in months
        /// </summary>
        private static decimal TimeInMonths(Account acc)
        {
            return (decimal)(acc.GoalEndDate.Subtract(acc.GoalStartDate).Days / (365.25 / 12));
        }

        /// <summary>
        /// a dynamic number which represents how many days have passed since account started to save
        /// </summary>
        private static decimal DaysPassed(Account acc)
        {
            return (decimal)DateTime.Now.Subtract(acc.GoalStartDate).TotalDays;
        }

        /// <summary>
        /// a dynamic number which represents how many months have passed since account started to save
        /// </summary>
        private static decimal MonthsPassed(Account acc)
        {
            if (DateTime.Now.DayOfYear != acc.GoalStartDate.DayOfYear)
            {
                return (decimal)(DateTime.Now.Subtract(acc.GoalStartDate).Days / (365.25 / 12));
            }

            else
            {
                return 1;
            }
        }

        /// <summary>
        /// avarage account saved everyday
        /// </summary>
        private static decimal Average(decimal daysPassed, decimal savedSum)
        {
            return savedSum / (daysPassed + 1);
        }
    }
}
