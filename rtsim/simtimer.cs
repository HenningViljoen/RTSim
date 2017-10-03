using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rtsim
{
    [Serializable]
    public class simtimer
    {
        //Properties
        public double days;
        public double hours;
        public double minutes;
        public double seconds;
        public double totalsecondsfortheday;

        //Methods
        public simtimer(double adays, double ahours, double aminutes, double aseconds)
        {
	        days = adays;
	        hours = ahours;
	        minutes = aminutes;
	        seconds = aseconds;
	        totalsecondsfortheday = 0;
        }

        public void tick()
        {
	        //Assume SampleT is in seconds
	        seconds += global.SampleT;
            totalsecondsfortheday += global.SampleT;
	        if (seconds >= 60)
	        {
		        seconds = 0;
		        minutes += 1;
	        }
	        if (minutes >= 60)
	        {
		        minutes = 0;
		        hours += 1;
	        }
	        if (hours >= 24)
	        {
		        hours = 0;
		        totalsecondsfortheday = 0;
		        days += 1;
	        }
        }

        public String timerstring()
        {

	        return String.Concat("Days: ", days.ToString(), "  Time: ", 
		        String.Concat(hours.ToString(),":",minutes.ToString(),
		        String.Concat(":",seconds.ToString())));
        }

        public static bool operator ==(simtimer simtimer1, simtimer simtimer2)
        {
            return (simtimer1.hours == simtimer2.hours &&
                simtimer1.minutes == simtimer2.minutes && simtimer1.seconds == simtimer2.seconds);
        }

        public static bool operator !=(simtimer simtimer1, simtimer simtimer2)
        {
            return !(simtimer1.hours == simtimer2.hours &&
                simtimer1.minutes == simtimer2.minutes && simtimer1.seconds == simtimer2.seconds);
        }

        public static bool operator <(simtimer simtimer1, simtimer simtimer2)
        {
            return (3600 * simtimer1.hours + 60 * simtimer1.minutes + simtimer1.seconds <
                3600 * simtimer2.hours + 60 * simtimer2.minutes + simtimer2.seconds);
        }

        public static bool operator >(simtimer simtimer1, simtimer simtimer2)
        {
            return (3600 * simtimer1.hours + 60 * simtimer1.minutes + simtimer1.seconds >
                3600 * simtimer2.hours + 60 * simtimer2.minutes + simtimer2.seconds);
        }

        public static bool operator >=(simtimer simtimer1, simtimer simtimer2)
        {
            return (3600 * simtimer1.hours + 60 * simtimer1.minutes + simtimer1.seconds >=
                3600 * simtimer2.hours + 60 * simtimer2.minutes + simtimer2.seconds);
        }

        public static bool operator <=(simtimer simtimer1, simtimer simtimer2)
        {
            return (3600 * simtimer1.hours + 60 * simtimer1.minutes + simtimer1.seconds <=
                3600 * simtimer2.hours + 60 * simtimer2.minutes + simtimer2.seconds);
        }
    }
}
