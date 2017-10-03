using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace rtsim
{
    [Serializable]
    public class building
    {
        //Properties
        public road roadon;
        public point location;
        public point middleofbuilding;
        public point closestpointinroad;
        public double direction; // 'radians
        //int pointpointedto, pointnotpointedto; //Let's define the point at the end of the road on teh side
				    //							//the building is on, as the point pointed to.

        //Methods
        public building(road aroadon, point alocation, 
					double adirection)
        {
            roadon = aroadon;

           /* pointpointedto = apointpointedto;
            pointpointedto == 0 ? pointnotpointedto = 1 : pointnotpointedto = 0;*/

	        direction = adirection;

	        location = alocation;
	        closestpointinroad = new point(0,0);
	        middleofbuilding = new point(0,0);
	        calcclosestpointinroad();
        }

        public void calcclosestpointinroad()
        {
	        closestpointinroad.x = location.x + Math.Cos(direction)*roadon.roadlanewidth*
		        (1 + roadon.nlanes);
	        closestpointinroad.y = location.y + Math.Sin(direction)*roadon.roadlanewidth*
		        (1 + roadon.nlanes);
	        middleofbuilding.x = location.x + Math.Cos(direction + Math.PI)*global.BuildingSideLength/2;
	        middleofbuilding.y = location.y + Math.Sin(direction + Math.PI)*global.BuildingSideLength/2;
        }

        public void update(Graphics G)
        {
        }

        public void draw(Graphics G)
        {
	        point p1 = new point(0,0);
	        point p2 = new point(0,0);
	        point p3 = new point(0,0);
	        point p4 = new point(0,0);

	        p1.x = location.x + global.BuildingSideLength/2*Math.Cos(direction + Math.PI/2);
	        p1.y = location.y + global.BuildingSideLength/2*Math.Sin(direction + Math.PI/2);
	        p2.x = p1.x + global.BuildingSideLength*Math.Cos(direction + 2*Math.PI/2);
	        p2.y = p1.y + global.BuildingSideLength*Math.Sin(direction + 2*Math.PI/2);
	        p3.x = p2.x + global.BuildingSideLength*Math.Cos(direction + 3*Math.PI/2);
	        p3.y = p2.y + global.BuildingSideLength*Math.Sin(direction + 3*Math.PI/2);
	        p4.x = p3.x + global.BuildingSideLength*Math.Cos(direction + 4*Math.PI/2);
	        p4.y = p3.y + global.BuildingSideLength*Math.Sin(direction + 4*Math.PI/2);

	        GraphicsPath plot1;
            Pen plotPen;
	        plot1 = new GraphicsPath();
            plotPen = new Pen(Color.White, 1);

	        Point[] myArray = new Point[] 
	           {new Point(global.OriginX + Convert.ToInt32(global.GScale*p1.x),
                   global.OriginY + Convert.ToInt32(global.GScale*p1.y)), 
		        new Point(global.OriginX + Convert.ToInt32(global.GScale*p2.x),
                    global.OriginY + Convert.ToInt32(global.GScale*p2.y)), 
		        new Point(global.OriginX + Convert.ToInt32(global.GScale*p3.x),
                    global.OriginY + Convert.ToInt32(global.GScale*p3.y)), 
		        new Point(global.OriginX + Convert.ToInt32(global.GScale*p4.x),
                    global.OriginY + Convert.ToInt32(global.GScale*p4.y))};

            plot1.AddPolygon(myArray);

	        G.DrawPath(plotPen, plot1);
        }


    }
}
