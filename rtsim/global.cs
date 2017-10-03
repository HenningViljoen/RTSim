using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace rtsim
{
    //carside
    public enum carsidet { Right, Left };
    public enum cartypes { OtherCar, MyCar };
    
                                     

    public static class global
    {
        //Specific dimensions #1
        public static double CarSpeed = 120.0 / 3600.0; // km/second
        public static double CarSpeed3Sigma = 10.0 / 60.0 / 60.0; //km/second
        public static double MaxCarSpeed = 240.0 / 3600.0; //km/second
        public static double MinCarSpeed = -10.0 / 3600.0; //km/second
        public static double CarWidth = 10.0 / 1000.0; // km
        public static double CarLength = 15.0 / 1000.0; //km
        public static double RoadWidth = 70.0 / 1000.0; // Pen Width: km
        public static double LaneWidth = RoadWidth / 4.0; //km
        public static double CrossingRadius = (20.0 / 1000.0); //km '50/1000
        public static int TrafficLightRadius = 5; //Pixels
        public static int NrCars = 50; //50
        public static int NrLanes = 1; //# lanes in each direction

        public static Color CarColour = Color.Red;

        //mycar
        public static Color MyCarColour = Color.Blue;

        //trafficlightcolour

        //lanes
        public static double TimeToChangeLane = 25; //seconds

        //carinjector
        public static double CarInjectorLength = 0.1; //km
        public static double CarInjectorWidth = 0.01; //km
        public static double CarInjectPeriod = 60; //seconds

        //carextractor
        public static double CarExtractorLength = 0.1; //km
        public static double CarExtractorWidth = 0.01; //km

        //circle
        public static int CircleNRoads = 16; //number
        public static double CircleRadius = 0.15; //km

        //crossing
        public static double CrossingRoadLength = 0.15; //km
        public static int CrossingNRoads = 4;

        //trafficlight
        public static double TrafficLightGreenLength = 40.0; //seconds
        public static double TrafficLightYellowLength = 10.0; //seconds 
        public static double TrafficLightRedLength = 110.0; //seconds

        public static double GScale = 290.0 / 1.0; //pixels per km

        public static simtimer LeaveForWorkTime = new simtimer(0,6,0,0);
	    public static simtimer LeaveForHomeTime = new simtimer(0,6,5,0);

        public static carsidet CarSide = carsidet.Right;

        //default network
        public static double[,] RoadPoints = new double[,] {{0, 0, 0}, 
                                      {1, 1, 0}, 
                                      {2, 2, 0}, 
                                      {3, 0, 1}, 
                                      {4, 1, 0.9}, 
                                      {5, 2.1, 1}, 
                                      {6, 0, 2}, 
                                      {7, 1, 2}, 
                                      {8, 2, 2}, 
                                      {9, 1.1, 1}, 
                                      {10, 1, 1.1}, 
                                      {11, 0.9, 1}, 
                                      {12, 2, 1.1}, 
                                      {13, 1.9, 1}, 
                                      {14, 2, 0.9}, 
                                      {15, 3, 0}, 
                                      {16, 3, 1}, 
                                      {17, 3, 2}, 
                                      {18, 3, 0.3}, 
                                      {19, 2.75, 0.3}, 
                                      {20, 2.75, 0.15}, 
                                      {21, 2.5, 0.3}, 
                                      {22, 2.5, 0.15}, 
                                      {23, 2.25, 0.3}, 
                                      {24, 2.25, 0.4}, 
                                      {25, 2.1, 0.4}, 
                                      {26, 2.25, 0.5}, 
                                      {27, 2.1, 0.5}, 
                                      {28, 2.25, 0.6}, 
                                      {29, 3, 0.6}};

        public static int[,] RoadNetwork = new int[,] {{0, 0, 1, (int)directionflowenum.Both}, 
                                        {1, 1, 2, (int)directionflowenum.Both}, 
                                        {2, 0, 3, (int)directionflowenum.Both}, 
                                        {3, 1, 4, (int)directionflowenum.Both}, 
                                        {4, 2, 14, (int)directionflowenum.Both}, 
                                        {5, 3, 11, (int)directionflowenum.Both}, 
                                        {6, 9, 13, (int)directionflowenum.Both}, 
                                        {7, 3, 6, (int)directionflowenum.Both}, 
                                        {8, 10, 7, (int)directionflowenum.Both}, 
                                        {9, 12, 8, (int)directionflowenum.Both}, 
                                        {10, 6, 7, (int)directionflowenum.Both}, 
                                        {11, 7, 8, (int)directionflowenum.Both}, 
                                        {12, 11, 4, (int)directionflowenum.Both}, 
                                        {13, 4, 9, (int)directionflowenum.Both}, 
                                        {14, 9, 10, (int)directionflowenum.Both}, 
                                        {15, 10, 11, (int)directionflowenum.Both},
                                        {16, 13, 14, (int)directionflowenum.Both}, 
                                        {17, 14, 5, (int)directionflowenum.Both}, 
                                        {18, 5, 12, (int)directionflowenum.Both}, 
                                        {19, 12, 13, (int)directionflowenum.Both}, 
                                        {20, 2, 15, (int)directionflowenum.Both}, 
                                        {21, 18, 29, (int)directionflowenum.Both}, 
                                        {22, 16, 5, (int)directionflowenum.Both}, 
                                        {23, 16, 17, (int)directionflowenum.Both}, 
                                        {24, 8, 17, (int)directionflowenum.Both}, 
                                        {25, 15, 18, (int)directionflowenum.Both}, 
                                        {26, 29, 16, (int)directionflowenum.Both}, 
                                        {27, 18, 19, (int)directionflowenum.Both}, 
                                        {28, 19, 20, (int)directionflowenum.Both}, 
                                        {29, 19, 21, (int)directionflowenum.Both}, 
                                        {30, 21, 22, (int)directionflowenum.Both}, 
                                        {31, 21, 23, (int)directionflowenum.Both}, 
                                        {32, 23, 24, (int)directionflowenum.Both}, 
                                        {33, 24, 25, (int)directionflowenum.Both}, 
                                        {34, 24, 26, (int)directionflowenum.Both}, 
                                        {35, 26, 27, (int)directionflowenum.Both}, 
                                        {36, 26, 28, (int)directionflowenum.Both}, 
                                        {37, 28, 29, (int)directionflowenum.Both}};

        public static double[,] RoadLaneWidths = new double[,] {{0, LaneWidth}, 
                                        {1, LaneWidth}, 
                                        {2, LaneWidth}, 
                                        {3, LaneWidth}, 
                                        {4, LaneWidth}, 
                                        {5, LaneWidth}, 
                                        {6, LaneWidth}, 
                                        {7, LaneWidth}, 
                                        {8, LaneWidth}, 
                                        {9, LaneWidth}, 
                                        {10, LaneWidth}, 
                                        {11, LaneWidth}, 
                                        {12, LaneWidth}, 
                                        {13, LaneWidth}, 
                                        {14, LaneWidth}, 
                                        {15, LaneWidth}, 
                                        {16, LaneWidth}, 
                                        {17, LaneWidth}, 
                                        {18, LaneWidth}, 
                                        {19, LaneWidth}, 
                                        {20, LaneWidth}, 
                                        {21, LaneWidth}, 
                                        {22, LaneWidth}, 
                                        {23, LaneWidth}, 
                                        {24, LaneWidth}, 
                                        {25, LaneWidth}, 
                                        {26, LaneWidth}, 
                                        {27, LaneWidth}, 
                                        {28, LaneWidth / 4}, 
                                        {29, LaneWidth}, 
                                        {30, LaneWidth / 4}, 
                                        {31, LaneWidth}, 
                                        {32, LaneWidth}, 
                                        {33, LaneWidth}, 
                                        {34, LaneWidth}, 
                                        {35, LaneWidth}, 
                                        {36, LaneWidth}, 
                                        {37, LaneWidth}};

        	//'ByVal aredlength As Double, ByVal ayellowlength As Double, ByVal agreenlength As Double, _
            //               ByVal ax As Double, ByVal ay As Double
	    public static double[,] TrafficLightDoubles = new double[,] 
	            {{50.0, 10.0, 40.0, 1.0, 0.0},
	             {50.0, 10.0, 40.0, 2.0, 0.0}};

	    //'ByVal acolour As trafficlightcolour, ByRef aroadon As road, _ByVal apointpointedto As Integer)

	    public static int[,] TrafficLightInts = new int[,]
	        {{(int)trafficlightcolour.Green, 0, 0},
	         {(int)trafficlightcolour.Green, 4, 1}};

        public static int OriginX = 10; //pixels
        public static int OriginY = 60; //pixels

        //Key timing settings
        public static int TimerInterval = 10; // micro seconds 10
        public static double SpeedUpFactor = 50; //factor 50
        public static double SampleT = TimerInterval / 1000.0 * SpeedUpFactor; // seconds - at this point SampleT 
																        // needs to be smaller than 60 seconds
																        // to make simtimer work correctly
        //Specific dimensions #2
        public static double TrafficLightOffSet = 0.8 * RoadWidth; //km
        public static double TrafficLightBlackOffSet = 3.0 / GScale; //km

        //Car acceleration #2
        public static double CarBreakAcceleration = -20.0 / 3600.0;  //km/second^2  from x km/h/s   to  x/3600 km/s/s
        public static double CarNormalAcceleration = 0.02 / GScale; //km/second^2

        //Public Const PDistanceController As Double = 0.04 //Proportional band of the distance controller between the cars
        public static double PSpeedController = 0.01; //0.01 'Prop band of the cars' speed controllers.
        public static double KDistanceController = 0.001; //0.001
        public static double IDistanceController = 0.5;
        public static double KSpeedController = 0.01; //0.5
        public static double ISpeedController = 1000000000000.0; //10
            //and the traffic lights and the other cars.


        //controldirection
        public static int Direct = -1;
        public static int Reverse = 1;

        public static carsidet DefaultCarSide = carsidet.Right;

        public static double MinDistanceFromPoint = 0.05; //km : Minimum Distance from each point for it to be selected

        //building
        public static double BuildingSideLength = 70.0 / 1000.0; //km



        //route calculation
        public static int MaxRouteLength = 100; //amount of roads that will be put together at most to get to destination
        public static int MaxRouteCalcIter = 50; //amount of iterations that the algorithm will go through in order
									        //to try and find the closest route to the destination

        static global()
        {
            

        }
    }
}
