using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WebServiceTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }
        Service.TradLevelSoapClient m_soap = new Service.TradLevelSoapClient();
        private void button1_Click(object sender, EventArgs e)
        {
           try
           {
               string m_retStr = m_soap.SearhModulePosition(textBox1.Text);
               listBox1.Items.Add(m_retStr);
           }
           catch (System.Exception ex)
           {
               string m_retStr = m_soap.SearhModulePosition(textBox1.Text);
               listBox1.Items.Add(ex.Message);
           }
            
         
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int m_workShopId =0;
            int m_flowId = 0;
            int m_level = 0;
            if (radioButton1.Checked) m_workShopId = 1;
            if (radioButton2.Checked) m_workShopId = 2;
           
            if (radioButton3.Checked) m_flowId = 1;
            if (radioButton4.Checked) m_flowId = 2;
            if (radioButton5.Checked) m_flowId = 3;

            if (radioButton6.Checked) m_level = 1;
            if (radioButton7.Checked) m_level = 2;
            if (radioButton8.Checked) m_level = 3;
            if(m_workShopId!=0 && m_flowId!=0 && m_level!=0 && textBox2.Text.Trim().Length>0)
            {
                try
                {
                    bool m_retFlag = m_soap.SetTradLevel(m_workShopId, m_flowId, "JN1231517212814", m_level);
                    listBox1.Items.Add("Exec SetTradLevel() Ret=" + m_retFlag.ToString());
                }
                catch (System.Exception ex)
                {
                    listBox1.Items.Add("please check JNModuleCYController102A.exe or JNModuleCYController102B.exe is Runing!");
                }
               
            }
            else
            {
                listBox1.Items.Add("Exec Set TradLevel() Ret=false,please check Parameters");
            }
          
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int m_workShopId = 0;
            int m_flowId = 0;
            int m_lineState = 0;
            if (radioButton1.Checked) m_workShopId = 1;
            if (radioButton2.Checked) m_workShopId = 2;

            if (radioButton3.Checked) m_flowId = 1;
            if (radioButton4.Checked) m_flowId = 2;
            if (radioButton5.Checked) m_flowId = 3;

            if (radioButton9.Checked) m_lineState = 1;
            if (radioButton10.Checked) m_lineState = 0;

            if (m_workShopId != 0 && m_flowId != 0)
            {
                try
                {
                    bool m_retFlag = m_soap.SetLineState(m_workShopId, m_flowId,Convert.ToInt32(comboBox1.Text), m_lineState);
                    listBox1.Items.Add("Exec SetLineState() Ret=" + m_retFlag.ToString());
                }
                catch (System.Exception ex)
                {
                    listBox1.Items.Add("please check JNModuleCYController102A.exe or JNModuleCYController102B.exe is Runing!");
                }
               
            }
            else
            {
                listBox1.Items.Add("Exec Set SetLineState() Ret=false,please check Parameters");
            }
        }
    }
}
