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
using System.Runtime.InteropServices;

namespace Testplan
{
    
    public partial class Control : DockContent
    {
        //string defaultText = comboBox1.SelectedItem.ToString();
        StringBuilder controlcodeBuilder = new StringBuilder();
        private string controlcode;
        ToolTip tooltip1 = new ToolTip();
        public static FormShare fs = new FormShare();
        int ns1 = 1, ns2 = 1, ns3 = 1;
        public Control()
        {
            InitializeComponent();
            fs.strevent += new strdelegate(patterntext);
            tooltip1.SetToolTip(pictureBox1,"从头开始运行");
            tooltip1.SetToolTip(pictureBox2, "格式刷");
            tooltip1.SetToolTip(pictureBox3, "Recipe编写");
            tooltip1.SetToolTip(pictureBox4, "单步运行");
            tooltip1.SetToolTip(pictureBox5, "删除");
            tooltip1.SetToolTip(pictureBox6, "文件导入");
            tooltip1.SetToolTip(pictureBox7, "文件导出");
            tooltip1.SetToolTip(pictureBox8, "上移");
            tooltip1.SetToolTip(pictureBox9, "下移");
            tooltip1.SetToolTip(pictureBox10, "从当前行向下运行");
        }

        [DllImport("kernel32.dll")]

        public static extern uint GetTickCount();
        public static void TimeDelay(uint ms)
        {
            uint start = GetTickCount();
            while (GetTickCount() - start < ms)
            {
                System.Windows.Forms.Application.DoEvents();
            }
        }


        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            if (ns2 == 1 && ns3 == 1)
            {
                MainForm.ImageSwitch(sender, Convert.ToInt16(((PictureBox)sender).Tag.ToString()), ns1);
                ns1 = 1 - ns1;
                switch (ns1)
                {
                    case 0:
                        {
                            MessageBox.Show("运行");
                            for (int i = 0; i < richTextBox1.Lines.Length; i++)
                            {
                                
                                firstCharPosition = richTextBox1.GetFirstCharIndexFromLine(i);
                                
                                currentLineNum = i;
                               
                                HighlightCurrentLine();
                                richTextBox1.Lines[i].Trim();
                                string str = Regex.Replace(richTextBox1.Lines[i], @"/s+", " ");
                                stringSplit(str);
                                controlCodeRun(item);
                                if (ns1 == 1 && ns2 == 1 && ns3 == 1)
                                {
                                   
                                    break;
                                }
                            }
                            break;
                        }
                
                }
            }

         }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            if (ns1 == 1 && ns3 == 1)
            {

                MainForm.ImageSwitch(sender, Convert.ToInt16(((PictureBox)sender).Tag.ToString()), ns2);
                ns2 = 1 - ns2;
                switch(ns2)
                {
                    case 0:
                        {
                            MessageBox.Show("运行");
                            for (int i = currentLineNum; i < richTextBox1.Lines.Length; i++)
                            {
                                firstCharPosition = richTextBox1.GetFirstCharIndexFromLine(i);
                                currentLineNum = i;
                                HighlightCurrentLine();
                                richTextBox1.Lines[i].Trim();
                                string str = Regex.Replace(richTextBox1.Lines[i], @"/s+", " ");
                                stringSplit(str);
                                controlCodeRun(item);
                                if (ns1 == 1 && ns2 == 1 && ns3 == 1)
                                {
                                    
                                    break;
                                }
                            }
                            break;
                        }                 
                }
                
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (ns1 == 1 && ns2 == 1)
            {
                //MainForm.ImageSwitch(sender, Convert.ToInt16(((PictureBox)sender).Tag.ToString()), ns3);
                ns3 = 1 - ns3;
                if (ns1 == 1 && ns2 == 1 && ns3 == 1)
                {
                    
                }
                //switch (ns3)
                //{
                //    case 0:
                //{
                //MessageBox.Show("运行");
                else
                {
                    int i = currentLineNum;
                    richTextBox1.Lines[i].Trim();
                    string str = Regex.Replace(richTextBox1.Lines[i], @"/s+", " ");
                    stringSplit(str);
                    controlCodeRun(item);
                    //break;
                    //}
                    //}
                }
                
            }

        }


        private void patterntext(string str)
        {
            textBox1.Text = str;
        }
        
        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.Text)
                {
                case "Delay":
                    numericUpDown1.Enabled = false;
                    numericUpDown2.Enabled = false;
                    numericUpDown3.Enabled = true;
                    numericUpDown4.Enabled = false;
                    numericUpDown5.Enabled = false;
                    numericUpDown6.Enabled = false;
                    checkBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    textBox3.Enabled = false;
                    break;

                case "VCOM":
                    numericUpDown1.Enabled = true;
                    numericUpDown1.Hexadecimal = true;
                    numericUpDown2.Enabled = true;
                    numericUpDown3.Enabled = false;
                    numericUpDown4.Enabled = true;
                    numericUpDown5.Enabled = true;
                    numericUpDown6.Enabled = false;
                    checkBox1.Enabled = true;
                    comboBox2.Enabled = false;
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    textBox3.Enabled = false;
                    break;

                case "Gamma":
                    numericUpDown1.Enabled = false;
                    numericUpDown2.Enabled = false;
                    numericUpDown3.Enabled = false;
                    numericUpDown4.Enabled = true;
                    numericUpDown5.Enabled = true;
                    numericUpDown6.Enabled = false;
                    checkBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    textBox3.Enabled = false;
                    break;

                case "VGH":
                case "VGL":
                case "CKH":
                case "CKV":
                    numericUpDown1.Enabled = true;
                    numericUpDown1.Hexadecimal = true;
                    numericUpDown2.Enabled = true;
                    numericUpDown3.Enabled = false;
                    numericUpDown4.Enabled = true;
                    numericUpDown5.Enabled = true;
                    numericUpDown6.Enabled = false;
                    checkBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    textBox3.Enabled = false;
                    break;

                case "Pattern":
                    numericUpDown1.Enabled = false;
                    numericUpDown2.Enabled = false;
                    numericUpDown3.Enabled = false;
                    numericUpDown4.Enabled = false;
                    numericUpDown5.Enabled = false;
                    numericUpDown6.Enabled = false;
                    checkBox1.Enabled = false;
                    comboBox2.Enabled = true;
                    textBox1.Enabled = true;
                    textBox2.Enabled = false;
                    textBox3.Enabled = false;
                    break;

                case "DownLoad":
                    numericUpDown1.Enabled = false;
                    numericUpDown2.Enabled = false;
                    numericUpDown3.Enabled = false;
                    numericUpDown4.Enabled = false;
                    numericUpDown5.Enabled = false;
                    numericUpDown6.Enabled = false;
                    checkBox1.Enabled = false;
                    comboBox2.Enabled = true;
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    textBox3.Enabled = true;
                    break;

                 case "Save":
                 case "Read":
                    numericUpDown1.Enabled = false;
                    numericUpDown2.Enabled = false;
                    numericUpDown3.Enabled = false;
                    numericUpDown4.Enabled = false;
                    numericUpDown5.Enabled = false;
                    numericUpDown6.Enabled = false;
                    checkBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    textBox1.Enabled = false;
                    textBox2.Enabled = true;
                    textBox3.Enabled = false;
                    break;

                case "Goto":
                    numericUpDown1.Enabled = true;
                    numericUpDown1.Hexadecimal = false;
                    numericUpDown2.Enabled = false;
                    numericUpDown3.Enabled = false;
                    numericUpDown4.Enabled = false;
                    numericUpDown5.Enabled = false;
                    numericUpDown6.Enabled = true;
                    checkBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    textBox3.Enabled = false;
                    break;

                case "Position":
                case "Lv":
                    numericUpDown1.Enabled = false;
                    numericUpDown2.Enabled = false;
                    numericUpDown3.Enabled = false;
                    numericUpDown4.Enabled = false;
                    numericUpDown5.Enabled = false;
                    numericUpDown6.Enabled = false;
                    checkBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    textBox3.Enabled = false;
                    break;

            }
        }

        public string[] arrstr;
        string item;
        private void stringSplit(string str)
        {
            arrstr = Regex.Split(str," ");
             item = " " + arrstr[0];
        }


        private void controlCodeEdit(string str)
        {
            switch (str)
            {
                case "Delay":
                    controlcodeBuilder.Append("Delay " + numericUpDown3.Value.ToString());
                    controlcode = controlcodeBuilder.ToString();
                  break;

                case "Gamma":
                    controlcodeBuilder.Append("Gamma "  +numericUpDown4.Value.ToString() + " "
                     + numericUpDown5.Value.ToString() );
                    controlcode = controlcodeBuilder.ToString();

                    break;

                case "VCOM":                    
                     controlcodeBuilder.Append("VCOM " + Convert.ToInt16(numericUpDown1.Value).ToString("X") + " "
                        + Convert.ToInt16(numericUpDown2.Value).ToString("X") + " " + Convert.ToInt16(numericUpDown4.Value).ToString("X") + " "
                        + numericUpDown5.Value.ToString() + " "+downcom);
                    controlcode = controlcodeBuilder.ToString();
                  break;

                case "VGH":              
                    controlcodeBuilder.Append("VGH " + Convert.ToInt16( numericUpDown1.Value).ToString("X") + " "
                       + Convert.ToInt16(numericUpDown2.Value).ToString("X") + " " + Convert.ToInt16(numericUpDown4.Value).ToString("X") + " "
                       + numericUpDown5.Value.ToString() );
                    controlcode = controlcodeBuilder.ToString();
                   break;

                case "VGL":
                    controlcodeBuilder.Append("VGL " + Convert.ToInt16(numericUpDown1.Value).ToString("X") + " "
                       + Convert.ToInt16(numericUpDown2.Value).ToString("X") + " " + Convert.ToInt16(numericUpDown4.Value).ToString("X") + " "
                       + numericUpDown5.Value.ToString());
                    controlcode = controlcodeBuilder.ToString();
                    break;

                case "CKH":
                    controlcodeBuilder.Append("CKH " + Convert.ToInt16(numericUpDown1.Value).ToString("X") + " "
                       + Convert.ToInt16(numericUpDown2.Value).ToString("X") + " " + Convert.ToInt16(numericUpDown4.Value).ToString("X") + " "
                       + numericUpDown5.Value.ToString());
                    controlcode = controlcodeBuilder.ToString();
                    break;

                case "CKV":
                    controlcodeBuilder.Append("CKV " + Convert.ToInt16(numericUpDown1.Value).ToString("X") + " "
                       + Convert.ToInt16(numericUpDown2.Value).ToString("X") + " " + Convert.ToInt16(numericUpDown4.Value).ToString("X") + " "
                       + numericUpDown5.Value.ToString());
                    controlcode = controlcodeBuilder.ToString();
                    break;

                case "Pattern":
                    if (textBox1.Text != "")
                    {
                        controlcodeBuilder.Append("Pattern " + textBox1.Text);
                        controlcode = controlcodeBuilder.ToString();
                    }
                    else
                    {
                        controlcode = "";
                    }
                    break;

                case "DownLoad":
                    if (textBox3.Text != "")
                    {
                        controlcodeBuilder.Append("DownLoad " + textBox3.Text);
                        controlcode = controlcodeBuilder.ToString();
                    }
                    else
                    {
                        controlcode = "";
                    }
                    break;

                case "Save":
                    if (textBox2.Text != "")
                    {
                        controlcodeBuilder.Append("Save " + textBox2.Text);
                        controlcode = controlcodeBuilder.ToString();
                    }
                    else
                    {
                        controlcode = "";
                    }
                    break;

                case "Read":
                    if (textBox2.Text != "")
                    {
                        controlcodeBuilder.Append("Read " + textBox2.Text);
                        controlcode = controlcodeBuilder.ToString();
                    }
                    else
                    {
                        controlcode = "";
                    }
                    break;

                case "Goto":
                    controlcodeBuilder.Append("Goto " + numericUpDown1.Value.ToString() + " "
                       + numericUpDown6.Value.ToString());
                    controlcode = controlcodeBuilder.ToString();
                    break;

                case "Position":
                    controlcode = "Position";
                    break;

                case "Lv":
                    controlcode = "Lv";
                    break;

                case "Flicker":
                    controlcode = "Flicker";
                    break;

            }

        }

        private double minFlicker = 0.0;
        private string minFlickercode,patterncode,text, datacontrol;
        private int codestart, codeend, codestep, steptime, itemNum;
        private uint uinttime;
        public static int runStat = 0;
        private string reset = "AA 97 0F 00 12 48";
        private void controlCodeRun(string str)
        {
            switch (str)
            {
                case " Delay":
                    {
                        int delaytime = Convert.ToInt16(arrstr[1])*1000;
                          uinttime = (uint)delaytime;
                        //System.Threading.Thread.Sleep(steptime);
                        //MessageBox.Show(uinttime.ToString());
                        TimeDelay(uinttime);
                        break;
                    }
                case " Pattern":
                    { 
                        //armCodeSend("AA 07 0F 00 12 48");
                        TimeDelay(500);
                        string patternMark = arrstr[1];
                        switch (patternMark)
                        {
                            case "00":
                                {
                                    patterncode = "AA 02" + "00" + arrstr[2] + "00" + arrstr[3] + "00" + arrstr[4] + "00" + arrstr[5] + "0F 00 12 48";
                                    break;
                                }
                            case "01":
                                {
                                    patterncode = "AA 02" + "00" + arrstr[2] + "00" + arrstr[3] + "0F 00 12 48";

                                    break;
                                }
                            case "02":
                                {
                                    patterncode = "AA 02" + "00" + arrstr[2] + "00" + arrstr[3] + "00" + arrstr[4] + "0F 00 12 48";

                                    break;
                                }
                            case "03":
                                {
                                    patterncode = "AA 02" + "00" + arrstr[2] + "0F 00 12 48";

                                    break;
                                }
                        }
                        armCodeSend(patterncode);
                                break;
                        }
                case " VCOM":
                    {
                        string interstr = "VCOM";

                        ARMCode.fs.intstrEdit(runStat, str);

                        codestart = Convert.ToInt16(arrstr[1], 16); codeend = Convert.ToInt16(arrstr[2], 16); codestep = Convert.ToInt16(arrstr[3],16);
                        steptime = Convert.ToInt16(arrstr[4]);
                        string codebool = arrstr[5];

                        uinttime = (uint)steptime;
                        TimeDelay(uinttime);
                        //System.Threading.Thread.Sleep(steptime);

                        ARMCode.fs.strEdit(interstr);
                         text = ARMCode.textstrControl;                  
                         itemNum = 1;
                        minFlicker = 0.00;
                        DataChart.fs.intEdit(0);
                        for (int codeint = codestart; codeint <= codeend; codeint += codestep)
                        {
                            if (ns1 == 1 && ns2 == 1 && ns3 == 1)
                            {
                                break;
                            }
                            string interText = text;
                            string codestr = codeint.ToString("X2");
                            interText = "AA01" + interText.Replace("VCOM", "02" + codestr) + "0F001248";
                            armCodeSend(interText);

                            uinttime = (uint)steptime;
                            TimeDelay(uinttime);

                            //System.Threading.Thread.Sleep(steptime);

                            CA410.Measurement();

                            double doubJEITA =  CA410.JEITA;
                            double doubFMA = CA410.FMA;

                            if (CA410.JEITAcheck == "true")
                            {
                                datacontrol = Convert.ToString(doubJEITA);
                            }
                            else if (CA410.FMAcheck == "true")
                            {
                                datacontrol = Convert.ToString(doubJEITA);
                            }
                          
                            //datacontrol = Convert.ToString(itemNum);
                            ARMCode.fs.intintstrstrEdit(runStat, itemNum, codestr, datacontrol);
                            DataChart.fs.intstrstrEdit(0, codestr, datacontrol);

                            if (doubJEITA < minFlicker)
                            {
                                minFlicker = doubJEITA;
                                minFlickercode = codestr;
                            }


                            itemNum = itemNum + 1;

                        }
                        
                        ARMCode.fs.intintstrstrEdit(runStat, itemNum,"BestVcom "+minFlickercode , minFlicker.ToString());

                        if (codebool == "true")
                        {
                            string DownloadCode = "AA01" + text.Replace("VCOM", "02" + minFlickercode) + "0F001248";
                            armCodeSend(DownloadCode);
                        }

                        ARMCode.fs.strEdit1(minFlickercode);
                        runStat = runStat + 1;
                        break;
                    }
                case " Gamma":
                    {
                        TimeDelay(500);
                        ARMCode.fs.intstrEdit(runStat, str);

                        codestep = Convert.ToInt16(arrstr[1],16); steptime = Convert.ToInt16(arrstr[2]);
                        itemNum = 1;
                        string itemStr = itemNum.ToString();
                        text = "AA 02 00 00 00 FF 00 FF 00 FF 0F 00 12 48";
                        //armCodeSend(text);
                        DataChart.fs.intEdit(0);
                        for (int codeint = 1; codeint <= 255; codeint += codestep)
                        {
                            if (ns1 == 1 && ns2 == 1 && ns3 == 1)
                            {
                                break;
                            }

                            string interText = text;
                            string codestrx2 = codeint.ToString("X2");
                            string codestr = codeint.ToString();
                            interText = interText.Replace("FF", codestrx2);
                            armCodeSend(interText);

                            uinttime = (uint)steptime;
                            TimeDelay(uinttime);

                            //System.Threading.Thread.Sleep(steptime);

                            CA410.Measurement();
                            datacontrol = Convert.ToString(CA410.Lv);

                            //datacontrol = Convert.ToString(itemNum);

                            ARMCode.fs.intintstrstrEdit(runStat, itemNum, codestr, datacontrol);
                            DataChart.fs.intstrstrEdit(0, itemStr, datacontrol);

                            itemNum = itemNum + 1;
                            itemStr = itemNum.ToString();
                        }
                        runStat = runStat + 1;
                        break;
                    }
                case " Goto":
                    {
                         int thisfirstPosition = richTextBox1.GetFirstCharIndexOfCurrentLine();
                        int thislineNumber = richTextBox1.GetLineFromCharIndex(thisfirstPosition);
                        int gotoLine = Convert.ToInt16(arrstr[1]), cycletimes = Convert.ToInt16(arrstr[2]);
                        while (cycletimes > 0)
                        {
                            for (int igoto = gotoLine; igoto < thislineNumber; igoto++)
                            {
                                if (ns1 == 1 && ns2 == 1 && ns3 == 1)
                                {
                                    break;
                                }

                                firstCharPosition = richTextBox1.GetFirstCharIndexFromLine(igoto);
                                currentLineNum = igoto;
                                HighlightCurrentLine();
                                richTextBox1.Lines[igoto].Trim();
                                string str2 = Regex.Replace(richTextBox1.Lines[igoto], @"/s+", " ");
                                stringSplit(str2);
                                controlCodeRun(item);
                            }
                            cycletimes--;
                        }
                        break;
                    }
                case " DownLoad":
                    {
                        string dlCode = "AA01" + textBox3.Text + "0F001248";
                        dlCode = Regex.Replace(dlCode,@"\s+","");
                        armCodeSend(dlCode);
                        break;
                    }

                case " CKH":
                case " CKV":
                case " VGH":
                case " VGL":
                    {
                        TimeDelay(500);
                        codeRun(str);
                        runStat = runStat + 1;
                        //armCodeSend(reset);
                        break;
                    }
               
                case " Position":
                    {
                        MessageBox.Show("请移动位置");
                        break;
                    }
                case " Lv":
                    {
                        CA410.Measurement();

                        datacontrol = Convert.ToString(CA410.Lv);
                        if (Lvset == 0)
                        {
                            ARMCode.fs.intstrEdit(runStat,str);
                            DataChart.fs.intEdit(0);
                            Lvset = runStat;
                            itemNum = 1;
                            runStat = runStat + 1;
                        }
                      
                        ARMCode.fs.intintstrstrEdit(Lvset,itemNum,itemNum.ToString(),datacontrol);
                        DataChart.fs.intstrstrEdit(0, itemNum.ToString(), datacontrol);
                        itemNum = itemNum + 1;  
                        break;
                    }

                case " Flicker":
                    {
                        CA410.Measurement();

                        if (CA410.JEITAcheck == "true")
                        {
                            datacontrol = Convert.ToString(CA410.JEITA);
                        }
                        else if (CA410.FMAcheck == "true")
                        {
                            datacontrol = Convert.ToString(CA410.FMA);
                        }


                        if (Flickerset == 0)
                        {
                            ARMCode.fs.intstrEdit(runStat, str);
                            DataChart.fs.intEdit(0);
                            Flickerset = runStat;
                            itemNum = 1;
                            runStat = runStat + 1;
                        }

                        ARMCode.fs.intintstrstrEdit(Flickerset, itemNum, itemNum.ToString(), datacontrol);
                        DataChart.fs.intstrstrEdit(0, itemNum.ToString(), datacontrol);
                        itemNum = itemNum + 1;
                        break;
                    }

                case " Save":
                    {
                        string name = DateTime.Now.ToLocalTime().ToString();
                        name = Regex.Replace(name,@"[^0-9]","");
                        string savepath = textBox2.Text;
                        savepath = savepath.Replace("\"", "");
                        ARMCode.fs.strstrEdit(savepath, name);
                        break;
                    }

                case " Read":
                    {
                        string strReadFilePath = textBox2.Text;
                        strReadFilePath = strReadFilePath.Replace("\"","");
                        StreamReader srReadFile = new StreamReader(strReadFilePath);
                        string strline;
                        while ((strline = srReadFile.ReadLine()) != null)
                        {
                            strtxt = strline.Trim();
                            strtxt = " " + strtxt;
                            if (strtxt != " ")
                            {
                                strtxt = Regex.Replace(strtxt, @"\s+", " ");
                                strtxt = strtxt.ToUpper();
                                codetxt = ARMCode.codeEdit(strtxt);
                                codetxt = "AA01" + codetxt + "0F001248";
                            }
                        }
                        srReadFile.Close();
                        MessageBox.Show(codetxt);
                        ARMCode.textBuilder.Clear();
                        break;
                    }

            }
       
        }
        public static int Lvset = 0, Flickerset =0;

        private string strtxt = "", codetxt;
      

        private void codeRun(string str)
        {
            string interstr = str.Trim();

            ARMCode.fs.intstrEdit(runStat, str);

            codestart = Convert.ToInt16(arrstr[1], 16); codeend = Convert.ToInt16(arrstr[2], 16); codestep = Convert.ToInt16(arrstr[3],16);
            steptime = Convert.ToInt16(arrstr[4]);

            ARMCode.fs.strEdit(interstr);
            text = ARMCode.textstrControl;
            itemNum = 1;
            DataChart.fs.intEdit(0);

            if (codestart < codeend)
            {
                for (int codeint = codestart; codeint <= codeend; codeint += codestep)
                {
                    if (ns1 == 1 && ns2 == 1 && ns3 == 1)
                    {
                        break;
                    }
                    string interText = text;
                    string codestr = codeint.ToString("X2");
                    interText = "AA01" + interText.Replace(interstr, "02" + codestr) + "0F001248";
                    armCodeSend(interText);

                    uinttime = (uint)steptime;
                    TimeDelay(uinttime);

                    //System.Threading.Thread.Sleep(steptime);

                    CA410.Measurement();

                    double doubLv = CA410.Lv;
                    datacontrol = Convert.ToString(doubLv);
                    //datacontrol = Convert.ToString(itemNum);
                    ARMCode.fs.intintstrstrEdit(runStat, itemNum, codestr, datacontrol);
                    DataChart.fs.intstrstrEdit(0, codestr, datacontrol);

                    itemNum = itemNum + 1;                  
                }
            }
            else
            {
                for (int codeint = codestart; codeint >= codeend; codeint -= codestep)
                {
                    if (ns1 == 1 && ns2 == 1 && ns3 == 1)
                    {
                        break;
                    }
                    string interText = text;
                    string codestr = codeint.ToString("X2");
                    interText = "AA01" + interText.Replace(interstr, "02" + codestr) + "0F001248";
                    armCodeSend(interText);

                    uinttime = (uint)steptime;
                    TimeDelay(uinttime);

                    //System.Threading.Thread.Sleep(steptime);

                    CA410.Measurement();

                    double doubLv = CA410.Lv;
                    datacontrol = Convert.ToString(doubLv);
                    //datacontrol = Convert.ToString(itemNum);
                    ARMCode.fs.intintstrstrEdit(runStat, itemNum, codestr, datacontrol);
                    DataChart.fs.intstrstrEdit(0, codestr, datacontrol);

                    itemNum = itemNum + 1;
                }

            }

            
            
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < richTextBox1.Lines.Length; i++)
            {
                int lineFirstCharIndex = richTextBox1.GetFirstCharIndexFromLine(i);
                string str = richTextBox1.Lines[i].Trim();
                str = Regex.Replace(str, @"\s+", " ");

                richTextBox1.SelectionStart = lineFirstCharIndex;
                richTextBox1.SelectionLength = richTextBox1.Lines[i].Length;
                richTextBox1.SelectedText = str;
            }
            }

        private void Control_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Delay");
            comboBox1.Items.Add("Pattern");
            comboBox1.Items.Add("Gamma");
            comboBox1.Items.Add("VCOM");
            comboBox1.Items.Add("VGH");
            comboBox1.Items.Add("VGL");
            comboBox1.Items.Add("CKH");
            comboBox1.Items.Add("CKV");
            comboBox1.Items.Add("Crosstalk");
            comboBox1.Items.Add("Flicker");
            comboBox1.Items.Add("DownLoad");
            comboBox1.Items.Add("Goto");
            comboBox1.Items.Add("Position");
            comboBox1.Items.Add("Lv");
            comboBox1.Items.Add("Read");
            comboBox1.Items.Add("Save");
            comboBox1.SelectedIndex = 0;
            comboBox2.Items.Add("NA");
            comboBox2.Items.Add("White");
            comboBox2.Items.Add("Black");
            comboBox2.Items.Add("Gray");
            comboBox2.Items.Add("Red");
            comboBox2.Items.Add("Green");
            comboBox2.Items.Add("Blue");
            comboBox2.Items.Add("Flicker");
            comboBox2.Items.Add("PixelCheck");
            comboBox2.Items.Add("DotCheck");
            comboBox2.Items.Add("Crosstalk-W");
            comboBox2.Items.Add("Crosstalk-B");
            comboBox2.SelectedIndex = 0;

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            string comboItem = comboBox1.SelectedItem.ToString();
            controlCodeEdit(comboItem);
            if (controlcode == "")
            {
                MessageBox.Show("Code 为空");
            }
            else
            {
                firstCharPosition = richTextBox1.GetFirstCharIndexOfCurrentLine();
                currentLineNum = richTextBox1.GetLineFromCharIndex(firstCharPosition);
                if (!string.IsNullOrWhiteSpace(richTextBox1.Text))
                {
                    if (richTextBox1.SelectionLength == 0 && richTextBox1.Lines[currentLineNum].Length != 0)
                    { richTextBox1.AppendText("\r\n" + controlcode); }
                    else if (richTextBox1.SelectionLength == 0 && richTextBox1.Lines[currentLineNum].Length == 0)
                    { richTextBox1.Text= richTextBox1.Text.Insert(firstCharPosition, controlcode); }
                    else if (richTextBox1.SelectionLength > 0)
                    {
                        richTextBox1.SelectionStart = firstCharPosition;
                        richTextBox1.SelectionLength = richTextBox1.Lines[currentLineNum].Length;
                        richTextBox1.SelectedText = controlcode;
                    }
                }

                else
                {
                    richTextBox1.AppendText(controlcode);
                }
                controlcodeBuilder.Clear();
            }
            

        }

        //int lastLine = 0;

        private void HighlightCurrentLine()
        {
            // Save current selection
            int selectionStart = firstCharPosition;
            int selectionLength = 0;

            // Get character positions for the current line
            int lastCharPosition = richTextBox1.GetFirstCharIndexFromLine(currentLineNum + 1);
            if (lastCharPosition == -1)
                lastCharPosition = richTextBox1.TextLength;

            // Clear any previous color

            //if (currentLineNum != lastLine)
            //{
            //    int previousFirstCharPosition = richTextBox1.GetFirstCharIndexFromLine(lastLine);
            //    int previousLastCharPosition = richTextBox1.GetFirstCharIndexFromLine(lastLine + 1);
            //    if (previousLastCharPosition == -1)
            //    {
            //        previousLastCharPosition = richTextBox1.TextLength;
            //        richTextBox1.SelectionStart = previousFirstCharPosition;
            //        richTextBox1.SelectionLength = previousLastCharPosition - previousFirstCharPosition;
            //        richTextBox1.SelectionBackColor = SystemColors.Window;
            //    }
            //    else
            //    {
            //        richTextBox1.SelectionStart = previousFirstCharPosition;
            //        richTextBox1.SelectionLength = previousLastCharPosition - previousFirstCharPosition;
            //        richTextBox1.SelectionBackColor = SystemColors.Window;
            //    }
            //   lastLine = currentLineNum;
            //}

            richTextBox1.SelectionStart = 0;
            richTextBox1.SelectionLength = richTextBox1.Text.Length;
            richTextBox1.SelectionBackColor = SystemColors.Window;

            // Set new color
            richTextBox1.SelectionStart = firstCharPosition;
            richTextBox1.SelectionLength = lastCharPosition - firstCharPosition;
            if (richTextBox1.SelectionLength > 0)
                richTextBox1.SelectionBackColor = Color.PaleTurquoise;

            // Reset selection
            richTextBox1.SelectionStart = selectionStart;
            richTextBox1.SelectionLength = selectionLength;
        }


        private void LineMoveUp()
        {
            int firstCharPosition = richTextBox1.GetFirstCharIndexOfCurrentLine();
            int lineNumber = richTextBox1.GetLineFromCharIndex(firstCharPosition);

            int lastCharPosition = richTextBox1.GetFirstCharIndexFromLine(lineNumber + 1);
            if (lastCharPosition == -1)
                lastCharPosition = richTextBox1.TextLength;

           int  moveLength= lastCharPosition - firstCharPosition;

            int upCharPosition = richTextBox1.GetFirstCharIndexFromLine(lineNumber - 1);
            if (upCharPosition == -1)
                upCharPosition = 0;

            richTextBox1.SelectionStart = firstCharPosition;
            richTextBox1.SelectionLength = lastCharPosition - firstCharPosition;


            cutText();
            richTextBox1.SelectionStart = upCharPosition;
            richTextBox1.SelectionLength = 0;
            pasteText(upCharPosition, moveLength);
        }

        private void LineMoveDown()
        {
            int firstCharPosition = richTextBox1.GetFirstCharIndexOfCurrentLine();
            int lineNumber = richTextBox1.GetLineFromCharIndex(firstCharPosition);

            int lastCharPosition = richTextBox1.GetFirstCharIndexFromLine(lineNumber + 1);
            if (lastCharPosition == -1)
                lastCharPosition = richTextBox1.TextLength;

            int moveLength = lastCharPosition - firstCharPosition;

            int setCharPosition = richTextBox1.GetFirstCharIndexFromLine(lineNumber + 2);
            if (setCharPosition == -1)
            {
                richTextBox1.Text = richTextBox1.Text.Insert(richTextBox1.TextLength,"\n");
                setCharPosition = richTextBox1.TextLength;
            }
            richTextBox1.SelectionStart = firstCharPosition;
            richTextBox1.SelectionLength = lastCharPosition - firstCharPosition;


            cutTextDown();
            richTextBox1.SelectionStart = setCharPosition-moveLength;
            richTextBox1.SelectionLength = 0;
            pasteTextDown(setCharPosition - moveLength, moveLength,firstCharPosition);
        }

        int currentLineNum = 0;
        int firstCharPosition = 0;
        private void richTextBox1_Click(object sender, EventArgs e)
        {
            
        }
        string midcutText;
        private void cutText()
        {
            Clipboard.Clear();
            if (richTextBox1.SelectionLength > 0)
            {
                if (richTextBox1.SelectedText.Contains("\n"))
                {
                     midcutText = richTextBox1.SelectedText;
                }
                else
                {
                    midcutText = richTextBox1.SelectedText + "\n";
                }
                    //cut selected text to clipboard
                    Clipboard.SetText(midcutText);
                richTextBox1.SelectedText ="";
            }
            else
            {
                MessageBox.Show("Please select text to modify");
            }
        }

        private void cutTextDown()
        {
            Clipboard.Clear();
            if (richTextBox1.SelectionLength > 0)
            {
                if (richTextBox1.SelectedText.Contains("\n"))
                {
                    midcutText = richTextBox1.SelectedText;
                }
                else
                {
                    midcutText = richTextBox1.SelectedText + "\n";
                }
                //cut selected text to clipboard
                Clipboard.SetText(midcutText);
                richTextBox1.SelectedText = "";
            }
            else
            {
                MessageBox.Show("Please select text to modify");
            }
        }


        private void copyText()
        {
            Clipboard.Clear();
            if (richTextBox1.SelectionLength > 0)
            {
                //copies selected text to clipboard
                Clipboard.SetText(richTextBox1.SelectedRtf);
            }
            else
            {
                MessageBox.Show("Please select text to modify");
            }

        }

        private void pasteText(int indexPosition,int nextindexPositon)
        {
            if (Clipboard.ContainsText())
            {
                //pastes text on clipboard to richtextbox
                string cutText = Clipboard.GetText();
                richTextBox1.Text= richTextBox1.Text.Insert(indexPosition, cutText);
                richTextBox1.SelectionStart = indexPosition;
                richTextBox1.SelectionLength = nextindexPositon;
            }
            else
            {
                MessageBox.Show("Please select text to modify");
            }
        }

        private void pasteTextDown(int index, int length,int downindex)
        {
            if (Clipboard.ContainsText())
            {
                //pastes text on clipboard to richtextbox
                string cutText = Clipboard.GetText();
                richTextBox1.Text = richTextBox1.Text.Insert(index, cutText);
                richTextBox1.SelectionStart = index;
                richTextBox1.SelectionLength = length;
            }
            else
            {
                MessageBox.Show("Please select text to modify");
            }
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            try
            {
                LineMoveUp();
                HighlightCurrentLine();
            }
            catch (Exception)
            {
            }
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            try
            {
                LineMoveDown();
                HighlightCurrentLine();
            }
            catch (Exception)
            {
                MessageBox.Show("NG");
            }

        }

   
        public static PictureBox temp = new PictureBox();
        private int step = 5;
        private void pictureBox3_MouseEnter(object sender, EventArgs e)
        {
           
            temp = (PictureBox)sender;
           int x= temp.Location.X;
            int y = temp.Location.Y;
            temp.Location = new Point(x-step/2,y-step/2);
            int w = temp.Width;
            int h = temp.Height;
            temp.Width = w+step;
            temp.Height = h+step;
 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 14)
            {
                textBox2.Text = "";
                FolderBrowserDialog folder = new FolderBrowserDialog();
                if (folder.ShowDialog() == DialogResult.OK)
                {
                    textBox2.Text = folder.SelectedPath;
                }
            }
            else if (comboBox1.SelectedIndex == 13)
            {
                textBox2.Text = "";
                OpenFileDialog dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    textBox2.SelectedText = dialog.FileName;

                }
            }

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox2.Text)
            {
                case "White":
                    textBox1.Text = "00 00 FF FF FF";
                    break;

                case "Black":
                    textBox1.Text = "00 00 00 00 00";
                    break;

                case "Gray":
                    textBox1.Text = "00 00 7F 7F 7F";
                    break;

                case "Red":
                    textBox1.Text = "00 00 FF 00 00";
                    break;

                case "Green":
                    textBox1.Text = "00 00 00 FF 00";
                    break;

                case "Blue":
                    textBox1.Text = "00 00 00 00 FF";
                    break;

                case "Flicker":
                    textBox1.Text = "";
                    break;

                case "PixelCheck":
                    textBox1.Text = "01 17 7F";
                    break;

                case "DotCheck":
                    textBox1.Text = "01 18 7F";
                    break;

                case "Crosstalk-W":
                    textBox1.Text = "01 07 7F";
                    break;

                case "Crosstalk-B":
                    textBox1.Text = "01 01 7F";
                    break;
            }
        }

        private void richTextBox1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                // Get character positions for the current line
                firstCharPosition = richTextBox1.GetFirstCharIndexOfCurrentLine();
                currentLineNum = richTextBox1.GetLineFromCharIndex(firstCharPosition);
                int nextCharPositon = richTextBox1.GetFirstCharIndexFromLine(currentLineNum + 1);
                if (nextCharPositon == -1)
                {
                    nextCharPositon = richTextBox1.TextLength;
                }
                HighlightCurrentLine();

            }
            catch (Exception)
            {
            }
        }

        private void pictureBox3_MouseLeave(object sender, EventArgs e)
        {
            temp = (PictureBox)sender;
            int x = temp.Location.X;
            int y = temp.Location.Y;
            temp.Location = new Point(x + step/2, y + step/2);
            int w = temp.Width;
            int h = temp.Height;
            temp.Width = w - step;
            temp.Height = h - step;
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        SaveFileDialog saveFileDialog1 = new SaveFileDialog();
        SaveFileDialog openFileDialog1 = new SaveFileDialog();

        string downcom="false" ;

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                downcom = "true";
            }
            else
            {
                downcom = "false";
            }
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text == "")
                return;
            saveFileDialog1.DefaultExt = "txt";
            saveFileDialog1.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string FileName = saveFileDialog1.FileName;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK && FileName.Length > 0)
            {
                // Save the contents of the RichTextBox into the file.
                richTextBox1.SaveFile(saveFileDialog1.FileName, RichTextBoxStreamType.PlainText);
                MessageBox.Show("文件已成功保存");
            }
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fName = openFileDialog1.FileName;
                richTextBox1.LoadFile(fName, RichTextBoxStreamType.PlainText);
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

    }
    }

