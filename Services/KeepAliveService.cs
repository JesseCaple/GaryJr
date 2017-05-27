using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GaryJr.Services
{
    class KeepAliveService
    {
        Timer timer;

        KeepAliveService()
        {
            int ms = (int)TimeSpan.FromHours(12).TotalMilliseconds;
            timer = new Timer(TimerCallback, null, 0, ms);
        }

        private static void TimerCallback(object o)
        {
            Console.WriteLine("Keep alive.");
        }
    }
}
