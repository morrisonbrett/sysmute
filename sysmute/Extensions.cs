using System;

namespace sysmute
{
    internal static class DateTimeExt
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
                var hour = int.Parse(timeString.Substring(0, timeString.IndexOf(":", StringComparison.Ordinal)));

                return hour;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Invalid hour {timeString}.  Must use HH:MM");
                throw e;
            }
        }

        public static int Minute(this string timeString)
        {
            try
            {
                var minute = int.Parse(timeString.Substring(timeString.IndexOf(":", StringComparison.Ordinal) + 1));

                return minute;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Invalid minute {timeString}.  Must use HH:MM");
                throw e;
            }
        }
    }
}