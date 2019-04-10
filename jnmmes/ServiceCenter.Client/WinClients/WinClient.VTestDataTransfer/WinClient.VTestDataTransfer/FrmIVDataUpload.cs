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
using WinClient.VTestDataTransfer.Configuration;
using System.IO;

namespace WinClient.VTestDataTransfer
{
    public partial class FrmIVDataUpload : Form
    {
        IVTestConfigurationSection _section = null;
        IVTestDeviceElement _deviceElement = null;

        string strIP = "";
        string strAccessConnection = "";
        private IVTestDataTransferAction dataTransferAction = null;
        private int nCount =1;
        
        public FrmIVDataUpload()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if( CommonFun.CheckFileExists(txtFilePath.Text.Trim())==false)
            {
                MessageBox.Show(string.Format("{0}不存在.",txtFilePath));
                return;
            }
            strAccessConnection = string.Format(_deviceElement.ConnectionString,txtFilePath.Text.Trim());

            dataTransferAction = new IVTestDataTransferAction(strAccessConnection);
            dataTransferAction.OnDataTransferFinished += new EventHandler<DataTransferFinishedArgs>(DataTransferFinished);
            int nSecond = 0;
            if( int.TryParse(txtInterval.Text,out nSecond)==true)
            {
                if(nSecond<3)
                {
                    MessageBox.Show("时间设置不能小于(3)秒");
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

        private void FrmIVDataUpload_Load(object sender, EventArgs e)
        {
            timer1.Enabled = false;
           
            bool blFound = false;
            string hostInfo = Dns.GetHostName();
            System.Net.IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            //IPAddress ipaddress = addressList[0];
            //string ips = ipaddress.ToString();
            MessageBox.Show(ips);
            
            //获取配置节信息
            this._section = (IVTestConfigurationSection)ConfigurationManager.GetSection("mes.ivtest");
            //增加线程个数。
            foreach (IVTestDeviceElement element in this._section.Devices)
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

                if (blFound == true)
                {
                    break;
                }
            }

            if(blFound == false )
            {
                MessageBox.Show("此电脑的IP地址没有在配置文件中找到对应的配置,请联系IT");
                Application.Exit();

                //return;
            }
            else
            {
                txtFilePath.Text = CommonFun.GetFullFile(_deviceElement.Path, _deviceElement.Format);
                
                txtSourceImagePath.Text = _deviceElement.SourceImagePathRoot;
            }

            string strSql = " select top 1 * from [dbo].[ZWIP_IV_TEST] where EQUIPMENT_CODE='"
                + _deviceElement.EqpName + "'  order by TEST_TIME desc";

            DataTable dt= DatabaseEx.getDbInstance().getDataTable(strSql);
            if(dt!=null &&dt.Rows.Count>0)
            {
                txtTestTime.Text = string.Format("{0:yyyy-MM-dd HH:mm:ss}",  dt.Rows[0]["TEST_TIME"].ToString());
                //txtTestTime.Text = string.Format("{0:yyyy-MM-dd HH:mm:ss}", "2015-06-11 09:10:10");
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

            notifyIVData.Visible = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //根据间隔时间运行UploadData的程序
            timer1.Enabled = false;
            DateTime dateTime;

            try 
            {
                dateTime = System.DateTime.Parse(txtTestTime.Text);

                dataTransferAction.Execute(_deviceElement, dateTime);

                //显示计数
                nCount = nCount + 1;
            }catch(Exception er)
            {
                MessageBox.Show(er.Message);

                return;
            }
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
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "*.mdb|*.mdb";
            DialogResult fResult = openFileDialog1.ShowDialog(this);
            txtFilePath.Text = openFileDialog1.FileName;
        }

        private void notifyIVData_DoubleClick(object sender, EventArgs e)
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
            notifyIVData.Visible = true;
        }

        private void ShowForm()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
            notifyIVData.Visible = false;
        }

        private void tsMenuHide_Click(object sender, EventArgs e)
        {
           HideForm();
        }
        
        private void CloseForm()
        {
            if ( MessageBox.Show("是否退出系统","IVDATA上传",MessageBoxButtons.YesNo)==DialogResult.Yes)
            {
                Application.Exit();
            }

        }
        private void tsMenuExit_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void FrmIVDataUpload_Resize(object sender, EventArgs e)
        {
            if ( this.WindowState ==FormWindowState.Minimized)
            {
                HideForm();
            }else
            {
                notifyIVData.Visible = false;
            }
        }

        private void FrmIVDataUpload_FormClosed(object sender, FormClosedEventArgs e)
        {
            //CloseForm();
        }

        private void btnImageUpload_Click(object sender, EventArgs e)
        {
            string strLotNumber = txtLotNumber.Text.Trim();
            string strImageFullPath = CommonFun.GetFullPath(txtSourceImagePath.Text.Trim(),_deviceElement.SourceImagePathFormat);
            string strFileName = strImageFullPath + strLotNumber + "." + _deviceElement.ImageExtensionName;
            string strResult = CommonFun.UploadFile(strFileName, _deviceElement);
            txtLotNumber.Text = "";
            richTextBox1.Text = strResult + "\n" + richTextBox1.Text;
        }
    }
}



#region 20160720备份
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using Microsoft.Practices.EnterpriseLibrary.Data;
//using Microsoft.Practices.EnterpriseLibrary.Common;
//using System.Data.Common;
//using System.Configuration;
//using WinClient.VTestDataTransfer.Configuration;
//using System.IO;

//namespace WinClient.VTestDataTransfer
//{
//    public partial class FrmIVDataUpload : Form
//    {
//        IVTestConfigurationSection _section = null;
//        IVTestDeviceElement _deviceElement = null;

//        string strIP = "";
//        string strAccessConnection = "";
//        private IVTestDataTransferAction dataTransferAction = null;
//        private int nCount =1;
//        private Database db = null;
//        DateTime dtGetIVtestLastTime;       //记录最后一次取得数据时间
//        bool bINIIVtestLastTime = false;    //是否与数据库进行了最后抓取数据同步

//        public FrmIVDataUpload()
//        {
//            InitializeComponent();
//        }

//        private void btnStart_Click(object sender, EventArgs e)
//        {
//            if( CommonFun.CheckFileExists(txtFilePath.Text.Trim())==false)
//            {
//                MessageBox.Show(string.Format("{0}不存在.",txtFilePath));
//                return;
//            }
//            strAccessConnection = string.Format(_deviceElement.ConnectionString,txtFilePath.Text.Trim());

//            dataTransferAction = new IVTestDataTransferAction(strAccessConnection);
//            dataTransferAction.OnDataTransferFinished += new EventHandler<DataTransferFinishedArgs>(DataTransferFinished);
//            int nSecond = 0;
//            if( int.TryParse(txtInterval.Text,out nSecond)==true)
//            {
//                if(nSecond<3)
//                {
//                    MessageBox.Show("时间设置不能小于(3)秒");
//                    return;
//                }
//            }
//            timer1.Interval = nSecond * 1000;
//            timer1.Enabled = true;
//            StartTransfer();
//        }

//        private void btnPause_Click(object sender, EventArgs e)
//        {
//            this.btnStart.Enabled = true;
//            this.timer1.Enabled = false;
//            dataTransferAction.OnDataTransferFinished -= new EventHandler<DataTransferFinishedArgs>(DataTransferFinished);
//            PauseTransfer();
//        }

//        private void groupBox1_Enter(object sender, EventArgs e)
//        {

//        }

//        private void FrmIVDataUpload_Load(object sender, EventArgs e)
//        {
//            timer1.Enabled = false;
           
//            bool blFound = false;
//            string hostInfo = Dns.GetHostName();
//            System.Net.IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
//            //IPAddress ipaddress = addressList[0];
//            //string ips = ipaddress.ToString();
//            //MessageBox.Show(ips);
            
//            //获取配置节信息
//            this._section = (IVTestConfigurationSection)ConfigurationManager.GetSection("mes.ivtest");
//            //增加线程个数。
//            foreach (IVTestDeviceElement element in this._section.Devices)
//            {
//                foreach(IPAddress  ipAddress in addressList )
//                {
//                    strIP = ipAddress.ToString();
//                    if (strIP == element.Name)
//                    {
//                        blFound = true;
//                        _deviceElement = element;
                                                
//                        break;
//                    }
//                }

//                if (blFound == true)
//                {
//                    break;
//                }
//            }

//            if(blFound == false )
//            {
//                MessageBox.Show("此电脑的IP地址没有在配置文件中找到对应的配置,请联系IT");
//                Application.Exit();

//                return;
//            }
//            else
//            {
//                txtFilePath.Text = CommonFun.GetFullFile(_deviceElement.Path, _deviceElement.Format);
//                //txtFilePath.Text = _deviceElement.Path;
//                txtSourceImagePath.Text = _deviceElement.SourceImagePathRoot;
//            }

//            string strSql = " select top 1 * from [dbo].[ZWIP_IV_TEST] where EQUIPMENT_CODE='"
//                + _deviceElement.EqpName + "'  order by TEST_TIME desc";

//            DataTable dt= DatabaseEx.getDbInstance().getDataTable(strSql);
//            if(dt!=null &&dt.Rows.Count>0)
//            {
//                txtTestTime.Text = string.Format("{0:yyyy-MM-dd HH:mm:ss}",  dt.Rows[0]["TEST_TIME"].ToString());
//                //txtTestTime.Text = string.Format("{0:yyyy-MM-dd HH:mm:ss}", "2015-06-11 09:10:10");
//            }else
//            {
//                txtTestTime.Text = "1900-01-01 01:01:01";
//            }
//            txtEqpName.Text = _deviceElement.EqpName;

//            btnStart.Enabled = true;
//            btnPause.Enabled = false;
//            txtEqpName.ReadOnly = true;
//            txtTestTime.ReadOnly = true;
//            //btnStart_Click(null, null);

//            notifyIVData.Visible = false;
//        }

//        private void timer1_Tick(object sender, EventArgs e)
//        {
//            //根据间隔时间运行UploadData的程序
//            timer1.Enabled = false;
//            DataTable dtable = null;
//            string sqlString = "";

//            //System.DateTime dt;

//            try 
//            {
//                //需要取得上次最后取得数据的测试时间
//                if (bINIIVtestLastTime == false)
//                {
//                    //dt = System.DateTime.Now;
//                    sqlString = string.Format(@" select max(test_time) testtime from [dbo].[ZWIP_IV_TEST] 
//                                                 where EQUIPMENT_CODE='{0}'", _deviceElement);

//                    dtable = DatabaseEx.getDbInstance().getDataTable(sqlString);

//                    if(dtable.Rows.Count > 0)
//                    {
//                        dtGetIVtestLastTime = Convert.ToDateTime(dtable.Rows[0]["testtime"].ToString());
//                    }
//                    else
//                    {
//                        dtGetIVtestLastTime = System.DateTime.Parse(txtTestTime.Text);
//                    }
//                }
                
//                //dt = System.DateTime.Parse(txtTestTime.Text);

//                dataTransferAction.Execute(_deviceElement, dtGetIVtestLastTime);

//                nCount = nCount + 1;
//            }catch(Exception er)
//            {
//                MessageBox.Show(er.Message);

//                return;
//            }
//        }

//        private void StartTransfer()
//        {
      
//            btnStart.Enabled = false;
//            btnFile.Enabled = false;
//            btnPause.Enabled = true;
//            txtInterval.ReadOnly = true;
//            txtFilePath.ReadOnly = true;
//        }

//        private void PauseTransfer()
//        {
           
//            btnStart.Enabled = true;
//            btnFile.Enabled = true;          
//            btnPause.Enabled = false;
//            txtInterval.ReadOnly = false;
//            txtFilePath.ReadOnly = false;
//        }

//        void DataTransferFinished(object sender, DataTransferFinishedArgs e)
//        {
//            if (nCount>300)
//            {
//                richTextBox1.Text = "";
//                nCount = 0;
//            }
//            richTextBox1.Text = e.TransferMsg + "\n" + richTextBox1.Text;
//            txtTestTime.Text =string.Format("{0:yyyy-MM-dd HH:mm:ss}", e.MaxTestDateTime.ToString());
//            txtFilePath.Text = e.TransferDbFile;
//            timer1.Enabled = true;
//        }

//        private void btnClose_Click(object sender, EventArgs e)
//        {
//            CloseForm();
//        }

//        private void btnFile_Click(object sender, EventArgs e)
//        {
//            openFileDialog1.Filter = "*.mdb|*.mdb";
//            DialogResult fResult = openFileDialog1.ShowDialog(this);
//            txtFilePath.Text = openFileDialog1.FileName;
//        }

//        private void notifyIVData_DoubleClick(object sender, EventArgs e)
//        {
//            if(this.Visible)
//            {
//                HideForm();
//            }else
//            {
//                ShowForm();
//            }
//        }

//        private void tsMenuShow_Click(object sender, EventArgs e)
//        {
//            this.ShowForm();
//        }

//        private void HideForm()
//        {
//            this.Hide();
//            notifyIVData.Visible = true;
//        }

//        private void ShowForm()
//        {
//            this.Show();
//            this.WindowState = FormWindowState.Normal;
//            this.Activate();
//            notifyIVData.Visible = false;
//        }

//        private void tsMenuHide_Click(object sender, EventArgs e)
//        {
//           HideForm();
//        }


//        private void CloseForm()
//        {
//            if ( MessageBox.Show("是否退出系统","IVDATA上传",MessageBoxButtons.YesNo)==DialogResult.Yes)
//            {
//                Application.Exit();
//            }

//        }
//        private void tsMenuExit_Click(object sender, EventArgs e)
//        {
//            CloseForm();
//        }

//        private void FrmIVDataUpload_Resize(object sender, EventArgs e)
//        {
//            if ( this.WindowState ==FormWindowState.Minimized)
//            {
//                HideForm();
//            }else
//            {
//                notifyIVData.Visible = false;
//            }
//        }

//        private void FrmIVDataUpload_FormClosed(object sender, FormClosedEventArgs e)
//        {
//            //CloseForm();
//        }

//        private void btnImageUpload_Click(object sender, EventArgs e)
//        {
//            string strLotNumber = txtLotNumber.Text.Trim();
//            string strImageFullPath = CommonFun.GetFullPath(txtSourceImagePath.Text.Trim(),_deviceElement.SourceImagePathFormat);
//            string strFileName = strImageFullPath + strLotNumber + "." + _deviceElement.ImageExtensionName;
//            string strResult = CommonFun.UploadFile(strFileName, _deviceElement);
//            txtLotNumber.Text = "";
//            richTextBox1.Text = strResult + "\n" + richTextBox1.Text;
//        }
//    }
//}

#endregion