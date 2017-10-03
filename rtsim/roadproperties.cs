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
    public partial class roadproperties : Form
    {
        public simulation sim;
		public road theroad;
        public int roadnr;

        public roadproperties(int aroadnr, simulation asim)
        {
            InitializeComponent();

            sim = asim;
			roadnr = aroadnr;
			theroad = sim.roads[roadnr];
			Text = String.Concat("Properties for road number: ", roadnr.ToString());
			numericUpDown1.Value = Convert.ToDecimal(theroad.nlanes);
			label3.Text = Math.Round(theroad.direction*180/Math.PI,2).ToString();
			label5.Text = Math.Round(theroad.distance,2).ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            theroad.nlanes = Convert.ToInt32(numericUpDown1.Value);
	        Hide();
        }



    }
}
