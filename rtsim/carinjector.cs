using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace rtsim
{
    [Serializable]
    public class carinjector
    {
        //Properties
        public point location;
        public point carstartlocation;
        public int pointn;
        public int cardestination01;
        public int carlane;
        public double direction; //radians.
        public bool highlighted;
        public road roadon;
        public double dt; //TSim for the simulation
        public double timer; //'seconds
        public double injectperiod; //seconds; average time between car injections.
        public bool injectnow;

        //Methods
        public carinjector(road aroadon, int apointn, double aninjectperiod, double adt)
        {
	        roadon = aroadon;
	        carlane = roadon.nlanes - 1;
	        location = roadon.points[apointn];
	        pointn = apointn;
	        carstartlocation = new point(0,0);

	        double bias; // 'for direction

	        if (apointn == 0)
	        {
		        bias = 0;
	        }
	        else
	        {
		        bias = Math.PI;
	        }

	        direction = roadon.direction + bias;
	        highlighted = false;
	        dt = adt;
	        timer = 0.0;
	        injectperiod = aninjectperiod;
	        injectnow = false;
	        calccarstartlocation();
        }

        public void calccarstartlocation() 
        {
	        double bias; // 'for direction
	        int side;

	        if (pointn == 0)
	        {
		        bias = 0;
		        cardestination01 = 1;
	        }
	        else
	        {
		        bias = Math.PI;
		        cardestination01 = 0;
	        }

	        side = global.CarSide == carsidet.Right ? 1 : -1;
	        carstartlocation.x = roadon.points[pointn].x + 
		        global.CrossingRadius * Math.Cos(roadon.direction + bias + Math.PI) + 
		        (carlane + 1)*roadon.roadlanewidth * 
		        Math.Cos(roadon.direction + bias + side*Math.PI/2);
	        carstartlocation.y = roadon.points[pointn].y + 
		        global.CrossingRadius * Math.Sin(roadon.direction + bias + Math.PI) + 
		        (carlane + 1)*roadon.roadlanewidth * 
		        Math.Sin(roadon.direction + bias + side*Math.PI/2);
        }

        public void draw(Graphics G)
        {
	        GraphicsPath plot1;
            Pen plotPen;
            float width = 1;

            plot1 = new GraphicsPath();
            plotPen = new Pen(Color.Yellow, width);

            Point[] pointarray = new Point[4];
	        pointarray[0] = new Point(global.OriginX + Convert.ToInt32(global.GScale*location.x), 
                    global.OriginY + Convert.ToInt32(global.GScale*location.y));
	        pointarray[1] = new Point(pointarray[0].X + Convert.ToInt32(global.GScale*
                global.CarInjectorLength * Math.Cos(direction - Math.PI)), 
                    pointarray[0].Y + Convert.ToInt32(global.GScale*global.CarInjectorLength * 
                    Math.Sin(direction - Math.PI)));
	        pointarray[2] = new Point(pointarray[1].X + Convert.ToInt32(global.GScale*
                global.CarInjectorWidth * Math.Cos(direction - Math.PI*1.5)), 
                    pointarray[1].Y + Convert.ToInt32(global.GScale*global.CarInjectorWidth * 
                    Math.Sin(direction - Math.PI*1.5)));
            pointarray[3] = new Point(pointarray[2].X + Convert.ToInt32(global.GScale*
                global.CarInjectorLength * Math.Cos(direction)), 
                    pointarray[2].Y + Convert.ToInt32(global.GScale*global.CarInjectorLength * Math.Sin(direction)));


            plot1.AddPolygon(pointarray);

	        SolidBrush brush = new SolidBrush(Color.Yellow);
	        if (highlighted) {brush.Color = Color.Orange;}
	        G.FillPath(brush, plot1);
            G.DrawPath(plotPen, plot1);
        }

        public void update()
        {
            timer += dt;
	        //double t = Math.Round(timer, 2);
            if (timer >= injectperiod)
	        {
                injectnow = true;
		        timer = 0.0;
	        }
	        else
	        {
		        injectnow = false;
	        }
        }




    }
}
