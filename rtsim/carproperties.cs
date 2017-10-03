using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace rtsim
{
    public partial class carproperties : Form
    {
        public simulation sim;
		public car thecar;
        public int carnr;

        public carproperties(int acarnr, car acar, simulation asim)
        {
            InitializeComponent();

            carnr = acarnr;
			sim = asim;
			thecar = acar;
			Text = String.Concat("Properties for car number: ", carnr.ToString());
            string[] decisionarray = Enum.GetNames(typeof(decisiontype));
            for (int i = 0; i < decisionarray.Length; i++)
                { listBox1.Items.Add(decisionarray[i]); }
			listBox1.SetSelected((int)thecar.decisiont, true);

			if (sim.buildings.Count > 0)
			{
				for (int i = 0; i < sim.buildings.Count; i++)
					{listBox2.Items.Add(i.ToString());}
				listBox2.SetSelected(thecar.buildingfrom, true);

				for (int i = 0; i < sim.buildings.Count; i++)
					{listBox3.Items.Add(i.ToString());}
				listBox3.SetSelected(thecar.buildingto, true);
			}
			numericUpDown1.Value = Convert.ToDecimal(Math.Round(thecar.speed*3600));
        }

        private void button1_Click(object sender, EventArgs e)
			 //OK button
        {
	        for (int i = 0; i < listBox1.Items.Count; i++)
	        {
		        if (listBox1.GetSelected(i)) {thecar.decisiont = (decisiontype)i;}
	        }

	        for (int i = 0; i < listBox2.Items.Count; i++)
	        {
		        if (listBox2.GetSelected(i)) {thecar.buildingfrom = i;}
	        }

	        for (int i = 0; i < listBox3.Items.Count; i++)
	        {
		        if (listBox3.GetSelected(i)) {thecar.buildingto = i;}
	        }
	        thecar.speed = Convert.ToDouble(numericUpDown1.Value)/3600.0;
	        thecar.calcdeltalocation();
	        sim.configroadlists();
	        sim.plancarroute(carnr);
	        Hide();
        }




    }
}
