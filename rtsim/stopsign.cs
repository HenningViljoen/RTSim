using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace rtsim
{
    [Serializable]
    public class stopsign
    {
        //Properties
	    public road roadon;
        public point location;
        public point graphlocation, graphblacklocation;
        public double direction; // 'radians
        public int pointpointedto, pointnotpointedto;
        public double dt;

        //Methods
        public stopsign(road aroadon, double ax, double ay, int apointpointedto, double adt)
        {
            roadon = aroadon;

            pointpointedto = apointpointedto;
            if (pointpointedto == 0) 
	        {
                pointnotpointedto = 1;
	        }
            else
	        {
                pointnotpointedto = 0;
	        }
	        location = new point(0,0);
	        graphlocation = new point(0,0);
            graphblacklocation = new point(0,0);

	        calcdirectionlocation(ax, ay);

            dt = adt;
        }

        public void calcdirectionlocation(double ax, double ay)
        {
	        double deltax = roadon.points[pointpointedto].x - roadon.points[pointnotpointedto].x;
            double deltay = roadon.points[pointpointedto].y - roadon.points[pointnotpointedto].y;
            double bias;
            if (deltax < 0) 
	        {
                bias = Math.PI;
	        }
            else
	        {
                bias = 0;
	        }
            direction = bias + Math.Atan(deltay / deltax);

	        location.x = ax + global.RoadWidth*global.NrLanes*Math.Cos(direction);
	        location.y = ay + global.RoadWidth*global.NrLanes*Math.Sin(direction);

            graphlocation.setxy(location.x + global.TrafficLightOffSet * Math.Cos(direction - Math.PI / 2),
                                        location.y + global.TrafficLightOffSet * Math.Sin(direction - Math.PI / 2));

            graphblacklocation.setxy(graphlocation.x + global.TrafficLightBlackOffSet * Math.Cos(direction - Math.PI), 
                                             graphlocation.y + global.TrafficLightBlackOffSet * Math.Sin(direction - Math.PI));
        }

        public void update()
        {

        }

        public void draw(Graphics G)
        {
            GraphicsPath plot1, plot2;
            Pen plotpen1, plotpen2;
            Color tempcolour;
            plot1 = new GraphicsPath();
            plotpen1 = new Pen(Color.Black, 5);
            plot1.AddEllipse(global.OriginX + Convert.ToInt32(graphblacklocation.x * global.GScale), 
		        global.OriginY + Convert.ToInt32(graphblacklocation.y * global.GScale), 
                            global.TrafficLightRadius, global.TrafficLightRadius);

            G.DrawPath(plotpen1, plot1);

            tempcolour = Color.Purple;
	
	        plot2 = new GraphicsPath();
            plotpen2 = new Pen(tempcolour, 5);
            plot2.AddEllipse(global.OriginX + Convert.ToInt32(graphlocation.x * global.GScale),
                global.OriginY + Convert.ToInt32(graphlocation.y * global.GScale),
                            global.TrafficLightRadius, global.TrafficLightRadius);

            G.DrawPath(plotpen2, plot2);
        }




    }
}
