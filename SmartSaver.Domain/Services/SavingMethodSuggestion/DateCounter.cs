using System;

namespace SmartSaver.Domain.Services.SavingMethodSuggestion
{
    public struct DateCounter
    {
        /// <summary>
        /// shows how many days left until goal
        /// </summary>
        public static int DaysLeft(DateTime goalEndDate)
        {
            return goalEndDate.Subtract(DateTime.Now).Days;
        }

        /// <summary>
        /// a dynamic number which represents how many days have passed since account started to save
        /// </summary>
        public static decimal DaysPassed(DateTime goalStartDate)
        {
            return (decimal)DateTime.Now.Subtract(goalStartDate).TotalDays;
        }

        /// <summary>
        /// a static number to represent a goal time in days
        /// </summary>
        public static decimal GoalTimeInDays(DateTime goalStartDate, DateTime goalEndDate)
        {
            return (decimal)goalEndDate.Subtract(goalStartDate).Days;
        }

        /// <summary>
        /// returns remaining days until the end of a month
        /// </summary>
        public static decimal DaysUntilMonthEnd(DateTime goalStartDate, DateTime goalEndDate)
        {
            if (DateTime.Now.Year == goalEndDate.Year && DateTime.Now.Month == goalEndDate.Month)
            {
                return goalEndDate.Day;
            }

            else if (DateTime.Now.Year == goalStartDate.Year && DateTime.Now.Month == goalStartDate.Month)
            {
                return DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - goalStartDate.Day + 1;
            }

            else
            {
                return DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            }
        }

        /// <summary>
        /// a dynamic number which represents how many months have passed since account started to save
        /// </summary>
        public static decimal MonthsPassed(DateTime goalStartDate)
        {
            if (DateTime.Now.DayOfYear != goalStartDate.DayOfYear)
            {
                return (decimal)(DateTime.Now.Subtract(goalStartDate).Days / (365.25 / 12));
            }

            else
            {
                return 1;
            }
        }
    }
}
