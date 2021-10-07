using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Text.RegularExpressions;

namespace Testplan
{
    public partial class ARMCode : DockContent
    {
        private PictureBox pictemp = new PictureBox();
        public static FormShare fs = new FormShare();
        public static string str = "";
        public string codeHex;
        public static StringBuilder textBuilder = new StringBuilder();
        static StringBuilder textBuilderControl = new StringBuilder();
        private string code="null";
        private int lineIndexTotal;
        private string insertStr;
        ToolTip tooltip1 = new ToolTip();
        public ARMCode()
        {
            InitializeComponent();
            fs.strevent += new strdelegate(codeEditControl);
            fs.strevent1 += new strdelegate(Bestvcomcode);
            fs.intintstrstrevent += new intintstrstrdelegate(dataAdd);
            fs.intstrevent += new intstrdelegate(dataColAdd);
            fs.strstrevent += new strstrdelegate(saveCsv);

            toolTip1.SetToolTip(pictureBox1,"格式刷");
            toolTip1.SetToolTip(pictureBox2, "DownLoad");
            toolTip1.SetToolTip(pictureBox4, "删除");
            toolTip1.SetToolTip(pictureBox5, "注释");
            toolTip1.SetToolTip(pictureBox3, "数据删除");
            toolTip1.SetToolTip(pictureBox6, "数据导出");
        }
               
        private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)
        {

        }

        private void Bestvcomcode(string str1)
        {
            int intstr = Convert.ToInt16(str1,16);
            //decimal dec = Convert.ToDecimal(intstr);
            numericUpDown3.Value = intstr;
        }

        private void codeSwitchHex(string text)
        {
            switch (text)
            {
                case " VGH":
                    int intVGH = Convert.ToInt16(numericUpDown1.Value);
                    codeHex = "02" + intVGH.ToString("X2");
                    break;
                case " VGL":
                    int intVGL = Convert.ToInt16(numericUpDown2.Value);
                    codeHex = "02" + intVGL.ToString("X2");
                    break;
                case " VCOM":
                    int intVCOM = Convert.ToInt16(numericUpDown3.Value);
                    codeHex = "02" + intVCOM.ToString("X2");
                    break;
                case " CKV":
                    int intCKV = Convert.ToInt16(numericUpDown4.Value);
                    codeHex = "02" + intCKV.ToString("X2");
                    break;
                case " CKH":
                    int intCKH = Convert.ToInt16(numericUpDown5.Value);
                    codeHex = "02" + intCKH.ToString("X2");
                    break;
                case " EQ":
                    int intEQ = Convert.ToInt16(numericUpDown6.Value);
                    codeHex = "02" + intEQ.ToString("X2");
                    break;
            }
        }

        public static string codeEdit(string str)
        {
            textBuilder.Append(Regex.Replace(str, @"\s", "0"));
            string textstr = textBuilder.ToString();
            //textstr.Trim();
            return textstr;
        }

        string[] codeStr = new string[6];
        string[] codeMarkStr = new string[12];
        static Dictionary<string, int> codeMarkDic = new Dictionary<string, int>();
        public static string textstrControl;

        public void codeEditControl(string codemark)
        {
            string star = "//" + codemark;
            string end = "//END" + codemark;
            string repcode = " " + codemark;
            textBuilderControl.Clear();
            textstrControl = "";
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

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            code = "";
            //规定标识符；
            codeMarkDic.Clear();
            codeMarkStr[0] = "//CKV"; codeMarkStr[1] = "//ENDCKV";
            codeMarkStr[2] = "//CKH"; codeMarkStr[3] = "//ENDCKH";
            codeMarkStr[4] = "//VCOM"; codeMarkStr[5] = "//ENDVCOM";
            codeMarkStr[6] = "//VGH"; codeMarkStr[7] = "//ENDVGH";
            codeMarkStr[8] = "//VGL"; codeMarkStr[9] = "//ENDVGL";
            codeMarkStr[10] = "//EQ"; codeMarkStr[11] = "//ENDEQ";

            codeStr[0] = " VGH"; codeStr[1] = " VGL"; codeStr[2] = " VCOM";
            codeStr[3] = " CKV"; codeStr[4] = " CKH"; codeStr[5] = " EQ";

            //ccd();
            //pictemp = (PictureBox)sender;
            //pictemp.BorderStyle = BorderStyle.Fixed3D;
            textBuilder.Clear();
            //删除空行
            richTextBox1.Text = Regex.Replace(richTextBox1.Text, @"(?s)\n\s*\n", "\n");
            richTextBox1.Text = richTextBox1.Text.Trim();

            lineIndexTotal = 0;

            for (int i = 0; i < richTextBox1.Lines.Length; i++)
            {
                int lineFirstCharIndex = richTextBox1.GetFirstCharIndexFromLine(i);
                //str 空格及大小写格式限定；
                str = richTextBox1.Lines[i].Trim();
                str = " " + str;
                str = Regex.Replace(str, @"\s+", " ");
                str = str.ToUpper();

                // richTextBox1格式调整；
                richTextBox1.SelectionStart = lineFirstCharIndex;
                richTextBox1.SelectionLength = richTextBox1.Lines[i].Length;
                richTextBox1.SelectionFont = new Font("微软雅黑", 9, FontStyle.Regular);
                richTextBox1.SelectedText = str;

                //调整完的格式重新读进str;
                str = Regex.Replace(str, @" //(.*)", "");           
                str = Regex.Replace(str, @"(/\*)(.*)(\*/)", "");

                // Dic标识符相关信息收集； 
                foreach (string codemarkstr in codeMarkStr)
                {
                    if (richTextBox1.Lines[i].Contains(codemarkstr))
                    {
                        try
                        {
                            codeMarkDic.Add(codemarkstr, i);
                        }
                        catch
                        {
                            MessageBox.Show("注释标识符" + codemarkstr + "出现两次以上");
                        }
                    }
                }

                // "//标识符"显示格式刷新；
                int readIndex = richTextBox1.Lines[i].IndexOf("//");
                int readIndexstar = richTextBox1.Lines[i].IndexOf("/*");
                int readIndexend = richTextBox1.Lines[i].IndexOf("*/");
                if (readIndex >= 0)
                {
                    richTextBox1.SelectionStart = readIndex + lineIndexTotal;
                    richTextBox1.SelectionLength = richTextBox1.Lines[i].Length - readIndex;
                    richTextBox1.SelectionColor = Color.Gray;
                }

                if (readIndexstar >= 0)
                {
                    if (readIndexend >= 0)
                    {
                        richTextBox1.SelectionStart = readIndexstar + lineIndexTotal;
                        richTextBox1.SelectionLength =  readIndexend-readIndexstar+2;
                        richTextBox1.SelectionColor = Color.Gray;
                    }
                    else
                    {
                        MessageBox.Show("注释符不完整！");
                    }
                    
                }

                // code标识符显示格式刷新+code标识符替换成code_string；
                foreach (string codestr in codeStr)
                {
                    if (richTextBox1.Lines[i].Contains(codestr))
                    {
                        richTextBox1.SelectionStart = richTextBox1.Lines[i].IndexOf(codestr) + lineIndexTotal;
                        richTextBox1.SelectionLength = codestr.Length;
                        richTextBox1.SelectionBackColor = Color.Yellow;
                        codeSwitchHex(codestr);
                        str = Regex.Replace(str, codestr, codeHex, RegexOptions.IgnoreCase);
                    }
                }
                code = codeEdit(str);
                lineIndexTotal = richTextBox1.Lines[i].Length + lineIndexTotal + 1;
            }

           
            

            richTextBox1.Select(0, 0);
            richTextBox1.Focus();           
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            try
            {
                if (code == "null")
                {
                    MessageBox.Show("先进行格式刷新");
                }
                else if (code == "")
                {
                    MessageBox.Show("是否有实际的code输入;");
                }
                else
                {
                  string codeDownload = "AA01" + code + "0F001248";
                    //MessageBox.Show(codeDownload);
                    armCodeSend(codeDownload);
                    codeDownload = "";
                }
            }
            catch (Exception)
            {
                MessageBox.Show("先进行格式刷新");
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            try
            {
                int insertIndex = richTextBox1.GetFirstCharIndexOfCurrentLine();
                int linenum = richTextBox1.GetLineFromCharIndex(insertIndex);

                int insertPosition = richTextBox1.SelectionStart;
                int insertLength = richTextBox1.SelectionLength;
                int lineLength = richTextBox1.Lines[linenum].Length;

                if (insertPosition == insertIndex && insertLength == lineLength)
                {
                    insertStr = " //" + richTextBox1.SelectedText;
                    richTextBox1.SelectedText = insertStr;
                    richTextBox1.SelectionStart = insertPosition;
                    richTextBox1.SelectionLength = insertStr.Length;
                    richTextBox1.SelectionColor = Color.Gray;
                }

               else if (richTextBox1.SelectedText.Contains("\n"))
                {
                    richTextBox1.SelectionStart = insertIndex;
                    richTextBox1.SelectionLength = richTextBox1.Lines[linenum].Length;
                    richTextBox1.SelectionColor = Color.Gray;
                    insertStr = " //" + richTextBox1.SelectedText;
                    richTextBox1.SelectedText = insertStr;
                    

                    richTextBox1.SelectionStart = insertIndex+ richTextBox1.Lines[linenum].Length+1;
                    richTextBox1.SelectionLength = richTextBox1.Lines[linenum + 1].Length;
                    richTextBox1.SelectionColor = Color.Gray;
                    insertStr = " //" + richTextBox1.SelectedText;
                    richTextBox1.SelectedText = insertStr;
                    
                }
                else
                {
                    insertStr = "/*" + richTextBox1.SelectedText + "*/";
                    richTextBox1.SelectedText = insertStr;
                    richTextBox1.SelectionStart = insertPosition;
                    richTextBox1.SelectionLength = insertStr.Length;
                    richTextBox1.SelectionColor = Color.Gray;
                }
            }
            catch (Exception)
            {

            }
        }

        public static PictureBox temp = new PictureBox();
        private int step = 3;
        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            temp = (PictureBox)sender;
            int x = temp.Location.X;
            int y = temp.Location.Y;
            temp.Location = new Point(x - step / 2, y - step / 2);
            int w = temp.Width;
            int h = temp.Height;
            temp.Width = w + step;
            temp.Height = h + step;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            temp = (PictureBox)sender;
            int x = temp.Location.X;
            int y = temp.Location.Y;
            temp.Location = new Point(x + step / 2, y + step / 2);
            int w = temp.Width;
            int h = temp.Height;
            temp.Width = w - step;
            temp.Height = h - step;
        }

        private int rindex;
        //private int cindex;
        private void dataAdd(int i, int n,string code,string data)
        {
            if (i == 0)
            {
                rindex = dataGridView2.Rows.Add();
                dataGridView2.Rows[rindex].Cells[0].Value = code;
                dataGridView2.Rows[rindex].Cells[1].Value = data;
                dataGridView2.CurrentCell = dataGridView2.Rows[rindex].Cells[1];
            }
            else
            {
                if (n-1 <= rindex)  //之前列，行已添加，不必新增行；
                {
                    dataGridView2.Rows[n-1].Cells[2*i].Value = code;
                    dataGridView2.Rows[n-1].Cells[2 * i+1].Value = data;
                    dataGridView2.CurrentCell = dataGridView2.Rows[n - 1].Cells[2 * i + 1];
                }
               else 
                {
                    rindex = dataGridView2.Rows.Add();
                    dataGridView2.Rows[rindex].Cells[2 * i].Value = code;
                    dataGridView2.Rows[rindex].Cells[2 * i + 1].Value = data;
                    dataGridView2.CurrentCell = dataGridView2.Rows[rindex].Cells[2 * i + 1];
                }
            }
            //dataGridView2.FirstDisplayedScrollingRowIndex = dataGridView2.Rows.Count - 1;
            //dataGridView2.FirstDisplayedScrollingColumnIndex = dataGridView2.Columns.Count - 1;
            
        }

        private void dataColAdd(int i,string str)
        {
   
                int colint = 2* i;
                string colstr1 = "Column" + colint+1;
                string colstr2 = "Column" + colint + 2;
                string header1 = str + 1;
                string header2 = str + 2;
        
                dataGridView2.Columns.Add(colstr1, header1);
                dataGridView2.Columns.Add(colstr2, header2);

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
           
            if (dataGridView2.Columns.Count != 0)
            {
                if (MessageBox.Show("是否要删除", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    Control.runStat = 0;
                    Control.Lvset = 0;
                    Control.Flickerset = 0;
                    dataGridView2.Columns.Clear();
                }              
            }
           
        }

        private void armCodeSend(string code)
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

        private bool dataGridViewToCSV(DataGridView dataGridView)
        {
            if (dataGridView.Rows.Count == 0)
            {
                MessageBox.Show("没有数据可导出!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.FileName = null;
            saveFileDialog.Title = "保存";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream stream = saveFileDialog.OpenFile();
                StreamWriter sw = new StreamWriter(stream, System.Text.Encoding.GetEncoding(-0));
                string strLine = "";
                try
                {
                    //表头
                    for (int i = 0; i < dataGridView.ColumnCount; i++)
                    {
                        if (i > 0)
                            strLine += ",";
                        strLine += dataGridView.Columns[i].HeaderText;
                    }
                    strLine.Remove(strLine.Length - 1);
                    sw.WriteLine(strLine);
                    strLine = "";
                    //表的内容
                    for (int j = 0; j < dataGridView.Rows.Count; j++)
                    {
                        strLine = "";
                        int colCount = dataGridView.Columns.Count;
                        for (int k = 0; k < colCount; k++)
                        {
                            if (k > 0 && k < colCount)
                                strLine += ",";
                            if (dataGridView.Rows[j].Cells[k].Value == null)
                                strLine += "";
                            else
                            {
                                string cell = dataGridView.Rows[j].Cells[k].Value.ToString().Trim();
                                //防止里面含有特殊符号
                                cell = cell.Replace("\"", "\"\"");
                                cell = "\"" + cell + "\"";
                                strLine += cell;
                            }
                        }
                        sw.WriteLine(strLine);
                    }
                    sw.Close();
                    stream.Close();
                    MessageBox.Show("数据被导出到：" + saveFileDialog.FileName.ToString(), "导出完毕", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "导出错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }
            return true;
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            dataGridViewToCSV(dataGridView2);
        }

        private void saveCsv(string path,string name)
        {
            if (dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("没有数据可导出!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                string fileName = path + "\\" + name + ".csv";
                StreamWriter sw = new StreamWriter(fileName, true, System.Text.Encoding.GetEncoding(-0));
                string strLine = "";
                try
                {
                    //表头
                    for (int i = 0; i < dataGridView2.ColumnCount; i++)
                    {
                        if (i > 0)
                            strLine += ",";
                        strLine += dataGridView2.Columns[i].HeaderText;
                    }
                    strLine.Remove(strLine.Length - 1);
                    sw.WriteLine(strLine);
                    strLine = "";
                    //表的内容
                    for (int j = 0; j < dataGridView2.Rows.Count; j++)
                    {
                        strLine = "";
                        int colCount = dataGridView2.Columns.Count;
                        for (int k = 0; k < colCount; k++)
                        {
                            if (k > 0 && k < colCount)
                                strLine += ",";
                            if (dataGridView2.Rows[j].Cells[k].Value == null)
                                strLine += "";
                            else
                            {
                                string cell = dataGridView2.Rows[j].Cells[k].Value.ToString().Trim();
                                //防止里面含有特殊符号
                                cell = cell.Replace("\"", "\"\"");
                                cell = "\"" + cell + "\"";
                                strLine += cell;
                            }
                        }
                        sw.WriteLine(strLine);
                    }
                    sw.Flush();
                    sw.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "导出错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private Label labeltemp = new Label();
        private void label1_MouseEnter(object sender, EventArgs e)
        {
            labeltemp = (Label)sender;
            labeltemp.BackColor = Color.FromArgb(255, 200, 200, 200);
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            labeltemp.BackColor = Color.FromArgb(0, 0, 0, 0);
        }
    }
}
