using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ELImageDownload
{
    public partial class Form1 : Form
    {
        public  string username;
        DataTable dtNoDownLoadImg = new DataTable();
        DataColumn dc = new DataColumn();
        
        public Form1()
        {
            InitializeComponent();
            dc.ColumnName = "路径";
            dtNoDownLoadImg.Columns.Add(dc);
        }

        private void but_Query_Click(object sender, EventArgs e)
        {
           string packageNo = txt_PackageNo.Text.Trim();
           if (packageNo == "" && packageNo.Length <= 0)
           {
               MessageBox.Show("请输入包装号！");
               return;
           }

           StringBuilder where = new StringBuilder();
           char[] splitChars = new char[] { ',', '$' };
           string[] packageNos = packageNo.TrimEnd(splitChars).Split(splitChars);


           if (packageNos.Length <= 1)
           {
               where.AppendFormat("'" + packageNos[0].Trim() + "'");
           }
           else
           {
               foreach (string pNo in packageNos)
               {
                   where.AppendFormat("'{0}',", pNo.Trim());
               }
               where.Remove(where.Length - 1, 1);
           }
           DataTable dt = new DataTable();
            try
            {
                string strSql = string.Format(@"   SELECT   t2.PACKAGE_NO, t1.LOT_NUMBER, t2.ITEM_NO, t1.ATTRIBUTE_VALUE
                                                          FROM     dbo.WIP_LOT_ATTR AS t1 INNER JOIN
                                                                        dbo.WIP_PACKAGE_DETAIL AS t2 ON t1.LOT_NUMBER = t2.OBJECT_NUMBER
				                                                        WHERE t1.ATTRIBUTE_NAME='EL1ImagePath' and t2.PACKAGE_NO IN({0})
				                                                        order by t2.PACKAGE_NO,t2.ITEM_NO", where);
      
                dt = SqlHelper.ExecuteDataTable(strSql, CommandType.Text);
                dataGridView1.DataSource = dt;


                this.dataGridView1.DataSource = dt;
                int width = this.dataGridView1.Width;
                int avgWidth = width / dt.Columns.Count;//求出每一列的header宽度
                for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
                {
                    this.dataGridView1.Columns[i].Width = avgWidth;//设置每一列的宽度
                }

                //this.dataGridView1.DataSource = dt;//绑定
                //this.dataGridView1.RowHeadersVisible = false;//datagridview前面的空白部分去除
                //this.dataGridView1.ScrollBars = ScrollBars.Vertical;//滚动条去除
                //this.dataGridView1.Width = this.dataGridView1.Columns[0].HeaderCell.Size.Width * 4;//根据column[0]的headercell的width计算整个datagridview的宽度
            }
            catch (Exception ex)
            {
                
            }
            
        }

        private void btn_Download_Click(object sender, EventArgs e)
        {
            string fileName = txt_address.Text.Trim();
            string packageNo = txt_PackageNo.Text.Trim();
            if (fileName == null && fileName.Length <= 0)
            {
                MessageBox.Show("请选择文件夹！");
                return;
            }
            else
            {
                if (Directory.Exists(txt_address.Text.Trim()))
                {
                    if (packageNo == "" && packageNo.Length <= 0)
                    {
                        MessageBox.Show("请输入包装号！");
                        return;
                    }                    
                }
                else
                {
                    MessageBox.Show("文件夹不存在！请创建文件夹");
                    return;
                }
            }

            StringBuilder where = new StringBuilder();
            char[] splitChars = new char[] { ',', '$' };
            string[] packageNos = packageNo.TrimEnd(splitChars).Split(splitChars);


            if (packageNos.Length <= 1)
            {
                where.AppendFormat("'" + packageNos[0].Trim() + "'");
            }
            else
            {
                foreach (string pNo in packageNos)
                {
                    where.AppendFormat("'{0}',", pNo.Trim());
                }
                where.Remove(where.Length - 1, 1);
            }
            DataTable dt = new DataTable();            
            try
            {
                string strSql = string.Format(@"   SELECT   t2.PACKAGE_NO, t1.LOT_NUMBER, t2.ITEM_NO, t1.ATTRIBUTE_VALUE
                                                          FROM     dbo.WIP_LOT_ATTR AS t1 INNER JOIN
                                                                        dbo.WIP_PACKAGE_DETAIL AS t2 ON t1.LOT_NUMBER = t2.OBJECT_NUMBER
				                                                        WHERE t1.ATTRIBUTE_NAME='EL1ImagePath' and t2.PACKAGE_NO IN({0})
				                                                        order by t2.PACKAGE_NO,t2.ITEM_NO", where);

                dataGridView1.DataSource = dt;
                //this.dataGridView1.DataSource = dt;//绑定
                //this.dataGridView1.RowHeadersVisible = false;//datagridview前面的空白部分去除
                //this.dataGridView1.ScrollBars = ScrollBars.Vertical;//滚动条去除
                //this.dataGridView1.Width = this.dataGridView1.Columns[0].HeaderCell.Size.Width * 10;//根据column[0]的headercell的width计算整个datagridview的宽度
                if (dt.Rows.Count > 0)
                {
                    WebClient web = new WebClient();
                    string address = null;
                    this.Hide();
                    Form3 msgFrom = new Form3();
                    msgFrom.Show();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        address = null;                        
                        address = dt.Rows[i]["ATTRIBUTE_VALUE"].ToString();
                        string imageName = address.Split('/').Last<string>();
                        web.DownloadFile(address, fileName +"\\" +imageName);
                        //while (i < dt.Rows.Count)
                        //{

                        //}
                        lab_msg.Text = string.Format("下载第：{0}张完成", i);
                    }
                    msgFrom.Hide();
                    this.Show();
                    MessageBox.Show("下载完成！");

                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
            folderBrowserDialog1.ShowNewFolderButton = true;//显示“新建文件夹”按钮  
            //folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Personal;//只能看到“我的文档”  
            //DialogResult result = folderBrowserDialog1.ShowDialog();//指定标示符,对话框的返回值  
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txt_address.Text = folderBrowserDialog1.SelectedPath;//得到你选择的路径  
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lab_name.Text = username;
            if (username==""&&username.Length<=0)
            {
                MessageBox.Show("无效用户名");
                Application.Exit();
            }
        }

        private void btn_logout_Click(object sender, EventArgs e)
        {
            this.username = null;
            if (MessageBox.Show("您确定要注销登录吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
                System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (MessageBox.Show("您确定要退出登录吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btn_LotQuery_Click(object sender, EventArgs e)
        {
            string lotNo = txt_LotNo.Text.Trim();
           if (lotNo == "" && lotNo.Length <= 0)
           {
               MessageBox.Show("请输入批次号！");
               return;
           }

            StringBuilder where = new StringBuilder();
            char[] splitChars = new char[] { ',', '$' };
            string[] LotNos = txt_LotNo.Text.Trim().TrimEnd(splitChars).Split(splitChars);


            if (LotNos.Length <= 1)
            {
                where.AppendFormat("'" + LotNos[0].Trim() + "'");
            }
            else
            {
                foreach (string lot in LotNos)
                {
                    where.AppendFormat("'{0}',", lot.Trim());
                }
                where.Remove(where.Length - 1, 1);
            }
           DataTable dt = new DataTable();
           try
           {
//               string strSql = string.Format(@"   SELECT   t2.PACKAGE_NO, t1.LOT_NUMBER, t1.ATTRIBUTE_VALUE
//                                                          FROM     dbo.WIP_LOT_ATTR AS t1 INNER JOIN
//                                                                        dbo.WIP_PACKAGE_DETAIL AS t2 ON t1.LOT_NUMBER = t2.OBJECT_NUMBER
//				                                                        WHERE t1.ATTRIBUTE_NAME='EL1ImagePath' and t1.LOT_NUMBER in ({0})
//				                                                        order by t2.ITEM_NO", where);
               string strSql = string.Format(@"   SELECT   t1.LOT_NUMBER, t1.ATTRIBUTE_VALUE
                                                          FROM     dbo.WIP_LOT_ATTR AS t1 INNER JOIN
                                                                        dbo.WIP_LOT AS t2 ON t1.LOT_NUMBER = t2.LOT_NUMBER
				                                                        WHERE t1.ATTRIBUTE_NAME='EL1ImagePath' and t1.LOT_NUMBER in ({0})
                                                                            ",where);
               dt = SqlHelper.ExecuteDataTable(strSql, CommandType.Text);
               dataGridView1.DataSource = dt;


               this.dataGridView1.DataSource = dt;
               int width = this.dataGridView1.Width;
               int avgWidth = width / dt.Columns.Count;//求出每一列的header宽度
               for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
               {
                   this.dataGridView1.Columns[i].Width = avgWidth;//设置每一列的宽度
               }
           }
           catch (Exception ex)
           {
               //MessageBox.Show(ex);
           }
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = 0; i < e.RowCount; i++)
            {
                dataGridView1.Rows[e.RowIndex + i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Rows[e.RowIndex + i].HeaderCell.Value = (e.RowIndex + i + 1).ToString();
            }

            for (int i = e.RowIndex + e.RowCount; i < this.dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }        
        }

        private void btn_LotDownload_Click(object sender, EventArgs e)
        {
            string fileName = txt_address.Text.Trim();
            string lotNo = txt_LotNo.Text.Trim();
            if (fileName == null && fileName.Length <= 0)
            {
                MessageBox.Show("请选择文件夹！");
                return;
            }
            else
            {
                if (Directory.Exists(txt_address.Text.Trim()))
                {
                    if (lotNo == "" && lotNo.Length <= 0)
                    {
                        MessageBox.Show("请输入批次号！");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("文件夹不存在！请选择文件夹");
                    return;
                }
            }
            

            StringBuilder where = new StringBuilder();
            char[] splitChars = new char[] { ',', '$' };
            string[] LotNos = txt_LotNo.Text.Trim().TrimEnd(splitChars).Split(splitChars);


            if (LotNos.Length <= 1)
            {
                where.AppendFormat("'" + LotNos[0].Trim() + "'");
            }
            else
            {
                foreach (string lot in LotNos)
                {
                    where.AppendFormat("'{0}',", lot.Trim());
                }
                where.Remove(where.Length - 1, 1);
            }
            DataTable dt = new DataTable();
            try
            {
//                string strSql = string.Format(@"   SELECT   t2.PACKAGE_NO, t1.LOT_NUMBER, t1.ATTRIBUTE_VALUE
//                                                          FROM     dbo.WIP_LOT_ATTR AS t1 INNER JOIN
//                                                                        dbo.WIP_PACKAGE_DETAIL AS t2 ON t1.LOT_NUMBER = t2.OBJECT_NUMBER
//				                                                        WHERE t1.ATTRIBUTE_NAME='EL1ImagePath' and t1.LOT_NUMBER in ({0})
//				                                                        order by t2.ITEM_NO", where);
                string strSql = string.Format(@"   SELECT   t1.LOT_NUMBER, t1.ATTRIBUTE_VALUE
                                                          FROM     dbo.WIP_LOT_ATTR AS t1 INNER JOIN
                                                                        dbo.WIP_LOT AS t2 ON t1.LOT_NUMBER = t2.LOT_NUMBER
				                                                        WHERE t1.ATTRIBUTE_NAME='EL1ImagePath' and t1.LOT_NUMBER in ({0})
                                                                            ", where);
                dt = SqlHelper.ExecuteDataTable(strSql, CommandType.Text);
                dataGridView1.DataSource = dt;


                if (dt.Rows.Count > 0)
                {
                    WebClient web = new WebClient();
                    string address = null;
                    this.Hide();
                    Form3 msgFrom = new Form3();
                    msgFrom.Show();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        address = null;
                        address = dt.Rows[i]["ATTRIBUTE_VALUE"].ToString();
                        string imageName = address.Split('/').Last<string>();
                        if (Exists(address))
                        {                          
                            web.DownloadFile(address, fileName + "\\" + imageName);
                            //while (i < dt.Rows.Count)
                            //{

                            //}
                            lab_msg.Text = string.Format("下载第：{0}张完成", i);
                        }
                        else
                        {
                            DataRow dr = dtNoDownLoadImg.NewRow();
                            dr[0] = address;
                            dtNoDownLoadImg.Rows.Add(dr);
                            dataGridView2.DataSource = null;
                            dataGridView2.DataSource = dtNoDownLoadImg;
                            continue;
                        }
                       
                    }
                    msgFrom.Hide();
                    this.Show();
                    MessageBox.Show("下载完成！");
                }

                this.dataGridView1.DataSource = dt;
                int width = this.dataGridView1.Width;
                int avgWidth = width / dt.Columns.Count;//求出每一列的header宽度
                for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
                {
                    this.dataGridView1.Columns[i].Width = avgWidth;//设置每一列的宽度
                }
            }
            catch (Exception ex)
            {
                string lotNo1 = txt_LotNo.Text.Trim();
                MessageBox.Show(lotNo1+"下载失败");
            }
        }

        static bool Exists(string url)
        {
            try
            {
                using (new WebClient().OpenRead(url)) { }
                return true;
            }
            catch (WebException)
            {
                return false;
            }
        } 
    }
}
