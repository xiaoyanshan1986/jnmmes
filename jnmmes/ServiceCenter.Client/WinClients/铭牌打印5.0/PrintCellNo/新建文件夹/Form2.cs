using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LabelManager2;
using IV.SQLServerDAL;
namespace WindowsFormsApplication2
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox2.Text.Equals(""))
            {
                MessageBox.Show("请选择品质等级！");
                return;
            }
            else if (comboBox1.Text.Equals(""))
            {
                MessageBox.Show("请选择颜色！");
                return;
            }
            else if (comboBox3.Text.Equals(""))
            {
                MessageBox.Show("请选择效率！");
                return;
            }
            else if (comboBox4.Text.Equals(""))
            {
                MessageBox.Show("请选择线别 ！");
                return;
            }
            ApplicationClass lbl = new ApplicationClass();
            lbl.Documents.Open(@"C:\Users\Administrator\Desktop\CellPrint\JCP-0001.Lab", false);// 调用设计好的label文件
            Document doc = lbl.ActiveDocument;
            string _data = System.DateTime.Now.ToString("yyyy-MM-dd");
            //1.自增1 2.日期3.最大值
            //和客户端电脑日期做对比，如果有就取最大值 ，反之插入对应数据 最大值从001开始
            string _PackageNo = "";
            DataSet ds = DbHelperAccess.Query("select Maxids from PackagePrint where Dates='" + _data + "'");

            try
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string _len = "";
                    switch (ds.Tables[0].Rows[0][0].ToString().Length)
                    {
                        case 1:
                            _len = "000";
                            _PackageNo = "C" + Convert.ToDateTime(_data).ToString("yyMMdd") +this.comboBox4.Text.Trim() + _len + (int.Parse(ds.Tables[0].Rows[0][0].ToString()) + 1);
                            //update
                            //插入SQl语句
                            string _update = "update PackagePrint set Maxids=" + (int.Parse(ds.Tables[0].Rows[0][0].ToString()) + 1).ToString() + " where Dates= '" + _data + "'";
                            DbHelperAccess.ExecuteSql(_update);
                            break;
                        case 2:
                            _len = "00";
                            _PackageNo = "C" + Convert.ToDateTime(_data).ToString("yyMMdd") + this.comboBox4.Text.Trim() + _len + (int.Parse(ds.Tables[0].Rows[0][0].ToString()) + 1);
                            //update
                            //插入SQl语句
                            string _update1 = "update PackagePrint set Maxids=" + (int.Parse(ds.Tables[0].Rows[0][0].ToString()) + 1).ToString() + " where Dates= '" + _data + "'";
                            DbHelperAccess.ExecuteSql(_update1);
                            break;
                        case 3:
                            _len = "0";
                            _PackageNo = "C" + Convert.ToDateTime(_data).ToString("yyMMdd") + this.comboBox4.Text.Trim() + _len + (int.Parse(ds.Tables[0].Rows[0][0].ToString()) + 1);
                            //update
                            //插入SQl语句
                            string _update2 = "update PackagePrint set Maxids=" + (int.Parse(ds.Tables[0].Rows[0][0].ToString()) + 1).ToString() + " where Dates= '" + _data + "'";
                            DbHelperAccess.ExecuteSql(_update2);
                            break;
                        default:
                            _len = "";
                            _PackageNo = "C" + Convert.ToDateTime(_data).ToString("yyMMdd") + this.comboBox4.Text.Trim() + _len + (int.Parse(ds.Tables[0].Rows[0][0].ToString()) + 1);
                            //update
                            //插入SQl语句
                            string _update3 = "update PackagePrint set Maxids=" + (int.Parse(ds.Tables[0].Rows[0][0].ToString()) + 1).ToString() + " where Dates= '" + _data + "'";
                            DbHelperAccess.ExecuteSql(_update3);
                            break;

                    }

                }
                else
                {

                    _PackageNo = "C" + Convert.ToDateTime(_data).ToString("yyMMdd") + this.comboBox4.Text.Trim() + "0001";
                    string _NO = "insert into PackagePrint(Dates,Maxids) values ('" + _data + "','0001')";//插入SQl语句
                    DbHelperAccess.ExecuteSql(_NO);

                }

                doc.Variables.FormVariables.Item("PackageNo").Value = _PackageNo; //给参数传值Package号
                //doc.Variables.FormVariables.Item("Line").Value = this.comboBox4.Text.Trim();//给参数传值线别
                doc.Variables.FormVariables.Item("Grade").Value = this.comboBox2.Text.Trim(); //给参数传值等级
                doc.Variables.FormVariables.Item("Eff").Value = this.comboBox3.Text.Trim(); //给参数传值效率档
                doc.Variables.FormVariables.Item("Color").Value = this.comboBox1.Text.Trim(); //给参数传值颜色
                doc.Variables.FormVariables.Item("QTY").Value = this.textBox2.Text.Trim();//给参数传值数量
               //doc.PrintDocument(1); //打印
                //数据插入的SQL语句
                string _in = "insert into PackageRecord(PackageNo,Grade,Eff,Color,QTY) values ('" + this.textBox1.Text.Trim() + "','" + this.comboBox2.Text.Trim() + "','" + this.comboBox3.Text.Trim() + "','" + this.comboBox1.Text.Trim() + "','" + this.textBox2.Text.Trim() + "');";//插入SQl语句
                DbHelperAccess.ExecuteSql(_in);

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

        private void ID_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
