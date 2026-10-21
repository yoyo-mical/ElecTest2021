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
            ApplyWorkspaceLayout();
            fs.intevent+= new intdelegate(seriesAdd);
            fs.intstrstrevent += new intstrstrdelegate(chartAdd);
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void seriesAdd(int i)
        {
            if (!AppLicense.AllowOperation()) return;
            chart1.Series.Clear();
            chart1.Series.Add(i.ToString());
            Series series = chart1.Series[i];
            StyleSeries(series);
            series.BorderWidth = 2;
            series.Color = WorkspaceStyle.Accent;
            series.LegendText = "";
            foreach(ChartArea area in chart1.ChartAreas){area.AxisX.Title=CurrentTest=="Gamma"?"采样序号":"参数码值（HEX 标签）";area.AxisY.Title=CurrentTest=="VCOM"?(CA410.FMAcheck=="true"?"FMA · %":"JEITA · dB"):"Lv · cd/m²";}
        }

        private void chartAdd(int i,string x,string y)
        {
            if (!AppLicense.AllowOperation()) return;
            chart1.Series[i].Points.AddXY(x, y);
            HighlightLatest(chart1.Series[i]);
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
