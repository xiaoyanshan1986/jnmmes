using LabelManager2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        public static DataTable ExcelToDataTable(string strExcelFileName, string strSheetName)
        {
            //源的定义
            // string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + strExcelFileName + ";" + "Extended Properties='Excel 8.0;HDR=NO;IMEX=1';";
            string connString = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + strExcelFileName + ";" + "Extended Properties='Excel 12.0;HDR=YES;IMEX=1';";
            //Sql语句
            //string strExcel = string.Format("select * from [{0}$]", strSheetName); 这是一种方法


            //定义存放的数据表
            DataSet ds = new DataSet();

            //连接数据源
            OleDbConnection conn = new OleDbConnection(connString);

            conn.Open();

            string tableName = "Sheet1";
            string strExcel = "select * from [Sheet1$]";
            OleDbDataAdapter myCmd = null; ;
            myCmd = new OleDbDataAdapter(strExcel, conn);
            // ds = new DataSet();
            myCmd.Fill(ds, "table1");

            conn.Close();

            return ds.Tables["table1"];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable myT = ExcelToDataTable("D:/电池人员名单.xlsx", "sheet1");
            dataGridView1.DataSource = null;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = myT;
        }
         
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    ApplicationClass lbl = new ApplicationClass();
                    string _str = Convert.ToString(dataGridView1.Rows[i].Cells[0].Value);
                    string _str1 = Convert.ToString(dataGridView1.Rows[i].Cells[1].Value);
                    lbl.Documents.Open(@"C:\Users\Administrator\Desktop\Printer\NameLable.Lab", false);// 调用设计好的label文件
                    Document doc = lbl.ActiveDocument;
                    doc.Variables.FormVariables.Item("Number").Prefix = _str; //给参数传值lot号
                    doc.Variables.FormVariables.Item("Name").Value = _str1; //给参数传值bom号
                    doc.PrintDocument(1); //打印
                }

            }
        }
    }
}
