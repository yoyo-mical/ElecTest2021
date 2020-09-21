using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Testplan
{

    public partial class Pattern :DockContent
    {
  
        public Pattern()
        {
            InitializeComponent();
        }

        private void CodeSend()
        {
            if (UART.isOpen == true)
            {
                int n = 0;
                patterncode();
                try
                {
                    if (UART.isHex)
                    {
                        UART.HexSend(strcode);
                        string text = "";
                        for (int i = 0; i < UART.values.Length; i++)
                        {
                            text += UART.values[i].ToString("X2");
                        }
                        //MessageBox.Show(Convert.ToString(text));
                    }
                    else
                    {
                        UART.sp.Write(strcode);
                        n = strcode.Length;

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


        private int a = 0;
        private PictureBox boxtemp = new PictureBox();
        private void imagesizechange(object sender)
        {
            boxtemp = (PictureBox)sender;
            switch (a)
            {
                case 0:
                    {
                        boxtemp.Height = 110;
                        boxtemp.Width = 80;
                        break;
                    }
                case 1:
                    {
                        boxtemp.Height = 100;
                        boxtemp.Width = 70;
                        break;
                    }
            }         
         }

        private string codetext,ptnNum="00",varR="00",varG="00", varB="00",strcode, k;
        private string  FinalptnNum = "00",FinalvarR = "00", FinalvarG = "00", FinalvarB = "00";
        //private byte[] hexptnNum, hexvarR, hexvarG, hexvarB;
        private int intptnNum, intvarR, intvarG, intvarB,b;

        private Label labeltemp = new Label();

        private void label3_MouseLeave(object sender, EventArgs e)
        {
            
            labeltemp.BackColor = Color.FromArgb(0, 0, 0, 0);
            //labeltemp.BorderStyle = System.Windows.Forms.BorderStyle.None;
        }

        private void label4_MouseEnter(object sender, EventArgs e)
        {
            labeltemp = (Label)sender;
            labeltemp.BackColor = Color.FromArgb(255, 0, 255, 0);
            //labeltemp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label6_MouseEnter(object sender, EventArgs e)
        {
            labeltemp = (Label)sender;
            labeltemp.BackColor = Color.FromArgb(255, 200, 200,200);
        }

        private void label6_Click(object sender, EventArgs e)
        {
            intvarR = Convert.ToInt32(numericUpDown4.Value);
            if (intvarR < 10)
            {
                FinalvarR = "0" + Convert.ToString(intvarR, 16);
            }
            else
            {
                FinalvarR = Convert.ToString(intvarR, 16);
            }
            CodeSend();
        }

        private void label5_MouseEnter(object sender, EventArgs e)
        {
            labeltemp = (Label)sender;
            labeltemp.BackColor = Color.FromArgb(255, 0, 0, 255);
        }

        private void label3_MouseEnter(object sender, EventArgs e)
        {
            labeltemp = (Label)sender;
            labeltemp.BackColor = Color.FromArgb(255, 255, 0, 0);
        }

        private void label3_Click(object sender, EventArgs e)
        {
        

            intvarR = Convert.ToInt32(numericUpDown1.Value);
            if (intvarR < 16)
            {
                FinalvarR = "0" + Convert.ToString(intvarR, 16);
            }
            else
            {
                FinalvarR = Convert.ToString(intvarR, 16);
            }
            
            
            intvarG = Convert.ToInt32(numericUpDown2.Value);
            if (intvarG < 16)
            {
                FinalvarG = "0" + Convert.ToString(intvarG, 16);
            }
            else
            {
                FinalvarG = Convert.ToString(intvarG, 16);
            }

            intvarB = Convert.ToInt32(numericUpDown3.Value);
            if (intvarB < 16)
            {
                FinalvarB= "0" + Convert.ToString(intvarB, 16);
            }
            else
            {
                FinalvarB = Convert.ToString(intvarB, 16);
            }

            CodeSend();
            //labeltemp = (Label)sender;
            //labeltemp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        }

        private void intcode()
        {
  
        }

        private void patterncode()
        {
            b = Convert.ToInt16(k);
            switch (b)
            {
                case 0:
                    {
                        strcode = "AA 02" + "00" + FinalptnNum + "00" + FinalvarR + "00" + FinalvarG + "00" + FinalvarB + "0F 00 12 48";
                        break;
                    }
                case 1:
                    {
                        strcode = "AA 02" + "00" + FinalptnNum + "00" + FinalvarR + "0F 00 12 48";
                        break;
                    }
                case 2:
                    {
                        strcode = "AA 02" + "00" + FinalptnNum + "00" + FinalvarR +"00"+ FinalvarG + "0F 00 12 48";
                        break;
                    }
                case 3:
                    {
                        strcode = "AA 02" + "00" + FinalptnNum + "0F 00 12 48";
                        break;
                    }
            }
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            FinalptnNum = ptnNum;
            FinalvarR = varR;
            FinalvarG = varG;
            FinalvarB = varB;

            CodeSend();
            if (b == 0)
            {
                numericUpDown1.Enabled = true;
                numericUpDown2.Enabled = true;
                numericUpDown3.Enabled = true;
                numericUpDown4.Enabled = true;
                numericUpDown1.Value = intvarR;
                numericUpDown2.Value = intvarG;
                numericUpDown3.Value = intvarB;
                numericUpDown4.Value = 0;
                numericUpDown4.Enabled = false;
            }
            else if (b==3)
            {
                numericUpDown1.Enabled = true;
                numericUpDown2.Enabled = true;
                numericUpDown3.Enabled = true;
                numericUpDown4.Enabled = true;
                numericUpDown1.Value = 0;
                numericUpDown2.Value = 0;
                numericUpDown3.Value = 0;
                numericUpDown4.Value = 0;
                numericUpDown1.Enabled = false;
                numericUpDown2.Enabled = false;
                numericUpDown3.Enabled = false;
                numericUpDown4.Enabled = false;
            }
            else 
            {
                numericUpDown1.Enabled = true;
                numericUpDown2.Enabled = true;
                numericUpDown3.Enabled = true;
                numericUpDown4.Enabled = true;
                numericUpDown1.Value = 0;
                numericUpDown2.Value = 0;
                numericUpDown3.Value = 0;
                numericUpDown4.Value = intvarR;
                numericUpDown1.Enabled = false;
                numericUpDown2.Enabled = false;
                numericUpDown3.Enabled = false;
            }

        }

        private void pictureBox1_MouseEnter_1(object sender, EventArgs e)
        {
            a = 0;
            imagesizechange(sender);
            codetext = boxtemp.Tag.ToString();
            string[] codearray = codetext.Split(',');
            for (int i = 0; i <= codearray.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        {
                            k = codearray[i];
                            break;
                        }
                    case 1:
                        {
                            ptnNum = codearray[i];
                            //intptnNum = System.Convert.ToInt32(ptnNum, 16);
                            break;
                        }
                    case 2:
                        {
                            varR = codearray[i];
                            if (varR == "")
                            {
                                intvarR = 0;
                            }
                            else
                            {
                                intvarR = System.Convert.ToInt32(varR, 16);
                            }
                            break;
                        }
                    case 3:
                        {
                            varG = codearray[i];
                            if (varG == "")
                            {
                                intvarG = 0;
                            }
                            else
                            {
                                intvarG = System.Convert.ToInt32(varG, 16);
                            }

                            break;
                        }
                    case 4:
                        {
                            varB = codearray[i];
                            if (varB == "")
                            {
                                intvarB = 0;
                            }
                            else
                            {
                                intvarB = System.Convert.ToInt32(varB, 16);
                            }
                            break;
                        }
                }
            }
        }

        private void pictureBox1_MouseLeave_1(object sender, EventArgs e)
        {
            a = 1;
            imagesizechange(sender); 
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            
        }

        private void pictureBox3_MouseLeave(object sender, EventArgs e)
        {
           
        }

        private void pictureBox4_MouseLeave(object sender, EventArgs e)
        {
       
        }
    }
}
