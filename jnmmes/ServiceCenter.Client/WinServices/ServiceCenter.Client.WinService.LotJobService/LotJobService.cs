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
using ServiceCenter.Model;
using ServiceCenter.Common;
using WIPModel=ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Service.Contract.WIP;

namespace ServiceCenter.Client.WinService.LotJob
{
    /// <summary>
    /// 批次作业服务。
    /// </summary>
    public partial class LotJobService : ServiceBase
    {
        public const string EVENT_SOURCE_NAME = "MES.LotJob";
        public const string EVENT_LOG_NAME = "MES LotJob";
        IList<LotJobTransferThreadWrapper> lstWrapper = new List<LotJobTransferThreadWrapper>();
        private EventLog _eventLog = null;
        /// <summary>
        /// 构造函数。
        /// </summary>
        public LotJobService()
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
                this._eventLog.WriteEntry("MES.LotJob 服务启动");
                if (lstWrapper == null)
                {
                    lstWrapper = new List<LotJobTransferThreadWrapper>();
                }
                //增加线程个数。
                ParameterizedThreadStart threadStart = new ParameterizedThreadStart(Execute);
                LotJobTransferThreadWrapper wrapper = new LotJobTransferThreadWrapper(threadStart);
                lstWrapper.Add(wrapper);
                //启动线程。
                foreach (LotJobTransferThreadWrapper item in lstWrapper)
                {
                    Thread.Sleep(500);
                    item.Start();
                }
            }
            catch (Exception ex)
            {
                this._eventLog.WriteEntry(string.Format("MES.LotJob:{0}",ex.Message),EventLogEntryType.Error);
            }
        }
        /// <summary>
        /// 执行批次作业。
        /// </summary>
        private void Execute(object obj)
        {
            try
            {
                LotJobTransferThreadWrapper wrapper = obj as LotJobTransferThreadWrapper;
                if (wrapper == null)
                {
                    return;
                }
                Execute(wrapper);
            }
            catch (Exception ex)
            {
                this._eventLog.WriteEntry("MES.LotJob: " + ex.Message);
            }
        }
        /// <summary>
        /// 执行批次作业。
        /// </summary>
        private void Execute(LotJobTransferThreadWrapper wrapper)
        {
            while (wrapper.Loop)
            {
                try
                {
                    //查询前10个批次定时作业。
                    PagingConfig cfg = new PagingConfig() { 
                          PageNo=0,
                          PageSize=10,
                          Where = string.Format(@"Status=1 
                                                  AND CloseType=0
                                                  AND NextRunTime<='{0}'"
                                                 ,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                          OrderBy = "NextRunTime"
                    };

                    IList<WIPModel.LotJob> lstJob = new List<WIPModel.LotJob>();
                    using (LotJobServiceClient client = new LotJobServiceClient())
                    {
                        MethodReturnResult<IList<WIPModel.LotJob>> rst = client.Get(ref cfg);
                        if (rst.Data==null)
                        {
                            this._eventLog.WriteEntry(string.Format("MES.LotJob:{0}", rst.Message), EventLogEntryType.Error);
                            client.Close();
                            continue;
                        }
                        lstJob = rst.Data;
                    }

                    //遍历JOB数据
                    foreach (WIPModel.LotJob job in lstJob)
                    {
                        job.RunCount++;
                        job.NotifyMessage = string.Empty;
                        #region //自动出站。
                        if (job.Type == EnumJobType.AutoTrackOut)
                        {
                            TrackOutParameter p = new TrackOutParameter()
                            {
                                Creator=job.Creator,
                                LineCode=job.LineCode,
                                LotNumbers=new List<string>(),
                                OperateComputer=System.Net.Dns.GetHostName(),
                                Operator=job.Creator,
                                RouteOperationName=job.RouteStepName,
                                EquipmentCode=job.EquipmentCode
                            };
                            p.LotNumbers.Add(job.LotNumber);

                            using (LotTrackOutServiceClient client = new LotTrackOutServiceClient())
                            {
                                MethodReturnResult result = client.TrackOut(p);
                                if (result.Code > 0)
                                {
                                    job.NotifyMessage = result.Message;
                                    job.NextRunTime = job.NextRunTime.AddMinutes(1);
                                    this._eventLog.WriteEntry(string.Format("MES.LotJob:{0} {1}"
                                                                           , job.LotNumber 
                                                                           , job.Type.GetDisplayName()
                                                                           , result.Message), 
                                                              EventLogEntryType.Error);
                                }
                                else
                                {
                                    job.CloseType = EnumCloseType.Normal;
                                }
                            }
                        }
                        #endregion
                        #region //自动进站。
                        else if(job.Type==EnumJobType.AutoTrackIn)
                        {
                            TrackInParameter p = new TrackInParameter()
                            {
                                Creator = job.Creator,
                                LineCode = job.LineCode,
                                LotNumbers = new List<string>(),
                                OperateComputer = System.Net.Dns.GetHostName(),
                                Operator = job.Creator,
                                RouteOperationName = job.RouteStepName,
                                EquipmentCode=job.EquipmentCode
                            };
                            p.LotNumbers.Add(job.LotNumber);

                            using (LotTrackInServiceClient client = new LotTrackInServiceClient())
                            {
                                MethodReturnResult result = client.TrackIn(p);
                                if (result.Code > 0)
                                {
                                    job.NotifyMessage = result.Message;
                                    job.NextRunTime = job.NextRunTime.AddMinutes(1);
                                    this._eventLog.WriteEntry(string.Format("MES.LotJob:{0} {1}"
                                                                           , job.LotNumber
                                                                           , job.Type.GetDisplayName()
                                                                           , result.Message),
                                                              EventLogEntryType.Error);
                                }
                                else
                                {
                                    job.CloseType = EnumCloseType.Normal;
                                }
                            }
                        }
                        #endregion
                        //超过5次没有完成，则设置为手动关闭。定时作业失败。
                        if (job.RunCount >= 2 && job.CloseType == EnumCloseType.None)
                        {
                            job.CloseType = EnumCloseType.Manual;
                        }
                        #region //更新批次定时作业。
                        using (LotJobServiceClient client = new LotJobServiceClient())
                        {
                            MethodReturnResult result = client.Modify(job);
                            if (result.Code > 0)
                            {
                                this._eventLog.WriteEntry(string.Format("MES.LotJob:{0} {1}"
                                                                       , job.LotNumber
                                                                       , job.Type.GetDisplayName()
                                                                       , result.Message),
                                                          EventLogEntryType.Error);
                            }
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    this._eventLog.WriteEntry(string.Format("MES.LotJob:{0}", ex.Message), EventLogEntryType.Error);
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
                foreach (LotJobTransferThreadWrapper item in lstWrapper)
                {
                    item.Stop();
                    item.Dispose();
                }
                lstWrapper.Clear();
                lstWrapper = null;
                this._eventLog.WriteEntry("MES.LotJob 服务停止");
            }
            catch (Exception ex)
            {
                this._eventLog.WriteEntry("MES.LotJob 服务停止,"+ex.Message);
            }
        }
    }

    /// <summary>
    /// 批次作业线程封装类。
    /// </summary>
    public class LotJobTransferThreadWrapper : IDisposable
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
        /// 构造函数。
        /// </summary>
        public LotJobTransferThreadWrapper(ParameterizedThreadStart threadStart)
        {
            this.Loop = true;
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
