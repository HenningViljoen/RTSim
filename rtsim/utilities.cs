using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rtsim
{
    public static class utilities
    {
        public static double calcdirection(double deltay, double deltax)
        {
	        double bias;
            if (deltax < 0) 
	        {
                bias = Math.PI;
	        }
            else
	        {
                bias = 0;
	        }
	        if (deltax == 0) {deltax = 0.0001;}
            return bias + Math.Atan(deltay/deltax);
        }

        public static double distance(point p1, point p2)
        {
            return Math.Sqrt(Math.Pow(p1.x - p2.x,2) + Math.Pow(p1.y - p2.y,2));
        }

        public static double distance(double deltay, double deltax)
        {
            return Math.Sqrt(Math.Pow(deltax,2) + Math.Pow(deltay,2));
        }

        public static double normsinv(double p)
        {
            const double a1 = -39.6968302866538, a2 = 220.946098424521, a3 = -275.928510446969;
            const double a4 = 138.357751867269, a5 = -30.6647980661472, a6 = 2.50662827745924;
            const double b1 = -54.4760987982241, b2 = 161.585836858041, b3 = -155.698979859887;
            const double b4 = 66.8013118877197, b5 = -13.2806815528857, c1 = -0.00778489400243029;
            const double c2 = -0.322396458041136, c3 = -2.40075827716184, c4 = -2.54973253934373;
            const double c5 = 4.37466414146497, c6 = 2.93816398269878, d1 = 0.00778469570904146;
            const double d2 = 0.32246712907004, d3 = 2.445134137143, d4 = 3.75440866190742;
            const double p_low = 0.02425, p_high = 1 - p_low;
            double q, r;
            if (p < 0 || p > 1) 
		    {
                return 0; //Error case
                //MessageBox.Show(
                //        "Error", 
                //        "OK?", MessageBoxButtons.YesNo, 
                //        MessageBoxIcon.Question);
		    }
		    else if (p < p_low) 
		    {
                q = Math.Sqrt(-2 * Math.Log(p));
                return (((((c1 * q + c2) * q + c3) * q + c4) * q + c5) * q + c6) / 
                    ((((d1 * q + d2) * q + d3) * q + d4) * q + 1);
		    }
		    else if (p <= p_high) 
		    {
                q = p - 0.5;
			    r = q * q;
                return (((((a1 * r + a2) * r + a3) * r + a4) * r + a5) * r + a6) * q / 
                    (((((b1 * r + b2) * r + b3) * r + b4) * r + b5) * r + 1);
		    }
		    else
		    {
                q = Math.Sqrt(-2 * Math.Log(1 - p));
                return -(((((c1 * q + c2) * q + c3) * q + c4) * q + c5) * q + c6) / 
                    ((((d1 * q + d2) * q + d3) * q + d4) * q + 1);
		    }
        }

        //Returns a normall distributed value around 0, with limits of 3 sigma above and below 0
        public static double randnormv(double a3sigma, Random r)
        {
            return (normsinv(r.NextDouble()) * 2 - 1)*a3sigma;
        }

    }
}
