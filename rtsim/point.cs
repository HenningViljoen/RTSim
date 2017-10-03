using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace rtsim
{
    [Serializable]
    public class point : IDisposable
    {
        //Properties
        public double x, y;
        public List<int> roadlist; //index numbers of the roads that connect to this point
	    public bool highlighted; //true if the mouse is hovering over this point in drawing mode
	    public double crossingradius; //The distance before the end of the road that cars should start turning to/
							//from it.

        //Methods
        public point(double ax, double ay)
        {
            x = ax;
            y = ay;
	        highlighted = false;
	        roadlist = new List<int>(); 
	        crossingradius = global.CrossingRadius;
        }

        public void Dispose()
        {
            //Dispose(true);

            // Use SupressFinalize in case a subclass
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        public void copyfrom(point acopyfrom)
        {
	        if (this != acopyfrom)
	        {
		        x = acopyfrom.x;
		        y = acopyfrom.y;

		        if (roadlist != null)
		        {
			        roadlist.Clear();
			        //roadlist = gcnew List<int>();
			        for (int i = 0; i < acopyfrom.roadlist.Count; i++)
			        {
				        roadlist.Add(acopyfrom.roadlist[i]);
			        }
		        }
	        }
        }

        public void setxy(double ax, double ay)
        {
	        x = ax;
	        y = ay;
        }


    }
}
