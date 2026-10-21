using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Testplan {
    internal static class WorkspaceStyle {
        internal static Color Surface = Color.White, Chrome = Color.FromArgb(247,249,253), Ink = Color.FromArgb(38,50,69), Muted = Color.FromArgb(89,107,130), Accent = Color.FromArgb(50,105,235);
        internal static void SetTheme(int theme) {
            Chrome = theme == 2 ? Color.FromArgb(250,248,243) : theme == 1 ? Color.FromArgb(247,248,250) : Color.FromArgb(247,249,253);
            Accent = theme == 2 ? Color.FromArgb(147,103,39) : theme == 1 ? Color.FromArgb(75,93,120) : Color.FromArgb(50,105,235);
            Surface = theme == 2 ? Color.FromArgb(255,254,251) : Color.White;
        }
        internal static void Apply(System.Windows.Forms.Control root) {
            root.BackColor = root is TextBoxBase || root is NumericUpDown || root is ComboBox ? Surface : Chrome;
            if (!object.Equals(root.Tag,"connection-status")) root.ForeColor = Ink;
            foreach (System.Windows.Forms.Control child in root.Controls) {
                if (child is PictureBox || child is System.Windows.Forms.DataVisualization.Charting.Chart) continue;
                Apply(child);
                var button = child as Button; if (button != null) { button.FlatStyle=FlatStyle.Flat;button.FlatAppearance.BorderColor=Color.FromArgb(220,228,239); }
            }
        }
        internal static TableLayoutPanel Grid(int columns) {
            var grid = new TableLayoutPanel { Dock=DockStyle.Fill, ColumnCount=columns, RowCount=1, Margin=Padding.Empty, Padding=new Padding(6), AutoSize=false };
            for(int i=0;i<columns;i++)grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100f/columns));
            return grid;
        }
        internal static System.Windows.Forms.Control Field(string text,System.Windows.Forms.Control input) {
            var box=new TableLayoutPanel { Dock=DockStyle.Fill,RowCount=2,ColumnCount=1,Margin=new Padding(3),Padding=Padding.Empty };
            box.RowStyles.Add(new RowStyle(SizeType.Absolute,19));box.RowStyles.Add(new RowStyle(SizeType.Percent,100));
            box.Controls.Add(new Label {Text=text,Dock=DockStyle.Fill,AutoEllipsis=true,ForeColor=Muted,TextAlign=ContentAlignment.MiddleLeft},0,0);
            input.Dock=DockStyle.Fill;input.Margin=Padding.Empty;box.Controls.Add(input,0,1);return box;
        }
        internal static FlowLayoutPanel Flow(params System.Windows.Forms.Control[] controls) {
            var panel=new FlowLayoutPanel {Dock=DockStyle.Fill,AutoSize=false,WrapContents=false,Padding=new Padding(2),Margin=Padding.Empty,AutoScroll=true};
            foreach(var c in controls){c.Margin=new Padding(2);panel.Controls.Add(c);}return panel;
        }
        internal static Button Action(string text,string icon,EventHandler click,object original) {
            var button=new VectorButton(icon) {Text=text,AutoSize=true,Height=30,MinimumSize=new Size(28,30),Padding=new Padding(3),AccessibleName=text};
            button.Click+=(s,e)=>click(original??button,e);return button;
        }
        internal static void Detach(params System.Windows.Forms.Control[] controls) {foreach(var c in controls)if(c.Parent!=null)c.Parent.Controls.Remove(c);}
        internal static Label State(string text) {return new Label {Text=text,AutoSize=false,AutoEllipsis=true,Height=26,Dock=DockStyle.Fill,ForeColor=Muted,Tag="connection-status",TextAlign=ContentAlignment.MiddleLeft,Padding=new Padding(8,0,0,0)};}
    }
    // Vector-drawn actions stay sharp at the current WinForms DPI; test-pattern images remain original resources.
    internal sealed class VectorButton : Button {
        private readonly string icon;
        internal VectorButton(string value) {icon=value;FlatStyle=FlatStyle.Flat;FlatAppearance.BorderColor=Color.FromArgb(220,228,239);TextAlign=ContentAlignment.MiddleRight;}
        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.Clear(Enabled?BackColor:WorkspaceStyle.Chrome);
            using(var border=new Pen(Color.FromArgb(220,228,239)))e.Graphics.DrawRectangle(border,0,0,Math.Max(0,Width-1),Math.Max(0,Height-1));
            if(Focused)ControlPaint.DrawFocusRectangle(e.Graphics,Rectangle.Inflate(ClientRectangle,-3,-3));
            int size=Math.Max(12,(int)(14*DeviceScale));int x=5,y=(Height-size)/2;
            e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;
            using(var p=new Pen(Enabled?WorkspaceStyle.Accent:SystemColors.GrayText,Math.Max(1.3f,(float)DeviceScale))) {
                var g=e.Graphics;float u=size/20f;var old=g.Save();g.TranslateTransform(x,y);g.ScaleTransform(u,u);
                switch(icon){
                    case "run":g.DrawPolygon(p,new[]{new Point(5,3),new Point(17,10),new Point(5,17)});break;
                    case "step":g.DrawPolygon(p,new[]{new Point(3,3),new Point(13,10),new Point(3,17)});g.DrawLine(p,17,3,17,17);break;
                    case "up":g.DrawLines(p,new[]{new Point(4,10),new Point(10,4),new Point(16,10)});g.DrawLine(p,10,4,10,18);break;
                    case "down":g.DrawLines(p,new[]{new Point(4,10),new Point(10,16),new Point(16,10)});g.DrawLine(p,10,2,10,16);break;
                    case "delete":g.DrawRectangle(p,5,6,10,12);g.DrawLine(p,3,4,17,4);g.DrawLine(p,8,2,12,2);break;
                    case "format":g.DrawRectangle(p,3,2,14,6);g.DrawLines(p,new[]{new Point(7,8),new Point(7,12),new Point(12,12),new Point(12,18)});break;
                    case "download":g.DrawLine(p,10,2,10,13);g.DrawLines(p,new[]{new Point(5,8),new Point(10,13),new Point(15,8)});g.DrawLines(p,new[]{new Point(3,14),new Point(3,18),new Point(17,18),new Point(17,14)});break;
                    case "export":g.DrawLine(p,10,17,10,3);g.DrawLines(p,new[]{new Point(5,8),new Point(10,3),new Point(15,8)});g.DrawLine(p,3,18,17,18);break;
                    case "comment":g.DrawRectangle(p,2,3,16,11);g.DrawLines(p,new[]{new Point(5,14),new Point(5,18),new Point(10,14)});break;
                    default:g.DrawRectangle(p,3,4,14,13);g.DrawLine(p,6,9,14,9);g.DrawLine(p,6,12,14,12);break;
                }g.Restore(old);
            }
            TextRenderer.DrawText(e.Graphics,Text,Font,new Rectangle(x+size+5,0,Math.Max(0,Width-x-size-9),Height),Enabled?ForeColor:SystemColors.GrayText,TextFormatFlags.VerticalCenter|TextFormatFlags.Left|TextFormatFlags.EndEllipsis);
        }
        private double DeviceScale {get {using(var g=CreateGraphics())return g.DpiX/96.0;}}
        public override Size GetPreferredSize(Size proposedSize) {var text=TextRenderer.MeasureText(Text,Font);return new Size(text.Width+38,Math.Max(30,text.Height+12));}
    }
}
