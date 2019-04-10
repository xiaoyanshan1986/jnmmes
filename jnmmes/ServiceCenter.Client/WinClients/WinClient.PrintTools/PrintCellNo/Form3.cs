using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using LabelManager2;
using System.Text;

using System.Windows.Forms;
using IV.SQLServerDAL;

namespace WindowsFormsApplication2
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox2.Text.Equals(""))
            {
                MessageBox.Show("请选择效率档！");
                return;
            }
            else if (comboBox1.Text.Equals(""))
            {
                MessageBox.Show("请选择等级！");
                return;
            }
            else if (comboBox3.Text.Equals(""))
            {
                MessageBox.Show("请选择颜色！");
                return;
            }
             ApplicationClass lbl = new ApplicationClass();

             string str = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
             string strTemplate = str + "\\" + "JCB-0001.Lab";

             lbl.Documents.Open(strTemplate, false);// 调用设计好的label文件
            
             Document doc = lbl.ActiveDocument;
             string _data = (System.DateTime.Now.AddHours(-8)).ToString("yyyy-MM-dd");
            //1.自增1 2.日期3.最大值
            //和客户端电脑日期做对比，如果有就取最大值 ，反之插入对应数据 最大值从001开始
             string _CartonNo = "";
             DataSet ds = DbHelperAccess.Query("select Maxids from BoxPrint where Dates='" + _data + "'");
             try {
                 if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
             {
                 string strMaxId = ds.Tables[0].Rows[0][0].ToString();
                 int nMaxId = 0;
                 if (string.IsNullOrEmpty(strMaxId) == false)
                 {
                     nMaxId = int.Parse(strMaxId);
                 }
                 nMaxId = nMaxId + 1;
                 strMaxId = nMaxId.ToString();

                 strMaxId = "00" + strMaxId;
                 int nLength = strMaxId.Length;
                 int nStart = nLength - 3;
                 strMaxId = strMaxId.Substring(nStart, 3);

                 _CartonNo = "JNC" + Convert.ToDateTime(_data).ToString("yyMMdd") + strMaxId;

                 string _update = "update BoxPrint set Maxids=" + nMaxId + " where Dates= '" + _data + "'";
                 DbHelperAccess.ExecuteSql(_update);

                 /*
                  string _len = "";
                 switch (ds.Tables[0].Rows[0][0].ToString().Length)
                 {
                     case 1:
                         _len = "00";
                          _CartonNo = "JNC" + Convert.ToDateTime(_data).ToString("yyMMdd") + _len + (int.Parse(ds.Tables[0].Rows[0][0].ToString()) + 1);
                 //update
                 //插入SQl语句
                string _update = "update BoxPrint set Maxids=" + (int.Parse(ds.Tables[0].Rows[0][0].ToString()) + 1).ToString() + " where Dates= '" + _data+"'";
                DbHelperAccess.ExecuteSql(_update);
                         break;
                     case 2:
                         _len = "0";
                          _CartonNo = "JNC" + Convert.ToDateTime(_data).ToString("yyMMdd") + _len + (int.Parse(ds.Tables[0].Rows[0][0].ToString()) + 1);
                 //update
                 //插入SQl语句
                string _update1 = "update BoxPrint set Maxids=" + (int.Parse(ds.Tables[0].Rows[0][0].ToString()) + 1).ToString() + " where Dates= '" + _data+"'";
                DbHelperAccess.ExecuteSql(_update1);
                         break;
                     default:
                         _len = "";
                          _CartonNo = "JNC" + Convert.ToDateTime(_data).ToString("yyMMdd") + _len + (int.Parse(ds.Tables[0].Rows[0][0].ToString()) + 1);
                 //update
                 //插入SQl语句
                string _update2 = "update BoxPrint set Maxids=" + (int.Parse(ds.Tables[0].Rows[0][0].ToString()) + 1).ToString() + " where Dates= '" + _data+"'";
                DbHelperAccess.ExecuteSql(_update2);
                         break;
                 
                 }*/

             }
                 else{

                _CartonNo = "JNC" +Convert.ToDateTime(_data).ToString("yyMMdd") + "001";
                string _NO = "insert into BoxPrint(Dates,Maxids) values ('" + _data + "','001')";//插入SQl语句
                DbHelperAccess.ExecuteSql(_NO);
                
              }
                 //分类编号
                string _PartNo = "";
                string c = "";
                if(this.comboBox3.Text.Trim() == "正蓝"){
                     c = "B";
                }
                else if (this.comboBox3.Text.Trim() == "深蓝")
                {
                    c = "D";
                }
                else if (this.comboBox3.Text.Trim() == "浅蓝")
                {
                    c = "L";
                }

                string _e = "";
                if (this.comboBox2.Text.Trim() == "17.2")
                {
                    _e = "172";
                }
                else if (this.comboBox2.Text.Trim() == "17.4")
                {
                    _e = "174";
                }
                else if (this.comboBox2.Text.Trim() == "17.6")
                {
                    _e = "176";
                }
                else if (this.comboBox2.Text.Trim() == "17.7")
                {
                    _e = "177";
                }
                else if (this.comboBox2.Text.Trim() == "17.8")
                {
                    _e = "178";
                }
                else if (this.comboBox2.Text.Trim() == "17.9")
                {
                    _e = "179";
                }
                else if (this.comboBox2.Text.Trim() == "18.0")
                {
                    _e = "180";
                }
                else if (this.comboBox2.Text.Trim() == "18.1")
                {
                    _e = "181";
                }
                else if (this.comboBox2.Text.Trim() == "18.2")
                {
                    _e = "182";
                }
                else if (this.comboBox2.Text.Trim() == "18.3")
                {
                    _e = "183";
                }

                _PartNo = "P" + this.comboBox1.Text.Trim() + c + _e;

            doc.Variables.FormVariables.Item("CartonNo").Value = _CartonNo; //给参数传值箱号
            doc.Variables.FormVariables.Item("PartNo").Value = _PartNo; //给参数传值分类编号
            doc.Variables.FormVariables.Item("ProdID").Value = this.textBox2.Text.Trim(); //给参数传值产品编号
            doc.Variables.FormVariables.Item("Grade").Value = this.comboBox1.Text.Trim(); //给参数传值等级
            doc.Variables.FormVariables.Item("Eff").Value = this.comboBox2.Text.Trim(); //给参数传值效率档
            doc.Variables.FormVariables.Item("Color").Value = this.comboBox3.Text.Trim(); //给参数传值颜色
            doc.Variables.FormVariables.Item("QTY").Value = this.textBox4.Text.Trim(); //给参数传值数量
            doc.Variables.FormVariables.Item("Date").Value = this.textBox3.Text.Trim(); //给参数传值日期
            doc.PrintDocument(1); //打印
            //数据插入的SQL语句
            string _in = "insert into BoxRecord(ProdID,Grade,Eff,Color,QTY,Dates) values ('" + this.textBox2.Text.Trim() + "','" + this.comboBox1.Text.Trim() + "','" + this.comboBox2.Text.Trim() + "','" + this.comboBox3.Text.Trim() + "','" + this.textBox4.Text.Trim() + "','" + this.textBox3.Text.Trim() + "');";//插入SQl语句
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

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
