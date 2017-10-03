using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rtsim
{
    public class crossingclean
    {
        //Properties
	    public simulation sim;
        public point location;
        public int nroads;
        public List<int> roadlist; //index numbers of the roads that are part of this crossing
        public List<int> pointlist; //index numbers of the points that are part of this crossing.

        //Methods
        public crossingclean(point alocation, simulation asim)
        {
	        location = alocation;
	        sim = asim;
	        nroads = global.CrossingNRoads;
	        roadlist = new List<int>();
	        pointlist = new List<int>();
	        for (int i = 0; i < nroads + 1; i++) //the last point will be the middle of the crossing
	        {
		        pointlist.Add(sim.points.Count);
		        sim.points.Add(new point(location.x, location.y));
	        }

	        for (int i = 0; i < nroads; i++)
	        {
		        /*road(int anr, point^ p0, point^ p1, int anlanes, directionflowenum adirectionflow, 
			        double aroadlanewidth);*/
		
		        roadlist.Add(sim.roads.Count);
		        sim.roads.Add(new road(sim.roads.Count, 
				        sim.points[pointlist[i]], 
				        sim.points[pointlist[nroads]], 
				        1,
                        directionflowenum.Both, global.LaneWidth));
	        }
	        positionelements();
        }

        public void positionelements()
        {
	        int side;
	        side = global.CarSide == carsidet.Right ? 1 : -1;
	        double angle = 0; //radians
	        double deltaangle = -side*2*Math.PI/nroads;
	        double x1, y1, x2, y2;
	        x2 = location.x;
	        y2 = location.y;
	        for (int i = 0; i < nroads; i++)
	        {
		        x1 = location.x + global.CrossingRoadLength*Math.Cos(angle);
                y1 = location.y + global.CrossingRoadLength * Math.Sin(angle);
		
		        sim.roads[roadlist[i]].points[0].x = x1;
		        sim.roads[roadlist[i]].points[0].y = y1;
		        sim.roads[roadlist[i]].points[1].x = x2;
		        sim.roads[roadlist[i]].points[1].y = y2;
		        sim.roads[roadlist[i]].updatedirection();
		        angle += deltaangle;
	        }
        }

        public void setlocation(double ax, double ay)
        {
	        location.x = ax;
	        location.y = ay;
	        positionelements();
        }

        ~crossingclean()
        {
	        for (int i = nroads - 1; i >= 0; i--)
	        {
		        //delete sim->roads[roadlist[i]]; //I do not think this is needed in the code.  Will happen through GC.
		        sim.roads.RemoveAt(roadlist[i]);
	        }
	        for (int i = nroads; i >= 0; i--)
	        {

                //delete sim->points[pointlist[i]]; //I do not think this is needed in the code.  Will happen through GC.
		        sim.points.RemoveAt(pointlist[i]);
	        }
        }





    }
}
