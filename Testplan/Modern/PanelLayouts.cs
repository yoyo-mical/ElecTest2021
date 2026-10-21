using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using UiControl = System.Windows.Forms.Control;

namespace Testplan {
    public partial class ARMCode {
        private void ApplyWorkspaceLayout() {
            SuspendLayout();Font=new Font("Segoe UI",9f);AutoScaleMode=AutoScaleMode.Dpi;
            tabControl1.Dock=DockStyle.Fill;
            var tools=WorkspaceStyle.Flow(
                WorkspaceStyle.Action("格式化","format",pictureBox1_Click,pictureBox1),
                WorkspaceStyle.Action("注释","comment",pictureBox5_Click,pictureBox5),
                WorkspaceStyle.Action("下载","download",pictureBox2_Click,pictureBox2),
                WorkspaceStyle.Action("删除","delete",pictureBox4_Click,pictureBox4));
            tools.Dock=DockStyle.Top;tools.Height=40;
            var parameters=WorkspaceStyle.Grid(6);parameters.Dock=DockStyle.Bottom;parameters.Height=62;
            var inputs=new NumericUpDown[]{numericUpDown1,numericUpDown2,numericUpDown3,numericUpDown4,numericUpDown5,numericUpDown6};
            var names=new[]{"VGH","VGL","VCOM","CKV","CKH","EQ"};
            for(int i=0;i<6;i++)parameters.Controls.Add(WorkspaceStyle.Field(names[i],inputs[i]),i,0);
            tabPage1.Controls.Clear();richTextBox1.Dock=DockStyle.Fill;richTextBox1.Font=new Font("Consolas",10f);richTextBox1.WordWrap=false;richTextBox1.BorderStyle=BorderStyle.None;
            tabPage1.Controls.Add(richTextBox1);tabPage1.Controls.Add(tools);tabPage1.Controls.Add(parameters);
            panel2.Controls.Clear();panel2.Height=40;panel2.Controls.Add(WorkspaceStyle.Flow(
                WorkspaceStyle.Action("导出数据","export",pictureBox6_Click,pictureBox6),
                WorkspaceStyle.Action("清空数据","delete",pictureBox3_Click,pictureBox3)));
            dataGridView2.Dock=DockStyle.Fill;dataGridView2.BackgroundColor=Color.White;dataGridView2.BorderStyle=BorderStyle.None;
            WorkspaceStyle.Apply(this);ResumeLayout(true);
        }
    }
    public partial class UART {
        private Label connectionState;
        private void ApplyWorkspaceLayout() {
            SuspendLayout();Font=new Font("Segoe UI",9f);AutoScaleMode=AutoScaleMode.Dpi;
            var root=new TableLayoutPanel {Dock=DockStyle.Fill,ColumnCount=1,RowCount=5,Padding=new Padding(6)};
            root.RowStyles.Add(new RowStyle(SizeType.Absolute,26));root.RowStyles.Add(new RowStyle(SizeType.Absolute,118));root.RowStyles.Add(new RowStyle(SizeType.Absolute,40));root.RowStyles.Add(new RowStyle(SizeType.Percent,100));root.RowStyles.Add(new RowStyle(SizeType.Absolute,130));
            connectionState=WorkspaceStyle.State("串口未连接");root.Controls.Add(connectionState,0,0);
            var settings=WorkspaceStyle.Grid(3);settings.RowCount=2;settings.RowStyles.Add(new RowStyle(SizeType.Percent,50));settings.RowStyles.Add(new RowStyle(SizeType.Percent,50));
            var values=new UiControl[]{cbxComPort,cbxBaudRate,cbxDataBits,cbxStopBits,cbxParity};var titles=new[]{"端口","波特率","数据位","停止位","校验位"};
            for(int i=0;i<values.Length;i++)settings.Controls.Add(WorkspaceStyle.Field(titles[i],values[i]),i%3,i/3);root.Controls.Add(settings,0,1);
            btnOpenCom.AutoSize=true;btnCheckCom.AutoSize=true;root.Controls.Add(WorkspaceStyle.Flow(btnOpenCom,btnCheckCom),0,2);
            tbxRecvData.Dock=DockStyle.Fill;tbxRecvData.Font=new Font("Consolas",9f);root.Controls.Add(tbxRecvData,0,3);
            var send=new TableLayoutPanel {Dock=DockStyle.Fill,ColumnCount=1,RowCount=3};send.RowStyles.Add(new RowStyle(SizeType.Absolute,28));send.RowStyles.Add(new RowStyle(SizeType.Percent,100));send.RowStyles.Add(new RowStyle(SizeType.Absolute,36));
            rbnSendHex.AutoSize=true;rbnSendStr.AutoSize=true;send.Controls.Add(WorkspaceStyle.Flow(rbnSendHex,rbnSendStr),0,0);send.Controls.Add(WorkspaceStyle.Flow(btnSend,btnCleanData),0,2);tbxSendData.Dock=DockStyle.Fill;send.Controls.Add(tbxSendData,0,1);root.Controls.Add(send,0,4);
            Controls.Clear();Controls.Add(root);WorkspaceStyle.Apply(this);ResumeLayout(true);
        }
        internal void RefreshDeviceState(){isOpen=sp.IsOpen;btnOpenCom.Text=isOpen?"关闭串口":"打开串口";cbxComPort.Enabled=cbxBaudRate.Enabled=cbxDataBits.Enabled=cbxStopBits.Enabled=cbxParity.Enabled=!isOpen;UpdateConnectionState();}
        private void UpdateConnectionState(string failure=null) {
            if(connectionState==null)return;
            connectionState.Text=failure??(sp.IsOpen?"已连接 · "+sp.PortName:"串口未连接");
            connectionState.ForeColor=failure!=null?Color.Firebrick:sp.IsOpen?Color.FromArgb(20,116,95):WorkspaceStyle.Muted;
        }
    }
    public partial class CA410 {
        private Label connectionState;
        private static string deviceState="仪器未连接";
        private static bool sdkOperationFailed;
        private void ApplyWorkspaceLayout() {
            SuspendLayout();Font=new Font("Segoe UI",9f);AutoScaleMode=AutoScaleMode.Dpi;
            var root=new TableLayoutPanel {Dock=DockStyle.Fill,ColumnCount=1,RowCount=4,Padding=new Padding(6)};
            root.RowStyles.Add(new RowStyle(SizeType.Absolute,26));root.RowStyles.Add(new RowStyle(SizeType.Absolute,36));root.RowStyles.Add(new RowStyle(SizeType.Percent,100));root.RowStyles.Add(new RowStyle(SizeType.Absolute,42));
            connectionState=WorkspaceStyle.State(deviceState);root.Controls.Add(connectionState,0,0);radioButton1.AutoSize=radioButton2.AutoSize=radioButton3.AutoSize=true;
            root.Controls.Add(WorkspaceStyle.Flow(radioButton1,radioButton2,radioButton3),0,1);
            var fields=WorkspaceStyle.Grid(4);fields.RowCount=4;for(int i=0;i<4;i++)fields.RowStyles.Add(new RowStyle(SizeType.Percent,25));
            var boxes=new[]{Ca_Lv,Ca_sx,Ca_sy,Ca_X,Ca_Y,Ca_Z,Flicker,F};var titles=new[]{"Lv · cd/m²","sx","sy","X","Y","Z","JEITA · dB","FMA · %"};
            // Two columns of label/value pairs; preserve all original live text boxes.
            for(int i=0;i<boxes.Length;i++){var label=new Label {Text=titles[i],Dock=DockStyle.Fill,TextAlign=ContentAlignment.MiddleLeft};fields.Controls.Add(label,(i%2)*2,i/2);boxes[i].Dock=DockStyle.Fill;fields.Controls.Add(boxes[i],(i%2)*2+1,i/2);}
            root.Controls.Add(fields,0,2);button1.Text="连接";button4.Text="断开";button2.Text="测量";button1.AutoSize=button4.AutoSize=button2.AutoSize=true;button1.AutoSizeMode=button4.AutoSizeMode=button2.AutoSizeMode=AutoSizeMode.GrowAndShrink;
            root.Controls.Add(WorkspaceStyle.Flow(button1,button4,button2),0,3);Controls.Clear();Controls.Add(root);WorkspaceStyle.Apply(this);ResumeLayout(true);
        }
        internal void DisconnectFromWorkspace(){if(objCa200!=null)Disconnect();ShowDeviceState("仪器未连接",false);}
        private void ShowDeviceState(string text,bool success) {deviceState=text;if(connectionState!=null){connectionState.Text=text;connectionState.ForeColor=success?Color.FromArgb(20,116,95):Color.Firebrick;}}
    }
    public partial class Control {
        private void ApplyWorkspaceLayout() {
            SuspendLayout();Font=new Font("Segoe UI",9f);AutoScaleMode=AutoScaleMode.Dpi;
            var tools=WorkspaceStyle.Flow(
                WorkspaceStyle.Action("导入","download",pictureBox6_Click,pictureBox6),WorkspaceStyle.Action("导出","export",pictureBox7_Click,pictureBox7),
                WorkspaceStyle.Action("编写","step",pictureBox3_Click,pictureBox3),WorkspaceStyle.Action("从头 / 停止","run",pictureBox1_Click_1,pictureBox1),
                WorkspaceStyle.Action("当前行 / 停止","run",pictureBox10_Click,pictureBox10),WorkspaceStyle.Action("单步","step",pictureBox4_Click,pictureBox4),
                WorkspaceStyle.Action("格式刷","format",pictureBox2_Click,pictureBox2),WorkspaceStyle.Action("上移","up",pictureBox8_Click,pictureBox8),
                WorkspaceStyle.Action("下移","down",pictureBox9_Click,pictureBox9),WorkspaceStyle.Action("删除","delete",pictureBox5_Click,pictureBox5));
            tools.Height=42;tools.Dock=DockStyle.Top;
            var settings=WorkspaceStyle.Grid(7);settings.Height=64;settings.Dock=DockStyle.Top;
            var inputs=new UiControl[]{comboBox1,numericUpDown1,numericUpDown2,numericUpDown4,numericUpDown5,numericUpDown3,numericUpDown6};
            var titles=new[]{"ITEM","START · HEX","END · HEX","STEP · HEX","Delay · 原单位","Step Time · 原单位","GoTo 行号"};
            for(int i=0;i<inputs.Length;i++)settings.Controls.Add(WorkspaceStyle.Field(titles[i],inputs[i]),i,0);
            var more=new TableLayoutPanel {Dock=DockStyle.Top,Height=118,ColumnCount=3,RowCount=2,Padding=new Padding(6)};
            more.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,22));more.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,38));more.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,40));
            more.RowStyles.Add(new RowStyle(SizeType.Percent,50));more.RowStyles.Add(new RowStyle(SizeType.Percent,50));
            more.Controls.Add(WorkspaceStyle.Field("Patten · 图案预设",comboBox2),0,0);more.Controls.Add(WorkspaceStyle.Field("Pattern 内容",textBox1),1,0);more.Controls.Add(WorkspaceStyle.Field("DownLoad 内容",textBox3),2,0);
            more.Controls.Add(WorkspaceStyle.Flow(checkBox1,button1),0,1);more.SetColumnSpan(more.GetControlFromPosition(0,1),1);more.Controls.Add(WorkspaceStyle.Field("Read 文件 / Save 目录路径",textBox2),1,1);more.SetColumnSpan(more.GetControlFromPosition(1,1),2);
            var disclosure=new CheckBox {Text="图案参数 / DownLoad / 文件路径 ▸",Appearance=Appearance.Button,Dock=DockStyle.Top,Height=30,TextAlign=ContentAlignment.MiddleLeft};more.Visible=false;
            disclosure.CheckedChanged+=(s,e)=>{more.Visible=disclosure.Checked;disclosure.Text=disclosure.Checked?"图案参数 / DownLoad / 文件路径 ▾":"图案参数 / DownLoad / 文件路径 ▸";};
            Controls.Clear();richTextBox1.Dock=DockStyle.Fill;richTextBox1.Font=new Font("Consolas",10f);richTextBox1.WordWrap=false;richTextBox1.BorderStyle=BorderStyle.None;
            Controls.Add(richTextBox1);Controls.Add(more);Controls.Add(disclosure);Controls.Add(settings);Controls.Add(tools);
            WorkspaceStyle.Apply(this);ResumeLayout(true);
        }
        internal void StopForAuthorization() {ns1=ns2=ns3=1;runStat=0;}
    }
    public partial class Pattern {
        private PictureBox selectedPreview;
        private Panel previewRegion;
        private CheckBox previewDisclosure;
        private FlowLayoutPanel patternLibrary;
        private void ApplyWorkspaceLayout() {
            SuspendLayout();Font=new Font("Segoe UI",9f);AutoScaleMode=AutoScaleMode.Dpi;
            var images=panel2.Controls.OfType<PictureBox>().OrderBy(x=>int.Parse(x.Name.Replace("pictureBox",""))).ToArray();
            var top=WorkspaceStyle.Grid(2);top.Dock=DockStyle.Top;top.Height=62;top.Controls.Add(WorkspaceStyle.Field("宽度",textBox1),0,0);top.Controls.Add(WorkspaceStyle.Field("高度",textBox2),1,0);
            var rgb=WorkspaceStyle.Grid(4);rgb.Height=62;rgb.Dock=DockStyle.Top;
            var nums=new[]{numericUpDown1,numericUpDown2,numericUpDown3,numericUpDown5};var labels=new[]{label3,label4,label5,label7};
            for(int i=0;i<4;i++){var box=new TableLayoutPanel{Dock=DockStyle.Fill,RowCount=2,ColumnCount=1};box.RowStyles.Add(new RowStyle(SizeType.Absolute,20));labels[i].Dock=DockStyle.Fill;labels[i].TextAlign=ContentAlignment.MiddleLeft;box.Controls.Add(labels[i],0,0);nums[i].Dock=DockStyle.Fill;box.Controls.Add(nums[i],0,1);rgb.Controls.Add(box,i,0);}
            // Existing RGB and Gray label Click handlers retain their original send semantics.
            var bottom=WorkspaceStyle.Grid(1);bottom.Height=60;bottom.Dock=DockStyle.Bottom;
            var back=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=2};back.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50));back.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50));label6.Text="Back · 数值";label6.Dock=DockStyle.Fill;back.Controls.Add(label6,0,0);numericUpDown4.Dock=DockStyle.Fill;back.Controls.Add(numericUpDown4,1,0);bottom.Controls.Add(back);
            selectedPreview=new PictureBox {Dock=DockStyle.Fill,BackgroundImageLayout=ImageLayout.Zoom,BackColor=Color.White};previewRegion=new Panel{Dock=DockStyle.Bottom,Height=135,Visible=false,Padding=new Padding(8)};previewRegion.Controls.Add(selectedPreview);
            previewDisclosure=new CheckBox{Appearance=Appearance.Button,Text="图案预览 ▸",Dock=DockStyle.Bottom,Height=30,TextAlign=ContentAlignment.MiddleLeft};previewDisclosure.CheckedChanged+=(s,e)=>{previewRegion.Visible=previewDisclosure.Checked;};
            patternLibrary=new FlowLayoutPanel{Dock=DockStyle.Fill,AutoScroll=true,Padding=new Padding(4),WrapContents=true};
            foreach(var image in images){image.Parent=null;image.Dock=DockStyle.None;image.Size=new Size(75,48);image.Margin=new Padding(5);image.BackgroundImageLayout=ImageLayout.Stretch;image.Cursor=Cursors.Hand;image.TabStop=true;image.AccessibleName=image.Name+" · "+image.Tag;image.KeyDown+=(s,e)=>{if(e.KeyCode==Keys.Enter||e.KeyCode==Keys.Space){pictureBox1_Click_1(s,EventArgs.Empty);e.Handled=true;}};patternLibrary.Controls.Add(image);}
            patternLibrary.SizeChanged+=(s,e)=>{int width=Math.Max(52,(patternLibrary.ClientSize.Width-SystemInformation.VerticalScrollBarWidth-48)/3);foreach(var image in images)image.Size=new Size(width,Math.Max(35,width*3/5));};
            var filter=new ComboBox{Dock=DockStyle.Top,DropDownStyle=ComboBoxStyle.DropDownList};filter.Items.AddRange(new object[]{"全部 · 32 个图案","RGB 可调图案","Back 参数图案","固定图案"});filter.SelectedIndex=0;
            filter.SelectedIndexChanged+=(s,e)=>{foreach(var image in images){var mode=image.Tag.ToString().Split(',')[0];image.Visible=filter.SelectedIndex==0||filter.SelectedIndex==1&&mode=="0"||filter.SelectedIndex==2&&(mode=="1"||mode=="2")||filter.SelectedIndex==3&&mode=="3";}};
            Controls.Clear();Controls.Add(patternLibrary);Controls.Add(filter);Controls.Add(rgb);Controls.Add(top);Controls.Add(previewRegion);Controls.Add(previewDisclosure);Controls.Add(bottom);
            WorkspaceStyle.Apply(this);ResumeLayout(true);
        }
        private void UpdateSelectedPreview(PictureBox image) {if(selectedPreview==null)return;selectedPreview.BackgroundImage=image.BackgroundImage;selectedPreview.BackColor=image.BackColor;previewDisclosure.Text="图案预览 · "+image.Name+(previewDisclosure.Checked?" ▾":" ▸");}
    }
}
