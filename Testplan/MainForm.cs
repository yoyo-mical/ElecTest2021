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
    public partial class MainForm : Form
    {

        //AutoSizeFormClass asc = new AutoSizeFormClass();
       
        public MainForm()
        {
            InitializeComponent();

            InitializeWorkspace();

        }
        public static PictureBox temp = new PictureBox();
        public static void ImageSwitch(object sender, int n, int ns)
        {
            temp = (PictureBox)sender;
            switch (n)
            {
                case 0:
                    {
                        temp.Image = null;
                        temp.BackgroundImage = null;
                        if (ns == 0)
                        temp.Image = Properties.Resources.max2;
                        temp.SizeMode = PictureBoxSizeMode.StretchImage;
                        if (ns == 1)
                        temp.Image = Properties.Resources.max1;
                        temp.SizeMode = PictureBoxSizeMode.StretchImage;
                        break;
                    }
                case 1:
                    {
                        temp.Image = null;
                        temp.BackgroundImage = null;
                        if (ns == 0)
                            temp.Image = Properties.Resources.min2;
                            temp.SizeMode = PictureBoxSizeMode.StretchImage;
                        if (ns == 1)
                            temp.Image = Properties.Resources.min1;
                            temp.SizeMode = PictureBoxSizeMode.StretchImage;
                        break;
                    }
                case 2:
                    {
                        temp.Image = null;
                        temp.BackgroundImage = null;
                        if (ns == 0)
                           temp.Image = Properties.Resources.close2;
                           temp.SizeMode = PictureBoxSizeMode.StretchImage;
                        if (ns == 1)
                            temp.Image = Properties.Resources.close1;
                            temp.SizeMode = PictureBoxSizeMode.StretchImage;
                        break;
                    }
                case 3:
                    {
                        temp.Image = null;
                        temp.BackgroundImage = null;
                        if (ns == 0)
                            temp.Image = Properties.Resources.start;
                        temp.SizeMode = PictureBoxSizeMode.StretchImage;
                        if (ns == 1)
                          temp.Image = Properties.Resources.stop;
                          temp.SizeMode = PictureBoxSizeMode.StretchImage;
                        break;
                    }
                case 4:
                    {
                        temp.Image = null;
                        temp.BackgroundImage = null;
                        if (ns == 0)
                            temp.Image = Properties.Resources.开始运行;
                        temp.SizeMode = PictureBoxSizeMode.StretchImage;
                        if (ns == 1)
                            temp.Image = Properties.Resources.暂停;
                        temp.SizeMode = PictureBoxSizeMode.StretchImage;
                        break;
                    }
                case 5:
                    {
                        temp.Image = null;
                        temp.BackgroundImage = null;
                        if (ns == 0)
                            temp.Image = Properties.Resources.单步运行;
                        temp.SizeMode = PictureBoxSizeMode.StretchImage;
                        if (ns == 1)
                            temp.Image = Properties.Resources.danbutingzhi;
                        temp.SizeMode = PictureBoxSizeMode.StretchImage;
                        break;
                    }
            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //asc.controllInitializeSize(this);
            //UART uart = new UART();
            //uart.Show();
            //uart.MdiParent = this;

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            //this.Close();
            Application.Exit();
            //System.Environment.Exit(0);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            this.WindowState = FormWindowState.Maximized;
        }
        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            //asc.controlAutoSize(this);
            //this.WindowState = (System.Windows.Forms.FormWindowState)(2);
            //记录完控件的初始位置和大小后，再最大化
        }

        private void dockPanel1_ActiveContentChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            ImageSwitch(sender, Convert.ToInt16(((PictureBox)sender).Tag.ToString()), 0);
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            
            ImageSwitch(sender, Convert.ToInt16(((PictureBox)sender).Tag.ToString()), 1);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Title_DoubleClick(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        private void Title_MouseDown(object sender, MouseEventArgs e)
        {
            Win32.ReleaseCapture();
            Win32.SendMessage(this.Handle, Win32.WM_SYSCOMMAND, Win32.SC_MOVE + Win32.HTCAPTION, 0);
        }

        private void dockPanel1_ActiveContentChanged_1(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
