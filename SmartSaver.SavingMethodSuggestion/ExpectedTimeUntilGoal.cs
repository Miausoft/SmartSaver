using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SmartSaver.SavingMethodSuggestion
{
    class ExpectedTimeUntilGoal
    {
        public double AmountToAllocate(double goal, double savedSum, DateTime startDate, DateTime endDate)
        {
            //Console.WriteLine("Praejo dienu: " + DaysPassed(endDate, startDate));
            //Console.WriteLine("Liko dienu: " + DaysLeft(endDate));
            //Console.WriteLine("Vidurkis: " + Average(DaysPassed(endDate, startDate), savedSum));
            //Console.Write("Numatomas laikas (po kiek dienu nuo siandien datos): ");
            //Console.WriteLine(EstimatedTime(goal, savedSum, startDate, endDate));
           
            return (goal - savedSum) / DaysLeft(endDate);
        }

        public double EstimatedTime(double goal, double savedSum, DateTime startDate, DateTime endDate)
        {
            return Average(DaysPassed(endDate, startDate), savedSum) == 1 ? DaysLeft(endDate) : Math.Ceiling((goal - savedSum) / Average(DaysPassed(endDate, startDate), savedSum));
        }

        public double DaysLeft(DateTime endDate)
        {
            //return iki.DayOfYear - DateTime.Now.DayOfYear;
            return (endDate - new DateTime(2020, 9, 28)).TotalDays;
        }

        public double DaysPassed(DateTime endDate, DateTime startDate)
        {
            //return (DateTime.Now.Date - nuo).TotalDays;
            return (new DateTime(2020, 9, 28) - startDate).TotalDays;
        }

        public double Average(double daysPassed, double savedSum)
        {
            return savedSum / (daysPassed + 1);
        }
    }
}
