using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Testplan {
    public partial class DataChart {
        internal static string CurrentTest="";
        private Label pointReading;
        private ContextMenuStrip chartOptions;
        private string curveMode="折线 + 实测点";
        private void ApplyWorkspaceLayout() {
            Font=new Font("Segoe UI",9f);AutoScaleMode=AutoScaleMode.Dpi;
            tabPage1.Text="Chart1 · 当前测试";tabPage2.Text="Chart2";
            // No historical comparison is implemented without confirmation or comparable source data.
            var empty=WorkspaceStyle.State("Chart2 尚未定义用途");empty.Dock=DockStyle.Top;tabPage2.Controls.Add(empty);
            pointReading=new Label{Visible=false,AutoSize=true,Padding=new Padding(7),BackColor=Color.White,ForeColor=WorkspaceStyle.Ink,BorderStyle=BorderStyle.FixedSingle};
            tabPage1.Controls.Add(pointReading);pointReading.BringToFront();
            chartOptions=new ContextMenuStrip();
            foreach(var mode in new[]{"折线 + 实测点","仅实测点","平滑线（插值）"}){var item=chartOptions.Items.Add(mode);var selectedMode=mode;item.Click+=(s,e)=>{curveMode=selectedMode;foreach(Series series in chart1.Series)StyleSeries(series);};}
            chartOptions.Items.Add(new ToolStripSeparator());chartOptions.Items.Add("恢复缩放",null,(s,e)=>{foreach(ChartArea area in chart1.ChartAreas){area.AxisX.ScaleView.ZoomReset(0);area.AxisY.ScaleView.ZoomReset(0);}});
            chart1.ContextMenuStrip=chartOptions;
            var more=new Button{Text="⋯",Width=30,Height=24,Anchor=AnchorStyles.Top|AnchorStyles.Right,AccessibleName="图表设置"};
            more.Click+=(s,e)=>chartOptions.Show(more,new Point(0,more.Height));tabPage1.Controls.Add(more);more.BringToFront();tabPage1.Resize+=(s,e)=>more.Location=new Point(Math.Max(0,tabPage1.ClientSize.Width-more.Width-5),4);
            foreach(Legend legend in chart1.Legends)legend.Enabled=false;
            chart1.BackColor=Color.White;chart1.BorderlineColor=Color.FromArgb(220,228,239);
            foreach(ChartArea area in chart1.ChartAreas){area.BackColor=Color.White;area.AxisX.MajorGrid.LineColor=Color.FromArgb(232,237,245);area.AxisY.MajorGrid.LineColor=Color.FromArgb(232,237,245);area.AxisX.LabelStyle.ForeColor=WorkspaceStyle.Muted;area.AxisY.LabelStyle.ForeColor=WorkspaceStyle.Muted;area.CursorX.IsUserEnabled=true;area.CursorX.IsUserSelectionEnabled=true;area.AxisX.ScaleView.Zoomable=true;}
            chart1.MouseClick+=(s,e)=>{var hit=chart1.HitTest(e.X,e.Y);if(hit.ChartElementType!=ChartElementType.DataPoint||hit.Series==null||hit.PointIndex<0){pointReading.Visible=false;return;}var point=hit.Series.Points[hit.PointIndex];pointReading.Text="实测点 · "+(string.IsNullOrEmpty(point.AxisLabel)?point.XValue.ToString():point.AxisLabel)+" / "+point.YValues[0]+(curveMode.StartsWith("平滑")?" · 连线为插值":"");pointReading.Location=new Point(8,8);pointReading.Visible=true;pointReading.BringToFront();};
            WorkspaceStyle.Apply(this);
        }
        private void StyleSeries(Series series) {
            series.ChartType=curveMode=="仅实测点"?SeriesChartType.Point:curveMode.StartsWith("平滑")?SeriesChartType.Spline:SeriesChartType.Line;
            series.Color=WorkspaceStyle.Accent;series.BorderWidth=2;series.MarkerStyle=MarkerStyle.Circle;series.MarkerSize=6;
        }
        private void HighlightLatest(Series series) {
            if(CurrentTest=="VCOM"&&series.Points.Count>0){foreach(DataPoint point in series.Points)point.ToolTip="实测点";var minimum=series.Points.Cast<DataPoint>().OrderBy(p=>p.YValues[0]).First();minimum.ToolTip="已测最低点（不是自动写入建议）";}
            if(series.Points.Count>1){var previous=series.Points[series.Points.Count-2];previous.MarkerSize=6;previous.MarkerColor=WorkspaceStyle.Accent;}
            if(series.Points.Count>0){var last=series.Points[series.Points.Count-1];last.MarkerSize=9;last.MarkerColor=Color.FromArgb(20,116,95);}
        }
    }
}
