using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServiceCenter.Client.WinService.ImageDataTransfer.Configuration;
using ServiceCenter.Client.WinService.ImageDataTransfer.Transfer;
using System.Threading;
using System.Diagnostics;

namespace ServiceCenter.Client.WinService.ImageDataTransfer
{
    public partial class FrmImageDataTransfer : Form
    {
        ImageDeviceElement _deviceElement = null;
        ImageConfigurationSection _section = null;
        IList<ImageDataTransferThreadWrapper> lstWrapper = new List<ImageDataTransferThreadWrapper>();
        private ImageDataTransferAction dataTransferAction = new ImageDataTransferAction();
        private int nCount = 1;
        int nSecond = 5000;

        /// <summary>
        /// 窗体初始化
        /// </summary>
        public FrmImageDataTransfer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 启动数据传递
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                //取得计数器间隔


                if (int.TryParse(txtInterval.Text, out nSecond) == true)
                {
                    if (nSecond < 1)
                    {
                        MessageBox.Show("时间设置不能小于(1)秒");
                        return;
                    }
                }

                nSecond = nSecond * 1000;

                if (lstWrapper == null)
                {
                    lstWrapper = new List<ImageDataTransferThreadWrapper>();
                }

                dataTransferAction.OnDataTransferFinished += new EventHandler<DataTransferFinishedArgs>(DataTransferFinished);

                //获取配置节信息
                this._section = (ImageConfigurationSection)ConfigurationManager.GetSection("mes.image");

                //增加线程个数。
                foreach (ImageDeviceElement element in this._section.Devices)
                {
                    ParameterizedThreadStart threadStart = new ParameterizedThreadStart(TransferData);
                    ImageDataTransferThreadWrapper wrapper = new ImageDataTransferThreadWrapper(element, threadStart);
                    lstWrapper.Add(wrapper);
                }

                //启动线程
                foreach (ImageDataTransferThreadWrapper wrapper in lstWrapper)
                {
                    StartTransfer();
                    Thread.Sleep(nSecond);
                    wrapper.Start();
                }
            }
            catch (Exception ex)
            {
                //显示错误日志信息
                richTextBox1.Text = ex.Message + "\n" + richTextBox1.Text;
            }
        }

        /// <summary>
        /// 设置数据传递时控件状态
        /// </summary>
        private void StartTransfer()
        {

            btnStart.Enabled = false;
            //btnFile.Enabled = false;
            btnPause.Enabled = true;
            txtInterval.ReadOnly = true;
            //txtFilePath.ReadOnly = true;
        }






















        /// <summary>
        /// 数据转置
        /// </summary>
        private void TransferData(object obj)
        {
            try
            {
                ImageDataTransferThreadWrapper wrapper = obj as ImageDataTransferThreadWrapper;
                if (wrapper == null)
                {
                    return;
                }
                TransferData(wrapper);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 数据转置
        /// </summary>
        private void TransferData(ImageDataTransferThreadWrapper wrapper)
        {
            while (wrapper.Loop)
            {
                try
                {
                    DateTime dtStartTime = DateTime.Now;
                    dataTransferAction.Execute(wrapper.Device);
                    DateTime dtEndTime = DateTime.Now;
                    if (dataTransferAction.TransferCount > 0)
                    {
                        
                    }
                }
                catch (Exception ex)
                {
                }
                if (wrapper.Loop)
                {
                    Thread.Sleep(nSecond);
                }
            }
            wrapper.AutoResetEvent.Set();
        }
        
        private void PauseTransfer()
        {

            btnStart.Enabled = true;
            //btnFile.Enabled = true;
            btnPause.Enabled = false;
            txtInterval.ReadOnly = false;
            //txtFilePath.ReadOnly = false;
        }

        void DataTransferFinished(object sender, DataTransferFinishedArgs e)
        {
            if (nCount > 300)
            {
                richTextBox1.Text = "";
                nCount = 0;
            }
            richTextBox1.Text = e.TransferMsg + "\n" + richTextBox1.Text;
            //txtTestTime.Text = string.Format("{0:yyyy-MM-dd HH:mm:ss}", e.MaxTestDateTime.ToString());
            //txtFilePath.Text = e.TransferDbFile;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            this.btnStart.Enabled = true;
            dataTransferAction.OnDataTransferFinished -= new EventHandler<DataTransferFinishedArgs>(DataTransferFinished);
            PauseTransfer();

            CommonFun.eventInvoket(() =>
            {
                try
                {
                    foreach (ImageDataTransferThreadWrapper wrapper in lstWrapper)
                    {
                        wrapper.Stop();
                        wrapper.Dispose();
                    }
                    lstWrapper.Clear();
                    lstWrapper = null;

                }
                catch (Exception ex)
                {
                }
                this.btnStart.Enabled = true;
                dataTransferAction.OnDataTransferFinished -= new EventHandler<DataTransferFinishedArgs>(DataTransferFinished);
                PauseTransfer();
            });
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void CloseForm()
        {
            if (MessageBox.Show("是否退出系统", "Images", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                CommonFun.eventInvoket(() =>
                {
                    try
                    {
                        foreach (ImageDataTransferThreadWrapper wrapper in lstWrapper)
                        {
                            wrapper.Stop();
                            wrapper.Dispose();
                        }
                        lstWrapper.Clear();
                        lstWrapper = null;

                    }
                    catch (Exception ex)
                    {
                    }
                    dataTransferAction.OnDataTransferFinished -= new EventHandler<DataTransferFinishedArgs>(DataTransferFinished);
                });
                this.Close();
            }
        }

        private void btnClose_Resize(object sender, EventArgs e)
        {

        }
        
        private void tsMenuHide_Click(object sender, EventArgs e)
        {
            HideForm();
        }

        private void tsMenuExit_Click(object sender, EventArgs e)
        {
            CloseForm();
        }
        
        private void notifyIVData_DoubleClick(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                HideForm();
            }
            else
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

        private void FrmImageDataTransfer_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                HideForm();
            }
            else
            {
                notifyIVData.Visible = false;
            }
        }

        private void FrmImageDataTransfer_Load(object sender, EventArgs e)
        {

            this._section = (ImageConfigurationSection)ConfigurationManager.GetSection("mes.image");
            if (_section.Devices.Count>0)
            {
                this._deviceElement = _section.Devices[0];
            }
            //this.txtFilePath.Text = this._deviceElement.SourcePathRoot;

            btnStart.Enabled = true;
            btnPause.Enabled = false;
            //txtEqpName.ReadOnly = true;
            //txtTestTime.ReadOnly = true;
            notifyIVData.Visible = false;
        }
    }

    /// <summary>
    /// EL/IV图片数据转置线程封装类。
    /// </summary>
    public class ImageDataTransferThreadWrapper : IDisposable
    {
        /// <summary>
        /// 获取线程执行的循环标志
        /// </summary>
        public bool Loop { get; private set; }

        /// <summary>
        /// 获取线程执行异步事件
        /// </summary>
        public AutoResetEvent AutoResetEvent { get; private set; }

        /// <summary>
        /// 获取线程对象
        /// </summary>
        public Thread Thread { get; private set; }

        /// <summary>
        /// 获取EL/IV图片设备对象。
        /// </summary>
        public ImageDeviceElement Device { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ImageDataTransferThreadWrapper(ImageDeviceElement device, ParameterizedThreadStart threadStart)
        {
            this.Loop = true;
            this.Device = device;
            this.AutoResetEvent = new AutoResetEvent(false);
            this.Thread = new Thread(threadStart);
        }

        /// <summary>
        /// 启动线程
        /// </summary>
        public void Start()
        {
            this.Thread.Start(this);
        }

        /// <summary>
        /// 停止线程
        /// </summary>
        public void Stop()
        {
            this.Loop = false;
            if (!this.AutoResetEvent.WaitOne(10000))
            {
                this.Thread.Abort();
            }
            this.Thread = null;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.AutoResetEvent.Close();
            this.AutoResetEvent = null;
            this.Thread = null;
        }
    }
}
