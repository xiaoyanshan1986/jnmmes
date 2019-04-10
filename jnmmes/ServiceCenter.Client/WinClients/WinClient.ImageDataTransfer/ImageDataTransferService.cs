using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.IO;
using System.Configuration;
using ServiceCenter.Client.WinService.ImageDataTransfer.Configuration;
using ServiceCenter.Client.WinService.ImageDataTransfer.Transfer;

namespace ServiceCenter.Client.WinService.ImageDataTransfer
{
    /// <summary>
    /// 将EL/IV图片数据集成到MES。
    /// </summary>
    public partial class ImageDataTransferService : ServiceBase
    {
        public const string EVENT_SOURCE_NAME = "MES.ImageDataTransfer";
        public const string EVENT_LOG_NAME = "MES ImageDataTransfer";

        ImageConfigurationSection _section = null;
        IList<ImageDataTransferThreadWrapper> lstWrapper = new List<ImageDataTransferThreadWrapper>();
        private EventLog _eventLog = null;
        public ImageDataTransferService()
        {
            InitializeComponent();
            this._eventLog = new System.Diagnostics.EventLog();
            // Turn off auto logging
            this.AutoLog = false;
            // create an event source, specifying the name of a log that
            // does not currently exist to create a new, custom log
            if (!System.Diagnostics.EventLog.SourceExists(EVENT_SOURCE_NAME))
            {
                System.Diagnostics.EventLog.CreateEventSource(EVENT_SOURCE_NAME, EVENT_LOG_NAME);
            }
            // configure the event log instance to use this source name
            this._eventLog.Source = EVENT_SOURCE_NAME;
        }
        /// <summary>
        /// 启动服务。
        /// </summary>
        protected override void OnStart(string[] args)
        {
//#if DEBUG
//            Debugger.Launch();    //Launches and attaches a debugger to the process.
//#endif

            try 
            {
                this._eventLog.WriteEntry("MES.ImageDataTransfer 服务启动");
                if (lstWrapper == null)
                {
                    lstWrapper = new List<ImageDataTransferThreadWrapper>();
                }
                //获取配置节信息
                this._section = (ImageConfigurationSection)ConfigurationManager.GetSection("mes.image");
                //增加线程个数。
                foreach (ImageDeviceElement element in this._section.Devices)
                {
                    ParameterizedThreadStart threadStart = new ParameterizedThreadStart(TransferData);
                    ImageDataTransferThreadWrapper wrapper = new ImageDataTransferThreadWrapper(element, threadStart);
                    lstWrapper.Add(wrapper);
                }
                //启动线程。
                foreach (ImageDataTransferThreadWrapper wrapper in lstWrapper)
                {
                    Thread.Sleep(500);
                    wrapper.Start();
                }
            }
            catch (Exception ex)
            {
                this._eventLog.WriteEntry(string.Format("MES.ImageDataTransfer:{0}",ex.Message),EventLogEntryType.Error);
            }
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
                this._eventLog.WriteEntry("MES.ImageDataTransfer: " + ex.Message);
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
                    ImageDataTransferAction sdgData = new ImageDataTransferAction();
                    sdgData.Execute(wrapper.Device);
                    DateTime dtEndTime = DateTime.Now;
                    if (sdgData.TransferCount > 0)
                    {
                        string msg = string.Format("MES.ImageDataTransfer：开始时间:{0};结束时间:{1};耗用时间:{2}秒;转置 {4} 设备数据数量:{3}。",
                                            dtStartTime
                                            , dtEndTime
                                            , (dtEndTime - dtStartTime).TotalSeconds
                                            , sdgData.TransferCount
                                            , wrapper.Device.Name);
                        this._eventLog.WriteEntry(msg);
                    }
                }
                catch (Exception ex)
                {
                    this._eventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
                }
                if (wrapper.Loop)
                {
                    Thread.Sleep(1000);
                }
            }
            wrapper.AutoResetEvent.Set();
        }
        /// <summary>
        /// 停止服务。
        /// </summary>
        protected override void OnStop()
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
                this._eventLog.WriteEntry("MES.ImageDataTransfer 服务停止");
            }
            catch (Exception ex)
            {
                this._eventLog.WriteEntry("MES.ImageDataTransfer 服务停止," + ex.Message);
            }
        }
    }

    /// <summary>
    /// EL/IV图片数据转置线程封装类。
    /// </summary>
    public class ImageDataTransferThreadWrapper : IDisposable
    {
        /// <summary>
        /// 获取线程执行的循环标志。
        /// </summary>
        public bool Loop { get; private set; }
        /// <summary>
        /// 获取线程执行异步事件。
        /// </summary>
        public AutoResetEvent AutoResetEvent { get; private set; }
        /// <summary>
        /// 获取线程对象。
        /// </summary>
        public Thread Thread { get; private set; }
        /// <summary>
        /// 获取EL/IV图片设备对象。
        /// </summary>
        public ImageDeviceElement Device { get; private set; }
        /// <summary>
        /// 构造函数。
        /// </summary>
        public ImageDataTransferThreadWrapper(ImageDeviceElement device, ParameterizedThreadStart threadStart)
        {
            this.Loop = true;
            this.Device = device;
            this.AutoResetEvent = new AutoResetEvent(false);
            this.Thread = new Thread(threadStart);
        }
        /// <summary>
        /// 启动线程。
        /// </summary>
        public void Start()
        {
            this.Thread.Start(this);
        }
        /// <summary>
        /// 停止线程。
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
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            this.AutoResetEvent.Close();
            this.AutoResetEvent = null;
            this.Thread = null;
        }
    }
}
