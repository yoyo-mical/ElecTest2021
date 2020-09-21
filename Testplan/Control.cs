using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Testplan
{
    public partial class Control : DockContent
    {
        public Control()
        {
            InitializeComponent();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void Control_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Delay");
            comboBox1.Items.Add("Pattern");
            comboBox1.Items.Add("Gamma");
            comboBox1.Items.Add("VCOM");
            comboBox1.Items.Add("VGH");
            comboBox1.Items.Add("VGL");
            comboBox1.Items.Add("CKH");
            comboBox1.Items.Add("Crosstalk");
            comboBox1.Items.Add("Flicker");
        }
    }
}
