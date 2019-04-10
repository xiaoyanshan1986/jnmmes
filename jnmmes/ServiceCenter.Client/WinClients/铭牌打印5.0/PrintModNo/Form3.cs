using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using IV.Common;
using IV.SQLServerDAL;
using LabelManager2;
namespace WindowsFormsApplication1
{
    public partial class Form3 : Form
    {
        public Form3()
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
                        for (int i = 0; i < _num + 1; i++)
                        {
                            _Lot = _prefix + (long.Parse(_LotNumber) + i);
                            //数据库
                            string _lotsql = "select ORDER_NUMBER from wip_lot where LOT_NUMBER='" + _Lot + "'";
                            DataSet ds = IV.SQLServerDAL.SQLServerDALServer.Query(_lotsql);
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                lbl.Documents.Open(@"D:\JNL-0004.Lab", false);// 调用设计好的label文件
                                Document doc = lbl.ActiveDocument;
                                
                                doc.Variables.FormVariables.Item("LotNumber").Prefix = "JN"; //给参数传值lot号
                                doc.Variables.FormVariables.Item("LotNumber").Shared = true;
                                doc.Variables.FormVariables.Item("LotNumber").Value = "1231";
                                doc.Variables.FormVariables.Item("LotNumber").Increment ="+1";
                               // doc.Variables.FormVariables.Item("LotNumber").Value = "JN2230913216098"; //给参数传值lot号
                                doc.Variables.FormVariables.Item("WorkNoNUmber").Value = ds.Tables[0].Rows[0][0].ToString(); //给参数传值bom号

                                int Num = Convert.ToInt32(txtQuentity.Text);        //打印数量                               
                                //doc.Variables.FormVariables.Item("LotNumber").Value = "JN2230913216099" ;
                                //doc.Variables.FormVariables.Item("WorkNoNUmber").Value = ds.Tables[0].Rows[0][0].ToString();
                                //doc.PrintLabel(1,1,1,1,1,)
                                //doc.PrintLabel(1, 3, 1, 1, 1, "JN2230913216099");//1.数量2.第一行打印几个标签(几列)4.列打印几个标签
                                doc.PrintDocument(Num); //打印
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
                else
                {
                    //位数不正确
                }
            }
            else
            {

            }
        }
    }
}
