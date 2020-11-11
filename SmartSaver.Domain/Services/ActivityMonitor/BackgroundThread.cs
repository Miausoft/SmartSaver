using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Timer = System.Timers.Timer;

namespace SmartSaver.Domain.Services.ActivityMonitor
{
    public static class BackgroundThread
    {
        private static Timer _timer;

        static void SetTimer()
        {
            _timer = new Timer(300000); // 5min
            _timer.Elapsed += TimerOnElapsed;
            _timer.AutoReset = true; //?
            _timer.Enabled = true;
        }

        private static void TimerOnElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine($"Timer ended at {e.SignalTime}");
        }

        public static void BackgroundTimer()
        {
            Thread t = new Thread(new ThreadStart(SetTimer));
        }

            /*
            public delegate void TimeRemainingSessionHandler();
            public static void ThreadProc()
            {
                DateTime startTime = DateTime.Now;
                DateTime endTime = DateTime.Now.AddMinutes(10);
                TimeSpan interval = endTime - startTime;


            }*/
        }
}
