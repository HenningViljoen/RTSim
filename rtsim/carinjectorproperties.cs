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
    public partial class carinjectorproperties : Form
    {
        private simulation thesim;
        private carinjector thecarinjector;
        private int carinjectornr;

        public carinjectorproperties(int acarinjectornr, simulation asim)
        {
            InitializeComponent();

            carinjectornr = acarinjectornr;
            thesim = asim;
            thecarinjector = thesim.carinjectors[carinjectornr];

            textBox1.Text = thecarinjector.injectperiod.ToString("N");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            thecarinjector.injectperiod = Convert.ToDouble(textBox1.Text);
            Hide();
        }
    }
}
