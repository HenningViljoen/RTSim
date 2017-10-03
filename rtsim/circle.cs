using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace rtsim
{

    class circle
    {
        //Properties
	    simulation sim;
	    point location;
	    int nroads; //number of roads to be in the circle.
	    double radius;
	    List<int> roadlist; //index numbers of the roads that are part of this circle
	    List<int> pointlist; //index numbers of the points that are part of this circle.

        //Methods
        public circle(point alocation, int anroads, double aradius, simulation asim)
        {
	        location = alocation;
	        nroads = anroads;
	        sim = asim;
	        radius = aradius;
	        roadlist = new List<int>();
	        pointlist = new List<int>();
	        for (int i = 0; i < nroads; i++)
	        {
		        pointlist.Add(sim.points.Count);
		        sim.points.Add(new point(location.x, location.y));
	        }

	        for (int i = 0; i < nroads; i++)
	        {
		        /*road(int anr, point^ p0, point^ p1, int anlanes, directionflowenum adirectionflow, 
			        double aroadlanewidth);*/
		
		        roadlist.Add(sim.roads.Count);
		        if (i == 0)
		        {
			        sim.roads.Add(new road(sim.roads.Count, 
					        sim.points[pointlist[nroads - 1]], 
					        sim.points[pointlist[i]], 
					        1,
                            directionflowenum.Direction01, global.LaneWidth));
		        }
		        else
		        {
			        sim.roads.Add(new road(sim.roads.Count, 
					        sim.points[pointlist[i - 1]], 
					        sim.points[pointlist[i]], 
					        1,
                            directionflowenum.Direction01, global.LaneWidth));
		        }
	        }
	        positionroads();
        /*
	        sim->roads->Add(gcnew road(sim->roads->Count, 
					        dynamicpoint, 
					        sim->points[sim->points->Count - 1], 
					        Convert::ToInt32(numericUpDown1->Value), 
					        Both, LaneWidth));*/
        }

        public void positionroads()
        {
	        int side;
	        side = global.CarSide == carsidet.Right ? 1 : -1;
	        double angle = 0; //radians
	        double deltaangle = -side*2*Math.PI/nroads;
	        double x1, y1, x2, y2;
	        for (int i = 0; i < nroads; i++)
	        {
		        x1 = location.x + radius*Math.Cos(angle);
		        y1 = location.y + radius*Math.Sin(angle);
		        x2 = location.x + radius*Math.Cos(angle + deltaangle);
		        y2 = location.y + radius*Math.Sin(angle + deltaangle);
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
	        positionroads();
        }

        ~circle()
        {
	        for (int i = nroads - 1; i >= 0; i--)
	        {
		        //sim.roads[roadlist[i]].Dispose(); //I do not think this line is needed.
		        sim.roads.RemoveAt(roadlist[i]);
	        }
	        for (int i = nroads - 1; i >= 0; i--)
	        {

                //.points[pointlist[i]].Dispose(); //I do not think this line is needed.
		        sim.points.RemoveAt(pointlist[i]);
	        }
        }




    }
}
