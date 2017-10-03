using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace rtsim
{
    [Serializable]
    public class simulation : IDisposable
    {
        //Properties
		public List<point> points;
        public point mobilepoint, mobilepoint2;
        public List<road> roads;
        public List<car> cars;
        public List<mycar> mycars;
        public List<trafficlight> trafficlights;
        public List<stopsign> stopsigns;
        public List<building> buildings;
        public List<carinjector> carinjectors;
        public List<carextractor> carextractors;
        public Random r;
        public bool simulating;
        public simtimer simtime;
        public bool carstobedeleted; //if any cars are to be deleted during an iteration, this flag will be true

        //Methods
        public simulation(Graphics G)
        {
	        r = new Random();
	        points = new List<point>();
            roads = new List<road>();
            cars = new List<car>();
            mycars = new List<mycar>();
            trafficlights = new List<trafficlight>();
	        stopsigns = new List<stopsign>();
	        buildings = new List<building>();
	        carinjectors = new List<carinjector>();
	        carextractors = new List<carextractor>();
	        mobilepoint = new point(0, 0);
            mobilepoint2 = new point(0, 0);
	        simulating = false;
	        simtime = new simtimer(0,6,0,0);
	        carstobedeleted = false;
        }

        public void Dispose()
        {
        }

        public void loadnetwork(Graphics G)
        {
            /*npoints = globals::RoadPoints->GetLength(0);
            nroads = globals::RoadNetwork->GetLength(0);
            ncars = NrCars;
            ntrafficlights = globals::TrafficLightDoubles->GetLength(0);*/
    
            int i;
            for (i = 0; i < global.RoadPoints.GetLength(0); i++)
	        {
                points.Add(new point(global.RoadPoints[i, 1], global.RoadPoints[i, 2]));
		        //points[i]->roadlist = gcnew ArrayList(1); 
	        }
    
            for (i = 0; i < global.RoadNetwork.GetLength(0); i++)
	        {
		        //road(int anr, point^ p0, point^ p1, int anlanes, directionflowenum adirectionflow, 
			        //double aroadlanewidth);
                roads.Add(new road(i, points[global.RoadNetwork[i, 1]], 
			        points[global.RoadNetwork[i, 2]], global.NrLanes,
                    (directionflowenum)global.RoadNetwork[i, 3], global.LaneWidth));
	        }

            for (i = 0; i < global.NrCars; i++)
	        {
                mobilepoint.x = points[1].x + 
                            global.CrossingRadius * Math.Cos(roads[0].direction + Math.PI) + 
                            roads[0].roadlanewidth * Math.Cos(roads[0].direction + Math.PI / 2);
                mobilepoint.y = points[1].y + 
                    global.CrossingRadius * Math.Sin(roads[0].direction + Math.PI) + 
                    roads[0].roadlanewidth * Math.Sin(roads[0].direction + Math.PI / 2);
                mobilepoint2.x = points[0].x + roads[0].roadlanewidth * Math.Cos(roads[0].direction + 
			        Math.PI / 2);
                mobilepoint2.y = points[0].y +
			        roads[0].roadlanewidth * Math.Sin(roads[0].direction + Math.PI / 2);
		        //car::car(double aspeed, double adt, point^ alocation, 
                //            point^ adestination, road^ aroadon, int adestination01, 
                 //           double abreakacceleration)
                cars.Add(new car(global.CarSpeed + utilities.randnormv(global.CarSpeed3Sigma,r), global.SampleT, 
			        new point(mobilepoint2.x, mobilepoint2.y), mobilepoint, roads[0], 1, global.CarBreakAcceleration));
                cars[i].draw(G, false);
	        }

            for (i = 0; i < global.TrafficLightDoubles.GetLength(0); i++)
	        {
                trafficlights.Add(new trafficlight(global.TrafficLightDoubles[i, 0], 
			        global.TrafficLightDoubles[i, 1], 
                    global.TrafficLightDoubles[i, 2], 
			        0, 
			        roads[global.TrafficLightInts[i, 1]], 
                    global.TrafficLightDoubles[i, 3], global.TrafficLightDoubles[i, 4], 
			        global.TrafficLightInts[i, 2], global.SampleT));
                //cars(i).draw()
	        }
        }

        public void configroadlists()
        {
	        for (int i = 0; i < points.Count; i++)
	        {
		        points[i].roadlist.Clear();
                for (int j = 0; j < roads.Count; j++)
		        {
                    //if (globals::RoadNetwork[j, 1] == i || globals::RoadNetwork[j, 2] == i) 
			        if (roads[j].points[0] == points[i] || roads[j].points[1] == points[i]) 
			        {
				        points[i].roadlist.Add(j);
			        }
		        }
	        }
	        for (int i = 0; i < points.Count; i++)
	        {
		        double maxradius = global.CrossingRadius;
		        for (int j = 0; j < points[i].roadlist.Count; j++)
		        {
			        int k = points[i].roadlist[j];
			        double v = roads[k].nlanes*roads[k].roadlanewidth;
			        if (v > maxradius) {maxradius = v;}
		        }
		        points[i].crossingradius = maxradius;
	        }
        }

        public void drawnetwork(Graphics G)
        {
	        G.Clear(Color.Green);
            for (int i = 0; i < roads.Count; i++) {roads[i].draw(G);}
	        for (int i = 0; i < roads.Count; i++) {roads[i].drawhighlightedends(G);}
	        for (int i = 0; i < trafficlights.Count; i++) {trafficlights[i].draw(G);}
	        for (int i = 0; i < stopsigns.Count; i++) {stopsigns[i].draw(G);}
	        for (int i = 0; i < buildings.Count; i++) {buildings[i].draw(G);}
	        for (int i = 0; i < carinjectors.Count; i++) {carinjectors[i].draw(G);}
	        for (int i = 0; i < carextractors.Count; i++) {carextractors[i].draw(G);}
	        for (int i = 0; i < cars.Count; i++) {cars[i].draw(G, false);}
        }

        public void calcmobilepointendofroad(road theroad, int destination01, int lanenr) 
	        //This method might become redundant if the new equivalent method in the car class can be used.
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
	        mobilepoint.x = theroad.points[destination01].x + 
		        theroad.points[destination01].crossingradius*
		        Math.Cos(theroad.direction + bias + Math.PI) + 
		        (lanenr + 1)*theroad.roadlanewidth * 
		        Math.Cos(theroad.direction + bias + side*Math.PI/2);
	        mobilepoint.y = theroad.points[destination01].y + 
		        theroad.points[destination01].crossingradius*
		        Math.Sin(theroad.direction + bias + Math.PI) + 
		        (lanenr + 1)*theroad.roadlanewidth * 
		        Math.Sin(theroad.direction + bias + side*Math.PI/2);
        }

        public void calcmobilepointstartofroad(car thecar, road newroad, int lanenr) 
        {
	        double bias; // 'for direction
	        int side;

	        if (thecar.roadon.points[thecar.destination01] == newroad.points[0])
	        {
		        bias = 0;
	        }
	        else
	        {
		        bias = Math.PI;
	        }

	        side = global.CarSide == carsidet.Right ? 1 : -1;
	        mobilepoint.x = thecar.roadon.points[thecar.destination01].x + 
		        thecar.roadon.points[thecar.destination01].crossingradius*
		        Math.Cos(newroad.direction + bias) + 
		        newroad.nlanes*newroad.roadlanewidth * 
		        Math.Cos(newroad.direction + bias + side*Math.PI / 2);
	        mobilepoint.y = thecar.roadon.points[thecar.destination01].y + 
		        thecar.roadon.points[thecar.destination01].crossingradius*
		        Math.Sin(newroad.direction + bias) + 
		        newroad.nlanes*newroad.roadlanewidth * 
		        Math.Sin(newroad.direction + bias + side*Math.PI / 2);
        }

        public void calcnewroadn(int j)
        {
	        int nroadlist;
	        bool newroadgood = false;
	        if (cars[j].roadon.points[cars[j].destination01].roadlist == null)
	        {
		        nroadlist = 0;
	        }
	        else
	        {
		        nroadlist = cars[j].roadon.points[cars[j].destination01].roadlist.Count;
	        }
	        if (nroadlist == 0 || nroadlist == 1) 
	        {
		        cars[j].newroadn = cars[j].roadon.nr;
		        for (int k = 0; k < carextractors.Count; k++)
		        {
			        if (carextractors[k].location == 
				        cars[j].roadon.points[cars[j].destination01])
			        {
				        cars[j].cartobedeleted = true;
				
			        }
		        }
	        }
	        else
	        {
		        do
		        {
			        newroadgood = true;
			        cars[j].newroadn = (int) cars[j].roadon.points[cars[j].destination01].
				        roadlist[Convert.ToInt32(Math.Round(r.NextDouble()*(nroadlist - 1)))];
                    if (roads[cars[j].newroadn].directionflow == directionflowenum.Direction01)
			        {
				        newroadgood = cars[j].roadon.points[cars[j].destination01] ==
					        roads[cars[j].newroadn].points[0];
			        }
                    else if (roads[cars[j].newroadn].directionflow == directionflowenum.Direction10)
			        {
				        newroadgood = cars[j].roadon.points[cars[j].destination01] == 
					        roads[cars[j].newroadn].points[1];
			        }
			        newroadgood = (newroadgood && (roads[cars[j].newroadn] != cars[j].roadon));
		        } while (!newroadgood);
	        }
        }

        public void calcnewroadnforall()
        {
	        for (int i = 0; i < cars.Count; i++)
	        {
		        calcnewroadn(i);
	        }
        }

        public bool safetodrive(int j)
        {
	        bool safe = true;
	
	        for (int i = 0; i < cars.Count; i++)
	        {
		        if (roads[cars[j].newroadn] == roads[cars[i].newroadn] &&
			        cars[j].roadon != cars[i].roadon)
		        {
			        if (utilities.distance(cars[j].location, 
				        cars[j].roadon.points[cars[j].destination01]) > 
				        utilities.distance(cars[i].location, 
						        cars[i].roadon.points[cars[i].destination01]))
			        {
				        safe = false;
			        }
		        }
	        }

	        return safe;
        }

        public void simulate(Graphics G)
        {
	
            double tempdistance;
            double bias; // 'for direction
    
	        int side;

	        //G->Clear(Color::Green);
	        drawnetwork(G);

            for (int j = 0; j < cars.Count; j++)
	        {
                if (simtime == global.LeaveForHomeTime) {plancarroute(j);}
		        cars[j].move(G);

                for (int k = 0; k < cars.Count; k++)
		        {

			        //car breaking code
                    if (k != j && cars[j].roadon == cars[k].roadon && 
				        cars[j].lane == cars[k].lane &&
				        cars[j].destination01 == cars[k].destination01)
			        {
                        tempdistance = utilities.distance(cars[j].location, 
					        cars[j].roadon.points[cars[j].destination01]) - 
                            utilities.distance(cars[k].location, 
					        cars[k].roadon.points[cars[k].destination01]);
                        if ((tempdistance < (cars[j].breakdistance + global.CarLength*2)) && (tempdistance >= 0)) 
				        {
					        if (cars[j].lane > 0 && cars[j].ontrajectory == false 
						        && cars[j].changinglanes == false)
					        {
						        cars[j].lanechange(-1);
					        }
					        if (cars[j].changinglanes == false)
					        {
						        if (cars[j].mode != carmode.Breaking) 
						        {
							        cars[j].distancecontroller.init(cars[j].breakdistance, 
								        tempdistance, cars[j].speed);
						        }
						        if ((tempdistance < global.CarLength*2) && (cars[j].speed < 1.0/3600.0))
						        {
							        cars[j].speed = 0;
						        }
						        else
						        {
							        cars[j].speed = cars[j].distancecontroller.update(cars[j].breakdistance + 
								        global.CarLength * 2, tempdistance);
						        }
						        //'If cars(j).speed < 0 Then cars(j).speed = 0
						        cars[j].mode = carmode.Breaking;
					        }
				        }
                        else if (cars[j].mode != carmode.Normal) 
				        {
                            cars[j].speedcontroller.init(cars[j].preferredspeed, 
						        cars[j].speed, cars[j].speed);
                            cars[j].mode = carmode.Normal;
				        }
			        }
		        }

                


		        //traffic light handling code
                for (int k = 0; k < trafficlights.Count; k++)
		        {
                    if (trafficlights[k].roadon == cars[j].roadon)
			        {
                        if (cars[j].destination01 == trafficlights[k].pointnotpointedto)
				        {
                            tempdistance = utilities.distance(cars[j].location, 
						        cars[j].roadon.points[cars[j].destination01]) - 
                                utilities.distance(trafficlights[k].location, 
						        cars[j].roadon.points[cars[j].destination01]);
                            if (tempdistance < (cars[j].breakdistance)) 
					        {
                                if (trafficlights[k].colour != trafficlightcolour.Green)
						        {
                                    if (cars[j].mode != carmode.Breaking)
							        {
                                        cars[j].distancecontroller.init(cars[j].breakdistance, tempdistance, 
                                                                        cars[j].speed);
							        }
                                    if (tempdistance <= 0) 
							        {
                                        cars[j].speed = 0;
							        }
                                    else
							        {
                                        cars[j].speed = 
									        cars[j].distancecontroller.update(cars[j].breakdistance, 
									        tempdistance);
							        }
                                    cars[j].mode = carmode.Breaking;
						        }
					        }
                            if ((trafficlights[k].colour == trafficlightcolour.Green) &&
                                    (cars[j].mode != carmode.Normal))
					        {
                                cars[j].speedcontroller.init(cars[j].preferredspeed, 
							        cars[j].speed, cars[j].speed);
                                cars[j].mode = carmode.Normal;
					        }
				        }
			        }
		        }

		        //stop sign handling code
                for (int k = 0; k < stopsigns.Count; k++)
		        {
                    if (stopsigns[k].roadon == cars[j].roadon)
			        {
                        if (cars[j].destination01 == stopsigns[k].pointnotpointedto)
				        {
                            tempdistance = utilities.distance(cars[j].location, 
						        cars[j].roadon.points[cars[j].destination01]) - 
                                utilities.distance(stopsigns[k].location, 
						        cars[j].roadon.points[cars[j].destination01]);
                            if (tempdistance < (cars[j].breakdistance)) 
					        {
                                if (cars[j].mode != carmode.Breaking)
						        {
                                    cars[j].distancecontroller.init(cars[j].breakdistance, tempdistance, 
                                                                    cars[j].speed);
						        }
                                if (tempdistance <= 0) 
						        {
                                    cars[j].speed = 0;
						        }
                                else
						        {
                                    cars[j].speed = 
								        cars[j].distancecontroller.update(cars[j].breakdistance, 
								        tempdistance);
						        }
                                cars[j].mode = carmode.Breaking;
					        }
                            if (safetodrive(j) &&
                                    cars[j].mode != carmode.Normal)
					        {
                                cars[j].speedcontroller.init(cars[j].preferredspeed, 
							        cars[j].speed, cars[j].speed); 
                                cars[j].mode = carmode.Normal;
					        }

				        }
			        }
		        }



		        //code executed once the car has reached its destination
                if (cars[j].atdestination)
		        {
			        cars[j].readlane();
			        if (cars[j].changinglanes)
			        {
				        cars[j].stoplanechange();
			        }
                    else if (cars[j].ontrajectory == false)
			        {
				        if (cars[j].decisiont == decisiontype.BuildingToBuilding)
				        {
					        if (cars[j].currentstepinroute < 0)
					        {
						        cars[j].currentstepinroute++;
						        calcmobilepointendofroad(cars[j].roadon, cars[j].destination01, 
							        cars[j].roadon.nlanes - 1);
						        cars[j].newdestination(mobilepoint, 
										        cars[j].roadon, 
                                                cars[j].destination01);
					        }
					        else if (cars[j].currentstepinroute < cars[j].route.Count - 1)
					        {
						        cars[j].currentstepinroute++;
						        cars[j].newroadn = cars[j].route[cars[j].currentstepinroute];
					        }
					        if (cars[j].currentstepinroute == cars[j].route.Count)
					        {
						        cars[j].currentstepinroute++;
						        cars[j].newdestination(cars[j].ultimatedest, cars[j].roadon, 
                                                cars[j].destination01);
					        }
				        }
				        else
				        {
					        //calcnewroadn(j); //index of car sent.
					        if (cars[j].cartobedeleted) {carstobedeleted = true;}

				        }
				        if (cars[j].decisiont != decisiontype.BuildingToBuilding || 
					        (cars[j].decisiont == decisiontype.BuildingToBuilding && 
					        cars[j].currentstepinroute <= cars[j].route.Count - 1 &&
					        cars[j].currentstepinroute > 0))
				        {
					        calcmobilepointstartofroad(cars[j], roads[cars[j].newroadn], 
						        roads[cars[j].newroadn].nlanes - 1);

					        if (cars[j].roadon.points[cars[j].destination01] == 
						        roads[cars[j].newroadn].points[0])
					        {
						        cars[j].newdestination(mobilepoint, roads[cars[j].newroadn], 1);
					        }
					        else
					        {
						        cars[j].newdestination(mobilepoint, roads[cars[j].newroadn], 0);
					        }
					        cars[j].ontrajectory = true;
				        }
			        }
                    else if (cars[j].ontrajectory == true) //there could be a bug here... was '=' in c++
			        {
                        if (cars[j].decisiont == decisiontype.BuildingToBuilding && 
					        cars[j].currentstepinroute == cars[j].route.Count - 1)
				        {
					        cars[j].currentstepinroute++;
					        mobilepoint.x = cars[j].penultimatedest.x;
					        mobilepoint.y = cars[j].penultimatedest.y;
				        }
				        else
				        {
					        calcmobilepointendofroad(cars[j].roadon, cars[j].destination01, 
						        cars[j].roadon.nlanes - 1);
				        }
                        cars[j].newdestination(mobilepoint, cars[j].roadon, 
                                cars[j].destination01);
                        cars[j].ontrajectory = false;

				        calcnewroadn(j); //index of car sent.
			        }
		        }
	        }

            

	        //delete cars that are to be deleted
	        if (carstobedeleted)
	        {
		        for (int j = 0; j < cars.Count; j++)
		        {
			        if (cars[j].cartobedeleted) {cars.RemoveAt(j);}
		        }
		        carstobedeleted = false;
	        }

	        //update trafficlights
            for (int j = 0; j < trafficlights.Count; j++)
	        {
                trafficlights[j].update();
	        }

	        //update carinjectors
	        for (int j = 0; j < carinjectors.Count; j++)
	        {
		        /*car(double aspeed, double adt, point^ alocation, 
                            point^ adestination, road^ aroadon, int adestination01, 
                            double abreakacceleration);*/
                carinjectors[j].update();
		        if (carinjectors[j].injectnow) 
		        {
                    point startpoint = new point(0, 0);
                    startpoint.copyfrom(carinjectors[j].carstartlocation);
			        calcmobilepointendofroad(carinjectors[j].roadon, 
				        carinjectors[j].cardestination01, carinjectors[j].carlane);
			        cars.Add(new car(global.CarSpeed + utilities.randnormv(global.CarSpeed3Sigma, r), 
				        global.SampleT, startpoint, mobilepoint, 
                        carinjectors[j].roadon, carinjectors[j].cardestination01, 
                        global.CarBreakAcceleration));

                //    sim.cars.Add(new car(global.CarSpeed + utilities.randnormv(global.CarSpeed3Sigma, sim.r),
                //global.SampleT,
                //        new point(0, 0), sim.roads[0].points[1], sim.roads[0], 1, global.CarBreakAcceleration));


		        }
	        }

	        simtime.tick();
        }

        public void plancarroute(int i)
        {
	        if (cars[i].decisiont == decisiontype.BuildingToBuilding)
	        {
		        if (cars[i].atdestination)
		        {
			        cars[i].newdestination(cars[i].penultimatedest, cars[i].roadon, 
                                                cars[i].destination01);
			        cars[i].currentstepinroute = -1;
		        }
		        else
		        {
			        cars[i].currentstepinroute = 0;
		        }
		        List<int>[] routes = new List<int>[global.MaxRouteCalcIter];
		        double[] distances = new double[global.MaxRouteCalcIter];
		        double shortestlength = 10000000000000;
		        int targetroad;
		        point targetpoint;
		        int currentroad, newroadn;
		        int destination01;
		        int nroadlist;
		        int k;
		
		        if (simtime < global.LeaveForWorkTime || simtime >= global.LeaveForHomeTime)
		        {
			        targetroad = buildings[cars[i].buildingfrom].roadon.nr;
			        cars[i].penultimatedest = buildings[cars[i].buildingfrom].closestpointinroad;
			        cars[i].ultimatedest = buildings[cars[i].buildingfrom].middleofbuilding;
		        }
		        else
		        {
			        targetroad = buildings[cars[i].buildingto].roadon.nr;
			        cars[i].penultimatedest = buildings[cars[i].buildingto].closestpointinroad;	
			        cars[i].ultimatedest = buildings[cars[i].buildingto].middleofbuilding;	
		        }
		        targetpoint = cars[i].penultimatedest;
		        for (int j = 0; j < global.MaxRouteCalcIter; j++)
		        {
			        routes[j] = new List<int>(0);
			        currentroad = cars[i].roadon.nr;
			        routes[j].Add(currentroad);
			        destination01 = cars[i].destination01;
			        distances[j] = utilities.distance(cars[i].location,
				        roads[currentroad].points[destination01]);
			        k = 0;
			        while (currentroad != targetroad && k < global.MaxRouteLength)
			        {
				        nroadlist = roads[currentroad].points[destination01].roadlist.Count;
				        newroadn = (int)roads[currentroad].points[destination01].
                            roadlist[Convert.ToInt32(Math.Round(r.NextDouble()*(nroadlist - 1)))];
				        destination01 = roads[newroadn].points[0] == roads[currentroad].points[destination01] ? 1 : 0;
				        currentroad = newroadn;
				        routes[j].Add(newroadn);
				        if (newroadn != targetroad)
				        {
					        distances[j] += utilities.distance(roads[newroadn].points[0],
						        roads[newroadn].points[1]);
				        }
				        else
				        {
					        distances[j] += utilities.distance(roads[newroadn].points[destination01],
						        targetpoint);
				        }
				
				        k++;
			        }
			        if (distances[j] < shortestlength) 
			        {
				        shortestlength = distances[j];
				        cars[i].route = routes[j];
			        };
		        }
	        }
        }





    }
}
