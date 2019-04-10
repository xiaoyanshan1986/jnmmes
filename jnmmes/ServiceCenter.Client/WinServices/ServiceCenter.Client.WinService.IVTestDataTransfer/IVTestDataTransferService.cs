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
using ServiceCenter.Client.WinService.IVTestDataTransfer.Configuration;
using ServiceCenter.Client.WinService.IVTestDataTransfer.Transfer;

namespace ServiceCenter.Client.WinService.IVTestDataTransfer
{
    /// <summary>
    /// 将IV测试数据集成到MES。
    /// </summary>
    public partial class IVTestDataTransferService : ServiceBase
    {
        public const string EVENT_SOURCE_NAME = "MES.IVTestDataTransfer";
        public const string EVENT_LOG_NAME = "MES IVTestDataTransfer";

        IVTestConfigurationSection _section = null;
        IList<IVTestDataTransferThreadWrapper> lstWrapper = new List<IVTestDataTransferThreadWrapper>();
        private EventLog _eventLog = null;
        public IVTestDataTransferService()
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
            try 
            {
                this._eventLog.WriteEntry("MES.IVTestDataTransfer 服务启动"); 
                if (lstWrapper == null)
                {
                    lstWrapper = new List<IVTestDataTransferThreadWrapper>();
                }
                //获取配置节信息
                this._section = (IVTestConfigurationSection)ConfigurationManager.GetSection("mes.ivtest");
                //增加线程个数。
                foreach (IVTestDeviceElement element in this._section.Devices)
                {
                    ParameterizedThreadStart threadStart = new ParameterizedThreadStart(TransferData);
                    IVTestDataTransferThreadWrapper wrapper = new IVTestDataTransferThreadWrapper(element, threadStart);
                    lstWrapper.Add(wrapper);
                }
                //启动线程。
                foreach (IVTestDataTransferThreadWrapper wrapper in lstWrapper)
                {
                    Thread.Sleep(500);
                    wrapper.Start();
                }
            }
            catch (Exception ex)
            {
                this._eventLog.WriteEntry(string.Format("MES.IVTestDataTransfer:{0}",ex.Message),EventLogEntryType.Error);
            }
        }
        /// <summary>
        /// 数据转置
        /// </summary>
        private void TransferData(object obj)
        {
            try
            {
                IVTestDataTransferThreadWrapper wrapper = obj as IVTestDataTransferThreadWrapper;
                if (wrapper == null)
                {
                    return;
                }
                TransferData(wrapper);
            }
            catch (Exception ex)
            {
                this._eventLog.WriteEntry("MES.IVTestDataTransfer: " + ex.Message);
            }
        }
        /// <summary>
        /// 数据转置
        /// </summary>
        private void TransferData(IVTestDataTransferThreadWrapper wrapper)
        {
            while (wrapper.Loop)
            {
                try
                {
                    IVTestDeviceElement device = wrapper.Device;
                    DateTime dtStartTime = DateTime.Now;
                    //获取IV测试数据文件路径。
                    string strFileFullName = IVTestDataTransferAction.GetFullFile(device.Path, device.Format);
                    string msg = string.Empty;
                    if (!string.IsNullOrEmpty(strFileFullName))
                    {
                        string accConString = string.Format(device.ConnectionString, strFileFullName);
                        IVTestDataTransferAction sdgData = new IVTestDataTransferAction(accConString);
                        sdgData.Execute(device);
                        DateTime dtEndTime = DateTime.Now;
                        if (sdgData.TransferCount > 0)
                        {
                            msg = string.Format("MES.IVTestDataTransfer：开始时间:{0};结束时间:{1};耗用时间:{2}秒;转置 {5} 设备数据数量:{3}。{4}",
                                                dtStartTime
                                                , dtEndTime
                                                , (dtEndTime - dtStartTime).TotalSeconds
                                                , sdgData.TransferCount
                                                , strFileFullName
                                                , wrapper.Device.Name);
                            this._eventLog.WriteEntry(msg);
                        }
                    }
                    else
                    {
                        msg = string.Format("MES.IVTestDataTransfer：开始时间:{0};获取{1} ACCESS数据库文件失败。", dtStartTime, wrapper.Device.Name);
                        this._eventLog.WriteEntry(msg);
                    }
                }
                catch (Exception ex)
                {
                    this._eventLog.WriteEntry(ex.ToString(), EventLogEntryType.Error);
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
                foreach (IVTestDataTransferThreadWrapper wrapper in lstWrapper)
                {
                    wrapper.Stop();
                    wrapper.Dispose();
                }
                lstWrapper.Clear();
                lstWrapper = null;
                this._eventLog.WriteEntry("MES.IVTestDataTransfer 服务停止");
            }
            catch (Exception ex)
            {
                this._eventLog.WriteEntry("MES.IVTestDataTransfer 服务停止," + ex.Message);
            }
        }
    }

    /// <summary>
    /// IV测试数据转置线程封装类。
    /// </summary>
    public class IVTestDataTransferThreadWrapper : IDisposable
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
        /// 获取IV测试设备对象。
        /// </summary>
        public IVTestDeviceElement Device { get; private set; }
        /// <summary>
        /// 构造函数。
        /// </summary>
        public IVTestDataTransferThreadWrapper(IVTestDeviceElement device, ParameterizedThreadStart threadStart)
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
            if (!this.AutoResetEvent.WaitOne(5000))
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
