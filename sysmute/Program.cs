using System;
using System.Threading;

namespace sysmute
{
    static class DateTimeExt
    {
        // DateTime extention to easily compare 2 times
        // http://stackoverflow.com/questions/10631044/c-sharp-compare-time-between-two-time-intervals
        public static bool TimeOfDayIsBetween(this DateTime t, DateTime start, DateTime end)
        {
            var time_of_day = t.TimeOfDay;
            var start_time_of_day = start.TimeOfDay;
            var end_time_of_day = end.TimeOfDay;

            if (start_time_of_day <= end_time_of_day)
                return start_time_of_day <= time_of_day && time_of_day <= end_time_of_day;

            return start_time_of_day <= time_of_day || time_of_day <= end_time_of_day;
        }
    }

    class Program
    {
        public static readonly DateTime startTime = new DateTime(1969, 2, 25, 22, 00, 00); // Date doesn't matter. 10pm
        public static readonly DateTime endTime = new DateTime(1969, 2, 26, 9, 00, 00); // Date doesn't matter. 9am
        public static readonly int SleepInterval = 1000 * 60; // Check the time every 1 minute

        static void Main(string[] args)
        {
            Console.WriteLine($"sysmute. Program will mute system audio between {startTime.TimeOfDay} and {endTime.TimeOfDay}");

            var needToMute = true; // Controls to not mute if already muted.

            while (true)
            {
                var timeNow = DateTime.Now;
                Console.WriteLine($"Current Time: {timeNow.TimeOfDay}");

                // If the current time falls within the range of the quite time, mute
                // If user unmutes during restricted time, it's up to them to remember to remute manually
                if (timeNow.TimeOfDayIsBetween(startTime, endTime))
                {
                    if (needToMute)
                    {
                        Console.WriteLine("Muting Master Volume");
                        AudioManager.SetMasterVolumeMute(true);
                        needToMute = false;
                    }
                }
                else
                {
                    needToMute = true; // This resets each day, once it's out of the restricted range
                }

                Thread.Sleep(SleepInterval);
            }
        }
    }
}
