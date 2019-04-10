using LabelManager2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using IV.Common;
using IV.SQLServerDAL;
namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 注意：1. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            ApplicationClass lbl = new ApplicationClass();

            if (txtContent.Text.Trim().Length == 15)
            {
                if (txtContent2.Text.Trim().Length == 15)
                {
                    string _LotNumber = txtContent.Text.Trim().Substring(2, txtContent.Text.Trim().Length - 2);
                    string _LotNumber2 = txtContent2.Text.Trim().Substring(2, txtContent2.Text.Trim().Length - 2);
                    long _num = long.Parse(_LotNumber2) - long.Parse(_LotNumber);
                    string _prefix = "JN";
                    string _Lot = "";
                     try
                        {
                            for (int i = 0; i < _num+1;i++ )
                            {
                                _Lot = _prefix + (long.Parse(_LotNumber) + i);
                                    //数据库
                                   string _lotsql = "select count(*) _con from wip_lot where LOT_NUMBER='" + _Lot + "'";
                                    DataSet ds = IV.SQLServerDAL.SQLServerDALServer.Query(_lotsql);
                                    if (ds.Tables[0].Rows.Count>0)
                                    {
                                       lbl.Documents.Open(@"D:\label.Lab", false);// 调用设计好的label文件
                                        Document doc = lbl.ActiveDocument;

                                        doc.Variables.FormVariables.Item("LotNumber").Value = _Lot; //给参数传值
                                        //doc.Variables.FormVariables.Item("Var1").Value = txtContent2.Text.Trim(); //给参数传值

                                        int Num = Convert.ToInt32(txtQuentity.Text);        //打印数量
                                        doc.PrintDocument(Num);    //打印
                                 }
                            }
                        }
                     catch (Exception ex)
                     {
                         MessageBox.Show(ex.Message);
                     }
                     finally
                     {
                         lbl.Quit();                                         //退出
                     }
                   
                }
                else { 
                  //位数不正确
                }
            }
            else { 
            
            }            
        }
        /// <summary>
        /// 指令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            LPTControl Print = new LPTControl();
            StringBuilder psb = new StringBuilder();
            psb.Append("^XA");
            psb.Append("^MCY");
            psb.Append("^XZ");
            psb.Append("^XA");
            psb.Append("^FWN^CFD,24^PW1240^LH0,0");
            psb.Append("^CI0^PR2^MNY^MTT^MMT^MD0^PON^PMN^LRN");
            psb.Append("^XZ");
            psb.Append("^XA");
            psb.Append("^MCY");
            psb.Append("^XZ");
            psb.Append("^XA");
            psb.Append("^DFR:TEMP_FMT.ZPL");
            psb.Append("^LRN");
            psb.Append("^XZ");
            psb.Append("^MCY");
            psb.Append("^XZ");
            psb.Append("^XA");
            psb.Append("^XFR:TEMP_FMT.ZPL");
            psb.Append("^BY2^FO51,18^BCN,113,Y,N,N^FV>:1111^FS");
            psb.Append("^PQ1,0,1,Y");
            psb.Append("^XZ");
            psb.Append("^XA");
            psb.Append("^IDR:TEMP_FMT.ZPL");
            psb.Append("^XZ");
            Print.Write(psb.ToString());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.txtContent.Focus();
            this.button2.Enabled = false;
        }
    }
}
