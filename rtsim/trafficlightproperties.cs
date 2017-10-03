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
    public partial class trafficlightproperties : Form
    {
        public simulation sim;
		public trafficlight thetrafficlight;
        public int trafficlightnr;

        public trafficlightproperties(int atrafficlightnr, simulation asim)
        {
            InitializeComponent();

            sim = asim;
			trafficlightnr = atrafficlightnr;
			thetrafficlight = sim.trafficlights[trafficlightnr];
			Text = String.Concat("Properties for trafficlight number: ", trafficlightnr.ToString());
			numericUpDown1.Value = Convert.ToDecimal(Math.Round(thetrafficlight.timer));
			numericUpDown2.Value = Convert.ToDecimal(Math.Round(thetrafficlight.greenlength));
			numericUpDown3.Value = Convert.ToDecimal(Math.Round(thetrafficlight.yellowlength));
			numericUpDown4.Value = Convert.ToDecimal(Math.Round(thetrafficlight.redlength));
        }

        private void button1_Click(object sender, EventArgs e)
			 //OK button
        {
	        thetrafficlight.timer = Convert.ToDouble(numericUpDown1.Value);
	        thetrafficlight.greenlength = Convert.ToDouble(numericUpDown2.Value);
	        thetrafficlight.yellowlength = Convert.ToDouble(numericUpDown3.Value);
	        thetrafficlight.redlength = Convert.ToDouble(numericUpDown4.Value);
	        Hide();
        }

        
    }
}
