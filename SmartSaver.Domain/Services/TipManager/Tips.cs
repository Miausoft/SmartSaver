using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmartSaver.Domain.Services.TipManager
{
    public static class Tips
    {
        static string workingDirectory = Environment.CurrentDirectory;
        static string path = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName + "\\SmartSaver.Domain\\Resources\\AllTips.txt";
        static List<string> tips = System.IO.File.ReadLines(path).ToList();


        /// <summary>
        /// completely random tip from list
        /// </summary>
        public static string RandomTip()
        {
            var random = new Random();
            int index = random.Next(tips.Count);

            return tips[index];
        }

        /// <summary>
        /// tip based on day of the year
        /// </summary>
        public static string DayBasedTip()
        {
            Random random = new Random(DateTime.Now.Day);
            int index = random.Next(tips.Count);

            return tips[index];
        }
    }
}
