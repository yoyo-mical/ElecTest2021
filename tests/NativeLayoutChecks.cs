// NON-SHIPPING layout harness. No valid license, no send/measure actions, no fake live readings.
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
class NativeLayoutChecks {
    static object Field(object target,string name){return target.GetType().GetField(name,BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance).GetValue(target);}
    static void Check(bool condition,string label){if(!condition)throw new Exception(label);Console.WriteLine("PASS "+label);}
    [STAThread]static void Main(string[] args){
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);PatchController.EnableAll=true;
        if(args.Length>1&&args[1]=="startup"){
            var main=new Testplan.MainForm();var startupTimer=new Timer{Interval=1500};startupTimer.Tick+=(s,e)=>{startupTimer.Stop();var startupDock=(DockPanel)Field(main,"dockPanel1");Check(!startupDock.Enabled&&startupDock.Contents.Count==0,"startup fails closed before creating hardware workspace");Capture(main,args[0]);main.Close();};startupTimer.Start();Application.Run(main);return;
        }
        var form=new Form{Text="ElecTest2026 · 原生布局验证（无授权/无硬件；图表为 Mono 占位）",StartPosition=FormStartPosition.Manual,Bounds=new Rectangle(0,0,1600,980)};
        var dock=new DockPanel{Dock=DockStyle.Fill,DocumentStyle=DocumentStyle.DockingWindow,Theme=new VS2015LightTheme()};form.Controls.Add(dock);
        var uart=new Testplan.UART{TabText="串口通信 · UART"};uart.Show(dock,DockState.DockLeft);dock.DockLeftPortion=.19;
        var ca=new Testplan.CA410{TabText="色彩分析仪 · CA410"};ca.Show(uart.Pane,DockAlignment.Bottom,.43);
        var code=new Testplan.ARMCode{TabText="脚本编辑 · ARMCode"};code.Show(dock,DockState.Document);
        var recipe=new Testplan.Control{TabText="配方与运行控制 · Control"};recipe.Show(code.Pane,DockAlignment.Bottom,.54);
        var chart=new DockContent{TabText="DataChart · Mono 占位"};chart.Controls.Add(new Label{Text="Mono Chart 控件不支持完整渲染\n此区域是明确标注的占位\n生产版继续使用原 Chart1 数据",Dock=DockStyle.Fill,TextAlign=ContentAlignment.MiddleCenter,BackColor=Color.LightYellow});chart.Show(code.Pane,DockAlignment.Right,.48);
        var pattern=new Testplan.Pattern{TabText="测试图案 · Pattern"};pattern.Show(dock,DockState.DockRight);dock.DockRightPortion=.21;
        var timer=new Timer{Interval=1200};timer.Tick+=(s,e)=>{timer.Stop();foreach(System.Windows.Forms.Control child in dock.Controls)if(child.Visible&&child.GetType().Name.Contains("DockWindow"))child.BringToFront();
            foreach(IDockContent content in dock.Contents){var panel=(Form)content;panel.Show();Console.WriteLine(panel.GetType().Name+" visible="+panel.Visible+" bounds="+panel.Bounds+" pane="+content.DockHandler.Pane.Bounds);}
            Check(dock.Contents.Count==6,"six dock panels");
            var a=(NumericUpDown)Field(code,"numericUpDown1");var z=(NumericUpDown)Field(code,"numericUpDown6");Check(a.PointToScreen(Point.Empty).Y==z.PointToScreen(Point.Empty).Y,"six ARM parameters share one row");Check(((RichTextBox)Field(code,"richTextBox1")).Height>100,"ARM editor gets usable height");
            var library=(FlowLayoutPanel)Field(pattern,"patternLibrary");Check(library.Controls.OfType<PictureBox>().Count()==32,"all 32 original Pattern images preserved");Check(!((Panel)Field(pattern,"previewRegion")).Visible,"Pattern preview collapsed by default");
            Check(((RichTextBox)Field(recipe,"richTextBox1")).Visible,"original rich-text recipe remains visible");Application.DoEvents();form.Refresh();Application.DoEvents();Capture(form,args[0]);form.Close();};timer.Start();Application.Run(form);
    }
    static void Capture(Form form,string path){using(var bmp=new Bitmap(form.Width,form.Height)){using(var g=Graphics.FromImage(bmp))g.CopyFromScreen(form.Location,Point.Empty,bmp.Size);bmp.Save(path,ImageFormat.Png);}}
}
