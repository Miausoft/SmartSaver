using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmartSaver.Domain.Services.TipManager
{
    public static class Tips
    {
        private static readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "AllTips.txt");
        private static readonly List<string> tips = File.ReadLines(filePath).ToList();

        /// <summary>
        /// completely random tip from the list
        /// </summary>
        public static string RandomTip()
        {
            return tips[new Random().Next(tips.Count)];
        }

        /// <summary>
        /// tip based on day of the year
        /// </summary>
        public static string DayBasedTip()
        {
            return tips[new Random(DateTime.Now.Day).Next(tips.Count)];
        }
    }
}
