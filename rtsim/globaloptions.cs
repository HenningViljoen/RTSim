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
    public partial class globaloptions : Form
    {
        public globaloptions()
        {
            InitializeComponent();

            numericUpDown1.Value = Convert.ToDecimal(global.GScale);
        }

        private void button1_Click(object sender, EventArgs e) //OK button
        {
            global.GScale = Convert.ToDouble(numericUpDown1.Value);
            Hide();
        }
    }
}
