using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace rtsim
{
    [Serializable]
    public class carextractor
    {
        //Properties
        public point location;
	    public int pointn;
	    public double direction; //radians.
	    public bool highlighted;
	    public road roadon;
	    public double dt; //TSim for the simulation

        //Methods
        public carextractor(road aroadon, int apointn, double adt)
        {
	        roadon = aroadon;
	        location = roadon.points[apointn];
	        pointn = apointn;

	        double bias; // 'for direction
	        if (apointn == 1)
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
	        pointarray[1] = new Point(pointarray[0].X + Convert.ToInt32(global.GScale*global.CarExtractorLength * 
                Math.Cos(direction)), 
                    pointarray[0].Y + Convert.ToInt32(global.GScale*global.CarExtractorLength * Math.Sin(direction)));
	        pointarray[2] = new Point(pointarray[1].X + Convert.ToInt32(global.GScale*global.CarExtractorWidth *
                Math.Cos(direction + Math.PI*0.5)), 
                    pointarray[1].Y + Convert.ToInt32(global.GScale*global.CarExtractorWidth * 
                    Math.Sin(direction + Math.PI*0.5)));
            pointarray[3] = new Point(pointarray[2].X + Convert.ToInt32(global.GScale*global.CarExtractorLength * 
                Math.Cos(direction + Math.PI)), 
                    pointarray[2].Y + Convert.ToInt32(global.GScale*global.CarInjectorLength * Math.Sin(direction + Math.PI)));


            plot1.AddPolygon(pointarray);

	        SolidBrush brush = new SolidBrush(Color.Orange);
	        if (highlighted) {brush.Color = Color.Yellow;}
	        G.FillPath(brush, plot1);
            G.DrawPath(plotPen, plot1);
        }

        public void update()
        {
    
        }




    }
}
