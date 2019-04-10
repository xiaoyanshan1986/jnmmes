using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using System.Data.Common;
using System.Configuration;
using WinClient.JYNYTestDataTransfer.Configuration;
using System.Diagnostics;

namespace WinClient.JYNYTestDataTransfer
{
    public partial class FrmJYNYDataUpload : Form
    {
        JYNYTestConfigurationSection _section = null;
        JYNYTestDeviceElement _deviceElement = null;

        string strIP = "";
        string strAccessConnection = "";
        private JYNYTestDataTransferAction dataTransferAction = null;
        private int nCount =1;
        private Database db = null;
      
        public FrmJYNYDataUpload()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (CommonFun.CheckFileExists(txtFilePath.Text.Trim()) == false)
            {
                MessageBox.Show(string.Format("{0}不存在.", txtFilePath));
                return;
            }
            strAccessConnection = string.Format(_deviceElement.ConnectionString, txtFilePath.Text.Trim());

            dataTransferAction = new JYNYTestDataTransferAction(strAccessConnection);
            dataTransferAction.OnDataTransferFinished += new EventHandler<DataTransferFinishedArgs>(DataTransferFinished);
            int nSecond = 0;
            if (int.TryParse(txtInterval.Text, out nSecond) == true)
            {
                if (nSecond < 5)
                {
                    MessageBox.Show("时间设置不能小于(5)秒");
                    return;
                }
            }
            timer1.Interval = nSecond * 1000;
            timer1.Enabled = true;
            StartTransfer();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            this.btnStart.Enabled = true;
            this.timer1.Enabled = false;
            dataTransferAction.OnDataTransferFinished -= new EventHandler<DataTransferFinishedArgs>(DataTransferFinished);
            PauseTransfer();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void FrmJYNYDataUpload_Load(object sender, EventArgs e)
        {
            timer1.Enabled = false;
           
            bool blFound = false;
            string hostInfo = Dns.GetHostName();
            System.Net.IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            //IPAddress ipaddress = addressList[0];
            //string ips = ipaddress.ToString();
            //MessageBox.Show(ips);


            //获取配置节信息
            this._section = (JYNYTestConfigurationSection)ConfigurationManager.GetSection("mes.JYNYtest");
            //增加线程个数。
            foreach (JYNYTestDeviceElement element in this._section.Devices)
            {
                foreach(IPAddress  ipAddress in addressList )
                {
                    strIP = ipAddress.ToString();
                    if (strIP == element.Name)
                    {
                        blFound = true;
                        _deviceElement = element;
                        break;
                    }
                }
            }
            if(blFound==false )
            {
                MessageBox.Show("此电脑的IP地址没有在配置文件中找到对应的配置,请联系IT");
                Application.Exit();
            }else
            {
                txtFilePath.Text = CommonFun.GetFullFile(_deviceElement.Path, _deviceElement.Format);
                //txtFilePath.Text = _deviceElement.Path;
            }

            string strSql = " select top 1 * from [dbo].[ZWIP_VIR_TESTDATA] where EQUIPMENT_CODE='"
                + _deviceElement.EqpName + "'  order by TEST_TIME desc";

            DataTable dt= DatabaseEx.getDbInstance().getDataTable(strSql);
            if(dt!=null &&dt.Rows.Count>0)
            {
                txtTestTime.Text = string.Format("{0:yyyy-MM-dd HH:mm:ss}",  dt.Rows[0]["TEST_TIME"].ToString());
            }else
            {
                txtTestTime.Text = "1900-01-01 01:01:01";
            }
            txtEqpName.Text = _deviceElement.EqpName;

            btnStart.Enabled = true;
            btnPause.Enabled = false;
            txtEqpName.ReadOnly = true;
            txtTestTime.ReadOnly = true;
            //btnStart_Click(null, null);

            notifyJYNYData.Visible = false;


            if (CommonFun.CheckFileExists(txtFilePath.Text.Trim()) == false)
            {
                MessageBox.Show(string.Format("{0}不存在.", txtFilePath));
                return;
            }
            strAccessConnection = string.Format(_deviceElement.ConnectionString, txtFilePath.Text.Trim());

            dataTransferAction = new JYNYTestDataTransferAction(strAccessConnection);
            dataTransferAction.OnDataTransferFinished += new EventHandler<DataTransferFinishedArgs>(DataTransferFinished);
            int nSecond = 0;
            if (int.TryParse(txtInterval.Text, out nSecond) == true)
            {
                if (nSecond < 10)
                {
                    MessageBox.Show("时间设置不能小于(10)秒");
                    return;
                }
            }
            timer1.Interval = nSecond * 1000;
            timer1.Enabled = true;
            StartTransfer();


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //根据间隔时间运行UploadData的程序
            timer1.Enabled = false;
            System.DateTime dt = System.DateTime.Now;
            nCount = nCount + 1;
            try { 
                dt = System.DateTime.Parse(txtTestTime.Text);
            }catch(Exception er)
            {

            }
            dataTransferAction.Execute(_deviceElement, dt);
        }

        private void StartTransfer()
        {
      
            btnStart.Enabled = false;
            btnFile.Enabled = false;
            btnPause.Enabled = true;
            txtInterval.ReadOnly = true;
            txtFilePath.ReadOnly = true;
        }

        private void PauseTransfer()
        {
           
            btnStart.Enabled = true;
            btnFile.Enabled = true;          
            btnPause.Enabled = false;
            txtInterval.ReadOnly = false;
            txtFilePath.ReadOnly = false;
        }

        void DataTransferFinished(object sender, DataTransferFinishedArgs e)
        {
            if (nCount>300)
            {
                richTextBox1.Text = "";
                nCount = 0;
            }
            richTextBox1.Text = e.TransferMsg + "\n" + richTextBox1.Text;
            txtTestTime.Text =string.Format("{0:yyyy-MM-dd HH:mm:ss}", e.MaxTestDateTime.ToString());
            txtFilePath.Text = e.TransferDbFile;
            timer1.Enabled = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
            Process.GetCurrentProcess().Kill();
            Application.Exit();
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "*.mdb|*.mdb";
            DialogResult fResult = openFileDialog1.ShowDialog(this);
            txtFilePath.Text = openFileDialog1.FileName;
        }

        private void notifyJYNYData_DoubleClick(object sender, EventArgs e)
        {
            if(this.Visible)
            {
                HideForm();
            }else
            {
                ShowForm();
            }
        }

        private void tsMenuShow_Click(object sender, EventArgs e)
        {
            this.ShowForm();
        }

        private void HideForm()
        {
            this.Hide();
            notifyJYNYData.Visible = true;
        }

        private void ShowForm()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
            notifyJYNYData.Visible = false;
        }

        private void tsMenuHide_Click(object sender, EventArgs e)
        {
           HideForm();
        }


        private void CloseForm()
        {
            if ( MessageBox.Show("是否退出系统","安规数据上传",MessageBoxButtons.YesNo)==DialogResult.Yes)
            {
                //Application.Exit();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }

        }
        private void tsMenuExit_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void FrmJYNYDataUpload_Resize(object sender, EventArgs e)
        {
            if ( this.WindowState ==FormWindowState.Minimized)
            {
                HideForm();
            }else
            {
                notifyJYNYData.Visible = false;
            }
        }

        private void FrmJYNYDataUpload_FormClosed(object sender, FormClosedEventArgs e)
        {
            //CloseForm();
        }
    }
}
