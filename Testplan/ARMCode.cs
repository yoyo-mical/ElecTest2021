using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Text.RegularExpressions;

namespace Testplan
{
    public partial class ARMCode : DockContent
    {
        private PictureBox pictemp = new PictureBox();
        public ARMCode()
        {
            InitializeComponent();
        }

        private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)
        {

        }

        public static string str = "";
        private string indextag;
        public string hexVGH, hexVGL, hexVCOM, hexCKV, hexCKH, hexEQ;
        private void hightLightText(RichTextBox r, string text)
        {
            int indexFind = richTextBox1.Find(text);
            if (indexFind >= 0)
            {
                indextag = "1";
                r.SelectionStart = indexFind;
                r.SelectionLength = text.Length;
                r.SelectionBackColor = Color.Yellow;
                if (text == "VGH")
                {
                    byte byteVGH = Convert.ToByte(numericUpDown1.Value);
                    hexVGH = byteVGH.ToString("X2");
                }
                else if (text == "VGL")
                {
                    byte byteVGL = Convert.ToByte(numericUpDown2.Value);
                    hexVGL = byteVGL.ToString("X2");
                }
                else if (text == "VCOM")
                {
                    byte byteVCOM = Convert.ToByte(numericUpDown3.Value);
                    hexVCOM = byteVCOM.ToString("X2");
                }
                else if (text == "CKV")
                {
                    byte byteCKV = Convert.ToByte(numericUpDown4.Value);
                    hexCKV = byteCKV.ToString("X2");
                }
                else if (text == "CKH")
                {
                    byte byteCKH = Convert.ToByte(numericUpDown5.Value);
                    hexCKH = byteCKH.ToString("X2");
                }
                else if (text == "EQ")
                {
                    byte byteEQ = Convert.ToByte(numericUpDown6.Value);
                    hexEQ = byteEQ.ToString("X2");
                }

            }
            else
            {
                indextag = "0";
            }

        }

        StringBuilder textBuilder = new StringBuilder();
        static StringBuilder textBuilderControl = new StringBuilder();
        private string code;
        private string codeEdit(string str)
        {
            textBuilder.Append(Regex.Replace(str, @"\s", "0"));
            string textstr = textBuilder.ToString();
            return textstr;
        }

        string[] codeMarkStr = new string[12];
        static Dictionary<string, int> codeMarkDic = new Dictionary<string, int>();

        public string codeEditControl(string codemark)
        {
            string star = "//" + codemark;
            string end = "//END" + codemark;
            string repcode = " " + codemark;
            textBuilderControl.Clear();
            string textstrControl = "";
            for (int i = codeMarkDic[star] + 1; i <= codeMarkDic[end]; i++)
            {
                str = richTextBox1.Lines[i].Trim();
                str = " " + str;
                str = Regex.Replace(str, @"\s+", " ");
                str = Regex.Replace(str, @" //\w+", "");
                str = Regex.Replace(str, @repcode, codemark);
                textBuilderControl.Append(Regex.Replace(str, @"\s", "0"));
                textstrControl = textBuilderControl.ToString();
            }
            return textstrControl;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            codeMarkDic.Clear();
            codeMarkStr[0] = "//CKV"; codeMarkStr[1] = "//ENDCKV";
            codeMarkStr[2] = "//CKH"; codeMarkStr[3] = "//ENDCKH";
            codeMarkStr[4] = "//VCOM"; codeMarkStr[5] = "//ENDVCOM";
            codeMarkStr[6] = "//VGH"; codeMarkStr[7] = "//ENDVGH";
            codeMarkStr[8] = "//VGL"; codeMarkStr[9] = "//ENDVGL";
            codeMarkStr[10] = "//EQ"; codeMarkStr[11] = "//ENDEQ";

            //pictemp = (PictureBox)sender;
            pictemp.BorderStyle = BorderStyle.Fixed3D;
            textBuilder.Clear();
            richTextBox1.Text = Regex.Replace(richTextBox1.Text, @"(?s)\n\s*\n", "\n");
            ////richTextBox1.Lines[0] = str;
            //MessageBox.Show(str);
            //richTextBox1.Clear();
            //richTextBox1.Text = str;
            int lineIndexTotal = 0;
            for (int i = 0; i < richTextBox1.Lines.Length; i++)
            {
                int lineFirstCharIndex = richTextBox1.GetFirstCharIndexFromLine(i);

                str = richTextBox1.Lines[i].Trim();
                str = " " + str;
                str = Regex.Replace(str, @"\s+", " ");
                str = Regex.Replace(str, @" //\w+", "");

                int readIndex = richTextBox1.Lines[i].IndexOf("//");
                //int endIndex = richTextBox1.Lines[i].Length;

                foreach (string codemarkstr in codeMarkStr)
                {
                    if (richTextBox1.Lines[i].Contains(codemarkstr))
                    {
                        codeMarkDic.Add(codemarkstr, i);
                    }
                }

                if (readIndex >= 0)
                {
                    richTextBox1.SelectionStart = readIndex + lineIndexTotal;
                    richTextBox1.SelectionLength = richTextBox1.Lines[i].Length - readIndex;
                    richTextBox1.SelectionColor = Color.Gray;
                }

                lineIndexTotal = richTextBox1.Lines[i].Length + lineIndexTotal + 1;

                str = Regex.Replace(str, "vgl", "VGL", RegexOptions.IgnoreCase);
                hightLightText(richTextBox1, "VGL");
                if (indextag == "1")
                {
                    str = Regex.Replace(str, " VGL", hexVGL, RegexOptions.IgnoreCase);
                }

                str = Regex.Replace(str, "vgh", "VGH", RegexOptions.IgnoreCase);
                hightLightText(richTextBox1, "VGH");
                if (indextag == "1")
                {
                    str = Regex.Replace(str, " VGH", hexVGH, RegexOptions.IgnoreCase);
                }

                str = Regex.Replace(str, "vcom", "VCOM", RegexOptions.IgnoreCase);
                hightLightText(richTextBox1, "VCOM");
                if (indextag == "1")
                {
                    str = Regex.Replace(str, " VCOM", hexVCOM, RegexOptions.IgnoreCase);
                }

                str = Regex.Replace(str, "ckv", "CKV", RegexOptions.IgnoreCase);
                hightLightText(richTextBox1, "CKV");
                if (indextag == "1")
                {
                    str = Regex.Replace(str, " CKV", hexCKV, RegexOptions.IgnoreCase);
                }

                str = Regex.Replace(str, "ckh", "CKH", RegexOptions.IgnoreCase);
                hightLightText(richTextBox1, "CKH");
                if (indextag == "1")
                {
                    str = Regex.Replace(str, " CKH", hexCKH, RegexOptions.IgnoreCase);
                }

                str = Regex.Replace(str, "eq", "EQ", RegexOptions.IgnoreCase);
                hightLightText(richTextBox1, "EQ");
                if (indextag == "1")
                {
                    str = Regex.Replace(str, " EQ", hexEQ, RegexOptions.IgnoreCase);
                }
                //richTextBox1.SelectionStart = lineFirstCharIndex;
                //richTextBox1.SelectionLength = richTextBox1.Lines[i].Length;
                //richTextBox1.SelectedText = str;
                //code = codeEdit(str);

            }
            code = codeEditControl("VGH");
            code = "AA01" + code + "0F001248";

            hightLightText(richTextBox1, "VGH");

            hightLightText(richTextBox1, "VCOM");
            hightLightText(richTextBox1, "CKV");
            hightLightText(richTextBox1, "CKH");
            hightLightText(richTextBox1, "EQ");

            richTextBox1.Select(0, 0);
            richTextBox1.Focus();
            MessageBox.Show(code);
            //armCodeSend();
        }

        private void armCodeSend()
        {
            if (UART.isOpen == true)
            {
                int n = 0;
                try
                {
                    if (UART.isHex)
                    {
                        UART.HexSend(code);
                        string text = "";
                        for (int i = 0; i < UART.values.Length; i++)
                        {
                            text += UART.values[i].ToString("X2");
                        }
                        //MessageBox.Show(Convert.ToString(text));
                    }
                    else
                    {
                        UART.sp.Write(code);
                        n = code.Length;

                        //MessageBox.Show(Convert.ToString(values));
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("发送内容时发生错误", "错误提示");
                    return;
                }
                UART.cntSend += n;
            }
            else
            {
                MessageBox.Show("串口未打开", "错误提示");
                return;
            }

        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            pictemp = (PictureBox)sender;
            pictemp.BorderStyle = BorderStyle.FixedSingle;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            //pictemp = (PictureBox)sender;
            pictemp.BorderStyle = BorderStyle.None;
        }
    }
}
