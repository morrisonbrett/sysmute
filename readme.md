## sysmute

This program was built to silence the volume during a window of sleeping hours.
It came about because I would forget to turn the speakers off or mute at night,
and during the quietness of the night, alerts arriving in throughout the night with audible alerts
would disturb my family's sleep. Windows 10 doesn't currently have a built-in "volume silence" time window,
so I wrote this short program to do it.

On my [desktop machine](https://www.flickr.com/photos/morrisonbrett/19799056499), I added a task in Windows Task Scheduler to run 'At System Startup'.

The program will mute the volume during fixed hours.  Default time window is 10:00pm -> 8:00am.
This can be overridden by passing in start time / end time on the command line.  For example, `> sysmute.exe 23:00 07:30`.

If user manually unmutes, it stays unmuted.
