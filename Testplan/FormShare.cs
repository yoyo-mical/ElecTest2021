using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testplan
{
    public delegate void strdelegate (string str);
    public delegate void intstrstrdelegate(int i,string code,string data);
    public delegate void intdelegate(int i);
    public delegate void intstrfloatdelegate(int i, string x, float y);
    public delegate void intstrdelegate(int i ,string str);
    public delegate void strstrdelegate(string str1,string str2);
    public delegate void intintstrstrdelegate(int i1, int i2, string str1, string str2);

  public  class FormShare
    {
        public event strdelegate strevent;
        public event strdelegate strevent1;
        public event intstrstrdelegate intstrstrevent;
        public event intdelegate intevent;
        public event intstrfloatdelegate intstrfloatevent;
        public event intstrdelegate intstrevent;
        public event strstrdelegate strstrevent;
        public event intintstrstrdelegate intintstrstrevent;

        public void strEdit(string str)
        {
            if (str != "")
            {
                strevent(str);
            }
        }

        public void strEdit1(string str1)
        {
            if (str1 != "")
            {
                strevent1(str1);
            }
        }



        public void intstrEdit(int i, string str)
        {
            intstrevent(i, str);
        }

        public void strstrEdit(string str1, string str2)
        {
            strstrevent(str1,str2);
        }
        

        public void intstrstrEdit(int i, string code, string data)
        {
            intstrstrevent(i, code, data);
        }

        public void intEdit(int i)
        {
            intevent(i);
        }

        public void intstrfloatEdit(int i, string x, float y)
        {
            intstrfloatevent(i, x, y);
        }

        public void intintstrstrEdit(int i1, int i2, string str1, string str2)
        {
            intintstrstrevent(i1, i2, str1, str2);
        }

    }
}
