using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using IV.Common;
using LabelManager2;
using System.Data.SqlClient;
using IV.SQLServerDAL;
namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 判断花色
        /// </summary>
        /// <param name="_lot"></param>
        /// <returns></returns>
        private System.Drawing.Image getImageDataFromOracle(string _lot)
        {
            //通过组件批次号 查询到bom 通过bom查询 iv_test查询功率 和电流
            string _ivDataSQL1 = "select  COEF_PMAX,PS_SUBCODE from ZWIP_IV_TEST_PRINTLOG where   PS_SUBCODE is not null  and LOT_NUMBER='" + _lot + "' ";
            DataSet ds_iv1 = SQLServerDALServer.Query(_ivDataSQL1);
            string sql = "select Picture,PS_SUBCODE,PS_SUBNAME,MAX_VALUE from ZFMM_POWERSET_DETAIL  where PS_CODE='05w' and ITEM_NO=2 and PS_SUBCODE='γ'";

            byte[] fileData = (byte[])SQLServerDALServer.GetSingle(sql);
            System.IO.MemoryStream ms = new System.IO.MemoryStream(fileData);
            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
            return img;
            
        }
        /// <summary>
        /// 判断功率
        /// </summary>
        /// <param name="strMsg"></param>
        public int PMShowMsg(float strPM)
        {
            if (strPM >= 240 && strPM < 245)
            {//240
                return 240;
            }
            else if (strPM >= 245 && strPM < 250)
            {//245
                return 245;
            }
            else if (strPM >= 250 && strPM < 255)
            {//250
                return 250;
            }
            else if (strPM >= 255 && strPM < 260)
            {//255
                return 255;
            }
            else if (strPM >= 260 && strPM < 265)
            {//260
                return 260;
            }
            else if (strPM >= 265 && strPM < 270)
            {//265
                return 265;
            }
            return 0;

        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            //判断批次号位数
            if (txtContent.Text.Trim().Length == 15)
            {
                pictureBox1.Image = getImageDataFromOracle(txtContent.Text.Trim());// System.Drawing.Image.FromFile(@"图片路径");//图片显示
                //功率

                //判断iv数据和子档位 并打印数据

                ApplicationClass lbl = new ApplicationClass();

                try
                {
                    lbl.Documents.Open(@"D:\JNP-0002.Lab", false);// 调用设计好的label文件
                    Document doc = lbl.ActiveDocument;
                    //int _i=   doc.Variables.FormVariables.Count;
                    doc.Variables.FormVariables.Item("ProductType").Value = txtContent.Text.Trim(); //给参数传值
                    // doc.Variables.FormVariables.Item("Var1").Value = txtContent2.Text.Trim(); //给参数传值

                    int Num = 1;// Convert.ToInt32(txtQuentity.Text);        //打印数量
                    //doc.PrintDocument(Num);                             //打印
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
            
            }
           
        }
    }
}
