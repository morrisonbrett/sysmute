using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace sysmute
{
    internal class Program
    {
        public static int DefaultYear = 1969;
        public static int DefaultMonth = 2;
        public static int DefaultStartDay = 25;
        public static int DefaultEndDay = 26;
        public static int DefaultStartHour = 22;
        public static int DefaultEndHour = 9;
        public static int ExampleStartHour = 23;
        public static int ExampleEndHour = 8;
        public static int ExampleMouseIdleMinutes = 5;
        public static DateTime startTime = new DateTime(DefaultYear, DefaultMonth, DefaultStartDay, DefaultStartHour, 00, 00); // Date doesn't matter. 10pm
        public static DateTime endTime = new DateTime(DefaultYear, DefaultMonth, DefaultEndDay, DefaultEndHour, 00, 00); // Date doesn't matter. 9am
        public static int mouseIdleTime = 5; // Time mouse doesn't move to be considered idle in minutes
        public static readonly int SleepInterval = 1000 * 60; // Check the time every 1 minute
        public static readonly string ProjectUrl = "https://github.com/morrisonbrett/sysmute";

        private static void MuteVolume()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Muting Master Volume");
            Console.ForegroundColor = ConsoleColor.White;
            AudioManager.SetMasterVolumeMute(true);
        }

        private static void UnmuteVolume()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Unmuting Master Volume");
            Console.ForegroundColor = ConsoleColor.White;
            AudioManager.SetMasterVolumeMute(false);
        }

        public static void AddSystemIcon()
        {
            // Try to add notifications
            Thread notifyThread = new Thread(
                delegate ()
                {
                    NotifyIcon trayIcon = new NotifyIcon()
                    {
                        Text = "Sysmute",
                        Icon = new Icon(Resource.sysmute, 40, 40)
                    };
                    ContextMenu trayMenu = new ContextMenu();

                    trayMenu.MenuItems.Add("&Mute", (sender, eventArgs) =>
                    {
                        MuteVolume();
                    });

                    trayMenu.MenuItems.Add("&Unmute", (sender, eventArgs) =>
                    {
                        UnmuteVolume();
                    });

                    trayMenu.MenuItems.Add("-");

                    trayMenu.MenuItems.Add("&About", (sender, eventArgs) =>
                    {
                        Process.Start(ProjectUrl);
                    });

                    trayMenu.MenuItems.Add("-");

                    trayMenu.MenuItems.Add("E&xit", (sender, eventArgs) =>
                    {
                        Environment.Exit(0);
                    });

                    trayIcon.ContextMenu = trayMenu;
                    trayIcon.Visible = true;
                    Application.Run();
                });

            notifyThread.Start();
        }

        private static void Main(string[] args)
        {
            // Check the command args.  If set, override the default
            if (args.Length > 0 && args[0] != null)
            {
                try
                {
                    startTime = new DateTime(DefaultYear, DefaultMonth, DefaultStartDay, args[0].Hour(), args[0].Minute(), 00);
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
                    endTime = new DateTime(DefaultYear, DefaultMonth, DefaultEndDay, args[1].Hour(), args[1].Minute(), 00);
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid end time");
                    return;
                }
            }

            if (args.Length > 2 && args[2] != null)
            {
                try
                {
                    mouseIdleTime = int.Parse(args[2]);
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid mouse idle time");
                    return;
                }
            }

            // Add System Icon
            AddSystemIcon();

            Console.WriteLine($"sysmute. Program will mute system audio between {startTime.TimeOfDay} and {endTime.TimeOfDay} and check for mouse input every {mouseIdleTime} minutes");
            if (args.Length == 0)
                Console.WriteLine($"To override startTime, endTime and mouseIdleTime minutes, pass in via command line. E.g. > sysmute {ExampleStartHour}:00 {ExampleEndHour}:00 {ExampleMouseIdleMinutes}");

            var LastX = (uint)0;
            var LastY = (uint)0;
            var MouseIdleTimer = new Stopwatch();

            while (true)
            {
                var timeNow = DateTime.Now;
                Console.WriteLine($"Current Time: {timeNow.TimeOfDay}");

                // If the current time falls within the range of the quiet time, mute
                // If user unmutes during quiet time, it will re-mute after a period of idle time
                if (timeNow.TimeOfDayIsBetween(startTime, endTime))
                {
                    // Audio isn't muted, check mouse for idle to ensure we want to mute
                    if (!AudioManager.GetMasterVolumeMute())
                    {
                        var CurrentX = (uint)Cursor.Position.X;
                        var CurrentY = (uint)Cursor.Position.Y;

                        if (!MouseIdleTimer.IsRunning)
                        {
                            Console.WriteLine($"Starting timer to check for mouse activity every {mouseIdleTime} minutes");
                            MouseIdleTimer.Start();
                            LastX = CurrentX;
                            LastY = CurrentY;
                        }
                        else
                        {
                            if (CurrentX == LastX && CurrentY == LastY)
                            {
                                if (MouseIdleTimer.Elapsed.Minutes >= mouseIdleTime)
                                {
                                    Console.WriteLine($"User is idle.  Mouse hasn't moved in > {mouseIdleTime} minutes");
                                    MuteVolume();
                                    MouseIdleTimer.Stop();
                                }
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Detected Movement -> Restarting idle mouse timer and coordinates.");
                                Console.ForegroundColor = ConsoleColor.White;
                                MouseIdleTimer.Restart();
                                LastX = CurrentX;
                                LastY = CurrentY;
                            }
                        }
                    }
                    else
                    {
                        // Edge condition. Sound is muted, just check and ensure the mouse idle timer resets properly
                        if (MouseIdleTimer.IsRunning)
                        {
                            Console.WriteLine("Stopping idle mouse timer.");
                            MouseIdleTimer.Stop();
                        }
                    }
                }

                Thread.Sleep(SleepInterval);
            }
        }
    }
}