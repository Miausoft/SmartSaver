using System;

namespace SmartSaver.Domain.Helpers
{
    public struct DateTimeHelper
    {
        /// <summary>
        /// calculates the number of days between two dates
        /// </summary>
        public static decimal DifferenceInDays(DateTime startDate, DateTime endDate)
        {
            return (decimal)endDate.Subtract(startDate).TotalDays;
        }

        /// <summary>
        /// calculates the number of months between two dates
        /// </summary>
        public static decimal DifferenceInMonths(DateTime startDate, DateTime endDate)
        {
            return (endDate.Month - startDate.Month) + 12 * (endDate.Year - startDate.Year) + 1;
        }

        /// <summary>
        /// returns remaining days until the end of a month
        /// </summary>
        public static int DaysUntilMonthEnd(DateTime now, DateTime startDate, DateTime endDate)
        {
            if(startDate.Year == endDate.Year && startDate.Month == endDate.Month)
            {
                return endDate.Subtract(startDate).Days;
            }

            else if (now.Year == endDate.Year && now.Month == endDate.Month)
            {
                return endDate.Day - 1;
            }

            else if (now.Year == startDate.Year && now.Month == startDate.Month)
            {
                return DateTime.DaysInMonth(now.Year, now.Month) - startDate.Day + 1;
            }

            else
            {
                return DateTime.DaysInMonth(now.Year, now.Month);
            }
        }

        public static bool InRange(DateTime dateToCheck, DateTime startDate, DateTime endDate)
        {
            return dateToCheck >= startDate && dateToCheck < endDate;
        }

        public static DateTime TruncateToDayStart(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }
    }
}
