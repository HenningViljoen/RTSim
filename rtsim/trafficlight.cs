using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace rtsim
{
    public enum trafficlightcolour : int { Green = 0, Yellow = 1, Red = 2 };

    [Serializable]
    public class trafficlight
    {
        //Properties
        public double timer; //'seconds
        public double greenlength, yellowlength, redlength; //'seconds
        public trafficlightcolour colour;
        public road roadon;
        public point location;
        public point graphlocation, graphblacklocation;
        public double direction; // 'radians
        public int pointpointedto, pointnotpointedto;
        public bool highlighted;
        public double dt;

        //Methods
        public trafficlight(double aredlength, double ayellowlength, double agreenlength, 
                    double atime, road aroadon,
                    double ax, double ay, int apointpointedto, double adt)
        {
            redlength = aredlength;
            yellowlength = ayellowlength;
            greenlength = agreenlength;
            colour = trafficlightcolour.Green; //just to have any start-up colour
	        timer = atime;
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
	        highlighted = false;
            //resettimer();

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

	        location.x = ax + global.RoadWidth*global.NrLanes*Math.Cos(direction); //NrLanes will need to change here to 
                                                                                    //that of the road that we are on.
	        location.y = ay + global.RoadWidth*global.NrLanes*Math.Sin(direction);

            graphlocation.setxy(location.x + global.TrafficLightOffSet * Math.Cos(direction - Math.PI / 2),
                                        location.y + global.TrafficLightOffSet * Math.Sin(direction - Math.PI / 2));

            graphblacklocation.setxy(graphlocation.x + global.TrafficLightBlackOffSet * Math.Cos(direction - Math.PI), 
                                             graphlocation.y + global.TrafficLightBlackOffSet * 
                                             Math.Sin(direction - Math.PI));
        }

        public void resettimer()
        {
            timer = 0;
        }

        public void update()
        {
            timer += dt;
            if (timer <= greenlength)
	        {
                colour = trafficlightcolour.Green;
	        }
	        else if (timer > greenlength && timer <= greenlength + yellowlength)
	        {
                colour = trafficlightcolour.Yellow;
	        }
	        else if (timer > greenlength + yellowlength && timer <= greenlength + yellowlength + redlength)
	        {
                colour = trafficlightcolour.Red;
	        }
	        else if (timer > greenlength + yellowlength + redlength)
	        {
                colour = trafficlightcolour.Green;
                resettimer();
	        }
        }

        public void draw(Graphics G)
        {
            GraphicsPath plot1, plot2;
            Pen plotpen1, plotpen2;
            Color tempcolour = Color.Green;
            plot1 = new GraphicsPath();
            plotpen1 = new Pen(Color.Black, 5);
            plot1.AddEllipse(global.OriginX + Convert.ToInt32(graphblacklocation.x * global.GScale), 
		        global.OriginY + Convert.ToInt32(graphblacklocation.y * global.GScale), 
                            global.TrafficLightRadius, global.TrafficLightRadius);

            G.DrawPath(plotpen1, plot1);

            switch (colour)
	        {
		        case trafficlightcolour.Green:
                    tempcolour = highlighted ? Color.LightGreen : Color.Green;
			        break;
		        case trafficlightcolour.Yellow:
                    tempcolour = highlighted ? Color.LightYellow : Color.Yellow;
			        break;
		        case trafficlightcolour.Red:
                    tempcolour = highlighted ? Color.Orange : Color.Red;
			        break;
	        }

            plot2 = new System.Drawing.Drawing2D.GraphicsPath();
            plotpen2 = new Pen(tempcolour, 5);
            plot2.AddEllipse(global.OriginX + Convert.ToInt32(graphlocation.x * global.GScale),
                global.OriginY + Convert.ToInt32(graphlocation.y * global.GScale),
                            global.TrafficLightRadius, global.TrafficLightRadius);

            G.DrawPath(plotpen2, plot2);
        }




    }
}
