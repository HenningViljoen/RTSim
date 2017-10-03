using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

namespace rtsim
{
    public enum drawingmode { None, Roads, Cars };

    public partial class Form1 : Form
    {
        //Properties
        public simulation sim;
        Graphics dbGraphics;
        Graphics panelGraphics;
		Bitmap dbBitmap;
        Bitmap panelBitmap;
		drawingmode dmode;
		bool activedrawing; //road is being drawn since one click already occurred.
		point defaultcarlocation;
		int roadhoveringover;
		int pointpointedto; //especially for the traffic light case
		int carhoveringover;
		int trafficlighthoveringover;
		int carinjectorhoveringover;
		List<circle> circles;
		List<crossingclean> crossingcleans;
		List<crossingtrafficlight> crossingtrafficlights;
		//modelwindow activewin; //The active window on which the current model is being built and 
								//simulated.

        public Form1()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.Opaque, true);
            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, panel1, new object[] { true });
            panel1.Dock = DockStyle.Fill;
			timer1.Interval = global.TimerInterval;
			
			dbBitmap = null;
            panelBitmap = null;
            dbGraphics = null;
            panelGraphics = null;
			dmode = drawingmode.None;
			
			Form1_Resize(null, EventArgs.Empty);
            panel1_Resize(null, EventArgs.Empty);
			sim = new simulation(panelGraphics);

			sim.drawnetwork(panelGraphics);
			panelGraphics.DrawImageUnscaled(panelBitmap, 0, 0);

			circles = new List<circle>();
			crossingcleans = new List<crossingclean>();
            crossingtrafficlights = new List<crossingtrafficlight>();

            Focus();

            //toolStrip1.Anchor = AnchorStyles.None;
            
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            		// Get rid of old stuff
            if (dbGraphics != null)
            {
                dbGraphics.Dispose();
            }

            if (dbBitmap != null)
            {
                dbBitmap.Dispose();
            }

            if (ClientRectangle.Width > 0 && ClientRectangle.Height > 0)
            {
                // Create a bitmap
                dbBitmap = new Bitmap(ClientRectangle.Width,
                                        ClientRectangle.Height);

                // Grab its Graphics
                dbGraphics = Graphics.FromImage(dbBitmap);

                // Set up initial translation after resize (also at start)
                //dbGraphics->TranslateTransform((float)X, 25.0);
            }
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            if (panelGraphics != null)
            {
                panelGraphics.Dispose();
            }

            if (panelBitmap != null)
            {
                panelBitmap.Dispose();
            }

            if (panel1.ClientRectangle.Width > 0 && panel1.ClientRectangle.Height > 0)
            {
                panelBitmap = new Bitmap(panel1.AutoScrollMinSize.Width,
                                        panel1.AutoScrollMinSize.Height);

                panelGraphics = Graphics.FromImage(panelBitmap);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            sim.simulate(panelGraphics);
            toolStripLabel1.Text = sim.simtime.timerstring();
            panel1.Invalidate();
        }

        private void Form1_Scroll(object sender, ScrollEventArgs e)
        {
            //toolStrip1.Location = new Point(-AutoScrollPosition.X, -AutoScrollPosition.Y);
            //toolStrip1.Invalidate();
            //Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TranslateTransform((float)AutoScrollPosition.X, (float)AutoScrollPosition.Y);
            
            e.Graphics.DrawImageUnscaled(dbBitmap, 0, 0);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            //Rectangle cliprect = e.ClipRectangle;
            //cliprect.Offset(-panel1.AutoScrollPosition.X, -panel1.AutoScrollPosition.Y);
            //panel1_Resize(sender, e);
            e.Graphics.TranslateTransform((float)panel1.AutoScrollPosition.X, (float)panel1.AutoScrollPosition.Y);

            //e.Graphics.DrawImageUnscaled(panelBitmap, 0 , 0);

            // Create rectangle for displaying image.
            //int x = -panel1.AutoScrollPosition.X;
            //int y = -panel1.AutoScrollPosition.Y;
            //int width = panel1.ClientRectangle.Width;
            //int height = panel1.ClientRectangle.Height;
            //Rectangle destRect = new Rectangle(x, y, x + width, y + height);

            // Create coordinates of rectangle for source image. 
            
            //GraphicsUnit units = GraphicsUnit.Pixel;

            // Draw image to screen.
            //e.Graphics.DrawImage(panelBitmap, destRect, x, y, width, height, units);
            e.Graphics.DrawImageUnscaled(panelBitmap, 0, 0);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
            //Default Network button
        {
            sim.loadnetwork(panelGraphics);
            //sim->configroadlists();
            //panelGraphics->Clear(Color::Green);
            sim.drawnetwork(panelGraphics);
            panel1.Invalidate();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
            //Simulate button
        {
            sim.configroadlists();
            sim.calcnewroadnforall();
            timer1.Enabled = true;
            sim.simulating = true;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
	        
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!sim.simulating)
            {
                int pointerx = e.X + (-panel1.AutoScrollPosition.X);
                int pointery = e.Y + (-panel1.AutoScrollPosition.Y);
                if (toolStripButton2.Checked) //road button
                {
                    point dynamicpoint = null;
                    for (int i = 0; i < sim.points.Count; i++)
                    {
                        if (sim.points[i].highlighted) { dynamicpoint = sim.points[i]; }
                    }
                    if (dmode == drawingmode.Roads)
                    {
                        dmode = drawingmode.None;
                        if (dynamicpoint != null)
                        {
                            sim.roads[sim.roads.Count - 1].points[1] = dynamicpoint;
                            sim.points.RemoveAt(sim.points.Count - 1);
                        }

                    }
                    else
                    {
                        dmode = drawingmode.Roads;

                        if (dynamicpoint == null)
                        {
                            sim.points.Add(new point((pointerx - global.OriginX) / global.GScale, (pointery - global.OriginY) /
                                global.GScale));
                            dynamicpoint = sim.points[sim.points.Count - 1];
                        }

                        sim.points.Add(new point((pointerx - global.OriginX) / global.GScale, (pointery - global.OriginY) /
                            global.GScale));

                        sim.roads.Add(new road(sim.roads.Count,
                            dynamicpoint,
                            sim.points[sim.points.Count - 1],
                            Convert.ToInt32(global.NrLanes),
                            directionflowenum.Both, global.LaneWidth));
                    }
                }
                else if (toolStripButton4.Checked) //car button
                {
                    for (int i = 0; i < sim.roads.Count; i++)
                    {
                        if (sim.roads[i].highlighted) { addnewcar(); }
                    }
                }
                else if (toolStripButton17.Checked) //special car (mycar, or racing car) button
                {
                    for (int i = 0; i < sim.roads.Count; i++)
                    {
                        if (sim.roads[i].highlighted) { addnewspecialcar(); }
                    }
                }
                else if (toolStripButton5.Checked) //traffic light button
                {
                    for (int i = 0; i < sim.roads.Count; i++)
                    {
                        if (sim.roads[i].highlighted && roadhoveringover == i)
                        {
                            sim.trafficlights.Add(
                               new trafficlight(global.TrafficLightRedLength,
                                    global.TrafficLightYellowLength,
                                    global.TrafficLightGreenLength,
                                    0,
                                    sim.roads[i],
                                    sim.roads[i].points[pointpointedto].x,
                                    sim.roads[i].points[pointpointedto].y,
                                    pointpointedto == 1 ? 0 : 1, global.SampleT));
                        }
                    }
                }
                else if (toolStripButton12.Checked) //stop sign button
                {
                    for (int i = 0; i < sim.roads.Count; i++)
                    {
                        if (sim.roads[i].highlighted && roadhoveringover == i)
                        {
                            //stopsign(road^ aroadon, double ax, double ay, int apointpointedto, double adt);
                            sim.stopsigns.Add(
                                new stopsign(sim.roads[i],
                                    sim.roads[i].points[pointpointedto].x,
                                    sim.roads[i].points[pointpointedto].y,
                                    pointpointedto == 1 ? 0 : 1, global.SampleT));
                        }
                    }
                }
                else if (toolStripButton6.Checked) //building button
                {
                    for (int i = 0; i < sim.roads.Count; i++)
                    {
                        if (sim.roads[i].highlighted) { addnewbuilding(); }
                    }
                }
                else if (toolStripButton7.Checked) //carinjector button
                {
                    for (int i = 0; i < sim.roads.Count; i++)
                    {
                        if (sim.roads[i].highlighted && roadhoveringover == i)
                        {
                            //carinjector(road ^aroadon, int apointn);
                            sim.carinjectors.Add(new carinjector(sim.roads[i], pointpointedto,
                                global.CarInjectPeriod, global.SampleT));
                        }
                    }
                }
                else if (toolStripButton8.Checked) //carextractor button
                {
                    for (int i = 0; i < sim.roads.Count; i++)
                    {
                        if (sim.roads[i].highlighted && roadhoveringover == i)
                        {
                            //carinjector(road ^aroadon, int apointn);
                            sim.carextractors.Add(new carextractor(sim.roads[i], pointpointedto,
                                global.SampleT));
                        }
                    }
                }
                else if (toolStripButton9.Checked) //circle button
                {
                    addnewcircle();
                }
                else if (toolStripButton10.Checked) //crossingclean button
                {
                    addnewcrossingclean();
                }
                else if (toolStripButton11.Checked) //crossingtrafficlight button
                {
                    addnewcrossingtrafficlight();
                }
                else if (e.Button == System.Windows.Forms.MouseButtons.Right) //Right mouse button and no mode
                {

                    for (int i = 0; i < sim.cars.Count; i++)
                    {
                        if (sim.cars[i].highlighted)
                        {
                            contextMenuStrip1.Show(e.X, e.Y);
                        }
                    }
                    for (int i = 0; i < sim.roads.Count; i++)
                    {
                        if (sim.roads[i].highlighted)
                        {
                            contextMenuStrip1.Show(e.X, e.Y);
                        }
                    }
                    for (int i = 0; i < sim.trafficlights.Count; i++)
                    {
                        if (sim.trafficlights[i].highlighted)
                        {
                            contextMenuStrip1.Show(e.X, e.Y);
                        }
                    }
                    for (int i = 0; i < sim.carinjectors.Count; i++)
                    {
                        if (sim.carinjectors[i].highlighted)
                        {
                            contextMenuStrip1.Show(e.X, e.Y);
                        }
                    }
                }
            }
        }

        private int roadover(car acar, ref int pointpointedto) //for car
	    {
		    int roado = -1; //default - implying that no road is actually over at the moment.
		    double dx, dy, mousedirectionfrompoint0, deltadirection, distancefromroad, newdistancefromroad;
		    double distancetomiddlefrompoint0, distancefrompoint0tomouse;
		    double midroadx, midroady;
		    for (int i = 0; i < sim.roads.Count; i++)
		    {
			    dx = acar.location.x - sim.roads[i].points[0].x;
			    dy = acar.location.y - sim.roads[i].points[0].y;
			    mousedirectionfrompoint0 = utilities.calcdirection(dy,dx);
			    //mousedirectionfrompoint0 -= sim->roads[i]->direction;
			    deltadirection = mousedirectionfrompoint0 - sim.roads[i].direction;
			    if (Math.Cos(deltadirection) > 0)
			    {
				    distancefrompoint0tomouse = utilities.distance(dy,dx);
				    distancefromroad = Math.Sin(deltadirection)*distancefrompoint0tomouse;
				    if (Math.Abs(distancefromroad) <= 
					    sim.roads[i].nlanes*sim.roads[i].roadlanewidth && 
					    distancefrompoint0tomouse <= sim.roads[i].distance)
				    {
					    roado = i;
					    sim.roads[i].highlighted = true;
					    if (global.CarSide == carsidet.Right)
					    {
						    pointpointedto = distancefromroad >= 0 ? 1 : 0; 
					    }
					    else
					    {
						    pointpointedto = distancefromroad >= 0 ? 0 : 1;
					    }
					    distancetomiddlefrompoint0 = Math.Cos(deltadirection)*distancefrompoint0tomouse;
					    midroadx = sim.roads[i].points[0].x + 
						    distancetomiddlefrompoint0*Math.Cos(sim.roads[i].direction);
					    midroady = sim.roads[i].points[0].y + 
						    distancetomiddlefrompoint0*Math.Sin(sim.roads[i].direction);
					    acar.lane = Convert.ToInt32(
						    Math.Ceiling(Math.Abs(distancefromroad)/sim.roads[i].roadlanewidth) - 1);
					    newdistancefromroad = sim.roads[i].roadlanewidth*
						    (Math.Ceiling(distancefromroad/sim.roads[i].roadlanewidth));
					    acar.location.x = midroadx + 
						    newdistancefromroad*Math.Cos(Math.PI/2 + sim.roads[i].direction);
					    acar.location.y = midroady + 
						    newdistancefromroad*Math.Sin(Math.PI/2 + sim.roads[i].direction);
					    sim.calcmobilepointendofroad(sim.roads[i], pointpointedto,
						    acar.lane);
					    acar.newdestination(sim.mobilepoint, sim.roads[i], 
						    pointpointedto);
				    }
				    else
				    {
					    sim.roads[i].highlighted = false;
				    }
			    }
			    else
			    {
				    sim.roads[i].highlighted = false;
			    }
		    }
		    return roado;
	    }

        private int roadover(double x, double y, ref int pointpointedto) 
			 //for traffic light or carinjector or carextractor or stopsign
	    {
		    int roado = -1; //default - implying that no road is actually over at the moment.
		    double dx, dy, mousedirectionfrompoint0, deltadirection, distancefromroad, newdistancefromroad;
		    double distancetomiddlefrompoint0, distancefrompoint0tomouse;
		    double midroadx, midroady;
		    for (int i = 0; i < sim.roads.Count; i++)
		    {
			    dx = x - sim.roads[i].points[0].x;
			    dy = y - sim.roads[i].points[0].y;
			    mousedirectionfrompoint0 = utilities.calcdirection(dy,dx);
			    //mousedirectionfrompoint0 -= sim->roads[i]->direction;
			    deltadirection = mousedirectionfrompoint0 - sim.roads[i].direction;
			    if (Math.Cos(deltadirection) > 0)
			    {
				    distancefrompoint0tomouse = utilities.distance(dy,dx);
				    distancefromroad = Math.Sin(deltadirection)*distancefrompoint0tomouse;
				    if (Math.Abs(distancefromroad) <= 
					    sim.roads[i].nlanes*sim.roads[i].roadlanewidth && 
					    distancefrompoint0tomouse <= sim.roads[i].distance)
				    {
					    roado = i;
					    sim.roads[i].highlighted = true;
					    if (distancefromroad >= 0)
					    {
						    pointpointedto = 1;
						    sim.roads[i].points[1].highlighted = true;	
						    sim.roads[i].points[0].highlighted = false;	
					    }
					    else
					    {
						    pointpointedto = 0; /*side of road to be added*/
						    sim.roads[i].points[0].highlighted = true;	
						    sim.roads[i].points[1].highlighted = false;	
					    }
		
				    }
				    else
				    {
					    sim.roads[i].highlighted = false;
					    sim.roads[i].points[0].highlighted = false;
					    sim.roads[i].points[1].highlighted = false;
				    }
			    }
			    else
			    {
				    sim.roads[i].highlighted = false;
				    sim.roads[i].points[0].highlighted = false;
				    sim.roads[i].points[1].highlighted = false;
			    }
		    }
		    return roado;
	    }

        private int roadover(building abuilding, ref int pointpointedto) //for building
	    {
		    int roado = -1; //default - implying that no road is actually over at the moment.
		    double dx, dy, mousedirectionfrompoint0, deltadirection, distancefromroad, newdistancefromroad;
		    double distancetomiddlefrompoint0, distancefrompoint0tomouse;
		    double midroadx, midroady;
		    for (int i = 0; i < sim.roads.Count; i++)
		    {
			    dx = abuilding.location.x - sim.roads[i].points[0].x;
			    dy = abuilding.location.y - sim.roads[i].points[0].y;
			    mousedirectionfrompoint0 = utilities.calcdirection(dy,dx);
			    //mousedirectionfrompoint0 -= sim->roads[i]->direction;
			    deltadirection = mousedirectionfrompoint0 - sim.roads[i].direction;
			    if (Math.Cos(deltadirection) > 0)
			    {
				    distancefrompoint0tomouse = utilities.distance(dy,dx);
				    distancefromroad = Math.Sin(deltadirection)*distancefrompoint0tomouse;
				    if (Math.Abs(distancefromroad) <= 
					    sim.roads[i].nlanes*sim.roads[i].roadlanewidth && 
					    distancefrompoint0tomouse <= sim.roads[i].distance)
				    {
					    roado = i;
					    abuilding.roadon = sim.roads[i];
					    sim.roads[i].highlighted = true;
					    pointpointedto = distancefromroad >= 0 ? 0 : 1; /*side of road 
																					     to be added*/
					    distancetomiddlefrompoint0 = Math.Cos(deltadirection)*distancefrompoint0tomouse;
					    midroadx = sim.roads[i].points[0].x + 
						    distancetomiddlefrompoint0*Math.Cos(sim.roads[i].direction);
					    midroady = sim.roads[i].points[0].y + 
						    distancetomiddlefrompoint0*Math.Sin(sim.roads[i].direction);
					    newdistancefromroad = sim.roads[i].nlanes*sim.roads[i].roadlanewidth*
						    distancefromroad/Math.Abs(distancefromroad); //get the sign
					    abuilding.location.x = midroadx + 
						    newdistancefromroad*Math.Cos(Math.PI/2 + sim.roads[i].direction);
					    abuilding.location.y = midroady + 
						    newdistancefromroad*Math.Sin(Math.PI/2 + sim.roads[i].direction);
					    abuilding.direction = sim.roads[i].direction - 
						    distancefromroad/Math.Abs(distancefromroad)*Math.PI/2;
					    abuilding.calcclosestpointinroad();
				    }
				    else
				    {
					    sim.roads[i].highlighted = false;
				    }
			    }
			    else
			    {
				    sim.roads[i].highlighted = false;
			    }
		    }
		    return roado;
	    }

        private int roadover(double x, double y) //for selection mode
	    {
		    int roado = -1; //default - implying that no road is actually over at the moment.
		    double dx, dy, mousedirectionfrompoint0, deltadirection, distancefromroad, newdistancefromroad;
		    double distancetomiddlefrompoint0, distancefrompoint0tomouse;
		    double midroadx, midroady;
		    for (int i = 0; i < sim.roads.Count; i++)
		    {
			    dx = x - sim.roads[i].points[0].x;
			    dy = y - sim.roads[i].points[0].y;
			    mousedirectionfrompoint0 = utilities.calcdirection(dy,dx);
			    //mousedirectionfrompoint0 -= sim->roads[i]->direction;
			    deltadirection = mousedirectionfrompoint0 - sim.roads[i].direction;
			    if (Math.Cos(deltadirection) > 0)
			    {
				    distancefrompoint0tomouse = utilities.distance(dy,dx);
				    distancefromroad = Math.Sin(deltadirection)*distancefrompoint0tomouse;
				    if (Math.Abs(distancefromroad) <= 
					    sim.roads[i].nlanes*sim.roads[i].roadlanewidth && 
					    distancefrompoint0tomouse <= sim.roads[i].distance)
				    {
					    roado = i;
					    sim.roads[i].highlighted = true;
				    }
				    else
				    {
					    sim.roads[i].highlighted = false;
				    }
			    }
			    else
			    {
				    sim.roads[i].highlighted = false;
			    }
		    }
		    return roado;
	    }

        private int carinjectorover(double x, double y) //for selection mode for carinjectors
        {
	        int injectoro = -1; //default - implying that no injector is actually over at the moment.
	        double dx, dy, mousedirectionfrompoint0, deltadirection, distancefrominjector, newdistancefrominjector;
	        double distancetomiddlefrompoint0, distancefrompoint0tomouse;
	        double midroadx, midroady;
	        for (int i = 0; i < sim.carinjectors.Count; i++)
	        {
		        dx = x - sim.carinjectors[i].location.x;
		        dy = y - sim.carinjectors[i].location.y;
		        mousedirectionfrompoint0 = utilities.calcdirection(dy,dx);
		        //mousedirectionfrompoint0 -= sim->roads[i]->direction;
		        deltadirection = mousedirectionfrompoint0 - (sim.carinjectors[i].direction - Math.PI);
		        if (Math.Cos(deltadirection) > 0)
		        {
			        distancefrompoint0tomouse = utilities.distance(dy,dx);
			        distancefrominjector = Math.Sin(deltadirection)*distancefrompoint0tomouse;
			        if (distancefrominjector <= 0 && Math.Abs(distancefrominjector) <= 
				        global.CarInjectorWidth && 
				        distancefrompoint0tomouse <= global.CarInjectorLength)
			        {
				        injectoro = i;
				        sim.carinjectors[i].highlighted = true;
			        }
			        else
			        {
				        sim.carinjectors[i].highlighted = false;
			        }
		        }
		        else
		        {
			        sim.carinjectors[i].highlighted = false;
		        }
	        }
	        return injectoro;
        }

        private int carextractorover(double x, double y) //for selection mode for carextractor
        {
            int extractoro = -1; //default - implying that no extractor is actually over at the moment.
            double dx, dy, mousedirectionfrompoint0, deltadirection, distancefromextractor, newdistancefromextractor;
            double distancetomiddlefrompoint0, distancefrompoint0tomouse;
            double midroadx, midroady;
            for (int i = 0; i < sim.carextractors.Count; i++)
            {
                dx = x - sim.carextractors[i].location.x;
                dy = y - sim.carextractors[i].location.y;
                mousedirectionfrompoint0 = utilities.calcdirection(dy, dx);
                //mousedirectionfrompoint0 -= sim->roads[i]->direction;
                deltadirection = mousedirectionfrompoint0 - (sim.carextractors[i].direction - Math.PI);
                if (Math.Cos(deltadirection) > 0)
                {
                    distancefrompoint0tomouse = utilities.distance(dy, dx);
                    distancefromextractor = Math.Sin(deltadirection) * distancefrompoint0tomouse;
                    if (distancefromextractor <= 0 && Math.Abs(distancefromextractor) <=
                        global.CarInjectorWidth &&
                        distancefrompoint0tomouse <= global.CarExtractorLength)
                    {
                        extractoro = i;
                        sim.carextractors[i].highlighted = true;
                    }
                    else
                    {
                        sim.carextractors[i].highlighted = false;
                    }
                }
                else
                {
                    sim.carextractors[i].highlighted = false;
                }
            }
            return extractoro;
        }

        private int carover(double x, double y) //for selection mode
        {
	        int caro = -1; //default - implying that no car is actually over at the moment.
	        double dx, dy;
	        double distancefromcar;
	        for (int i = 0; i < sim.cars.Count; i++)
	        {
		        dx = x - sim.cars[i].location.x;
		        dy = y - sim.cars[i].location.y;

		        distancefromcar = utilities.distance(dy,dx);
		        if (distancefromcar <= sim.cars[i].cardiagkm)
		        {
			        caro = i;
		        }
		        else
		        {
			        sim.cars[i].highlighted = false;
		        }
	        }
	        if (caro >= 0) {sim.cars[caro].highlighted = true;}
	        return caro;
        }

        private int trafficlightover(double x, double y) //for selection mode
        {
	        int trafficlighto = -1; //default - implying that no car is actually over at the moment.
	        double dx, dy;
	        double distancefromtrafficlight;
	        for (int i = 0; i < sim.trafficlights.Count; i++)
	        {
		        dx = x - sim.trafficlights[i].graphlocation.x;
		        dy = y - sim.trafficlights[i].graphlocation.y;

		        distancefromtrafficlight = utilities.distance(dy,dx);
		        if (distancefromtrafficlight*global.GScale <= global.TrafficLightRadius)
		        {
			        trafficlighto = i;
		        }
		        else
		        {
			        sim.trafficlights[i].highlighted = false;
		        }
	        }
	        if (trafficlighto >= 0) {sim.trafficlights[trafficlighto].highlighted = true;}
	        return trafficlighto;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
	        
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!sim.simulating)
            {
                int pointerx = e.X + (-panel1.AutoScrollPosition.X);
                int pointery = e.Y + (-panel1.AutoScrollPosition.Y);
                if (toolStripButton4.Checked || toolStripButton2.Checked || toolStripButton5.Checked ||
                        toolStripButton6.Checked || toolStripButton7.Checked || toolStripButton8.Checked
                         || toolStripButton9.Checked || toolStripButton10.Checked ||
                         toolStripButton11.Checked || toolStripButton12.Checked ||
                         toolStripButton17.Checked)
                //car or road or trafficlight or building or carinjector or carextractor or circle or
                //crossingclean or crossingtrafficlight or stopsign or mycar
                {
                    
                    if (toolStripButton2.Checked) //Road button
                    {
                        int counttohighlight;
                        counttohighlight = dmode == drawingmode.Roads ? sim.points.Count - 1 :
                            sim.points.Count;
                        for (int i = 0; i < counttohighlight; i++)
                        {
                            if (utilities.distance(((pointery - global.OriginY) / global.GScale - sim.points[i].y),
                                ((pointerx - global.OriginX) / global.GScale - sim.points[i].x)) <=
                                global.MinDistanceFromPoint)
                            {
                                sim.points[i].highlighted = true;
                            }
                            else
                            {
                                sim.points[i].highlighted = false;
                            }
                        }

                        if (dmode == drawingmode.Roads)
                        {
                            sim.roads[sim.roads.Count - 1].points[1].x = (pointerx - global.OriginX) / global.GScale;
                            sim.roads[sim.roads.Count - 1].points[1].y = (pointery - global.OriginY) / global.GScale;
                            sim.roads[sim.roads.Count - 1].updatedirection();
                            //sim->roads[0]->draw(panelGraphics);
                        }
                    }
                    if ((toolStripButton4.Checked) && sim.roads.Count > 0) //car button
                    {
                        sim.cars[sim.cars.Count - 1].location.x = (pointerx - global.OriginX) / global.GScale;
                        sim.cars[sim.cars.Count - 1].location.y = (pointery - global.OriginY) / global.GScale;
                        roadhoveringover = roadover(sim.cars[sim.cars.Count - 1],
                            ref pointpointedto);
                    }

                    if ((toolStripButton17.Checked) && sim.roads.Count > 0) //racingcar button
                    {
                        sim.mycars[sim.mycars.Count - 1].location.x = (pointerx - global.OriginX) / global.GScale;
                        sim.mycars[sim.mycars.Count - 1].location.y = (pointery - global.OriginY) / global.GScale;
                        roadhoveringover = roadover(sim.mycars[sim.mycars.Count - 1],
                            ref pointpointedto);
                    }

                    if ((toolStripButton5.Checked || toolStripButton7.Checked || toolStripButton8.Checked ||
                        toolStripButton12.Checked)
                        && sim.roads.Count > 0)
                    //trafficlight or carinjector or carextractor or stop sign button
                    {
                        roadhoveringover = roadover((pointerx - global.OriginX) / global.GScale, (pointery - global.OriginY) /
                            global.GScale,
                            ref pointpointedto);
                    }
                    if (toolStripButton6.Checked && sim.roads.Count > 0) //building button
                    {
                        sim.buildings[sim.buildings.Count - 1].location.x = (pointerx - global.OriginX) / global.GScale;
                        sim.buildings[sim.buildings.Count - 1].location.y = (pointery - global.OriginY) / global.GScale;
                        roadhoveringover = roadover(sim.buildings[sim.buildings.Count - 1],
                            ref pointpointedto);
                    }
                    if (toolStripButton9.Checked) //circle button
                    {
                        circles[circles.Count - 1].setlocation((pointerx - global.OriginX) / global.GScale,
                            (pointery - global.OriginY) / global.GScale);
                    }
                    if (toolStripButton10.Checked) //crossingclean button
                    {
                        crossingcleans[crossingcleans.Count - 1].
                            setlocation((pointerx - global.OriginX) / global.GScale,
                            (pointery - global.OriginY) / global.GScale);
                    }
                    if (toolStripButton11.Checked) //crossingtrafficlight button
                    {
                        crossingtrafficlights[crossingtrafficlights.Count - 1].
                            setlocation((pointerx - global.OriginX) / global.GScale,
                            (pointery - global.OriginY) / global.GScale);
                    }

                }
                else //no button is clicked - selection and editing of properties mode
                {
                    if (sim.cars.Count > 0)
                    {
                        carhoveringover = carover((pointerx - global.OriginX) / global.GScale, (pointery - global.OriginY) /
                            global.GScale);
                    }

                    if (sim.roads.Count > 0)
                    {
                        roadhoveringover = roadover((pointerx - global.OriginX) / global.GScale, (pointery - global.OriginY) /
                            global.GScale);
                    }

                    if (sim.trafficlights.Count > 0)
                    {
                        trafficlighthoveringover = trafficlightover((pointerx - global.OriginX) / global.GScale,
                            (pointery - global.OriginY) /
                            global.GScale);
                    }
                    if (sim.carinjectors.Count > 0)
                    {
                        carinjectorhoveringover = carinjectorover((pointerx - global.OriginX) /
                            global.GScale, (pointery - global.OriginY) / global.GScale);
                    }
                    if (sim.carextractors.Count > 0)
                    {
                        carinjectorhoveringover = carextractorover((pointerx - global.OriginX) /
                            global.GScale, (pointery - global.OriginY) / global.GScale);
                    }
                }
                sim.drawnetwork(panelGraphics);
                panel1.Invalidate();
            }
        }

        private void addnewcar()
        {
	        sim.cars.Add(new car(global.CarSpeed + utilities.randnormv(global.CarSpeed3Sigma,sim.r), 
                global.SampleT, 
				        new point(0,0), sim.roads[0].points[1], sim.roads[0], 1, global.CarBreakAcceleration));
        }

        private void addnewspecialcar()
        {
            sim.mycars.Add(new mycar(0,
                global.SampleT,
                        new point(0, 0), sim.roads[0].points[1], sim.roads[0], 1, global.CarBreakAcceleration));
        }

        private void addnewbuilding()
        {
	        sim.buildings.Add(new building(sim.roads[0],new point(0,0),0));
        }

        private void addnewcircle()
        {
	        //circle(point ^alocation, int anroads, double aradius, simulation ^asim);
	        circles.Add(new circle(new point(0,0), global.CircleNRoads, global.CircleRadius, sim));
        }

        private void addnewcrossingclean()
        {
	        //crossingclean(point ^alocation, simulation ^asim);
	        crossingcleans.Add(new crossingclean(new point(0,0), sim));
        }

        private void addnewcrossingtrafficlight()
        {
	        //crossingclean(point ^alocation, simulation ^asim);
	        crossingtrafficlights.Add(new crossingtrafficlight(new point(0,0), sim));
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
 			 //Car button
	    {
		    if (toolStripButton4.Checked)
		    {
			    if (toolStripButton2.Checked) {toolStripButton2.Checked = false;}
			    if (sim.roads.Count > 0)
			    {
				    if (defaultcarlocation == null) 
					    {defaultcarlocation = new point(0,0);}
				    addnewcar();
			    }
		    }
		    if (!toolStripButton4.Checked)
		    {
			    if (sim.roads.Count > 0) 
			    {
				    sim.cars.RemoveAt(sim.cars.Count - 1);
				    sim.drawnetwork(panelGraphics);
				    Invalidate();
			    }
		    }
	    }

        private void toolStripButton2_Click(object sender, EventArgs e)
            //road
        {
            if (toolStripButton4.Checked)
            {
                toolStripButton4.Checked = false;
                toolStripButton4_Click(this, null);
            } //car should not be checked
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
			 //Building button
	    {
		    if (toolStripButton6.Checked)
		    {
			    if (toolStripButton2.Checked) {toolStripButton2.Checked = false;}
			    if (sim.roads.Count > 0)
			    {
				    if (defaultcarlocation == null) 
					    {defaultcarlocation = new point(0,0);}
				
				    addnewbuilding();
				    //road^ aroadon, double ax, double ay, 
				    //	double adirection
			    }
		    }
		    if (!toolStripButton6.Checked)
		    {
			    if (sim.roads.Count > 0) 
			    {
				    sim.buildings.RemoveAt(sim.buildings.Count - 1);
				    sim.drawnetwork(panelGraphics);
				    Invalidate();
			    }
		    }
	    }


        private void toolStripButton9_Click(object sender, EventArgs e)
			 //add new circle
        {
	        if (toolStripButton9.Checked)
	        {
		        if (toolStripButton2.Checked) {toolStripButton2.Checked = false;}
		        addnewcircle();
	        }
	        if (!toolStripButton9.Checked)
	        {
		        //delete circles[circles->Count - 1];
		        circles.RemoveAt(circles.Count - 1);
		        sim.drawnetwork(panelGraphics);
		        Invalidate();
	        }
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
			 //crossingclean event handler
        {
	        if (toolStripButton10.Checked)
	        {
		        if (toolStripButton2.Checked) {toolStripButton2.Checked = false;}
		        addnewcrossingclean();
	        }
	        if (!toolStripButton10.Checked)
	        {
		        //delete crossingcleans[crossingcleans->Count - 1];
		        crossingcleans.RemoveAt(crossingcleans.Count - 1);
		        sim.drawnetwork(panelGraphics);
		        Invalidate();
	        }
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
			 //CrossingTrafficLight button
        {
	        if (toolStripButton11.Checked)
	        {
		        if (toolStripButton2.Checked) {toolStripButton2.Checked = false;}
		        addnewcrossingtrafficlight();
	        }
	        if (!toolStripButton11.Checked)
	        {
		        //delete crossingtrafficlights[crossingtrafficlights->Count - 1];
		        crossingtrafficlights.RemoveAt(crossingtrafficlights.Count - 1);
		        sim.drawnetwork(panelGraphics);
		        Invalidate();
	        }
        }

        private void toolStripButton18_Click(object sender, EventArgs e) //Options button
        {
            globaloptions options = new globaloptions();
            options.Show();
            
            sim.drawnetwork(panelGraphics);
            Invalidate();
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        //Stop simulating button
        {
            timer1.Enabled = false;
            sim.simulating = false;
        }

        private void toolStripButton14_Click(object sender, EventArgs e)
			 //Save button
        {
	        Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "rtm files (*.rtm)|*.rtm";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if ( saveFileDialog1.ShowDialog() == DialogResult.OK )
            {
                if ( (myStream = saveFileDialog1.OpenFile()) != null)
                {

                // Code to write the stream goes here.
		        BinaryFormatter bf = new BinaryFormatter();
		        bf.Serialize(myStream, sim);
                myStream.Close();
                }
            }


	        /*FileStream ^fstream = File::Create("data.dat");
	        BinaryFormatter ^bf = gcnew BinaryFormatter();
	        bf->Serialize(fstream, sim);
	        fstream->Close();*/
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
			//Event handler for clicking "Properties" on the Context pop-up menu.
        {
	        for (int i = 0; i < sim.cars.Count; i++)
	        {
		        if (sim.cars[i].highlighted)
		        {
                    sim.cars[i].setproperties(i, sim.cars[i], sim);
		        }
	        }
	        for (int i = 0; i < sim.roads.Count; i++)
	        {
		        if (sim.roads[i].highlighted)
		        {
			        roadproperties roadprop = new roadproperties(i, sim);
			        roadprop.Show();
		        }
	        }
	        for (int i = 0; i < sim.trafficlights.Count; i++)
	        {
		        if (sim.trafficlights[i].highlighted)
		        {
			        trafficlightproperties trafficlightprop = new trafficlightproperties(i, sim);
			        trafficlightprop.Show();
		        }
	        }
            for (int i = 0; i < sim.carinjectors.Count; i++)
            {
                if (sim.carinjectors[i].highlighted)
                {
                    carinjectorproperties carinjectorprop = new carinjectorproperties(i, sim);
                    carinjectorprop.Show();
                }
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
            //Event handler for clicking "Delete" on the Context pop-up menu.
        {
            int idelete = -1, jdelete = -1;
            for (int i = 0; i < sim.cars.Count; i++)
            {
                if (sim.cars[i].highlighted)
                {
                    idelete = i;
                }
            }
            if (idelete >= 0) { sim.cars.RemoveAt(idelete); }

            idelete = -1;
            for (int i = 0; i < sim.roads.Count; i++)
            {
                if (sim.roads[i].highlighted)
                {
                    idelete = i;
                }
            }
            if (idelete >= 0)
            {
                if (sim.roads[idelete].points[0].roadlist.Count == 1)
                {
                    for (int i = 0; i < sim.points.Count; i++)
                    {
                        if (sim.roads[idelete].points[0] == sim.points[i]) { jdelete = i; }
                    }
                    sim.points.RemoveAt(jdelete);
                }
                if (sim.roads[idelete].points[1].roadlist.Count == 1)
                {
                    for (int i = 0; i < sim.points.Count; i++)
                    {
                        if (sim.roads[idelete].points[1] == sim.points[i]) { jdelete = i; }
                    }
                    sim.points.RemoveAt(jdelete);
                }
                sim.roads.RemoveAt(idelete);
            }

            idelete = -1;
            for (int i = 0; i < sim.trafficlights.Count; i++)
            {
                if (sim.trafficlights[i].highlighted)
                {
                    idelete = i;
                }
            }
            if (idelete >= 0) { sim.trafficlights.RemoveAt(idelete); }

            idelete = -1;
            for (int i = 0; i < sim.carinjectors.Count; i++)
            {
                if (sim.carinjectors[i].highlighted)
                {
                    idelete = i;
                }
            }
            if (idelete >= 0) { sim.carinjectors.RemoveAt(idelete); }

            sim.configroadlists();
        }

        private void toolStripButton15_Click(object sender, EventArgs e)
		    //Open button
        {
	        Stream myStream;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if ( openFileDialog1.ShowDialog() == DialogResult.OK )
            {
                if ( (myStream = openFileDialog1.OpenFile()) != null )
                {
                // Insert code to read the stream here.
		        if (sim != null) {sim.Dispose();}
		        BinaryFormatter bf = new BinaryFormatter();
		        sim = (simulation)bf.Deserialize(myStream);
                myStream.Close();
                }
            }
        }

        private void toolStripButton16_Click(object sender, EventArgs e)
			 //New button event handler
        {
            modelwindow win = new modelwindow();
            win.Show();
        }

        private void toolStripButton17_Click(object sender, EventArgs e)
            // Racing car button
        {
            if (toolStripButton17.Checked)
            {
                if (toolStripButton2.Checked) { toolStripButton2.Checked = false; } //checks for road 
                                                                                    // button clicked.
                if (sim.roads.Count > 0)
                {
                    if (defaultcarlocation == null)
                    { defaultcarlocation = new point(0, 0); }
                    addnewspecialcar();
                }
            }
            if (!toolStripButton17.Checked)
            {
                if (sim.roads.Count > 0)
                {
                    sim.cars.RemoveAt(sim.cars.Count - 1);
                    sim.drawnetwork(panelGraphics);
                    Invalidate();
                }
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                sim.mycars[0].speed++; 
            }

            if (e.KeyCode == Keys.Down)
            {
                sim.mycars[0].speed--; 
            }
        }

        

        

        

        

        
















    }
}
