using LabelManager2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form7 : Form
    {
        public Form7()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string Barcode = this.textBox1.Text.Trim();        //获取批次号

           // if (Barcode.Substring(0, 1).Equals("J"))
            
                string _Barcode = Barcode.Substring(0, 15);
                if (_Barcode.Length == 15)
                {
                    if (_Barcode.Substring(0, 2).Equals("JN"))
                    {
                        // Console.WriteLine(Barcode);
                        ApplicationClass lbl = new ApplicationClass();
                        try
                        {
                            //数据库
                            string _lotsql = "select PACKAGE_NO from wip_lot where LOT_NUMBER='" + _Barcode + "'";
                            DataSet ds = IV.SQLServerDAL.SQLServerDALServer.Query(_lotsql);
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                string _printsql = "select PACKAGE_Print from WIP_PACKAGE where PACKAGE_Print=1 and PACKAGE_NO='" + ds.Tables[0].Rows[0][0].ToString() + "'";
                                DataSet printds = IV.SQLServerDAL.SQLServerDALServer.Query(_printsql);
                                if (printds.Tables[0].Rows.Count > 0)
                                {
                                    return;
                                }
                                else
                                {
                                    lbl.Documents.Open(@"D:\Package.Lab", false);// 调用设计好的label文件
                                    Document doc = lbl.ActiveDocument;
                                    string strPackageNo = ds.Tables[0].Rows[0][0].ToString();
                                    doc.Variables.FormVariables.Item("PackageNo").Value = strPackageNo; //给参数传值
                                    doc.PrintDocument(1);    //打印
                                    //数据插入的SQL语句
                                    string _print = "update WIP_PACKAGE set PACKAGE_Print=1 where PACKAGE_NO='" + ds.Tables[0].Rows[0][0].ToString() + "'"; //插入SQl语句
                                    IV.SQLServerDAL.SQLServerDALServer.ExecuteSql(_print);


                                }

                            }
                        }
                        catch (Exception ex1)
                        {
                            //MessageBox.Show(ex1.Message);
                        }
                        finally
                        {
                            lbl.Quit();                                         //退出
                        }

                    }
                    else
                    {

                    }
                }
            

        }
    }
}
