using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;


namespace rtsim
{
    public enum directionflowenum : int { Both = 0, Direction01 = 1, Direction10 = 2 };

    [Serializable]
    public class road : IDisposable
    {
        //Properties
        public int nr; // the number assigned to this road
        public point[] points;
        public int nlanes;
        public double direction; //'radians
        public double distance;
        public directionflowenum directionflow; //directionflowenum
        public double roadlanewidth;
        public bool highlighted;

        //Methods
        public road(int anr, point p0, point p1, int anlanes, directionflowenum adirectionflow, 
	        double aroadlanewidth)
        {
	        points = new point[2];
	        nr = anr;
            points[0] = p0;
            points[1] = p1;
            updatedirection();
	        nlanes = anlanes;
            directionflow = adirectionflow;
            roadlanewidth = aroadlanewidth;
	        highlighted = false;
        }

        public void Dispose()
        {
            //Dispose(true);

            // Use SupressFinalize in case a subclass
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        public void updatedirection()
        {
	        direction = utilities.calcdirection(points[1].y - points[0].y, points[1].x - points[0].x);
	        distance = utilities.distance(points[0], points[1]);
        }

        public void draw(Graphics G)
        {
	        Color colour;
	        colour = highlighted ? Color.Gray : Color.Black;
	        Pen whitepen = new Pen(Color.White, 1);
	        whitepen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

	        GraphicsPath[][] plot = new GraphicsPath[nlanes + 1][];
            GraphicsPath plot2 = new GraphicsPath();
	        Pen plotPen = new Pen(colour, (float)(roadlanewidth * (2*nlanes + 2) * global.GScale));
            //plotPen.Color = Color.DarkMagenta

	        double[][] x0 = new double[nlanes + 1][];
	        double[][] y0 = new double[nlanes + 1][];
	        double[][] x1 = new double[nlanes + 1][];
	        double[][] y1 = new double[nlanes + 1][];
	
	        for (int i = 0; i < nlanes + 1; i++)
	        {
		        x0[i] = new double[2];
		        y0[i] = new double[2];
		        x1[i] = new double[2];
		        y1[i] = new double[2];

		        plot[i] = new GraphicsPath[2];

		        for (int j = 0; j < 2; j++)
		        {
			        plot[i][j] = new GraphicsPath();
			        //int k = (j == 0 ? 1 : -1);
			        x0[i][j] = points[0].x + i*roadlanewidth*Math.Cos(direction + Math.PI/2 + j*Math.PI);
			        y0[i][j] = points[0].y + i*roadlanewidth*Math.Sin(direction + Math.PI/2 + j*Math.PI);
			        x1[i][j] = points[1].x + i*roadlanewidth*Math.Cos(direction + Math.PI/2 + j*Math.PI);
			        y1[i][j] = points[1].y + i*roadlanewidth*Math.Sin(direction + Math.PI/2 + j*Math.PI);

			        if (i == 0)
			        {
				        plot[i][j].AddLine(global.OriginX + Convert.ToInt32(x0[0][0]*global.GScale), 
					        global.OriginY + Convert.ToInt32(y0[0][0]*global.GScale), 
					        global.OriginX + Convert.ToInt32(x1[0][0]*global.GScale), 
					        global.OriginY + Convert.ToInt32(y1[0][0]*global.GScale));

				        G.DrawPath(plotPen, plot[i][j]);
				
			        }
			        else
			        {
                        plot[i][j].AddLine(global.OriginX + Convert.ToInt32(x0[i][j] * global.GScale),
                            global.OriginY + Convert.ToInt32(y0[i][j] * global.GScale),
                            global.OriginX + Convert.ToInt32(x1[i][j] * global.GScale),
                            global.OriginY + Convert.ToInt32(y1[i][j] * global.GScale));
				        G.DrawPath(whitepen, plot[i][j]);
			        }

		        }

	        }	
        }

        public void drawhighlightedends(Graphics G)
        {
	        GraphicsPath plot2;
            Pen plotpen2;
            Color tempcolour;
            plot2 = new GraphicsPath();
            plotpen2 = new Pen(Color.Gold, 5);
	        for (int i = 0; i < 2; i++)
	        {	
		        if (points[i].highlighted)
                {
                    plot2.AddEllipse(global.OriginX + Convert.ToInt32(points[i].x * global.GScale),
                        global.OriginY + Convert.ToInt32(points[i].y * global.GScale),
                        global.TrafficLightRadius, global.TrafficLightRadius);
                }
	        }
	        G.DrawPath(plotpen2, plot2);
        }




    }
}
