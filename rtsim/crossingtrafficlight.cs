using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rtsim
{
    class crossingtrafficlight : crossingclean
    {
        //Properties
	    List<int> trafficlightlist;

        //Methods
        public crossingtrafficlight(point alocation, simulation asim) : 
	        base(alocation, asim)
        {
	        trafficlightlist = new List<int>();

	        for (int i = 0; i < nroads; i++)
	        {
		        /*trafficlight(double aredlength, double ayellowlength, double agreenlength, 
                            trafficlightcolour acolour, road^ aroadon,
                            double ax, double ay, int apointpointedto, double adt);*/
		
		        trafficlightlist.Add(sim.trafficlights.Count);
		        sim.trafficlights.Add(new trafficlight(global.TrafficLightRedLength,
                        global.TrafficLightYellowLength,
                        global.TrafficLightGreenLength,
                        i * (global.TrafficLightGreenLength + global.TrafficLightYellowLength + 
                        global.TrafficLightRedLength) / 4.0, 
				        sim.roads[roadlist[i]],
				        sim.roads[roadlist[i]].points[1].x,
				        sim.roads[roadlist[i]].points[1].y,
				        0, 
				        global.SampleT));
	        }

        }

        public void positionelements()
        {
	        base.positionelements();
        }

        public void setlocation(double ax, double ay)
        {
	        base.setlocation(ax, ay);
	        for (int i = 0; i < nroads; i++)
	        {
		        sim.trafficlights[trafficlightlist[i]].calcdirectionlocation(ax, ay);
	        }
        }

        ~crossingtrafficlight()
        {
	        //base class destructor will be called automatically.
	        for (int i = nroads - 1; i >= 0; i--)
	        {
		        //delete sim->trafficlights[trafficlightlist[i]]; //I do not think this step is needed due to the GC.
		        sim.trafficlights.RemoveAt(trafficlightlist[i]);
	        }
        }

    }
}
