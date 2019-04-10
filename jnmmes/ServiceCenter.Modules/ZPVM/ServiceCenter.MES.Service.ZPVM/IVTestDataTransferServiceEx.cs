using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Contract.ZPVM;
using ServiceCenter.MES.Service.ZPVM.Resources;
using ServiceCenter.Model;
using ServiceCenter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using NHibernate;

namespace ServiceCenter.MES.Service.ZPVM
{
    /// <summary>
    /// 实现IV测试数据移转服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public partial class IVTestDataTransferService : IIVTestDataTransferContract, IIVTestDataTransferCheck,IIVTestDataTransfer
    {
         /// <summary>
        /// 操作前检查事件。
        /// </summary>
        public event Func<IVTestDataTransferParameter, MethodReturnResult> CheckEvent;
        /// <summary>
        /// 执行操作时事件。
        /// </summary>
        public event Func<IVTestDataTransferParameter, MethodReturnResult> ExecutingEvent;
        /// <summary>
        /// 操作执行完成事件。
        /// </summary>
        public event Func<IVTestDataTransferParameter, MethodReturnResult> ExecutedEvent;

        /// <summary>
        /// 自定义操作前检查的清单列表。
        /// </summary>
        private IList<IIVTestDataTransferCheck> CheckList { get; set; }
        /// <summary>
        /// 自定义执行中操作的清单列表。
        /// </summary>
        private IList<IIVTestDataTransfer> ExecutingList { get; set; }
        /// <summary>
        /// 自定义执行后操作的清单列表。
        /// </summary>
        private IList<IIVTestDataTransfer> ExecutedList { get; set; }


        /// <summary>
        /// 注册自定义检查的操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterCheckInstance(IIVTestDataTransferCheck obj)
        {
            if (this.CheckList == null)
            {
                this.CheckList = new List<IIVTestDataTransferCheck>();
            }
            this.CheckList.Add(obj);
        }
        /// <summary>
        /// 注册执行中的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutingInstance(IIVTestDataTransfer obj)
        {
            if (this.ExecutingList == null)
            {
                this.ExecutingList = new List<IIVTestDataTransfer>();
            }
            this.ExecutingList.Add(obj);
        }

        /// <summary>
        /// 注册执行完成后的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutedInstance(IIVTestDataTransfer obj)
        {
            if (this.ExecutedList == null)
            {
                this.ExecutedList = new List<IIVTestDataTransfer>();
            }
            this.ExecutedList.Add(obj);
        }


        /// <summary>
        /// 操作前检查。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected virtual MethodReturnResult OnCheck(IVTestDataTransferParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            StringBuilder sbMessage = new StringBuilder();
            if (this.CheckEvent != null)
            {
                foreach (Func<IVTestDataTransferParameter, MethodReturnResult> d in this.CheckEvent.GetInvocationList())
                {
                    result = d(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                    if (!string.IsNullOrEmpty(result.Message))
                    {
                        sbMessage.Append(result.Message + "\n");
                    }
                }
            }
            if (this.CheckList != null)
            {
                foreach (IIVTestDataTransferCheck d in this.CheckList)
                {
                    result = d.Check(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                    if (!string.IsNullOrEmpty(result.Message))
                    {
                        sbMessage.Append(result.Message + "\n");
                    }
                }
            }
            result.Message = sbMessage.ToString();
            return result;
        }
        /// <summary>
        /// 操作执行中。
        /// </summary>
        protected virtual MethodReturnResult OnExecuting(IVTestDataTransferParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            StringBuilder sbMessage = new StringBuilder();
            if (this.ExecutingEvent != null)
            {
                foreach (Func<IVTestDataTransferParameter, MethodReturnResult> d in this.ExecutingEvent.GetInvocationList())
                {
                    result = d(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                    if (!string.IsNullOrEmpty(result.Message))
                    {
                        sbMessage.Append(result.Message + "\n");
                    }
                }
            }

            if (this.ExecutingList != null)
            {
                foreach (IIVTestDataTransfer d in this.ExecutingList)
                {
                    result = d.Execute(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                    if (!string.IsNullOrEmpty(result.Message))
                    {
                        sbMessage.Append(result.Message + "\n");
                    }
                }
            }
            result.Message = sbMessage.ToString();
            return result;
        }
        /// <summary>
        /// 执行完成。
        /// </summary>
        protected virtual MethodReturnResult OnExecuted(IVTestDataTransferParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            StringBuilder sbMessage = new StringBuilder();
            if (this.ExecutedEvent != null)
            {
                foreach (Func<IVTestDataTransferParameter, MethodReturnResult> d in this.ExecutedEvent.GetInvocationList())
                {
                    result = d(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                    if (!string.IsNullOrEmpty(result.Message))
                    {
                        sbMessage.Append(result.Message + "\n");
                    }
                }
            }
            if (this.ExecutedList != null)
            {
                foreach (IIVTestDataTransfer d in this.ExecutedList)
                {
                    result = d.Execute(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                    if (!string.IsNullOrEmpty(result.Message))
                    {
                        sbMessage.Append(result.Message + "\n");
                    }
                }
            }
            result.Message = sbMessage.ToString();
            return result;
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        public IVTestDataTransferService()
        {
            this.RegisterCheckInstance(this);
            this.RegisterExecutedInstance(this);
        }
        /// <summary>
        /// 批次数据访问类。
        /// </summary>
        public ILotDataEngine LotDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// IV测试数据访问类。
        /// </summary>
        public IIVTestDataDataEngine IVTestDataDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 工步数据访问类。
        /// </summary>
        public IRouteStepAttributeDataEngine RouteStepAttributeDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// IV测试数据移转操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>
        /// 代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult IIVTestDataTransferContract.Transfer(IVTestDataTransferParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (p == null || p.List==null || p.List.Count<=0)
            {
                return result;
            }
            try
            {
                StringBuilder sbMessage = new StringBuilder();
                //操作前检查。
                result = this.OnCheck(p);
                if (result.Code > 0)
                {
                    return result;
                }
                sbMessage.Append(result.Message);
   
                result = this.OnExecuted(p);
                if (result.Code > 0)
                {
                    return result;
                }
                result.Message = sbMessage.ToString();
            }
            catch(Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(StringResource.Error,ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 操作前检查。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        MethodReturnResult IIVTestDataTransferCheck.Check(IVTestDataTransferParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            return result;
        }
        /// <summary>
        /// 执行操作。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        MethodReturnResult IIVTestDataTransfer.Execute(IVTestDataTransferParameter p)
        {
            DateTime now = DateTime.Now;
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            List<IVTestData> lstIVTestDataDataEngineForInsert = new List<IVTestData>();
            List<IVTestData> lstIVTestDataDataEngineForUpdate = new List<IVTestData>();

            List<LotJob> lstLotJobsForInsert = new List<LotJob>();
            List<LotJob> lstLotJobsForUpdate = new List<LotJob>();

            PagingConfig cfg = null;
            //循环批次。
            foreach (IVTestData item in p.List)
            {

                //判断IV测试数据是否存在。
                if (this.IVTestDataDataEngine.IsExists(item.Key))
                {
                    continue;
                }
                //
                //获取批次数据。
                Lot lot = this.LotDataEngine.Get(item.Key.LotNumber);

                #region //IVTestData
                if (lot!=null)
                {
                    //获取校准板记录。
                    cfg = new PagingConfig()
                    {
                        PageNo=0,
                        PageSize=1,
                        Where = string.Format("Key.LotNumber LIKE 'JZ%' AND Key.EquipmentCode='{0}'"
                                              ,item.Key.EquipmentCode),//以JZ开头的为校准板数据。
                        OrderBy="Key.TestTime Desc"
                    };
                    IList<IVTestData> lstTestData = this.IVTestDataDataEngine.Get(cfg);
                    if (lstTestData!=null && lstTestData.Count > 0)
                    {
                        item.CalibrateTime = lstTestData[0].Key.TestTime;
                        item.CalibrationNo = lstTestData[0].Key.LotNumber;
                    }
                    //只有组件在允许测试数据有效的工步，才设置有效值为true
                    bool isAllowIVTestData = false;
                    cfg.Where = string.Format("");
                    RouteStepAttribute rsa = this.RouteStepAttributeDataEngine.Get(new RouteStepAttributeKey()
                    {
                        RouteName=lot.RouteName,
                        RouteStepName=lot.RouteStepName,
                        AttributeName = "IsAllowIVTestData"
                    });
                    if (rsa != null)
                    {
                        bool.TryParse(rsa.Value, out isAllowIVTestData);
                    }
                    item.IsDefault = isAllowIVTestData;
                }
                //如果当前数据为有效数据。
                if (item.IsDefault)
                {
                    //更新之前的测试数据为非默认值.
                    cfg = new PagingConfig()
                    {
                        PageNo = 0,
                        PageSize = 1,
                        Where = string.Format("Key.LotNumber = '{0}' AND IsDefault=1"
                                              , item.Key.LotNumber),
                        OrderBy = "Key.TestTime Desc"
                    };
                    IList<IVTestData> lst = this.IVTestDataDataEngine.Get(cfg);
                    foreach (IVTestData iv in lst)
                    {
                        IVTestData ivUpdate = iv.Clone() as IVTestData;
                        ivUpdate.IsDefault = false;
                        lstIVTestDataDataEngineForUpdate.Add(ivUpdate);
                        //this.IVTestDataDataEngine.Update(ivUpdate);
                    }
                }
                item.CreateTime = DateTime.Now;
                item.EditTime = DateTime.Now;
                item.Creator = "system";
                item.Editor = "system";
                //新增IV测试数据。
                //this.IVTestDataDataEngine.Insert(item);
                lstIVTestDataDataEngineForInsert.Add(item);
                #endregion

                #region AutoTrackOut
                /*
                if (item.IsDefault ==false)
                {
                    continue;
                }
                if (lot == null  //批次数据不为NULL
                || lot.StateFlag != EnumLotState.WaitTrackOut //批次等待出站。
                || this.RouteOperationEquipmentDataEngine.IsExists(new RouteOperationEquipmentKey()
                {
                    RouteOperationName = lot.RouteStepName,
                    EquipmentCode = item.Key.EquipmentCode
                }) == false //批次当前工序是否和设备所在工序匹配，如果匹配才进行过站。
            )
                {
                    continue;
                }
                //获取设备数据。
                Equipment eq = this.EquipmentDataEngine.Get(item.Key.EquipmentCode);
                if (eq == null)
                {
                    continue;
                }

                Location location = this.LocationDataEngine.Get(eq.LocationName);
                //设备所在车间和批次车间匹配。
                if (location == null || location.ParentLocationName != lot.LocationName)
                {
                    continue;
                }

                //不存在批次在指定工步自动出站作业。
                cfg = new PagingConfig()
                {
                    PageSize = 1,
                    PageNo = 0,
                    Where = string.Format(@"LotNumber='{0}' 
                                        AND CloseType=0 
                                        AND Status=1
                                        AND RouteStepName='{1}'
                                        AND Type='{2}'"
                                            , lot.Key
                                            , lot.RouteStepName
                                            , Convert.ToInt32(EnumJobType.AutoTrackOut))
                };
                IList<LotJob> lstJob = this.LotJobDataEngine.Get(cfg);
                if (lstJob.Count == 0)
                {
                    //新增批次自动出站作业。
                    LotJob job = new LotJob()
                    {
                        CloseType = EnumCloseType.None,
                        CreateTime = DateTime.Now,
                        Creator = "system",
                        Editor = "system",
                        EditTime = DateTime.Now,
                        EquipmentCode = item.Key.EquipmentCode,
                        JobName = string.Format("{0} {1}", lot.Key, lot.StateFlag.GetDisplayName()),
                        Key = Convert.ToString(Guid.NewGuid()),
                        LineCode = eq.LineCode,
                        LotNumber = lot.Key,
                        Type = EnumJobType.AutoTrackOut,
                        RouteEnterpriseName = lot.RouteEnterpriseName,
                        RouteName = lot.RouteName,
                        RouteStepName = lot.RouteStepName,
                        RunCount = 0,
                        Status = EnumObjectStatus.Available,
                        NextRunTime = DateTime.Now,
                        NotifyMessage = string.Empty,
                        NotifyUser = string.Empty
                    };
                    lstLotJobsForInsert.Add(job);
                    //this.LotJobDataEngine.Insert(job);
                }
                else
                {
                    LotJob jobUpdate = lstJob[0].Clone() as LotJob;
                    jobUpdate.NextRunTime = DateTime.Now;
                    jobUpdate.LineCode = eq.LineCode;
                    jobUpdate.EquipmentCode = eq.Key;
                    lstLotJobsForUpdate.Add(jobUpdate);
                    //this.LotJobDataEngine.Update(jobUpdate);
                }
                */
                #endregion
            }

            ISession session = this.LotDataEngine.SessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();
            try
            {
                #region //开始事物处理

                //更新IV测试数据 及 批次基本信息
                foreach (IVTestData iVTestData in lstIVTestDataDataEngineForUpdate)
                {
                    this.IVTestDataDataEngine.Update(iVTestData, session);
                }

                //更新IV测试数据 及 批次基本信息
                foreach (IVTestData iVTestData in lstIVTestDataDataEngineForInsert)
                {
                    this.IVTestDataDataEngine.Insert(iVTestData, session);
                }

                #region//判断是否自动进站
                foreach (LotJob lotJob in lstLotJobsForUpdate)
                {
                    this.LotJobDataEngine.Update(lotJob, session);
                }

                foreach (LotJob lotJob in lstLotJobsForInsert)
                {
                    this.LotJobDataEngine.Insert(lotJob, session);
                }
                #endregion               

                transaction.Commit();
                session.Close();
                #endregion
            }
            catch (Exception err)
            {
                transaction.Rollback();
                session.Close();
                result.Code = 1000;
                result.Message += string.Format(StringResource.Error, err.Message);
                result.Detail = err.ToString();
                return result;
            }
            return result;
        }
    }
}
