using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;
using System.Timers;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Collections;

namespace Testplan
{
    public partial class UART : DockContent
    
    {
        //AutoSizeFormClass asc = new AutoSizeFormClass();
        public static  SerialPort sp = new SerialPort();
        public static bool isOpen = false;
        public static bool isHex = false;
        bool isSetProperty = false;
        //public bool isSendHex = false;
        //public bool isResvHex = false;
        public string RecvDataTextL = null;
        public int comindex=-1;
        //public StringBuilder sp_portname = new StringBuilder("COM");
        //string k = new string(RecvDataTextL);
        

        public UART()
        {
            InitializeComponent();
        }
        private void UART_SizeChanged(object sender, EventArgs e)
        {
            //asc.controlAutoSize(this);
            //this.WindowState = (System.Windows.Forms.FormWindowState)(2);
            //记录完控件的初始位置和大小后，再最大化
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void UART_Load(object sender, EventArgs e)
        {
            //asc.controllInitializeSize(this);
            //this.MaximumSize = this.Size;
            //this.MinimumSize = this.Size;
            //this.MaximizeBox = false;

            for (int i = 0; i < 10; i++)
            {
                cbxComPort.Items.Add("COM" + (i + 1).ToString());
            }
            cbxComPort.SelectedIndex = 0;

            cbxBaudRate.Items.Add("1200");
            cbxBaudRate.Items.Add("2400");
            cbxBaudRate.Items.Add("4800");
            cbxBaudRate.Items.Add("9600");
            cbxBaudRate.Items.Add("19200");
            cbxBaudRate.Items.Add("38400");
            cbxBaudRate.Items.Add("43000");
            cbxBaudRate.Items.Add("56000");
            cbxBaudRate.Items.Add("57600");
            cbxBaudRate.Items.Add("115200");
            cbxBaudRate.Items.Add("117600");
            cbxBaudRate.Items.Add("240000");
            cbxBaudRate.SelectedIndex = 9;

            cbxDataBits.Items.Add("8");
            cbxDataBits.Items.Add("7");
            cbxDataBits.Items.Add("6");
            cbxDataBits.Items.Add("5");
            cbxDataBits.SelectedIndex = 0;

            cbxStopBits.Items.Add("0");
            cbxStopBits.Items.Add("1");
            cbxStopBits.Items.Add("1.5");
            cbxStopBits.Items.Add("2");
            cbxStopBits.SelectedIndex = 1;

            cbxParity.Items.Add("无");
            cbxParity.Items.Add("奇校验");
            cbxParity.Items.Add("偶校验");
            cbxParity.SelectedIndex = 0;

         }

        private void button3_Click(object sender, EventArgs e)
        {
            bool comExistence = false;
            cbxComPort.Items.Clear();

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    //sp = new SerialPort("COM" + (i + 1).ToString());
                    StringBuilder sp_portname = new StringBuilder("COM");
                    sp.PortName = sp_portname.Append((i + 1).ToString()).ToString();
                    sp.Open();                    
                    sp.Close();
                   
                    cbxComPort.Items.Add("COM" + (i + 1).ToString());
                    comExistence = true;
                    comindex+=1 ;
                }
                catch(Exception)
                {
                    continue;
                }
            }

            if (comExistence)
            {
                cbxComPort.SelectedIndex = comindex;
                //MessageBox.Show("有可用串口");
                
            }
            else
            {
                MessageBox.Show("没有发现任何串口", "错误提示");
            }
            comindex = -1;
        }

        private bool CheckPortSetting()
        {
            if (cbxComPort.Text.Trim() == "") return false;
            if (cbxBaudRate.Text.Trim() == "") return false;
            if (cbxDataBits.Text.Trim() == "") return false;
            if (cbxParity.Text.Trim() == "") return false;
            if (cbxStopBits.Text.Trim() == "") return false;
               return true;
        }

        private bool CheckSendData()
        {
            if (tbxSendData.Text.Trim() == "") return false;
            return true;
        }

        private void SetPortPerproty()
        {
            //sp = new SerialPort();
            sp.PortName = cbxComPort.Text.Trim();
            sp.BaudRate = Convert.ToInt32(cbxBaudRate.Text.Trim());
            float f = Convert.ToSingle(cbxStopBits.Text.Trim());
            if (f == 0)
            {
                sp.StopBits = StopBits.None;
            }
            else if (f == 1)
            {
                sp.StopBits = StopBits.One;
            }
            else if (f == 1.5)
            {
                sp.StopBits = StopBits.OnePointFive;
            }
            else if (f == 2)
            {
                sp.StopBits = StopBits.Two;
            }
            else
            {
                sp.StopBits = StopBits.One;
            }

            sp.DataBits= Convert.ToInt16(cbxDataBits.Text.Trim());

            string s = cbxParity.Text.Trim();
            if (s.CompareTo("无") == 0)
            {
                sp.Parity = Parity.None;
            }
            else if (s.CompareTo("奇校验") == 0)
            {
                sp.Parity = Parity.Odd;
            }
            else if (s.CompareTo("偶校验") == 0)
            {
                sp.Parity = Parity.Even;
            }
            else 
            {
                sp.Parity = Parity.None;
            }

            sp.ReadTimeout = -1;
            sp.RtsEnable = true;
            sp.ReceivedBytesThreshold = 1;
            sp.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);


            if (rbnSendHex.Checked)
            {
                isHex = true;
            }
            else
            {
                isHex = false;
            }
         }

        private void btnOpenCom_Click(object sender, EventArgs e)
        {
            if (isOpen == false)
            {
                if (!CheckPortSetting())
                {
                    MessageBox.Show("串口未设置", "错误提示");
                    return;
                }
                if (isSetProperty == false)
                {
                    SetPortPerproty();
                    isSetProperty = true;
                }
                try
                {
                    sp.Open();
                    isOpen = true;
                    btnOpenCom.Text = "关闭串口";
                    cbxComPort.Enabled = false;
                    cbxBaudRate.Enabled = false;
                    cbxDataBits.Enabled = false;
                    cbxParity.Enabled = false;
                    cbxStopBits.Enabled = false;
                }
                catch (Exception)
                {
                    isSetProperty = false;
                    isOpen = false;
                    MessageBox.Show("串口无效或已被占用", "错误提示");
                }
            }
            else
            {
                try
                {
                    sp.Close();
                    isOpen = false;
                    btnOpenCom.Text = "打开串口";
                    cbxComPort.Enabled = true;
                    cbxBaudRate.Enabled = true;
                    cbxDataBits.Enabled = true;
                    cbxParity.Enabled = true;
                    cbxStopBits.Enabled = true;
                }
                catch (Exception)
                {
                }
            }
            }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (isOpen == true)
            {
                int n = 0;
                try
                {
                    if (isHex)
                    {
                        HexSend(tbxSendData.Text);
                        string text = "";
                        for (int i = 0; i < values.Length; i++)
                        {
                            text += values[i].ToString("X2");
                        }
                        //MessageBox.Show(Convert.ToString(text));
                    }
                    else
                    {
                        sp.Write(tbxSendData.Text);
                        n = tbxSendData.Text.Length;
                        
                        MessageBox.Show(Convert.ToString(values));
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("发送内容时发生错误", "错误提示");
                    return;
                }
                cntSend += n;
            }
            else
            {
                MessageBox.Show("串口未打开", "错误提示");
                return;
            }
            if (!CheckSendData())
            {
                MessageBox.Show("请输入要发送的数据", "错误提示");
                return;
            }
            
        }
        public static long cntSend = 0;
        public static byte[] values;
     
        public static void HexSend(string strSend)
        {
            string str = strSend.Replace(" ", "");//去除所有空格
            str = str.Replace(",", "");//去除所有逗号
            ArrayList SendCharList = new ArrayList();

            int len = str.Length / 2;

            string sstr;
            Byte byt;
            for (int i = 0; i < len; i++)
            {
                //每次取出两个字符，转成十六进制数，加到ArrayList中
                sstr = str.Substring(2 * i, 2);
                //加入容错处理
                try
                {
                    byt = Convert.ToByte(sstr, 16);
                }
                catch
                {
                    MessageBox.Show("发送数据中的有无法转成16进制数的字节：" + sstr, "系统提示");
                    return;
                }

                SendCharList.Add(byt);
            }
             values = (Byte[])SendCharList.ToArray(typeof(Byte));//返回ArrayList包含的数组
             
            try
            {
                sp.Write(values, 0, len);
            }
            catch
            {
                
            }

        }
        private long cntReceived;
       public StringBuilder builder = new StringBuilder();

        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            System.Threading.Thread.Sleep(100);
            int n = sp.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
            byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据
            cntReceived += n;//增加接收计数
            sp.Read(buf, 0, n);//读取缓冲数据
            builder.Clear();//清除字符串构造器的内容
            //因为要访问ui资源，所以需要使用invoke方式同步ui。
            this.Invoke((EventHandler)(delegate
            {
                //判断是否是显示为16进制;
                if (isHex)
                {
                    //依次的拼接出16进制字符串
                    foreach (byte b in buf)
                    {
                        builder.Append(b.ToString("X2") + " ");

                        //MessageBox.Show("接收");
                    }
                    
                }
                else
                {
                    //直接按ASCII规则转换成字符串
                    builder.Append(Encoding.ASCII.GetString(buf));
                }
                this.tbxRecvData.AppendText(builder.ToString());
                //修改接收计数
                //labelGetCount.Text = "Get:" + cntReceived.ToString();
            }));
           
        }

        private void btnCleanData_Click(object sender, EventArgs e)
        {
            tbxRecvData.Text = "";
            tbxSendData.Text = "";
            RecvDataTextL = "";
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void rbnSendHex_CheckedChanged(object sender, EventArgs e)
        {
            isHex = true;
        }

        private void cbxBaudRate_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
           
        }

        private void tbxRecvData_TextChanged(object sender, EventArgs e)
        {

        }

        private void cbxComPort_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
    }

