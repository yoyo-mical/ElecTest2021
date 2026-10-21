using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Testplan {
    public partial class MainForm {
        private Label licenseState;
        private TextBox licensePath;
        private Button refreshLicense;
        private CheckBox saveLicensePath;
        private readonly Timer authorizationTimer=new Timer {Interval=60000};
        private DateTime nextLicenseRefresh=DateTime.MinValue;
        private bool refreshing,workspaceBuilt,closing;
        private Control recipePanel;
        private FlowLayoutPanel licenseBar;
        private Action disconnectDevices;
        private int currentDpi=96;
        private void InitializeWorkspace() {
            SuspendLayout();Font=new Font("Segoe UI",9f);AutoScaleMode=AutoScaleMode.Dpi;AutoScaleDimensions=new SizeF(96,96);
            FormBorderStyle=FormBorderStyle.Sizable;Text="ElecTest2026";MinimumSize=new Size(900,650);StartPosition=FormStartPosition.CenterScreen;
            var work=Screen.FromControl(this).WorkingArea;Size=new Size(Math.Min(1600,work.Width),Math.Min(980,work.Height));MaximumSize=Size.Empty;
            Title.Controls.Clear();Title.Height=52;Title.Padding=new Padding(12,4,12,4);
            var theme=new ComboBox {DropDownStyle=ComboBoxStyle.DropDownList,Width=105,Height=28};theme.Items.AddRange(new object[]{"清晰蓝","中性灰","柔和暖白"});theme.SelectedIndex=0;
            var identity=new Label{Text="ElecTest2026",AutoSize=true,Font=new Font("Segoe UI",15f,FontStyle.Bold),Margin=new Padding(4,5,8,0)};
            var credit=new Label{Text="Design by Wuhao Studio",AutoSize=true,ForeColor=WorkspaceStyle.Muted,Margin=new Padding(3,11,12,0)};
            var heading=WorkspaceStyle.Flow(identity,credit,theme);heading.Dock=DockStyle.Fill;Title.Controls.Add(heading);
            IsMdiContainer=false;
            licensePath=new TextBox {Width=330,AccessibleName="ElecTest2026 授权名单路径"};refreshLicense=new Button{Text="检查授权",AutoSize=true};saveLicensePath=new CheckBox{Text="验证后保存路径",AutoSize=true,Checked=false};
            licenseState=WorkspaceStyle.State("尚未授权 · 功能已锁定");licenseState.Dock=DockStyle.None;licenseState.Width=520;licenseState.Height=26;
            var disconnect=new Button{Text="断开设备",AutoSize=true};disconnect.Click+=(s,e)=>{try{if(disconnectDevices!=null)disconnectDevices();}catch(Exception ex){MessageBox.Show(ex.Message,"设备断开失败");}};
            licenseBar=WorkspaceStyle.Flow(licensePath,refreshLicense,saveLicensePath,disconnect,licenseState);licenseBar.Dock=DockStyle.Bottom;licenseBar.Height=70;licenseBar.WrapContents=true;licenseBar.AutoScroll=true;
            Controls.Add(licenseBar);licenseBar.BringToFront();dockPanel1.Enabled=false;dockPanel1.DocumentStyle=DocumentStyle.DockingWindow;
            dockPanel1.DockBackColor=WorkspaceStyle.Chrome;dockPanel1.Theme=vS2015LightTheme1;
            try {licensePath.Text=AppLicense.ReadSettings().licensePath;}catch(Exception ex){licenseState.Text="配置读取失败："+ex.Message;}
            refreshLicense.Click+=async(s,e)=>await RefreshAuthorization();
            theme.SelectedIndexChanged+=(s,e)=>{WorkspaceStyle.SetTheme(theme.SelectedIndex);WorkspaceStyle.Apply(this);dockPanel1.DockBackColor=WorkspaceStyle.Chrome;};
            AppLicense.Locked+=AuthorizationLost;
            Shown+=async(s,e)=>{using(var g=CreateGraphics())currentDpi=(int)g.DpiX;await RefreshAuthorization();authorizationTimer.Start();};
            authorizationTimer.Tick+=async(s,e)=>{if(refreshing||closing)return;try{AppLicense.Ensure();}catch{return;}if(DateTime.UtcNow>=nextLicenseRefresh)await RefreshAuthorization();};
            FormClosed+=(s,e)=>{closing=true;authorizationTimer.Stop();authorizationTimer.Dispose();AppLicense.Locked-=AuthorizationLost;AppLicense.Dispose();try{if(UART.sp.IsOpen)UART.sp.Close();}catch{}};
            WorkspaceStyle.Apply(this);ResumeLayout(true);
        }
        private async Task RefreshAuthorization() {
            if(refreshing||closing)return;
            refreshing=true;refreshLicense.Enabled=false;licenseState.Text="正在检查授权…";
            try {
                await AppLicense.RefreshAsync(licensePath.Text);
                if(closing)return;
                if(saveLicensePath.Checked)AppLicense.SavePath(licensePath.Text);
                if(!workspaceBuilt)BuildAuthorizedWorkspace();
                AppLicense.Ensure();dockPanel1.Enabled=true;
                licenseState.Text="已授权 · "+AppLicense.Account+" · "+AppLicense.Edition;licenseState.ForeColor=Color.FromArgb(20,116,95);nextLicenseRefresh=DateTime.UtcNow.AddHours(1);
            }catch(Exception ex){AppLicense.Lock(ex.Message);}
            finally{refreshing=false;if(!closing)refreshLicense.Enabled=true;}
        }
        private void AuthorizationLost(string reason) {
            if(closing||IsDisposed)return;
            if(InvokeRequired){BeginInvoke(new Action<string>(AuthorizationLost),reason);return;}
            dockPanel1.Enabled=false;if(recipePanel!=null)recipePanel.StopForAuthorization();
            licenseState.Text="已锁定 · "+reason;licenseState.ForeColor=Color.Firebrick;
        }
        private void BuildAuthorizedWorkspace() {
            AppLicense.Ensure();
            var uart=new UART{TabText="串口通信 · UART"};uart.Show(dockPanel1,DockState.DockLeft);dockPanel1.DockLeftPortion=.19;
            var ca=new CA410{TabText="色彩分析仪 · CA410"};ca.Show(uart.Pane,DockAlignment.Bottom,.43);
            var code=new ARMCode{TabText="脚本编辑 · ARMCode"};code.Show(dockPanel1,DockState.Document);
            recipePanel=new Control{TabText="配方与运行控制 · Control"};recipePanel.Show(code.Pane,DockAlignment.Bottom,.54);
            var chart=new DataChart{TabText="测量曲线 · DataChart"};chart.Show(code.Pane,DockAlignment.Right,.48);
            var pattern=new Pattern{TabText="测试图案 · Pattern"};pattern.Show(dockPanel1,DockState.DockRight);dockPanel1.DockRightPortion=.21;
            disconnectDevices=()=>{if(UART.sp.IsOpen)UART.sp.Close();uart.RefreshDeviceState();ca.DisconnectFromWorkspace();};
            foreach(IDockContent content in dockPanel1.Contents){content.DockHandler.CloseButton=false;content.DockHandler.CloseButtonVisible=false;}
            workspaceBuilt=true;WorkspaceStyle.Apply(this);
        }
        protected override void WndProc(ref Message m) {
            const int WM_DPICHANGED=0x02E0;
            if(m.Msg==WM_DPICHANGED&&currentDpi>0){int dpi=m.WParam.ToInt32()&0xffff;if(dpi>0&&dpi!=currentDpi){float factor=(float)dpi/currentDpi;currentDpi=dpi;Scale(new SizeF(factor,factor));var bounds=(DpiRect)Marshal.PtrToStructure(m.LParam,typeof(DpiRect));Bounds=Rectangle.FromLTRB(bounds.Left,bounds.Top,bounds.Right,bounds.Bottom);var work=Screen.FromRectangle(Bounds).WorkingArea;MinimumSize=new Size(Math.Min(MinimumSize.Width,work.Width),Math.Min(MinimumSize.Height,work.Height));}}
            base.WndProc(ref m);
        }
        [StructLayout(LayoutKind.Sequential)]private struct DpiRect{public int Left,Top,Right,Bottom;}
    }
}
