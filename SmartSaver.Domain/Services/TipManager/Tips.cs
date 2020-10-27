using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SmartSaver.Domain.Services.TipManager
{
    public static class Tips
    {
        static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"AllTips.txt");

        static List<string> tips = System.IO.File.ReadLines(path).ToList();

        public static string RandomTip() // completely random tip from list
        {
            var random = new Random();
            int index = random.Next(tips.Count);

            return tips[index];
        }

        public static string DayBasedTip()
        {
            Random random = new Random(DateTime.Now.Day); // tip based on day of the year
            int index = random.Next(tips.Count);

            return tips[index];
        }
    }
}
