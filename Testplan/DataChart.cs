using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms.DataVisualization.Charting;

namespace Testplan
{
    public partial class DataChart : DockContent
    {
        public static FormShare fs = new FormShare();
        public DataChart()
        {
            InitializeComponent();
            fs.intevent+= new intdelegate(seriesAdd);
            fs.intstrstrevent += new intstrstrdelegate(chartAdd);
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void seriesAdd(int i)
        {
            chart1.Series.Clear();
            chart1.Series.Add(i.ToString());
            Series series = chart1.Series[i];
            series.ChartType = SeriesChartType.Spline;
            series.BorderWidth = 2;
            series.Color = Color.Black;
            series.LegendText = "";
        }

        private void chartAdd(int i,string x,string y)
        {
            chart1.Series[i].Points.AddXY(x, y);
        }

        private void Timer_Tick(object sender, EventArgs e)

        {

            //this.chartAdd();

        }
        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
