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

        public static int Hour(this string timeString)
        {
            try
            {
                var hour = Int32.Parse(timeString.Substring(0, timeString.IndexOf(":", StringComparison.Ordinal)));

                return hour;
            }
            catch(Exception e)
            {
                Console.WriteLine($"Invalid hour {timeString}.  Must use HH:MM");
                throw e;
            }
        }

        public static int Minute(this string timeString)
        {
            try
            {
                var minute = Int32.Parse(timeString.Substring(timeString.IndexOf(":", StringComparison.Ordinal) + 1));

                return minute;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Invalid minute {timeString}.  Must use HH:MM");
                throw e;
            }
        }
    }

    class Program
    {
        public static DateTime startTime = new DateTime(1969, 2, 25, 22, 00, 00); // Date doesn't matter. 10pm
        public static DateTime endTime = new DateTime(1969, 2, 26, 9, 00, 00); // Date doesn't matter. 9am
        public static readonly int SleepInterval = 1000 * 60; // Check the time every 1 minute

        static void Main(string[] args)
        {
            // Check the command args.  If set, override the default
            if (args.Length > 0 && args[0] != null)
            {
                try
                {
                    startTime = new DateTime(1969, 2, 25, args[0].Hour(), args[0].Minute(), 00);
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid start time");
                    return;
                }
            }

            if (args.Length > 1 && args[1] != null)
            {
                try
                {
                    endTime = new DateTime(1969, 2, 26, args[1].Hour(), args[1].Minute(), 00);
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid end time");
                    return;
                }
            }

            Console.WriteLine($"sysmute. Program will mute system audio between {startTime.TimeOfDay} and {endTime.TimeOfDay}");
            if (args.Length == 0)
                Console.WriteLine($"To override startTime and endTime, pass in via command line. E.g. > sysmute 23:00 08:00");

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
