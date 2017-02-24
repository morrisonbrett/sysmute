## Sysmute

This program was built to silence the volume during a wide window of sleeping hours.
It came about because I would forget to turn the speakers off or mute at night,
and during the quietness of the night, alerts arriving in throughout the night with audible alerts
would disturb my family's sleep. Windows 10 doesn't currently have a built-in "volume silence" time window,
so I wrote this short program to do it.

On my desktop machine, I added a task in Windows Task Scheduler to run 'At System Startup'.

The program will mute the volume during fixed hours.  The time window is a constant in the C# code.

If user manually unmutes, it stays unmuted.
