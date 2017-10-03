using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace rtsim
{
    public enum decisiontype : int { RandomDecision, BuildingToBuilding }; //How decisions of final destination is made
    public enum carmode : int { Normal, Breaking };

    [Serializable]
    public class car
    {
        //Properties
	    public double speed, preferredspeed; //'km per second
        public double maxspeed;
        public double speedx, speedy;
        public double dt;
        public double deltalocation;
        public double direction; //'radians
        public double wlangle;  //radians - Width/Length angle for drawing the car
        public double cardiag; //'pixels - half the diagonal of the car
	    public double cardiagkm; //cardiag in kilometers;
        public point location;
        public double breakdistance;
        public double breakacceleration;
	    public int lane; //The lane number that the car is on in the road on the side it is on.  
				    //Lane 0 is the lane next to the middle man.
	    //int destinationlane; //Similar to lane, only that this time it is not the current lane.
	    public bool changinglanes;
        public road roadon;
        public point destination;
	    public point ultimatedest;
	    public point penultimatedest;
	    public List<int> route;
	    public int currentstepinroute; //which step in the List route the car is currently at as it goes 
							    //to its destination
        public int destination01; //'0 or 1 depending on which end point 
        //'of the road the car is heading to
        public bool atdestination;
        public carmode mode;
        public bool ontrajectory;
        public carsidet side;
        public pidcontroller distancecontroller, speedcontroller;
	    public bool highlighted;
	    public decisiontype decisiont; //Algorithm that will govern the destination setting of the car
	    public int buildingfrom, buildingto; //Start and destination if building-to-building algorithm is chosen
	    public bool cartobedeleted; //if the car is to be deleted from the simulation, this flag will be set true.
	    public int newroadn; //the road number of the new road that will be turned to at the end of the current
				    // road being driven on.
	    public double timetotargetspeed; //The time it takes the car to reach its intented set point speed from a
								    //complete stop.  Depends on the speed controller.
        public Color carcolour;
        public cartypes cartype;

        //Methods
        public car(double aspeed, double adt, point alocation, 
                    point adestination, road aroadon, int adestination01, 
                    double abreakacceleration)
        {
            speed = aspeed;
            preferredspeed = speed;
            dt = adt;
            calcdeltalocation();
	
            location = alocation;

            destination = new point(0, 0);
            newdestination(adestination, aroadon, adestination01);
            atdestination = false;
 
            breakacceleration = abreakacceleration;
            calcbreakdistance();
            mode = carmode.Normal;
            ontrajectory = false;
            side = global.DefaultCarSide;
            distancecontroller = new pidcontroller(global.KDistanceController, global.IDistanceController, 0, 
		        global.MinCarSpeed, global.MaxCarSpeed);
            speedcontroller = new pidcontroller(global.KSpeedController, global.ISpeedController, 0, 
		        global.MinCarSpeed, global.MaxCarSpeed);
            speedcontroller.direction = global.Reverse;
            speedcontroller.init(preferredspeed, speed, speed);
            maxspeed = global.MaxCarSpeed;
            wlangle = Math.Atan(global.CarWidth / global.CarLength);
            cardiag = Math.Sqrt(Math.Pow(global.CarWidth,2) + Math.Pow(global.CarLength,2)) / 2 * global.GScale;
	        cardiagkm = Math.Sqrt(Math.Pow(global.CarWidth,2) + Math.Pow(global.CarLength,2)) / 2;
	        highlighted = false;
	        decisiont = decisiontype.RandomDecision;
	        buildingfrom = 0;
	        buildingto = 1;
	        route = new List<int>(0);
	        currentstepinroute = 0;
	        lane = 0;
	        changinglanes = false;
	        cartobedeleted = false;
	        newroadn = 0;

	        timetotargetspeed = 50*global.PSpeedController/global.SampleT;
            carcolour = global.CarColour;
            cartype = cartypes.OtherCar;
        }

        public void calcbreakdistance()
        {
           breakdistance = -Math.Pow(speed, 2) / (2 * breakacceleration);
        }

        public void calcdeltalocation()
        {
            deltalocation = speed * dt;
        }

        public void updatedirection()
        {
	        double distancex  = destination.x - location.x;
            double distancey = destination.y - location.y;

            direction = utilities.calcdirection(distancey, distancex);
        }

        public void newdestination(point adestination, road aroadon, int adestination01)
        {
            destination.copyfrom(adestination);
	        //destination = adestination;  //This does not work for some reason.
            atdestination = false;
            roadon = aroadon;
            destination01 = adestination01;
            updatedirection();
	        //destinationlane = adestinationlane;
        }

        public void newdestinationlane(point adestination, road aroadon, int adestination01, int adestinationlane)
        {
        }

        public void readlane()
        {
	        double dx = location.x - roadon.points[0].x;
	        double dy = location.y - roadon.points[0].y;
	        double mousedirectionfrompoint0 = utilities.calcdirection(dy,dx);
	        //mousedirectionfrompoint0 -= sim->roads[i]->direction;
	        double deltadirection = mousedirectionfrompoint0 - roadon.direction;
	        if (Math.Cos(deltadirection) > 0)
	        {
		        double distancefrompoint0tomouse = utilities.distance(dy,dx);
		        double distancefromroad = Math.Sin(deltadirection)*distancefrompoint0tomouse;
		        lane = (int)Math.Ceiling(Math.Abs(distancefromroad)/roadon.roadlanewidth) - 1;
	        }
	        else {lane = 0;}
        }

        public void lanechange(int deltachange) //deltachange is the change in the lane number 
									  //that the car will be in
        {
	        int side;
	        side = global.CarSide == carsidet.Right ? 1 : -1;
	        double distanceduringlanechange = speed*global.TimeToChangeLane;
	        double anglechange = Math.Asin(deltachange*roadon.roadlanewidth/distanceduringlanechange)*side;
	        double distanceparallel = Math.Cos(Math.Abs(anglechange))*distanceduringlanechange;
	        if (utilities.distance(destination,location) >= distanceparallel)
	        {
		        direction += anglechange;
		        destination.x = location.x + distanceduringlanechange*Math.Cos(direction + anglechange);
		        destination.y = location.y + distanceduringlanechange*Math.Sin(direction + anglechange);
		        updatedirection();
		        changinglanes = true;
	        }
        }

        public void stoplanechange()
        {
	        changinglanes = false;
	        setdestendofroad();
        }

        public void setdestendofroad() //calcmobilepointendofroad should be replaced by this app at some point
        {
	        double bias; // 'for direction
	        int side;

	        if (destination01 == 1)
	        {
		        bias = 0;
	        }
	        else
	        {
		        bias = Math.PI;
	        }

	        side = global.CarSide == carsidet.Right ? 1 : -1;
	        destination.x = roadon.points[destination01].x + 
		        roadon.points[destination01].crossingradius*
		        Math.Cos(roadon.direction + bias + Math.PI) + 
		        (lane + 1)*roadon.roadlanewidth * 
		        Math.Cos(roadon.direction + bias + side*Math.PI/2);
	        destination.y = roadon.points[destination01].y + 
		        roadon.points[destination01].crossingradius*
		        Math.Sin(roadon.direction + bias + Math.PI) + 
		        (lane + 1)*roadon.roadlanewidth * 
		        Math.Sin(roadon.direction + bias + side*Math.PI/2);
	        updatedirection();
	        atdestination = false;
        }

        public void setproperties(int acarnr, car acar, simulation asim)
        {
            carproperties carprop = new carproperties(acarnr, acar, asim);
            carprop.Show();
        }

        public void draw(Graphics G, bool willerase) //should by default receive a false input
        {
            GraphicsPath plot1;
            Pen plotPen;
            float width;

            if (willerase)
	        {
                width = 2;
	        }
            else
	        {
                width = 1;
	        }
            plot1 = new GraphicsPath();
            plotPen = new Pen(Color.BlueViolet, width);

            Point[] myArray = new Point[] 
            {new Point(global.OriginX + Convert.ToInt32(location.x * global.GScale + cardiag * Math.Cos(direction - wlangle)), 
                    global.OriginY + Convert.ToInt32(location.y * global.GScale + cardiag * Math.Sin(direction - wlangle))), 
            new Point(global.OriginX + Convert.ToInt32(location.x * global.GScale + cardiag * Math.Cos(direction + wlangle)), 
                    global.OriginY + Convert.ToInt32(location.y * global.GScale + cardiag * Math.Sin(direction + wlangle))), 
            new Point(global.OriginX + Convert.ToInt32(location.x * global.GScale + cardiag * Math.Cos(direction + Math.PI - wlangle)), 
                    global.OriginY + Convert.ToInt32(location.y * global.GScale + cardiag * Math.Sin(direction + Math.PI - wlangle))), 
            new Point(global.OriginX + Convert.ToInt32(location.x * global.GScale + cardiag * Math.Cos(direction + Math.PI + wlangle)),
                    global.OriginY + Convert.ToInt32(location.y * global.GScale + cardiag * Math.Sin(direction + Math.PI + wlangle)))};


            plot1.AddPolygon(myArray);


            if (willerase)
	        {
                plotPen.Color = Color.Gray;
	        }
            else
	        {
                plotPen.Color = carcolour;
	        }

            SolidBrush brush = new SolidBrush(carcolour);
	        if (highlighted) {brush.Color = Color.Orange;}
	        G.FillPath(brush, plot1);
            G.DrawPath(plotPen, plot1);
        }

        public void controlspeed()
        {
            if (mode == carmode.Normal) 
	        {
                //speed = speedcontroller.update(preferredspeed, speed)
                speed += global.PSpeedController * (preferredspeed - speed);
	        }
            if (speed > global.MaxCarSpeed) 
	        {
                speed = global.MaxCarSpeed;
	        }
        }

        public void move(Graphics G)
        {
            //draw(G, true);
            calcbreakdistance();
            controlspeed();
            //calcdeltalocation()

            if (utilities.distance(location, destination) <= deltalocation) 
	        {
                atdestination = true;
                location.x = destination.x;
                location.y = destination.y;
	        }
            else
	        {
                location.x += speed * Math.Cos(direction) * dt;
                location.y += speed * Math.Sin(direction) * dt;
	        }

            //draw(G, false);
        }


    }
}
