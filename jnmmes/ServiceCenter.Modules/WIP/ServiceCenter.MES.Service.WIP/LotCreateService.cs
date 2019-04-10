using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.RBAC;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Service.WIP.Resources;
using ServiceCenter.Model;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.DataAccess.Interface.LSM;
using ServiceCenter.MES.Model.LSM;
using System.ServiceModel.Activation;
using ServiceCenter.MES.DataAccess.Interface.BaseData;
using ServiceCenter.MES.Model.BaseData;
using NHibernate;
using ServiceCenter.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;
using ServiceCenter.Common.DataAccess.NHibernate;

namespace ServiceCenter.MES.Service.WIP
{
    /// <summary>
    /// 实现批次创建服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LotCreateService : ILotCreateContract, ILotCreateCheck, ILotCreate, ILotNumberGenerate
    {
        //获取solaredgeType
        string solaredgeType = System.Configuration.ConfigurationSettings.AppSettings["SolaredgeType"];

        protected Database _db;
        /// <summary>
        /// 操作前检查事件。
        /// </summary>
        public event Func<CreateParameter, MethodReturnResult> CheckEvent;

        /// <summary>
        /// 执行操作时事件。
        /// </summary>
        public event Func<CreateParameter, MethodReturnResult> ExecutingEvent;

        /// <summary>
        /// 操作执行完成事件。
        /// </summary>
        public event Func<CreateParameter, MethodReturnResult> ExecutedEvent;

        /// <summary>
        /// 自定义操作前检查的清单列表。
        /// </summary>
        private IList<ILotCreateCheck> CheckList { get; set; }
        /// <summary>
        /// 自定义执行中操作的清单列表。
        /// </summary>
        private IList<ILotCreate> ExecutingList { get; set; }

        /// <summary>
        /// 自定义执行后操作的清单列表。
        /// </summary>
        private IList<ILotCreate> ExecutedList { get; set; }

        /// <summary>
        /// 工单属性访问类
        /// </summary>
        public IWorkOrderAttributeDataEngine WorkOrderAttributeDataEngine
        {
            get;
            set;
        }
        
        /// <summary>
        /// 注册自定义检查的操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterCheckInstance(ILotCreateCheck obj)
        {
            if (this.CheckList == null)
            {
                this.CheckList = new List<ILotCreateCheck>();
            }
            this.CheckList.Add(obj);
        }
        /// <summary>
        /// 注册执行中的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutingInstance(ILotCreate obj)
        {
            if (this.ExecutingList == null)
            {
                this.ExecutingList = new List<ILotCreate>();
            }
            this.ExecutingList.Add(obj);
        }

        /// <summary>
        /// 注册执行完成后的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutedInstance(ILotCreate obj)
        {
            if (this.ExecutedList == null)
            {
                this.ExecutedList = new List<ILotCreate>();
            }
            this.ExecutedList.Add(obj);
        }
        
        /// <summary>
        /// 操作前检查。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected virtual MethodReturnResult OnCheck(CreateParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (this.CheckEvent != null)
            {
                foreach (Func<CreateParameter, MethodReturnResult> d in this.CheckEvent.GetInvocationList())
                {
                    result = d(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                }
            }
            if (this.CheckList != null)
            {
                foreach (ILotCreateCheck d in this.CheckList)
                {
                    result = d.Check(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 操作执行中。
        /// </summary>
        protected virtual MethodReturnResult OnExecuting(CreateParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (this.ExecutingEvent != null)
            {
                foreach (Func<CreateParameter, MethodReturnResult> d in this.ExecutingEvent.GetInvocationList())
                {
                    result = d(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                }
            }

            if (this.ExecutingList != null)
            {
                foreach (ILotCreate d in this.ExecutingList)
                {
                    result = d.Execute(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 执行完成。
        /// </summary>
        protected virtual MethodReturnResult OnExecuted(CreateParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (this.ExecutedEvent != null)
            {
                foreach (Func<CreateParameter, MethodReturnResult> d in this.ExecutedEvent.GetInvocationList())
                {
                    result = d(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                }
            }
            if (this.ExecutedList != null)
            {
                foreach (ILotCreate d in this.ExecutedList)
                {
                    result = d.Execute(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        public LotCreateService()
        {
            this.RegisterCheckInstance(this);
            this.RegisterExecutedInstance(this);
            this.LotNumberGenerate = this;
            this._db = DatabaseFactory.CreateDatabase();
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
        /// 批次BOM数据访问类。
        /// </summary>
        public ILotBOMDataEngine LotBOMDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 批次自定义属性数据访问类。
        /// </summary>
        public ILotAttributeDataEngine LotAttributeDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 批次操作数据访问类。
        /// </summary>
        public ILotTransactionDataEngine LotTransactionDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 批次历史数据访问类。
        /// </summary>
        public ILotTransactionHistoryDataEngine LotTransactionHistoryDataEngine
        {
            get;
            set;
        }

        /// <summary>
        ///  批次附加参数数据访问类。
        /// </summary>
        public ILotTransactionParameterDataEngine LotTransactionParameterDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 工单数据访问类。
        /// </summary>
        public IWorkOrderDataEngine WorkOrderDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 工单工艺流程类
        /// </summary>
        public IWorkOrderRouteDataEngine WorkOrderRouteDataEngine 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// 工步访问类
        /// </summary>
        public IRouteEnterpriseDetailDataEngine RouteEnterpriseDetailDataEngine 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// 工步数据类
        /// </summary>
        public IRouteStepDataEngine RouteStepDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 工单领料明细数据访问类。
        /// </summary>
        public IMaterialReceiptDetailDataEngine MaterialReceiptDetailDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 线边仓物料明细数据访问类。
        /// </summary>
        public ILineStoreMaterialDetailDataEngine LineStoreMaterialDetailDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 供应商数据访问类。
        /// </summary>
        public ISupplierDataEngine SupplierDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 物料数据访问类。
        /// </summary>
        public IMaterialDataEngine MaterialDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 物料属性数据访问读写。
        /// </summary>
        public IMaterialAttributeDataEngine MaterialAttributeDataEngine { get; set; }

        /// <summary>
        /// 批次号生成对象。
        /// </summary>
        public ILotNumberGenerate LotNumberGenerate
        {
            get;
            set;
        }

        public IEquipmentDataEngine EquipmentDataEngine
        {
            get;
            set;
        }

        public WipEngineerService RefWipEngineerService
        {
            get;
            set;
        }
        
        /// <summary>
        /// 系统参数属性
        /// </summary>
        public IBaseAttributeValueDataEngine BaseAttributeValueDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 系统参数属性
        /// </summary>
        public IRouteStepAttributeDataEngine RouteStepAttributeDataEngine
        {
            get;
            set;
        }

        public ILotJobDataEngine LotJobDataEngine { get; set; }

        /// <summary>
        /// 批次创建操作。
        /// </summary>
        /// <param name="p">进站参数。</param>
        /// <returns>
        /// 代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult ILotCreateContract.Create(CreateParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (p == null)
            {
                result.Code = 1001;
                result.Message = StringResource.ParameterIsNull;
                return result;
            }
            try
            {
                //操作前检查。
                result = this.OnCheck(p);
                if (result.Code > 0)
                {
                    return result;
                }

                //执行操作
                //using (TransactionScope ts = new TransactionScope())
                //{
                //    result = this.OnExecuting(p);
                //    if (result.Code > 0)
                //    {
                //        return result;
                //    }

                result = this.OnExecuted(p);
                if (result.Code > 0)
                {
                    return result;
                }
                //ts.Complete();
                //}
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        //MethodReturnResult<IList<string>> ILotCreateContract.Generate(EnumLotType lotType, string orderNumber, int count, string prefix)
        //{
        //    MethodReturnResult<IList<string>> result = new MethodReturnResult<IList<string>>();

        //    result.Data = this.LotNumberGenerate.Generate(lotType, orderNumber, count, prefix);
           
        //    return result;
        //}

        /// <summary>
        /// 根据数量生成批次序列号。
        /// 标准组件序列号编码规则说明
        /// 第1、2位：公司名称（JN表示晋能）
        /// 第3位：生产车间（1表示102A车间，2表示102B车间）
        /// 第4、5位：组件生产年份加上8【如14年生产的组件，代码为22（14+8=22）】
        /// 第6、7位：组件生产月份【如7月份生产的组件，代码为15（7+8=15）】
        /// 第8、9位：组件生产日期【如30号生产的组件，代码为38（30+8）】
        /// 第10位：电池片类型：（1表示单晶，2表示多晶）
        /// 第11位：表示电池片数量：（1表示6*10组件，2表示6*12组件）
        /// 第12、13、14、15位：组件流水码
        /// </summary>
        /// <param name="lotType">批次类型</param>
        /// <param name="orderNumber">工单号</param>
        /// <param name="count">创建数量</param>
        /// <param name="lineCode">线别</param>
        /// <returns></returns>
        MethodReturnResult<IList<string>> ILotCreateContract.Generate(EnumLotType lotType, string orderNumber, int count, string lineCode)
        {
            MethodReturnResult<IList<string>> result = new MethodReturnResult<IList<string>>();

            try
            {
                IList<string> lstLotNumber = new List<string>();
                DateTime now = DateTime.Now;
                bool flagJN = false;
                string locationName = string.Empty;
                IList<WorkOrderAttribute> lstWorkOrderAttribute = null;
                WorkOrder wo = null;
                Material matrial = null;
                string qty = string.Empty;
                DateTime splitTime = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0);
                int year = Convert.ToInt32(now.ToString("yy"));
                MethodReturnResult<IList<WorkOrderAttribute>> resultFlagJN = new MethodReturnResult<IList<WorkOrderAttribute>>();
                MethodReturnResult<MaterialAttribute> resultOfMaterialAttr = new MethodReturnResult<MaterialAttribute>();
                MethodReturnResult<WorkOrderAttribute> workOrderAttrForTTSpecialDateOfEND = new MethodReturnResult<WorkOrderAttribute>();
                MethodReturnResult<WorkOrderAttribute> workOrderAttrForTTDateOfLot = new MethodReturnResult<WorkOrderAttribute>();
                MethodReturnResult<MaterialAttribute> materialAttrForTTSpecialDateOfEND = new MethodReturnResult<MaterialAttribute>();
                MethodReturnResult<MaterialAttribute> materialAttrForTTDateOfLot = new MethodReturnResult<MaterialAttribute>();
                PagingConfig cfgJN = new PagingConfig();
                PagingConfig cfgBanPian = new PagingConfig();
                string prefixLotNumber = null;
                int seqNo = 1;
                string minLotNumber = string.Empty;
                string maxLotNumber = string.Empty;
                IList<Lot> lstLot = null;
                string type = string.Empty;
                string lineCodeOfXX = string.Empty;

                #region 1.取得工单批次格式代码
                PagingConfig cfg = new PagingConfig()
                {
                    Where = string.Format(" Key.OrderNumber = '{0}' and Key.AttributeName = 'CreateCodeFormatNum'",
                                            orderNumber),
                    OrderBy = "Key.OrderNumber,Key.AttributeName"
                };

                lstWorkOrderAttribute = this.WorkOrderAttributeDataEngine.Get(cfg);

                if (lstWorkOrderAttribute == null || lstWorkOrderAttribute.Count == 0 || lstWorkOrderAttribute[0].AttributeValue == "XX02")
                {
                    #region 默认格式
                    #region 1.取得对应字段代码
                    //取得工单对象
                    wo = this.WorkOrderDataEngine.Get(orderNumber);
                    if (wo == null)
                    {
                        result.Code = 2001;
                        result.Message = string.Format("工单[{0}]获取失败！",
                                                       orderNumber);

                        return result;
                    }

                    //1.1 第3位：生产车间（1表示102A车间，2表示102B车间）                
                    if (wo.LocationName == "102A")
                    {
                        locationName = "1";
                    }
                    else if (wo.LocationName == "102B")
                    {
                        locationName = "2";
                    }
                    else if (wo.LocationName == "103A")
                    {
                        locationName = "3";
                    }
                    else
                    {
                        result.Code = 2002;
                        result.Message = string.Format("车间[{0}]不在范围内（102A、102B、103A）！",
                                                       wo.LocationName);

                        return result;
                    }

                    //1.2 第10位：电池片类型：（1表示单晶，2表示多晶）
                    //1201	单晶
                    //120101	125组件
                    //120102	156组件
                    //1202	多晶
                    //120201	125
                    //120202	156
                    
                    if (wo.MaterialCode.StartsWith("1201"))
                    {
                        type = "1";
                    }
                    else if (wo.MaterialCode.StartsWith("1202"))
                    {
                        type = "2";
                    }
                    else if (wo.MaterialCode.StartsWith("1203"))
                    {
                        type = "3";
                    }
                    else
                    {
                        result.Code = 2003;
                        result.Message = string.Format("物料类型[{0}]不在范围内(1201、1202、1203)！",
                                                       type);

                        return result;
                    }

                    //根据物料号获取物料对象
                    matrial = this.MaterialDataEngine.Get(wo.MaterialCode);
                    if (matrial == null)
                    {
                        result.Code = 2004;
                        result.Message = string.Format("物料[{0}]对象获取失败！",
                                                       wo.MaterialCode);

                        return result;
                    }

                    //1.3 第11位：表示电池片数量：（1表示6*10组件，2表示6*12组件）                  
                    if (matrial.MainRawQtyPerLot == 60)
                    {
                        qty = "1";
                    }
                    else if (matrial.MainRawQtyPerLot == 72)
                    {
                        qty = "2";
                    }
                    else
                    {
                        result.Code = 2005;
                        result.Message = string.Format("电池片数量[{0}]不在范围内（60、72）！",
                                                       matrial.MainRawQtyPerLot.ToString());

                        return result;
                    }

                    //1.4 6点为分割时间，6点前创建的批次使用前一天的流水号                    
                    if (now < splitTime)
                    {
                        now = now.AddDays(-1);
                    }                   
                    #endregion

                    #region 2.判断工单创批条件（JN/半片）
                    //是否加JN
                    cfgJN = new PagingConfig()
                    {
                        Where = string.Format(" Key.OrderNumber = '{0}' and Key.AttributeName='NoJN'"
                                                    , orderNumber),
                        OrderBy = "Key.OrderNumber,Key.AttributeName"
                    };
                    resultFlagJN.Data = this.WorkOrderAttributeDataEngine.Get(cfgJN);

                    if (resultFlagJN.Code > 0)
                    {
                        result.Code = resultFlagJN.Code;
                        result.Message = resultFlagJN.Message;

                        return result;
                    }

                    if (resultFlagJN.Data.Count > 0)
                    {
                        WorkOrderAttribute obj = resultFlagJN.Data[0];
                        if (Convert.ToBoolean(obj.AttributeValue))
                        {
                            flagJN = true;
                        }
                    }

                    //是否半片
                    MaterialAttributeKey materialAttributeKey = new MaterialAttributeKey()
                    {
                        MaterialCode = matrial.Key,
                        AttributeName = "IsBanPian"
                    };
                    resultOfMaterialAttr.Data = this.MaterialAttributeDataEngine.Get(materialAttributeKey);
                    if (resultOfMaterialAttr.Data != null)
                    {
                        if (Convert.ToBoolean(resultOfMaterialAttr.Data.Value.Trim()))
                        {
                            if (qty == "1")
                            {
                                qty = "4";
                            }
                            if (qty == "2")
                            {
                                qty = "5";
                            }
                        }
                    }

                    #endregion

                    #region 3.创建批次号固定部分
                    if (flagJN)
                    {
                        prefixLotNumber = string.Format("JN{0}{1}{2}{3}{4}{5}"
                                               , locationName
                                               , (year + 8).ToString("00")
                                               , (now.Month + 8).ToString("00")
                                               , (now.Day + 8).ToString("00")
                                               , type
                                               , qty);
                    }
                    else
                    {
                        prefixLotNumber = string.Format("{0}{1}{2}{3}{4}{5}"
                                                , locationName
                                                , (year + 8).ToString("00")
                                                , (now.Month + 8).ToString("00")
                                                , (now.Day + 8).ToString("00")
                                                , type
                                                , qty);
                    }
                    #endregion

                    #region 4.创建序号
                    minLotNumber = string.Format("{0}0001", prefixLotNumber);
                    maxLotNumber = string.Format("{0}9999", prefixLotNumber);

                    //按照线别生成不同的批次。
                    if (lineCode == "102A-A" || lineCode == "102B-A" || lineCode == "103A-A")
                    {
                        seqNo = 1;
                        minLotNumber = string.Format("{0}0001", prefixLotNumber);
                        maxLotNumber = string.Format("{0}2999", prefixLotNumber);
                    }
                    else if (lineCode == "102A-B" || lineCode == "102B-B" || lineCode == "103A-B")
                    {
                        seqNo = 3000;
                        minLotNumber = string.Format("{0}3000", prefixLotNumber);
                        maxLotNumber = string.Format("{0}5999", prefixLotNumber);
                    }
                    else if (lineCode == "102A-C")
                    {
                        seqNo = 6000;
                        minLotNumber = string.Format("{0}6000", prefixLotNumber);
                        maxLotNumber = string.Format("{0}8999", prefixLotNumber);
                    }
                    else if (lineCode == "102B-C")
                    {
                        seqNo = 6000;
                        minLotNumber = string.Format("{0}6000", prefixLotNumber);
                        maxLotNumber = string.Format("{0}8999", prefixLotNumber);
                    }
                    else if (lineCode == "103A-C")
                    {
                        seqNo = 6000;
                        minLotNumber = string.Format("{0}6000", prefixLotNumber);
                        maxLotNumber = string.Format("{0}8999", prefixLotNumber);
                    } 
                    else
                    {
                        seqNo = 9000;
                        minLotNumber = string.Format("{0}9000", prefixLotNumber);
                        maxLotNumber = string.Format("{0}9999", prefixLotNumber);
                    }

                    cfg = new PagingConfig()
                    {
                        PageNo = 0,
                        PageSize = 1,
                        Where = string.Format(@"Key>='{0}' AND Key<'{1}'
                                        AND IsMainLot=1"
                                                , minLotNumber
                                                , maxLotNumber),
                        OrderBy = "Key DESC"
                    };

                    lstLot = this.LotDataEngine.Get(cfg);
                    if (lstLot.Count > 0)
                    {
                        string maxSeqNo = lstLot[0].Key.Replace(prefixLotNumber, "");
                        if (int.TryParse(maxSeqNo, out seqNo))
                        {
                            seqNo = seqNo + 1;
                        }
                    }

                    for (int i = 0; i < count; i++)
                    {
                        lstLotNumber.Add(string.Format("{0}{1:0000}"
                                                        , prefixLotNumber
                                                        , seqNo + i));
                    }
                    #endregion
                    #endregion
                }
                else
                {
                    switch (lstWorkOrderAttribute[0].AttributeValue)
                    {
                        case "DF01":
                            #region 东方日升格式1（60组件）- 流水码分线别
                            #region 1.取得工单对象
                            //取得工单对象
                            wo = this.WorkOrderDataEngine.Get(orderNumber);
                            if (wo == null)
                            {
                                result.Code = 2001;
                                result.Message = string.Format("工单[{0}]获取失败！",
                                                               orderNumber);

                                return result;
                            }

                            //1.1 第3位：生产车间（1表示102A车间，2表示102B车间）                
                            if (wo.LocationName == "102A")
                            {
                                locationName = "1";
                            }
                            else if (wo.LocationName == "102B")
                            {
                                locationName = "2";
                            }
                            else
                            {
                                result.Code = 2002;
                                result.Message = string.Format("车间[{0}]不在范围内（102A、102B）！",
                                                               wo.LocationName);

                                return result;
                            }
                            #endregion

                            #region 2.6点为分割时间，6点前创建的批次使用前一天的流水号
                            if (now.Year == 2017 && now.Month == 2 && now.Day == 23)
                            {
                                now = now.AddDays(1);
                            }
                            
                            if (now < splitTime)
                            {
                                now = now.AddDays(-1);
                            }

                            if (now.Year == 2017 && now.Month == 2 && now.Day == 23)
                            {
                                now = now.AddDays(1);
                            }

                            #endregion

                            #region 3.创建批次号固定部分
                            prefixLotNumber = string.Format("{0}{1}{2}I610"
                                      , (year + 25).ToString("00")              //年
                                      , (now.Month + 11).ToString("00")         //月
                                      , (now.Day).ToString("00")                //日
                                      );
                            #endregion

                            #region 4.创建序号
                            minLotNumber = string.Format("{0}0001", prefixLotNumber);
                            maxLotNumber = string.Format("{0}9999", prefixLotNumber);

                            //按照线别生成不同的批次。
                            if (lineCode == "102A-A" || lineCode == "102B-A")
                            {
                                seqNo = 1;
                                minLotNumber = string.Format("{0}0001", prefixLotNumber);
                                maxLotNumber = string.Format("{0}1999", prefixLotNumber);
                            }
                            else if (lineCode == "102A-B" || lineCode == "102B-B")
                            {
                                seqNo = 2000;
                                minLotNumber = string.Format("{0}2000", prefixLotNumber);
                                maxLotNumber = string.Format("{0}3999", prefixLotNumber);
                            }
                            else if (lineCode == "102B-C")
                            {
                                seqNo = 4000;
                                minLotNumber = string.Format("{0}4000", prefixLotNumber);
                                maxLotNumber = string.Format("{0}5999", prefixLotNumber);
                            }
                            else if (lineCode == "102A-C")
                            {
                                seqNo = 6000;
                                minLotNumber = string.Format("{0}6000", prefixLotNumber);
                                maxLotNumber = string.Format("{0}8999", prefixLotNumber);
                            }
                            else
                            {
                                seqNo = 9000;
                                minLotNumber = string.Format("{0}9000", prefixLotNumber);
                                maxLotNumber = string.Format("{0}9999", prefixLotNumber);
                            }

                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key >= '{0}' AND Key < '{1}'
                                        AND IsMainLot=1"
                                                        , minLotNumber
                                                        , maxLotNumber),
                                OrderBy = "Key DESC"
                            };

                            lstLot = this.LotDataEngine.Get(cfg);
                            if (lstLot.Count > 0)
                            {
                                string maxSeqNo = lstLot[0].Key.Replace(prefixLotNumber, "");
                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            for (int i = 0; i < count; i++)
                            {
                                lstLotNumber.Add(string.Format("{0}{1:0000}"
                                                                , prefixLotNumber
                                                                , seqNo + i));
                            }
                            #endregion
                            #endregion

                            break;
                        case "DF02":
                            #region 东方日升格式2(72组件) - 流水码不分线别
                            #region 1.取得工单对象
                            //取得工单对象
                            wo = this.WorkOrderDataEngine.Get(orderNumber);
                            if (wo == null)
                            {
                                result.Code = 2001;
                                result.Message = string.Format("工单[{0}]获取失败！",
                                                               orderNumber);

                                return result;
                            }

                            //1.1 第3位：生产车间（1表示102A车间，2表示102B车间）                
                            if (wo.LocationName == "102A")
                            {
                                locationName = "1";
                            }
                            else if (wo.LocationName == "102B")
                            {
                                locationName = "2";
                            }
                            else
                            {
                                result.Code = 2002;
                                result.Message = string.Format("车间[{0}]不在范围内（102A、102B）！",
                                                               wo.LocationName);

                                return result;
                            }
                            #endregion

                            #region 2.6点为分割时间，6点前创建的批次使用前一天的流水号
                            if (now.Year == 2017 && now.Month == 2 && now.Day == 23)
                            {
                                now = now.AddDays(1);
                            }

                            if (now < splitTime)
                            {
                                now = now.AddDays(-1);
                            }

                            if (now.Year == 2017 && now.Month == 2 && now.Day == 23)
                            {
                                now = now.AddDays(1);
                            }
                            #endregion
                                                        
                            #region 3.创建批次号固定部分
                            prefixLotNumber = null;
                            prefixLotNumber = string.Format("{0}{1}{2}I610"
                                      , (year + 25).ToString("00")              //年
                                      , (now.Month + 11).ToString("00")         //月
                                      , (now.Day).ToString("00")                //日
                                      );
                            #endregion

                            #region 4.创建序号
                            seqNo = 1;
                            minLotNumber = string.Format("{0}0001", prefixLotNumber);
                            maxLotNumber = string.Format("{0}9999", prefixLotNumber);

                            //按照线别生成不同的批次。
                            //if (lineCode == "102A-A" || lineCode == "102B-A")
                            //{
                            //    seqNo = 1;
                            //    minLotNumber = string.Format("{0}0001", prefixLotNumber);
                            //    maxLotNumber = string.Format("{0}1999", prefixLotNumber);
                            //}
                            //else if (lineCode == "102A-B" || lineCode == "102B-B")
                            //{
                            //    seqNo = 2000;
                            //    minLotNumber = string.Format("{0}2000", prefixLotNumber);
                            //    maxLotNumber = string.Format("{0}3999", prefixLotNumber);
                            //}
                            //else if (lineCode == "102B-C")
                            //{
                            //    seqNo = 4000;
                            //    minLotNumber = string.Format("{0}4000", prefixLotNumber);
                            //    maxLotNumber = string.Format("{0}5999", prefixLotNumber);
                            //}
                            //else if (lineCode == "102A-C")
                            //{
                            //    seqNo = 6000;
                            //    minLotNumber = string.Format("{0}6000", prefixLotNumber);
                            //    maxLotNumber = string.Format("{0}8999", prefixLotNumber);
                            //}
                            //else
                            //{
                            //    seqNo = 9000;
                            //    minLotNumber = string.Format("{0}9000", prefixLotNumber);
                            //    maxLotNumber = string.Format("{0}9999", prefixLotNumber);
                            //}

                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key >= '{0}' AND Key < '{1}'
                                        AND IsMainLot=1"
                                                        , minLotNumber
                                                        , maxLotNumber),
                                OrderBy = "Key DESC"
                            };

                            lstLot = this.LotDataEngine.Get(cfg);
                            if (lstLot.Count > 0)
                            {
                                string maxSeqNo = lstLot[0].Key.Replace(prefixLotNumber, "");
                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            for (int i = 0; i < count; i++)
                            {
                                lstLotNumber.Add(string.Format("{0}{1:0000}"
                                                                , prefixLotNumber
                                                                , seqNo + i));
                            }
                            #endregion
                            #endregion

                            break;
                        case "DF03":
                            #region 东方日升格式3(72组件)
                            #region 1.取得工单对象
                            //取得工单对象
                            wo = this.WorkOrderDataEngine.Get(orderNumber);
                            if (wo == null)
                            {
                                result.Code = 2001;
                                result.Message = string.Format("工单[{0}]获取失败！",
                                                               orderNumber);

                                return result;
                            }

                            //1.1 第3位：生产车间（1表示102A车间，2表示102B车间）                
                            if (wo.LocationName == "102A")
                            {
                                locationName = "1";
                            }
                            else if (wo.LocationName == "102B")
                            {
                                locationName = "2";
                            }
                            else
                            {
                                result.Code = 2002;
                                result.Message = string.Format("车间[{0}]不在范围内（102A、102B）！",
                                                               wo.LocationName);

                                return result;
                            }
                            #endregion

                            #region 2.6点为分割时间，6点前创建的批次使用前一天的流水号
                            if (now.Year == 2017 && now.Month == 2 && now.Day == 23)
                            {
                                now = now.AddDays(1);
                            }

                            if (now < splitTime)
                            {
                                now = now.AddDays(-1);
                            }

                            #endregion

                            #region 3.创建批次号固定部分
                            prefixLotNumber = null;
                            prefixLotNumber = string.Format("{0}{1}{2}I610"
                                      , (year + 25).ToString("00")              //年
                                      , (now.Month + 11).ToString("00")         //月
                                      , (now.Day).ToString("00")                //日
                                      );
                            #endregion

                            #region 4.创建序号
                            seqNo = 1;
                            minLotNumber = string.Format("{0}0001", prefixLotNumber);
                            maxLotNumber = string.Format("{0}9999", prefixLotNumber);

                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key >= '{0}' AND Key < '{1}'
                                        AND IsMainLot=1"
                                                        , minLotNumber
                                                        , maxLotNumber),
                                OrderBy = "Key DESC"
                            };

                            lstLot = this.LotDataEngine.Get(cfg);
                            if (lstLot.Count > 0)
                            {
                                string maxSeqNo = lstLot[0].Key.Replace(prefixLotNumber, "");
                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            for (int i = 0; i < count; i++)
                            {
                                lstLotNumber.Add(string.Format("{0}{1:0000}"
                                                                , prefixLotNumber
                                                                , seqNo + i));
                            }
                            #endregion
                            #endregion

                            break;
                        case "XX01":
                            #region 协鑫格式(多晶72组件)
                            #region 1.取得工单对象
                            //取得工单对象
                            wo = this.WorkOrderDataEngine.Get(orderNumber);
                            if (wo == null)
                            {
                                result.Code = 2001;
                                result.Message = string.Format("工单[{0}]获取失败！",
                                                               orderNumber);

                                return result;
                            }

                            //1.1 第3位：生产车间（2表示102A车间，1表示102B车间）                
                            if (wo.LocationName == "102A")
                            {
                                locationName = "2";
                            }
                            else if (wo.LocationName == "102B")
                            {
                                locationName = "1";
                            }
                            else
                            {
                                result.Code = 2002;
                                result.Message = string.Format("车间[{0}]不在范围内（102A、102B）！",
                                                               wo.LocationName);

                                return result;
                            }
                            #endregion

                            #region 2.6点为分割时间，6点前创建的批次使用前一天的流水号
                            if (now < splitTime)
                            {
                                now = now.AddDays(-1);
                            }

                            #endregion

                            #region 3.创建批次号固定部分
                            prefixLotNumber = null;
                          
                            prefixLotNumber = string.Format("64{0}{1}{2}{3}{4}"
                                      , (year).ToString("00")              //年
                                      , (now.Month).ToString("00")         //月
                                      , (now.Day).ToString("00")           //日
                                      , locationName                       //线别
                                      ,orderNumber.Substring(14,2)         //工单号后两位                                  
                                      );
                            #endregion

                            #region 4.创建序号
                            seqNo = 1;
                            minLotNumber = string.Format("{0}0001", prefixLotNumber);
                            maxLotNumber = string.Format("{0}9999", prefixLotNumber);
                                                        
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key >= '{0}' AND Key < '{1}'
                                        AND IsMainLot=1"
                                                        , minLotNumber
                                                        , maxLotNumber),
                                OrderBy = "Key DESC"
                            };

                            lstLot = this.LotDataEngine.Get(cfg);
                            if (lstLot.Count > 0)
                            {
                                string maxSeqNo = lstLot[0].Key.Replace(prefixLotNumber, "");
                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            for (int i = 0; i < count; i++)
                            {
                                lstLotNumber.Add(string.Format("{0}{1:0000}"
                                                                , prefixLotNumber
                                                                , seqNo + i));
                            }
                            #endregion
                            #endregion

                            break;
                        case "XX03":
                            #region 协鑫格式(多晶60组件)
                            #region 1.取得工单对象
                            //取得工单对象
                            wo = this.WorkOrderDataEngine.Get(orderNumber);
                            if (wo == null)
                            {
                                result.Code = 2001;
                                result.Message = string.Format("工单[{0}]获取失败！",
                                                               orderNumber);

                                return result;
                            }

                            //1.1 第3位：生产车间（1表示102A车间，2表示102B车间）                
                            if (wo.LocationName == "102A")
                            {
                                locationName = "2";
                            }
                            else if (wo.LocationName == "102B")
                            {
                                locationName = "1";
                            }
                            else
                            {
                                result.Code = 2002;
                                result.Message = string.Format("车间[{0}]不在范围内（102A、102B）！",
                                                               wo.LocationName);

                                return result;
                            }
                            #endregion

                            #region 2.6点为分割时间，6点前创建的批次使用前一天的流水号
                            if (now < splitTime)
                            {
                                now = now.AddDays(-1);
                            }

                            #endregion

                            #region 3.创建批次号固定部分
                            prefixLotNumber = string.Format("64{0}{1}{2}{3}{4}"
                                      , (year).ToString("00")              //年
                                      , (now.Month).ToString("00")         //月
                                      , (now.Day).ToString("00")           //日
                                      , locationName                       //线别
                                      , orderNumber.Substring(14, 2)         //工单号后两位                                  
                                      );
                            #endregion

                            #region 4.创建序号
                            minLotNumber = string.Format("{0}0001", prefixLotNumber);
                            maxLotNumber = string.Format("{0}9999", prefixLotNumber);

                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key >= '{0}' AND Key < '{1}'
                                        AND IsMainLot=1"
                                                        , minLotNumber
                                                        , maxLotNumber),
                                OrderBy = "Key DESC"
                            };

                            lstLot = this.LotDataEngine.Get(cfg);
                            if (lstLot.Count > 0)
                            {
                                string maxSeqNo = lstLot[0].Key.Replace(prefixLotNumber, "");
                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            for (int i = 0; i < count; i++)
                            {
                                lstLotNumber.Add(string.Format("{0}{1:0000}"
                                                                , prefixLotNumber
                                                                , seqNo + i));
                            }
                            #endregion
                            #endregion

                            break;
                        case "XX04":
                            #region 协鑫格式(单晶72组件)
                            #region 1.取得工单对象
                            //取得工单对象
                            wo = this.WorkOrderDataEngine.Get(orderNumber);
                            if (wo == null)
                            {
                                result.Code = 2001;
                                result.Message = string.Format("工单[{0}]获取失败！",
                                                               orderNumber);

                                return result;
                            }

                            //1.1 第3位：生产车间（2表示102A车间，1表示102B车间）                
                            if (wo.LocationName == "102A")
                            {
                                locationName = "2";
                            }
                            else if (wo.LocationName == "102B")
                            {
                                locationName = "1";
                            }
                            else
                            {
                                result.Code = 2002;
                                result.Message = string.Format("车间[{0}]不在范围内（102A、102B）！",
                                                               wo.LocationName);

                                return result;
                            }
                            #endregion

                            #region 2.6点为分割时间，6点前创建的批次使用前一天的流水号
                            if (now < splitTime)
                            {
                                now = now.AddDays(-1);
                            }

                            #endregion

                            #region 3.创建批次号固定部分
                            prefixLotNumber = string.Format("64{0}{1}{2}{3}{4}"
                                      , (year).ToString("00")              //年
                                      , (now.Month).ToString("00")         //月
                                      , (now.Day).ToString("00")           //日
                                      , locationName                       //线别
                                      , orderNumber.Substring(14, 2)         //工单号后两位                                  
                                      );
                            #endregion

                            #region 4.创建序号
                            minLotNumber = string.Format("{0}0001", prefixLotNumber);
                            maxLotNumber = string.Format("{0}9999", prefixLotNumber);

                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key >= '{0}' AND Key < '{1}'
                                        AND IsMainLot=1"
                                                        , minLotNumber
                                                        , maxLotNumber),
                                OrderBy = "Key DESC"
                            };

                            lstLot = this.LotDataEngine.Get(cfg);
                            if (lstLot.Count > 0)
                            {
                                string maxSeqNo = lstLot[0].Key.Replace(prefixLotNumber, "");
                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            for (int i = 0; i < count; i++)
                            {
                                lstLotNumber.Add(string.Format("{0}{1:0000}"
                                                                , prefixLotNumber
                                                                , seqNo + i));
                            }
                            #endregion
                            #endregion

                            break;
                        case "XX05":
                            #region 协鑫格式(多晶60-永能代加工)
                            #region 1.取得工单对象
                            wo = this.WorkOrderDataEngine.Get(orderNumber);
                            if (wo == null)
                            {
                                result.Code = 2001;
                                result.Message = string.Format("工单[{0}]获取失败！",
                                                               orderNumber);

                                return result;
                            }

                            //1.1 第3位：生产车间（1表示102A车间，2表示102B车间，3代表103A车间）                
                            if (wo.LocationName == "102A")
                            {
                                locationName = "1";
                            }
                            else if (wo.LocationName == "102B")
                            {
                                locationName = "2";
                            }
                            else if (wo.LocationName == "103A")
                            {
                                locationName = "3";
                            }
                            else
                            {
                                result.Code = 2002;
                                result.Message = string.Format("车间[{0}]不在范围内（102A、102B、103A）！",
                                                               wo.LocationName);

                                return result;
                            }
                            #endregion

                            #region 2.6点为分割时间，6点前创建的批次使用前一天的流水号
                            if (now < splitTime)
                            {
                                now = now.AddDays(-1);
                            }

                            #endregion

                            #region 3.创建批次号固定部分
                            prefixLotNumber = string.Format("05{0}{1}{2}{3}{4}"    //晋能工厂代码：64/    永能工厂代码：05
                                      , (year).ToString("00")                      //年
                                      , (now.Month).ToString("00")                 //月
                                      , (now.Day).ToString("00")                   //日
                                      , locationName                               //线别 102A：1  102B：2  103A：3
                                      , "09"                                       //订单号：09                                  
                                      );
                            #endregion

                            #region 4.创建序号
                            minLotNumber = string.Format("{0}0001", prefixLotNumber);
                            maxLotNumber = string.Format("{0}9999", prefixLotNumber);

                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key >= '{0}' AND Key < '{1}'
                                        AND IsMainLot=1"
                                                        , minLotNumber
                                                        , maxLotNumber),
                                OrderBy = "Key DESC"
                            };

                            lstLot = this.LotDataEngine.Get(cfg);
                            if (lstLot.Count > 0)
                            {
                                string maxSeqNo = lstLot[0].Key.Replace(prefixLotNumber, "");
                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            for (int i = 0; i < count; i++)
                            {
                                lstLotNumber.Add(string.Format("{0}{1:0000}"
                                                                , prefixLotNumber
                                                                , seqNo + i));
                            }
                            #endregion
                            #endregion

                            break;

                        case "XX06":
                            #region 协鑫格式(单晶60-晋能代加工)

                            #region 1.取得工单对象
                            wo = this.WorkOrderDataEngine.Get(orderNumber);
                            if (wo == null)
                            {
                                result.Code = 2001;
                                result.Message = string.Format("工单[{0}]获取失败！",
                                                               orderNumber);

                                return result;
                            }

                            //1.1 第3位：生产车间（1表示102A车间，2表示102B车间，3代表103A车间）                
                            if (wo.LocationName == "102A")
                            {
                                locationName = "1";
                            }
                            else if (wo.LocationName == "102B")
                            {
                                locationName = "2";
                            }
                            else if (wo.LocationName == "103A")
                            {
                                locationName = "3";
                            }
                            else
                            {
                                result.Code = 2002;
                                result.Message = string.Format("车间[{0}]不在范围内（102A、102B、103A）！",
                                                               wo.LocationName);

                                return result;
                            }
                            #endregion

                            #region 2.6点为分割时间，6点前创建的批次使用前一天的流水号
                            if (now < splitTime)
                            {
                                now = now.AddDays(-1);
                            }

                            #endregion

                            #region 3.创建批次号固定部分
                            prefixLotNumber = string.Format("64{0}{1}{2}{3}{4}"    //晋能工厂代码：64/    永能工厂代码：05
                                      , (year).ToString("00")                      //年
                                      , (now.Month).ToString("00")                 //月
                                      , (now.Day).ToString("00")                   //日
                                      , locationName                               //线别 102A：1  102B：2  103A：3
                                      , orderNumber.Substring(14, 2)               //订单号：01  （工单号最后两位）                                
                                      );
                            #endregion

                            #region 4.创建序号
                            minLotNumber = string.Format("{0}0001", prefixLotNumber);
                            maxLotNumber = string.Format("{0}9999", prefixLotNumber);

                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key >= '{0}' AND Key < '{1}'
                                        AND IsMainLot=1"
                                                        , minLotNumber
                                                        , maxLotNumber),
                                OrderBy = "Key DESC"
                            };

                            lstLot = this.LotDataEngine.Get(cfg);
                            if (lstLot.Count > 0)
                            {
                                string maxSeqNo = lstLot[0].Key.Replace(prefixLotNumber, "");
                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            for (int i = 0; i < count; i++)
                            {
                                lstLotNumber.Add(string.Format("{0}{1:0000}"
                                                                , prefixLotNumber
                                                                , seqNo + i));
                            }
                            #endregion

                            #endregion

                            break;

                        case "XX07":
                            #region 协鑫格式(单晶72-晋能-张家港代加工)

                            #region 1.取得工单对象
                            wo = this.WorkOrderDataEngine.Get(orderNumber);
                            if (wo == null)
                            {
                                result.Code = 2001;
                                result.Message = string.Format("工单[{0}]获取失败！",
                                                               orderNumber);

                                return result;
                            }

                            //1.0 线别代码 晋中基地：4 / 文水基地：5
                            //1.1 订单号（103A车间：01 / 102A车间：02 / 102B车间：03）                
                            if (wo.LocationName == "103A")
                            {
                                locationName = "01";
                                lineCodeOfXX = "4";
                            }
                            else if (wo.LocationName == "102A")
                            {
                                locationName = "02";
                                lineCodeOfXX = "5";
                            }
                            else if (wo.LocationName == "102B")
                            {
                                locationName = "03";
                                lineCodeOfXX = "5";
                            }
                            else
                            {
                                result.Code = 2002;
                                result.Message = string.Format("车间[{0}]不在范围内（102A、102B、103A）！",
                                                               wo.LocationName);

                                return result;
                            }
                            #endregion

                            #region 2.6点为分割时间，6点前创建的批次使用前一天的流水号
                            if (now < splitTime)
                            {
                                now = now.AddDays(-1);
                            }

                            #endregion

                            #region 3.创建批次号固定部分
                            prefixLotNumber = string.Format("27{0}{1}{2}{3}{4}"    //晋能工厂代码：64  永能工厂代码：05  张家港工厂代码：27
                                      , (year).ToString("00")                      //年
                                      , (now.Month).ToString("00")                 //月
                                      , (now.Day).ToString("00")                   //日
                                      , lineCodeOfXX                               //线别 晋中基地：4 / 文水基地：5
                                      , locationName                               //订单号：103A车间：01 / 102A车间：02 / 102B车间：03                                
                                      );
                            #endregion

                            #region 4.创建序号
                            minLotNumber = string.Format("{0}0001", prefixLotNumber);
                            maxLotNumber = string.Format("{0}9999", prefixLotNumber);

                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key >= '{0}' AND Key <= '{1}'
                                        AND IsMainLot=1"
                                                        , minLotNumber
                                                        , maxLotNumber),
                                OrderBy = "Key DESC"
                            };

                            lstLot = this.LotDataEngine.Get(cfg);
                            if (lstLot.Count > 0)
                            {
                                string maxSeqNo = lstLot[0].Key.Replace(prefixLotNumber, "");
                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            for (int i = 0; i < count; i++)
                            {
                                lstLotNumber.Add(string.Format("{0}{1:0000}"
                                                                , prefixLotNumber
                                                                , seqNo + i));
                            }
                            #endregion

                            #endregion

                            break;

                        case "XX08":
                            #region 协鑫格式(72常规-晋能（文水加晋中）代加工)

                            #region 1.取得工单对象
                            wo = this.WorkOrderDataEngine.Get(orderNumber);
                            if (wo == null)
                            {
                                result.Code = 2001;
                                result.Message = string.Format("工单[{0}]获取失败！",
                                                               orderNumber);

                                return result;
                            }

                            //1.0 线别代码 晋中基地：4 / 文水基地：5
                            //1.1 订单号（103A车间：01 / 102A车间：02 / 102B车间：03）                
                            if (wo.LocationName == "103A")
                            {
                                locationName = "01";
                                lineCodeOfXX = "4";
                            }
                            else if (wo.LocationName == "102A")
                            {
                                locationName = "02";
                                lineCodeOfXX = "5";
                            }
                            else if (wo.LocationName == "102B")
                            {
                                locationName = "03";
                                lineCodeOfXX = "5";
                            }
                            else
                            {
                                result.Code = 2002;
                                result.Message = string.Format("车间[{0}]不在范围内（102A、102B、103A）！",
                                                               wo.LocationName);

                                return result;
                            }
                            #endregion

                            #region 2.6点为分割时间，6点前创建的批次使用前一天的流水号
                            if (now < splitTime)
                            {
                                now = now.AddDays(-1);
                            }

                            #endregion

                            #region 3.创建批次号固定部分
                            prefixLotNumber = string.Format("64{0}{1}{2}{3}{4}"    //晋能工厂代码：64  永能工厂代码：05  张家港工厂代码：27
                                      , (year).ToString("00")                      //年
                                      , (now.Month).ToString("00")                 //月
                                      , (now.Day).ToString("00")                   //日
                                      , lineCodeOfXX                               //线别 晋中基地：4 / 文水基地：5
                                      , locationName                               //订单号：103A车间：01 / 102A车间：02 / 102B车间：03                                
                                      );
                            #endregion

                            #region 4.创建序号
                            minLotNumber = string.Format("{0}0001", prefixLotNumber);
                            maxLotNumber = string.Format("{0}9999", prefixLotNumber);

                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key >= '{0}' AND Key <= '{1}'
                                        AND IsMainLot=1"
                                                        , minLotNumber
                                                        , maxLotNumber),
                                OrderBy = "Key DESC"
                            };

                            lstLot = this.LotDataEngine.Get(cfg);
                            if (lstLot.Count > 0)
                            {
                                string maxSeqNo = lstLot[0].Key.Replace(prefixLotNumber, "");
                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            for (int i = 0; i < count; i++)
                            {
                                lstLotNumber.Add(string.Format("{0}{1:0000}"
                                                                , prefixLotNumber
                                                                , seqNo + i));
                            }
                            #endregion

                            #endregion

                            break;

                        case "XX09":
                            #region 协鑫格式(72单晶-晋能（文水加晋中）代加工)

                            #region 1.取得工单对象
                            wo = this.WorkOrderDataEngine.Get(orderNumber);
                            if (wo == null)
                            {
                                result.Code = 2001;
                                result.Message = string.Format("工单[{0}]获取失败！",
                                                               orderNumber);

                                return result;
                            }

                            //1.0 线别代码 晋中基地：4 / 文水基地：5
                            //1.1 订单号（103A车间：01 / 102A车间：02 / 102B车间：03）                
                            if (wo.LocationName == "103A")
                            {
                                locationName = "01";
                                lineCodeOfXX = "4";
                            }
                            else if (wo.LocationName == "102A")
                            {
                                locationName = "02";
                                lineCodeOfXX = "5";
                            }
                            else if (wo.LocationName == "102B")
                            {
                                locationName = "03";
                                lineCodeOfXX = "5";
                            }
                            else
                            {
                                result.Code = 2002;
                                result.Message = string.Format("车间[{0}]不在范围内（102A、102B、103A）！",
                                                               wo.LocationName);

                                return result;
                            }
                            #endregion

                            #region 2.6点为分割时间，6点前创建的批次使用前一天的流水号
                            if (now < splitTime)
                            {
                                now = now.AddDays(-1);
                            }

                            #endregion

                            #region 3.创建批次号固定部分
                            prefixLotNumber = string.Format("64{0}{1}{2}{3}{4}"    //晋能工厂代码：64  永能工厂代码：05  张家港工厂代码：27
                                      , (year).ToString("00")                      //年
                                      , (now.Month).ToString("00")                 //月
                                      , (now.Day).ToString("00")                   //日
                                      , lineCodeOfXX                               //线别 晋中基地：4 / 文水基地：5
                                      , locationName                               //订单号：103A车间：01 / 102A车间：02 / 102B车间：03                                
                                      );
                            #endregion

                            #region 4.创建序号
                            minLotNumber = string.Format("{0}0001", prefixLotNumber);
                            maxLotNumber = string.Format("{0}9999", prefixLotNumber);

                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key >= '{0}' AND Key <= '{1}'
                                        AND IsMainLot=1"
                                                        , minLotNumber
                                                        , maxLotNumber),
                                OrderBy = "Key DESC"
                            };

                            lstLot = this.LotDataEngine.Get(cfg);
                            if (lstLot.Count > 0)
                            {
                                string maxSeqNo = lstLot[0].Key.Replace(prefixLotNumber, "");
                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            for (int i = 0; i < count; i++)
                            {
                                lstLotNumber.Add(string.Format("{0}{1:0000}"
                                                                , prefixLotNumber
                                                                , seqNo + i));
                            }
                            #endregion

                            #endregion

                            break;

                        case "SE01":
                            #region SE格式(单晶60组件)
                            #region 1.取得对应字段代码
                            //取得工单对象
                            wo = this.WorkOrderDataEngine.Get(orderNumber);
                            if (wo == null)
                            {
                                result.Code = 2001;
                                result.Message = string.Format("工单[{0}]获取失败！",
                                                               orderNumber);

                                return result;
                            }

                            //1.1 第3位：生产车间（1表示102A车间，2表示102B车间）                
                            if (wo.LocationName == "102A")
                            {
                                locationName = "1";
                            }
                            else if (wo.LocationName == "102B")
                            {
                                locationName = "2";
                            }
                            else if (wo.LocationName == "103A")
                            {
                                locationName = "3";
                            }
                            else
                            {
                                result.Code = 2002;
                                result.Message = string.Format("车间[{0}]不在范围内（102A、102B、103A）！",
                                                               wo.LocationName);

                                return result;
                            }

                            //1.2 第10位：电池片类型：（1表示单晶，2表示多晶）
                            //1201	单晶
                            //120101	125组件
                            //120102	156组件
                            //1202	多晶
                            //120201	125
                            //120202	156
                            if (wo.MaterialCode.StartsWith("1201"))
                            {
                                type = "1";
                            }
                            else if (wo.MaterialCode.StartsWith("1202"))
                            {
                                type = "2";
                            }
                            else if (wo.MaterialCode.StartsWith("1203"))
                            {
                                type = "3";
                            }
                            else
                            {
                                result.Code = 2003;
                                result.Message = string.Format("物料类型[{0}]不在范围内(1201、1202、1203)！",
                                                               type);

                                return result;
                            }

                            //根据物料号获取物料对象
                            matrial = this.MaterialDataEngine.Get(wo.MaterialCode);
                            if (matrial == null)
                            {
                                result.Code = 2004;
                                result.Message = string.Format("物料[{0}]对象获取失败！",
                                                               wo.MaterialCode);

                                return result;
                            }

                            //1.3 第11位：表示电池片数量：（1表示6*10组件，2表示6*12组件）                  
                            if (matrial.MainRawQtyPerLot == 60)
                            {
                                qty = "1";
                            }
                            else if (matrial.MainRawQtyPerLot == 72)
                            {
                                qty = "2";
                            }
                            else
                            {
                                result.Code = 2005;
                                result.Message = string.Format("电池片数量[{0}]不在范围内（60、72）！",
                                                               matrial.MainRawQtyPerLot.ToString());

                                return result;
                            }

                            //1.4 6点为分割时间，6点前创建的批次使用前一天的流水号                    
                            if (now < splitTime)
                            {
                                now = now.AddDays(-1);
                            }
                            #endregion

                            #region 2.判断工单是否创批时加JN
                            cfgJN = new PagingConfig()
                            {
                                Where = string.Format(" Key.OrderNumber = '{0}' and Key.AttributeName='NoJN'"
                                                            , orderNumber),
                                OrderBy = "Key.OrderNumber,Key.AttributeName"
                            };
                            resultFlagJN.Data = this.WorkOrderAttributeDataEngine.Get(cfgJN);

                            if (resultFlagJN.Code > 0)
                            {
                                result.Code = resultFlagJN.Code;
                                result.Message = resultFlagJN.Message;

                                return result;
                            }

                            if (resultFlagJN.Data.Count > 0)
                            {
                                WorkOrderAttribute obj = resultFlagJN.Data[0];
                                if (Convert.ToBoolean(obj.AttributeValue))
                                {
                                    flagJN = true;
                                }
                            }
                            #endregion

                            #region 3.创建批次号固定部分
                            if (flagJN)
                            {
                                prefixLotNumber = string.Format("JN{0}{1}{2}{3}{4}"
                                                       , locationName
                                                       , (year + 8).ToString("00")
                                                       , (now.Month + 8).ToString("00")
                                                       , (now.Day + 8).ToString("00")
                                                       , solaredgeType);
                            }
                            else
                            {
                                prefixLotNumber = string.Format("{0}{1}{2}{3}{4}"
                                                        , locationName
                                                        , (year + 8).ToString("00")
                                                        , (now.Month + 8).ToString("00")
                                                        , (now.Day + 8).ToString("00")
                                                        , solaredgeType);
                            }
                            #endregion

                            #region 4.创建序号
                            minLotNumber = string.Format("{0}0001", prefixLotNumber);
                            maxLotNumber = string.Format("{0}9999", prefixLotNumber);

                            //按照线别生成不同的批次。
                            if (lineCode == "102A-A" || lineCode == "102B-A" || lineCode == "103A-A")
                            {
                                seqNo = 1;
                                minLotNumber = string.Format("{0}0001", prefixLotNumber);
                                maxLotNumber = string.Format("{0}2999", prefixLotNumber);
                            }
                            else if (lineCode == "102A-B" || lineCode == "102B-B" || lineCode == "103A-B")
                            {
                                seqNo = 3000;
                                minLotNumber = string.Format("{0}3000", prefixLotNumber);
                                maxLotNumber = string.Format("{0}5999", prefixLotNumber);
                            }
                            else if (lineCode == "102A-C")
                            {
                                seqNo = 6000;
                                minLotNumber = string.Format("{0}6000", prefixLotNumber);
                                maxLotNumber = string.Format("{0}8999", prefixLotNumber);
                            }
                            else if (lineCode == "102B-C")
                            {
                                seqNo = 6000;
                                minLotNumber = string.Format("{0}6000", prefixLotNumber);
                                maxLotNumber = string.Format("{0}8999", prefixLotNumber);
                            }
                            else if (lineCode == "103A-C")
                            {
                                seqNo = 6000;
                                minLotNumber = string.Format("{0}6000", prefixLotNumber);
                                maxLotNumber = string.Format("{0}8999", prefixLotNumber);
                            }
                            else
                            {
                                seqNo = 9000;
                                minLotNumber = string.Format("{0}9000", prefixLotNumber);
                                maxLotNumber = string.Format("{0}9999", prefixLotNumber);
                            }

                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key>='{0}' AND Key<'{1}'
                                        AND IsMainLot=1"
                                                        , minLotNumber
                                                        , maxLotNumber),
                                OrderBy = "Key DESC"
                            };

                            lstLot = this.LotDataEngine.Get(cfg);
                            if (lstLot.Count > 0)
                            {
                                string maxSeqNo = lstLot[0].Key.Replace(prefixLotNumber, "");
                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            for (int i = 0; i < count; i++)
                            {
                                lstLotNumber.Add(string.Format("{0}{1:0000}"
                                                                , prefixLotNumber
                                                                , seqNo + i));
                            }
                            #endregion
                            #endregion

                            break;
                        case "TT01":
                            #region 塔塔项目创批规则
                            #region 1.取得对应字段代码
                            //取得工单对象
                            wo = this.WorkOrderDataEngine.Get(orderNumber);
                            if (wo == null)
                            {
                                result.Code = 2001;
                                result.Message = string.Format("工单[{0}]获取失败！",
                                                               orderNumber);

                                return result;
                            }

                            //1.1 第3位：生产车间（1表示102A车间，2表示102B车间）                
                            if (wo.LocationName == "102A")
                            {
                                locationName = "1";
                            }
                            else if (wo.LocationName == "102B")
                            {
                                locationName = "2";
                            }
                            else if (wo.LocationName == "103A")
                            {
                                locationName = "3";
                            }
                            else
                            {
                                result.Code = 2002;
                                result.Message = string.Format("车间[{0}]不在范围内（102A、102B、103A）！",
                                                               wo.LocationName);

                                return result;
                            }

                            //1.2 第10位：电池片类型：（1表示单晶，2表示多晶）
                            //1201	单晶
                            //120101	125组件
                            //120102	156组件
                            //1202	多晶
                            //120201	125
                            //120202	156
                            if (wo.MaterialCode.StartsWith("1201"))
                            {
                                type = "1";
                            }
                            else if (wo.MaterialCode.StartsWith("1202"))
                            {
                                type = "2";
                            }
                            else if (wo.MaterialCode.StartsWith("1203"))
                            {
                                type = "3";
                            }
                            else
                            {
                                result.Code = 2003;
                                result.Message = string.Format("物料类型[{0}]不在范围内(1201、1202、1203)！",
                                                               type);

                                return result;
                            }

                            //根据物料号获取物料对象
                            matrial = this.MaterialDataEngine.Get(wo.MaterialCode);
                            if (matrial == null)
                            {
                                result.Code = 2004;
                                result.Message = string.Format("物料[{0}]对象获取失败！",
                                                               wo.MaterialCode);

                                return result;
                            }

                            //1.3 第11位：表示电池片数量：（1表示6*10组件，2表示6*12组件）                  
                            if (matrial.MainRawQtyPerLot == 60)
                            {
                                qty = "1";
                            }
                            else if (matrial.MainRawQtyPerLot == 72)
                            {
                                qty = "2";
                            }
                            else
                            {
                                result.Code = 2005;
                                result.Message = string.Format("电池片数量[{0}]不在范围内（60、72）！",
                                                               matrial.MainRawQtyPerLot.ToString());

                                return result;
                            }

                            //1.4 6点为分割时间，6点前创建的批次使用前一天的流水号                    
                            if (now < splitTime)
                            {
                                now = now.AddDays(-1);
                            }
                            #endregion

                            #region 2.判断工单创批条件（JN/半片）
                            //是否加JN
                            cfgJN = new PagingConfig()
                            {
                                Where = string.Format(" Key.OrderNumber = '{0}' and Key.AttributeName='NoJN'"
                                                            , orderNumber),
                                OrderBy = "Key.OrderNumber,Key.AttributeName"
                            };
                            resultFlagJN.Data = this.WorkOrderAttributeDataEngine.Get(cfgJN);

                            if (resultFlagJN.Code > 0)
                            {
                                result.Code = resultFlagJN.Code;
                                result.Message = resultFlagJN.Message;

                                return result;
                            }

                            if (resultFlagJN.Data.Count > 0)
                            {
                                WorkOrderAttribute obj = resultFlagJN.Data[0];
                                if (Convert.ToBoolean(obj.AttributeValue))
                                {
                                    flagJN = true;
                                }
                            }

                            //是否半片
                            MaterialAttributeKey materialAttributeKey = new MaterialAttributeKey()
                            {
                                MaterialCode = matrial.Key,
                                AttributeName = "IsBanPian"
                            };
                            resultOfMaterialAttr.Data = this.MaterialAttributeDataEngine.Get(materialAttributeKey);
                            if (resultOfMaterialAttr.Data != null)
                            {
                                if (Convert.ToBoolean(resultOfMaterialAttr.Data.Value.Trim()))
                                {
                                    if (qty == "1")
                                    {
                                        qty = "4";
                                    }
                                    if (qty == "2")
                                    {
                                        qty = "5";
                                    }
                                }
                            }
                            
                            #endregion

                            #region 3.创建批次号固定部分
                            if (flagJN)
                            {
                                prefixLotNumber = string.Format("JN{0}{1}{2}{3}{4}{5}"
                                                       , locationName
                                                       , (year + 8).ToString("00")
                                                       , (now.Month + 8).ToString("00")
                                                       , (now.Day + 7).ToString("00")
                                                       , type
                                                       , qty);
                            }
                            else
                            {
                                prefixLotNumber = string.Format("{0}{1}{2}{3}{4}{5}"
                                                        , locationName
                                                        , (year + 8).ToString("00")
                                                        , (now.Month + 8).ToString("00")
                                                        , (now.Day + 7).ToString("00")
                                                        , type
                                                        , qty);
                            }
                            //查找工单非正常创批截至日期
                            WorkOrderAttributeKey workOrderAttributeKey0fDateAdd = new WorkOrderAttributeKey()
                            {
                                OrderNumber = orderNumber,
                                AttributeName = "SpecialDateOfEND"
                            };
                            workOrderAttrForTTSpecialDateOfEND.Data = this.WorkOrderAttributeDataEngine.Get(workOrderAttributeKey0fDateAdd);
                            if (workOrderAttrForTTSpecialDateOfEND.Data != null)
                            {
                                //在工单非正常创批截至日期创建指定日期批次号
                                WorkOrderAttributeKey workOrderAttributeKey0fDateAddMax = new WorkOrderAttributeKey()
                                {
                                    OrderNumber = orderNumber,
                                    AttributeName = "DateOfLot"
                                };
                                workOrderAttrForTTDateOfLot.Data = this.WorkOrderAttributeDataEngine.Get(workOrderAttributeKey0fDateAddMax);
                                if (workOrderAttrForTTDateOfLot.Data != null)
                                {
                                    string yearOfCreate = workOrderAttrForTTDateOfLot.Data.AttributeValue.Trim().Substring(0, 2);
                                    string monthOfCreate = workOrderAttrForTTDateOfLot.Data.AttributeValue.Trim().Substring(2, 2);
                                    string dayOfCreate = workOrderAttrForTTDateOfLot.Data.AttributeValue.Trim().Substring(4, 2);
                                    //(当前年份) < (非正常创批截至日期年份)
                                    if(year < Convert.ToInt32(workOrderAttrForTTSpecialDateOfEND.Data.AttributeValue.Trim().Substring(0,2)))
                                    {                                       
                                        //创建批次号固定部分
                                        prefixLotNumber = GetprefixLotNumber(flagJN, locationName, yearOfCreate, monthOfCreate, dayOfCreate, type, qty);
                                    }
                                    //(当前年份) == (非正常创批截至日期年份)
                                    if (year == Convert.ToInt32(workOrderAttrForTTSpecialDateOfEND.Data.AttributeValue.Trim().Substring(0, 2)))
                                    {
                                        //(当前月份) < (非正常创批截至日期月份)
                                        if (now.Month < Convert.ToInt32(workOrderAttrForTTSpecialDateOfEND.Data.AttributeValue.Trim().Substring(2, 2)))
                                        {
                                            //创建批次号固定部分
                                            prefixLotNumber = GetprefixLotNumber(flagJN, locationName, yearOfCreate, monthOfCreate, dayOfCreate, type, qty);
                                        }
                                        //(当前月份) == (非正常创批截至日期月份)
                                        if (now.Month == Convert.ToInt32(workOrderAttrForTTSpecialDateOfEND.Data.AttributeValue.Trim().Substring(2, 2)))
                                        {
                                            //(当前天数) < (非正常创批截至日期天数)
                                            if (now.Day < Convert.ToInt32(workOrderAttrForTTSpecialDateOfEND.Data.AttributeValue.Trim().Substring(4, 2)))
                                            {
                                                //创建批次号固定部分
                                                prefixLotNumber = GetprefixLotNumber(flagJN, locationName, yearOfCreate, monthOfCreate, dayOfCreate, type, qty);
                                            }
                                        }
                                    }                                    
                                }
                                else
                                {
                                    result.Code = 2006;
                                    result.Message = string.Format("工单{0}已设置非正常创批截止日期（SpecialDateOfEND）[{1}]号，请设置截止日期内需创建批次的指定日期(DateOfLot)！",
                                        orderNumber, Convert.ToInt32(workOrderAttrForTTDateOfLot.Data.AttributeValue.Trim()));

                                    return result;
                                }                           
                            }
                            else
                            {
                                ////查找产品编码非正常创批截至日期
                                MaterialAttributeKey materialAttributeKey0fDateAdd = new MaterialAttributeKey()
                                {
                                    MaterialCode = wo.MaterialCode,
                                    AttributeName = "SpecialDateOfEND"
                                };
                                materialAttrForTTSpecialDateOfEND.Data = this.MaterialAttributeDataEngine.Get(materialAttributeKey0fDateAdd);
                                if (materialAttrForTTSpecialDateOfEND.Data != null)
                                {
                                    //在产品编码非正常创批截至日期创建指定日期批次号
                                    MaterialAttributeKey materialAttributeKey0fDateAddMax = new MaterialAttributeKey()
                                    {
                                        MaterialCode = wo.MaterialCode,
                                        AttributeName = "DateOfLot"
                                    };
                                    materialAttrForTTDateOfLot.Data = this.MaterialAttributeDataEngine.Get(materialAttributeKey0fDateAddMax);
                                    if (materialAttrForTTDateOfLot.Data != null)
                                    {
                                        string yearOfCreate = materialAttrForTTDateOfLot.Data.Value.Trim().Substring(0, 2);
                                        string monthOfCreate = materialAttrForTTDateOfLot.Data.Value.Trim().Substring(2, 2);
                                        string dayOfCreate = materialAttrForTTDateOfLot.Data.Value.Trim().Substring(4, 2);
                                        //(当前年份) < (非正常创批截至日期年份)
                                        if (year < Convert.ToInt32(materialAttrForTTSpecialDateOfEND.Data.Value.Trim().Substring(0, 2)))
                                        {
                                            //创建批次号固定部分
                                            prefixLotNumber = GetprefixLotNumber(flagJN, locationName, yearOfCreate, monthOfCreate, dayOfCreate, type, qty);
                                        }
                                        //(当前年份) == (非正常创批截至日期年份)
                                        if (year == Convert.ToInt32(materialAttrForTTSpecialDateOfEND.Data.Value.Trim().Substring(0, 2)))
                                        {
                                            //(当前月份) < (非正常创批截至日期月份)
                                            if (now.Month < Convert.ToInt32(materialAttrForTTSpecialDateOfEND.Data.Value.Trim().Substring(2, 2)))
                                            {
                                                //创建批次号固定部分
                                                prefixLotNumber = GetprefixLotNumber(flagJN, locationName, yearOfCreate, monthOfCreate, dayOfCreate, type, qty);
                                            }
                                            //(当前月份) == (非正常创批截至日期月份)
                                            if (now.Month == Convert.ToInt32(materialAttrForTTSpecialDateOfEND.Data.Value.Trim().Substring(2, 2)))
                                            {
                                                //(当前天数) < (非正常创批截至日期天数)
                                                if (now.Day < Convert.ToInt32(materialAttrForTTSpecialDateOfEND.Data.Value.Trim().Substring(4, 2)))
                                                {
                                                    //创建批次号固定部分
                                                    prefixLotNumber = GetprefixLotNumber(flagJN, locationName, yearOfCreate, monthOfCreate, dayOfCreate, type, qty);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        result.Code = 1000;
                                        result.Message = string.Format("产品编码{0}已设置非正常创批截止日期（SpecialDateOfEND）[{1}]号，请设置截止日期内需创建批次的指定日期(DateOfLot)！",
                                            wo.MaterialCode, Convert.ToInt32(materialAttrForTTSpecialDateOfEND.Data.Value.Trim()));

                                        return result;
                                    }
                                }
                            }                         
                            #endregion

                            #region 4.创建序号
                            minLotNumber = string.Format("{0}0001", prefixLotNumber);
                            maxLotNumber = string.Format("{0}9999", prefixLotNumber);

                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key>='{0}' AND Key<'{1}'
                                        AND IsMainLot=1"
                                                        , minLotNumber
                                                        , maxLotNumber),
                                OrderBy = "Key DESC"
                            };

                            lstLot = this.LotDataEngine.Get(cfg);
                            if (lstLot.Count > 0)
                            {
                                string maxSeqNo = lstLot[0].Key.Replace(prefixLotNumber, "");
                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            for (int i = 0; i < count; i++)
                            {
                                lstLotNumber.Add(string.Format("{0}{1:0000}"
                                                                , prefixLotNumber
                                                                , seqNo + i));
                            }
                            #endregion
                            #endregion

                            break;
                        case "YL01":
                            #region 英利多晶项目创批规则-1

                            #region 1.创建批次号固定部分
                            prefixLotNumber = string.Format("{0}"
                                                        , "1844011115");
                            
                            #endregion

                            #region 2.创建序号
                            minLotNumber = string.Format("{0}00001", prefixLotNumber);
                            maxLotNumber = string.Format("{0}28420", prefixLotNumber);

                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key>='{0}' AND Key<='{1}'
                                        AND IsMainLot=1"
                                                        , minLotNumber
                                                        , maxLotNumber),
                                OrderBy = "Key DESC"
                            };

                            lstLot = this.LotDataEngine.Get(cfg);
                            if (lstLot.Count > 0)
                            {
                                string maxSeqNo = lstLot[0].Key.Replace(prefixLotNumber, "");
                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            for (int i = 0; i < count; i++)
                            {
                                lstLotNumber.Add(string.Format("{0}{1:00000}"
                                                                , prefixLotNumber
                                                                , seqNo + i));
                            }
                            #endregion

                            #endregion

                            break;

                        case "YL02":
                            #region 英利多晶项目创批规则-2

                            #region 1.创建批次号固定部分
                            prefixLotNumber = string.Format("{0}"
                                                        , "1846011115");

                            #endregion

                            #region 2.创建序号
                            minLotNumber = string.Format("{0}00001", prefixLotNumber);
                            maxLotNumber = string.Format("{0}55200", prefixLotNumber);

                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key>='{0}' AND Key<='{1}'
                                        AND IsMainLot=1"
                                                        , minLotNumber
                                                        , maxLotNumber),
                                OrderBy = "Key DESC"
                            };

                            lstLot = this.LotDataEngine.Get(cfg);
                            if (lstLot.Count > 0)
                            {
                                string maxSeqNo = lstLot[0].Key.Replace(prefixLotNumber, "");
                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            for (int i = 0; i < count; i++)
                            {
                                lstLotNumber.Add(string.Format("{0}{1:00000}"
                                                                , prefixLotNumber
                                                                , seqNo + i));
                            }
                            #endregion

                            #endregion

                            break;
                        default:
                            result.Code = 2006;
                            result.Message = string.Format("批次格式：[{0}]无法识别！", lstWorkOrderAttribute[0].AttributeValue);

                            return result;
                    }
                }
                #endregion

                result.Data = lstLotNumber;
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }

            return result;
        }

        string GetprefixLotNumber(bool flagJN,string locationName, string year,string month,string day,string type,string qty)
        {
            string prefixLotNumber = "";
            if (flagJN)
            {
                prefixLotNumber = string.Format("JN{0}{1}{2}{3}{4}{5}"
                                       , locationName
                                       , (Convert.ToInt32(year) + 8).ToString("00")
                                       , (Convert.ToInt32(month) + 8).ToString("00")
                                       , (Convert.ToInt32(day) + 7).ToString("00")
                                       , type
                                       , qty);
            }
            else
            {
                prefixLotNumber = string.Format("{0}{1}{2}{3}{4}{5}"
                                        , locationName
                                        , (Convert.ToInt32(year) + 8).ToString("00")
                                        , (Convert.ToInt32(month) + 8).ToString("00")
                                        , (Convert.ToInt32(day) + 7).ToString("00")
                                        , type
                                        , qty);
            }
            return prefixLotNumber;
        }

//        MethodReturnResult<IList<string>> ILotCreateContract.Generate(EnumLotType lotType, string orderNumber, int count, string lineCode)
//        {
//            MethodReturnResult<IList<string>> result = new MethodReturnResult<IList<string>>();

//            try
//            {
//                IList<string> lstLotNumber = new List<string>();
//                DateTime now = DateTime.Now;                
//                bool flagJN = false;
//                string locationName = string.Empty;
//                IList<WorkOrderAttribute> lstWorkOrderAttribute = null;

//                #region 1.取得工单批次格式代码
//                PagingConfig cfg = new PagingConfig()
//                {
//                    Where = string.Format(" Key.OrderNumber = '{0}' and Key.AttributeName = 'CreateCodeFormatNum'",
//                                            orderNumber),
//                    OrderBy = "Key.OrderNumber,Key.AttributeName"
//                };

//                lstWorkOrderAttribute = this.WorkOrderAttributeDataEngine.Get(cfg);

//                if (lstWorkOrderAttribute == null || lstWorkOrderAttribute.Count == 0)
//                {
//                    #region 默认格式
//                    #region 1.取得对应字段代码
//                    //取得工单对象
//                    WorkOrder wo = this.WorkOrderDataEngine.Get(orderNumber);
//                    if (wo == null)
//                    {
//                        result.Code = 2001;
//                        result.Message = string.Format("工单[{0}]获取失败！",
//                                                       orderNumber);

//                        return result;
//                    }

//                    //1.1 第3位：生产车间（1表示102A车间，2表示102B车间）                
//                    if (wo.LocationName == "102A")
//                    {
//                        locationName = "1";
//                    }
//                    else if (wo.LocationName == "102B")
//                    {
//                        locationName = "2";
//                    }
//                    else
//                    {
//                        result.Code = 2002;
//                        result.Message = string.Format("车间[{0}]不在范围内（102A、102B）！",
//                                                       wo.LocationName);

//                        return result;
//                    }

//                    //1.2 第10位：电池片类型：（1表示单晶，2表示多晶）
//                    //1201	单晶
//                    //120101	125组件
//                    //120102	156组件
//                    //1202	多晶
//                    //120201	125
//                    //120202	156
//                    string type = string.Empty;
//                    if (wo.MaterialCode.StartsWith("1201"))
//                    {
//                        type = "1";
//                    }
//                    else if (wo.MaterialCode.StartsWith("1202"))
//                    {
//                        type = "2";
//                    }
//                    else
//                    {
//                        result.Code = 2003;
//                        result.Message = string.Format("物料类型[{0}]不在范围内(1201、1202)！",
//                                                       type);

//                        return result;
//                    }

//                    //根据物料号获取物料对象
//                    Material matrial = this.MaterialDataEngine.Get(wo.MaterialCode);
//                    if (matrial == null)
//                    {
//                        result.Code = 2004;
//                        result.Message = string.Format("物料[{0}]对象获取失败！",
//                                                       wo.MaterialCode);

//                        return result;
//                    }

//                    //1.3 第11位：表示电池片数量：（1表示6*10组件，2表示6*12组件）
//                    string qty = string.Empty;
//                    if (matrial.MainRawQtyPerLot == 60)
//                    {
//                        qty = "1";
//                    }
//                    else if (matrial.MainRawQtyPerLot == 72)
//                    {
//                        qty = "2";
//                    }
//                    else
//                    {
//                        result.Code = 2005;
//                        result.Message = string.Format("电池片数量[{0}]不在范围内（60、72）！",
//                                                       matrial.MainRawQtyPerLot.ToString());

//                        return result;
//                    }

//                    //1.4 7点为分割时间，6点前创建的批次使用前一天的流水号
//                    DateTime splitTime = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0);

//                    if (now < splitTime)
//                    {
//                        now = now.AddDays(-1);
//                    }

//                    int year = Convert.ToInt32(now.ToString("yy"));
//                    #endregion

//                    #region 2.判断工单是否创批时加JN
//                    MethodReturnResult<IList<WorkOrderAttribute>> resultFlagJN = new MethodReturnResult<IList<WorkOrderAttribute>>();
//                    PagingConfig cfgJN = new PagingConfig()
//                    {
//                        Where = string.Format(" Key.OrderNumber = '{0}' and Key.AttributeName='NoJN'"
//                                                    , orderNumber),
//                        OrderBy = "Key.OrderNumber,Key.AttributeName"
//                    };

//                    resultFlagJN.Data = this.WorkOrderAttributeDataEngine.Get(cfgJN);

//                    if (resultFlagJN.Code > 0)
//                    {
//                        result.Code = resultFlagJN.Code;
//                        result.Message = resultFlagJN.Message;

//                        return result;
//                    }

//                    if (resultFlagJN.Data.Count > 0)
//                    {
//                        WorkOrderAttribute obj = resultFlagJN.Data[0];
//                        if (Convert.ToBoolean(obj.AttributeValue))
//                        {
//                            flagJN = true;
//                        }
//                    }
//                    #endregion

//                    #region 3.创建批次号固定部分
//                    string prefixLotNumber = null;
//                    if (flagJN)
//                    {

//                        prefixLotNumber = string.Format("JN{0}{1}{2}{3}{4}{5}"
//                                               , locationName
//                                               , (year + 8).ToString("00")
//                                               , (now.Month + 8).ToString("00")
//                                               , (now.Day + 8).ToString("00")
//                                               , type
//                                               , qty);
//                    }
//                    else
//                    {
//                        prefixLotNumber = string.Format("{0}{1}{2}{3}{4}{5}"
//                                                , locationName
//                                                , (year + 8).ToString("00")
//                                                , (now.Month + 8).ToString("00")
//                                                , (now.Day + 8).ToString("00")
//                                                , type
//                                                , qty);
//                    }
//                    #endregion

//                    #region 4.创建序号
//                    int seqNo = 1;
//                    string minLotNumber = string.Format("{0}0001", prefixLotNumber);
//                    string maxLotNumber = string.Format("{0}9999", prefixLotNumber);

//                    //按照线别生成不同的批次。
//                    if (lineCode == "102A-A" || lineCode == "102B-A")
//                    {
//                        seqNo = 1;
//                        minLotNumber = string.Format("{0}0001", prefixLotNumber);
//                        maxLotNumber = string.Format("{0}1999", prefixLotNumber);
//                    }
//                    else if (lineCode == "102A-B" || lineCode == "102B-B")
//                    {
//                        seqNo = 2000;
//                        minLotNumber = string.Format("{0}2000", prefixLotNumber);
//                        maxLotNumber = string.Format("{0}3999", prefixLotNumber);
//                    }
//                    else if (lineCode == "102B-C")
//                    {
//                        seqNo = 4000;
//                        minLotNumber = string.Format("{0}4000", prefixLotNumber);
//                        maxLotNumber = string.Format("{0}5999", prefixLotNumber);
//                    }
//                    else if (lineCode == "102A-C")
//                    {
//                        seqNo = 4000;
//                        minLotNumber = string.Format("{0}4000", prefixLotNumber);
//                        maxLotNumber = string.Format("{0}6999", prefixLotNumber);
//                    }
//                    else
//                    {
//                        seqNo = 6000;
//                        minLotNumber = string.Format("{0}6000", prefixLotNumber);
//                        maxLotNumber = string.Format("{0}9999", prefixLotNumber);
//                    }

//                    cfg = new PagingConfig()
//                    {
//                        PageNo = 0,
//                        PageSize = 1,
//                        Where = string.Format(@"Key>='{0}' AND Key<'{1}'
//                                        AND IsMainLot=1"
//                                                , minLotNumber
//                                                , maxLotNumber),
//                        OrderBy = "Key DESC"
//                    };

//                    IList<Lot> lstLot = this.LotDataEngine.Get(cfg);
//                    if (lstLot.Count > 0)
//                    {
//                        string maxSeqNo = lstLot[0].Key.Replace(prefixLotNumber, "");
//                        if (int.TryParse(maxSeqNo, out seqNo))
//                        {
//                            seqNo = seqNo + 1;
//                        }
//                    }

//                    for (int i = 0; i < count; i++)
//                    {
//                        lstLotNumber.Add(string.Format("{0}{1:0000}"
//                                                        , prefixLotNumber
//                                                        , seqNo + i));
//                    }
//                    #endregion
//                    #endregion
//                }
//                #endregion





//                result.Data = lstLotNumber;
//            }
//            catch (Exception ex)
//            {
//                result.Code = 1000;
//                result.Message = ex.Message;
//                result.Detail = ex.ToString();
//            }
            
//            return result;
//        }

        /// <summary>
        /// 根据批次个数生成批次号。
        /// </summary>
        /// <param name="count">批次个数。</param>
        /// <returns>批次号集合。</returns>
        IList<string> ILotNumberGenerate.Generate(EnumLotType lotType, string orderNumber, int count, string lineCode)
        {
            IList<string> lstLotNumber = new List<string>();
            DateTime now = DateTime.Now;
            string prefixLotNumber = "";
            int seqNo = 1;

            prefixLotNumber = string.Format("{0}{1:yyMMdd}", orderNumber, now);

            PagingConfig cfg = new PagingConfig()
            {
                PageNo = 0,
                PageSize = 1,
                Where = string.Format(@"Key LIKE '{0}%' 
                                        AND OrderNumber='{1}' 
                                        AND IsMainLot=1"
                                        , prefixLotNumber
                                        , orderNumber),
                OrderBy = "Key DESC"
            };

            IList<Lot> lstLot = this.LotDataEngine.Get(cfg);
            if (lstLot.Count > 0)
            {
                string maxSeqNo = lstLot[0].Key.Replace(prefixLotNumber, "");
                if (int.TryParse(maxSeqNo, out seqNo))
                {
                    seqNo = seqNo + 1;
                }
            }

            for (int i = 0; i < count; i++)
            {
                lstLotNumber.Add(string.Format("{0}{1:0000}"
                                                , prefixLotNumber
                                                , seqNo + i));
            }
            return lstLotNumber;
        }

        /// <summary>
        /// 操作前检查。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        MethodReturnResult ILotCreateCheck.Check(CreateParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            //if ((string.IsNullOrEmpty(p.RouteEnterpriseName)
            //    || string.IsNullOrEmpty(p.RouteName)
            //    || string.IsNullOrEmpty(p.RouteStepName)))
            //{
            //    result.Code = 2004;
            //    result.Message = StringResource.LotCreate_RouteIsNotNull;
            //    return result;
            //}

            //检查工单是否存在。
            WorkOrder wo = this.WorkOrderDataEngine.Get(p.OrderNumber ?? string.Empty);
            if (wo == null)
            {
                result.Code = 2001;
                result.Message = string.Format(StringResource.LotCreate_WorkOrderIsNotExists, p.OrderNumber);
                return result;
            }
            return result;
        }
        /// <summary>
        /// 执行优化器和批次匹配动作
        /// </summary>
        /// <param name="lot"></param>
        /// <returns></returns>
        public MethodReturnResult UpdateLotSEModules(Lot lot)
        {
            ISession session = this.LotDataEngine.SessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();
            MethodReturnResult result = new MethodReturnResult();
            result.Code = 0;
            try
            {
               // PagingConfig cfg = new PagingConfig()
               // {
               //     Where=string.Format("Key='{0}' and Attr3='{1}'",lot.Key,lot.Attr3),
               //     IsPaging=false
               // };
               //IList<Lot> lstLot=  this.LotDataEngine.Get(cfg);
               //if (lstLot.Count > 0)
               //{
               //    result.Code = 1000;
               //    result.Message = string.Format("组件{0}已经匹配过优化器{1}",lot.Key,lot.Attr3);
               //    return result;
               //}

                this.LotDataEngine.Update(lot, session);
                transaction.Commit();
                session.Close();
                
            }
            catch (Exception ex)
            {
                transaction.Rollback();     //事物回退
                session.Close();            //关闭事物连接
                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                return result;
            }
            return result;
        }

        /// <summary> 执行创批操作 </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        MethodReturnResult ILotCreate.Execute(CreateParameter p)
        {            
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            DateTime now = DateTime.Now;
            DateTime splitTime = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0);

            IList<RouteStep> lstRouteStep = null;
            WorkOrderRoute woRoute = null;

            List<Lot> lstLotDataEngineForInsert = new List<Lot>();
            List<LotTransaction> lstLotTransactionForInsert = new List<LotTransaction>();
            List<LotTransactionHistory> lstLotTransactionHistoryForInsert = new List<LotTransactionHistory>();
            MethodReturnResult<MaterialAttribute> materialAttrForTTSpecialDateOfEND = new MethodReturnResult<MaterialAttribute>();
            MethodReturnResult<MaterialAttribute> materialAttrForTTDateOfLot = new MethodReturnResult<MaterialAttribute>();
             
            #region 工单信息

            WorkOrder wo = this.WorkOrderDataEngine.Get(p.OrderNumber);

            //更新工单记录
            //判断创建批次数量是否超出工单未建批次数量
            if (wo.LeftQuantity < p.LotQuantity * p.LotNumbers.Count)
            {
                result.Code = 2001;
                result.Message = string.Format("建批数量({0})超出工单未建批数量（{1}）)", p.LotQuantity * p.LotNumbers.Count, wo.LeftQuantity);

                return result;
            }

            ////查找非正常创批截至日期
            //MaterialAttributeKey materialAttributeKey0fDateAdd = new MaterialAttributeKey()
            //{
            //    MaterialCode = wo.MaterialCode,
            //    AttributeName = "SpecialDateOfEND"
            //};
            //materialAttrForTTSpecialDateOfEND.Data = this.MaterialAttributeDataEngine.Get(materialAttributeKey0fDateAdd);
            //if (materialAttrForTTSpecialDateOfEND.Data != null)
            //{
            //    //在非正常创批截至日期创建指定日期批次号
            //    MaterialAttributeKey materialAttributeKey0fDateAddMax = new MaterialAttributeKey()
            //    {
            //        MaterialCode = wo.MaterialCode,
            //        AttributeName = "DateOfLot"
            //    };
            //    materialAttrForTTDateOfLot.Data = this.MaterialAttributeDataEngine.Get(materialAttributeKey0fDateAddMax);
            //    if (materialAttrForTTDateOfLot.Data != null)
            //    {
            //        if (now < splitTime)
            //        {
            //            now = now.AddDays(-1);
            //        } 
            //        if (now.Day < Convert.ToInt32(materialAttrForTTSpecialDateOfEND.Data.Value.Trim()))
            //        {
            //            now = Convert.ToDateTime(now.Year + "-" + now.Month + "-" + materialAttrForTTDateOfLot.Data.Value.Trim()).AddHours(8);
            //        }
            //    }
            //    else
            //    {
            //        result.Code = 1000;
            //        result.Message = string.Format("产品编码{0}已设置非正常创批截止日期（SpecialDateOfEND）[{1}]号，请设置截止日期内需创建批次的指定日期(DateOfLot)！",
            //            wo.MaterialCode, Convert.ToInt32(materialAttrForTTSpecialDateOfEND.Data.Value.Trim()));

            //        return result;
            //    }
            //}

            wo.WIPQuantity += p.LotQuantity * p.LotNumbers.Count;     //工单WIP数量
            wo.LeftQuantity -= p.LotQuantity * p.LotNumbers.Count;    //未投批数量
            wo.Editor = p.Creator;                                    //编辑人
            wo.EditTime = now;                                        //编辑时间

            #endregion

            #region 工艺流程信息

            //1.取得工单主生产工艺流程组
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format("Key.OrderNumber='{0}' AND IsRework = false", p.OrderNumber)
            };

            IList<WorkOrderRoute> woRouteList = this.WorkOrderRouteDataEngine.Get(cfg);
            
            if (woRouteList != null && woRouteList.Count > 0)
            {
                woRoute = woRouteList.First<WorkOrderRoute>();
            }
            else
            {
                result.Code = 1000;
                result.Message = string.Format("工单{0}的主流程未设置！", p.OrderNumber);

                return result;
            }

            //2.取得工艺流程开始工步
            cfg = new PagingConfig()
            {
                PageNo = 0,
                PageSize = 1,
                Where = string.Format(@"Key.RouteEnterpriseName='{0}'
                                            AND ItemNo='{1}'"
                                        , woRoute.RouteEnterpriseName
                                        , 1),
                OrderBy = "ItemNo"
            };

            IList<RouteEnterpriseDetail> lstRouteEnterpriseDetail = this.RouteEnterpriseDetailDataEngine.Get(cfg);

            if (lstRouteEnterpriseDetail.Count > 0)
            {
                //3.获取工艺流程的工步                
                cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format(@"Key.RouteName='{0}'"
                                            , lstRouteEnterpriseDetail[0].Key.RouteName),
                    OrderBy = "SortSeq"
                };

                lstRouteStep = this.RouteStepDataEngine.Get(cfg);

                if (lstRouteStep.Count == 0)
                {
                    result.Code = 1000;
                    result.Message = string.Format("工艺流程{0}的工艺工步未设置！", lstRouteEnterpriseDetail[0].Key.RouteName);

                    return result;
                }
            }
            else
            {
                result.Code = 1000;
                result.Message = string.Format("工艺流程组{0}的工艺流程未设置！", woRoute.RouteEnterpriseName);

                return result;
            }

            #endregion

            string strCurrRouteStepName = "创批";

            //循环新增批次。
            foreach (string lotNo in p.LotNumbers)
            {
                //string lotNumber = lotNo.Trim().ToUpper();
                string lotNumber = lotNo;

                DataSet ds = new DataSet();
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" select * from WIP_LOT_his where LOT_NUMBER ='{0}'", lotNumber);
                    ds = _db.ExecuteDataSet(cmd);
                }

                //判定是否存在批次记录。
                if (this.LotDataEngine.IsExists(lotNumber) || ds.Tables[0].Rows.Count > 0)
                {
                    result.Code = 1002;
                    result.Message = string.Format("批次（{0}）已存在。", lotNumber);

                    return result;
                }

                //生成操作事务主键。
                string transactionKey = Guid.NewGuid().ToString();
                
                #region 创建批次对象
                Lot lot = new Lot()
                {
                    Key = lotNumber,
                    ContainerNo = "",                    
                    DeletedFlag = false,
                    Description = p.Remark,                    
                    EquipmentCode = "",
                    Grade = "",
                    HoldFlag = false,
                    InitialQuantity = p.LotQuantity,
                    Quantity = p.LotQuantity,
                    IsMainLot = true,
                    LineCode = p.LineCode,
                    LocationName = wo.LocationName,
                    LotType = p.LotType,
                    MaterialCode = wo.MaterialCode,
                    OperateComputer = p.OperateComputer,
                    OrderNumber = p.OrderNumber,
                    OriginalOrderNumber = p.OrderNumber,
                    PackageFlag = false,
                    PreLineCode = "",
                    Priority = EnumLotPriority.Normal,
                    RepairFlag = 0,
                    Reworker = "",
                    ReworkFlag = 0,
                    ReworkTime = null,
                    RouteEnterpriseName = woRoute.RouteEnterpriseName,
                    RouteName = lstRouteStep[0].Key.RouteName,
                    RouteStepName = lstRouteStep[0].Key.RouteStepName,
                    ShippedFlag = false,
                    SplitFlag = false,
                    StartProcessTime = null,
                    StartWaitTime = now,
                    StateFlag = EnumLotState.WaitTrackIn,
                    Status = EnumObjectStatus.Available,
                    CreateTime = now,
                    Creator = p.Creator,
                    Editor = p.Creator,
                    EditTime = now,                    
                    Color = "",
                    Attr1 = "",
                    Attr2 = "",
                    Attr3 = "",
                    Attr4 = "",
                    Attr5 = ""
                };

                lstLotDataEngineForInsert.Add(lot);

                #endregion

                #region 记录操作事物
                LotTransaction transObj = new LotTransaction()
                {
                    Key = transactionKey,
                    Activity = EnumLotActivity.Create,
                    CreateTime = now,
                    Creator = p.Creator,
                    Description = p.Remark,
                    Editor = p.Creator,
                    EditTime = now,
                    InQuantity = 0,
                    LotNumber = lotNumber,
                    LocationName = wo.LocationName,
                    LineCode = p.LineCode,
                    OperateComputer = p.OperateComputer,
                    OrderNumber = p.OrderNumber,
                    OutQuantity = p.LotQuantity,
                    RouteEnterpriseName = lot.RouteEnterpriseName,
                    RouteName = lot.RouteName,
                    RouteStepName = strCurrRouteStepName,
                    ShiftName = "",
                    UndoFlag = false,
                    UndoTransactionKey = null
                };
                
                lstLotTransactionForInsert.Add(transObj);

                //批次事物对象
                LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lot);

                lotHistory.RouteStepName = strCurrRouteStepName;

                lstLotTransactionHistoryForInsert.Add(lotHistory);

                #endregion                                
            }

            #region 开始事物处理
            ITransaction transaction = null;
            ISession session = this.LotDataEngine.SessionFactory.OpenSession();
            transaction = session.BeginTransaction();

            try
            {                
                //更新批次基本信息
                foreach (Lot obj in lstLotDataEngineForInsert)
                {
                    this.LotDataEngine.Insert(obj, session);
                }

                //更新批次Transaction信息
                foreach (LotTransaction obj in lstLotTransactionForInsert)
                {
                    this.LotTransactionDataEngine.Insert(obj, session);
                }

                //更新批次LotTransaction信息 
                foreach (LotTransactionHistory obj in lstLotTransactionHistoryForInsert)
                {
                    this.LotTransactionHistoryDataEngine.Insert(obj, session);
                }

                //更新工单信息
                this.WorkOrderDataEngine.Update(wo, session);
                
                transaction.Commit();       //事物提交
                session.Close();            //关闭事物连接
            }
            catch (Exception err)
            {
                transaction.Rollback();     //事物回退
                session.Close();            //关闭事物连接

                result.Code = 1000;
                result.Message += string.Format(StringResource.Error, err.Message);
                result.Detail = err.ToString();
                return result;
            }
            #endregion

            return result;
        }

        MethodReturnResult ExecuteForBak(CreateParameter p)
        {
            DateTime now = DateTime.Now;
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            //PagingConfig cfg = new PagingConfig()
            //{
            //    IsPaging=false,
            //    Where = string.Format(@" Key.CategoryName='{0}' and Key.AttributeName='{1}'"
            //     ,"SystemConfigAttribute","IsCreateLotForConsumedCell")

            //};

            bool isCreateLotForConsumedCell = true;
            //获取工步属性数据。
            RouteStepAttributeKey key = new RouteStepAttributeKey()
            {
                RouteName = p.RouteName,
                RouteStepName = p.RouteStepName,
                AttributeName = "IsCreateLotForConsumedCell"
            };
            RouteStepAttribute rsa = this.RouteStepAttributeDataEngine.Get(key);
            if (rsa != null)
            {
                if (String.IsNullOrEmpty(rsa.Value) == false)
                {
                    Boolean.TryParse(rsa.Value, out isCreateLotForConsumedCell);
                }
            }

            //获取工单记录
            WorkOrder wo = this.WorkOrderDataEngine.Get(p.OrderNumber);
            //更新工单记录。
            WorkOrder woUpdate = wo.Clone() as WorkOrder;
            woUpdate.WIPQuantity += p.LotQuantity * p.LotNumbers.Count;
            woUpdate.LeftQuantity -= p.LotQuantity * p.LotNumbers.Count;
            woUpdate.Editor = p.Creator;
            woUpdate.EditTime = now;
            this.WorkOrderDataEngine.Update(woUpdate);

            LineStoreMaterialDetail lsmd = null;
            Material m = null;
            Supplier s = null;
            if (isCreateLotForConsumedCell)
            {
                #region //获取原材料库存记录，物料信息及供应商信息

                lsmd = this.LineStoreMaterialDetailDataEngine.Get(new LineStoreMaterialDetailKey()
                {
                    LineStoreName = p.LineStoreName,
                    OrderNumber = p.OrderNumber,
                    MaterialCode = p.RawMaterialCode,
                    MaterialLot = p.RawMaterialLot
                });
                if (lsmd == null)
                {
                    result.Code = 1003;
                    result.Message = string.Format(StringResource.LotCreate_MaterialReceiptError
                                                   , p.OrderNumber
                                                   , p.RawMaterialCode
                                                   , p.RawMaterialLot
                                                   , p.LineStoreName);
                    return result;
                }
                //更新原材料记录。
                double totalRawQuantity = (p.RawQuantity * p.LotNumbers.Count);
                if (lsmd.CurrentQty < totalRawQuantity)
                {
                    result.Code = 1004;
                    result.Message = string.Format("物料数量（{0}）不足，需要物料数量为({1})。"
                                                   , lsmd.CurrentQty
                                                   , totalRawQuantity);
                    return result;
                }
                LineStoreMaterialDetail lsmdUpdate = lsmd.Clone() as LineStoreMaterialDetail;
                lsmdUpdate.CurrentQty -= totalRawQuantity;
                lsmdUpdate.Editor = p.Creator;
                lsmdUpdate.EditTime = now;
                this.LineStoreMaterialDetailDataEngine.Update(lsmdUpdate);

                //获取物料
                m = this.MaterialDataEngine.Get(lsmd.Key.MaterialCode ?? string.Empty);
                //获取供应商
                s = this.SupplierDataEngine.Get(lsmd.SupplierCode ?? string.Empty);

                #endregion
            }

            p.TransactionKeys = new Dictionary<string, string>();

            //循环新增批次。
            foreach (string lotNo in p.LotNumbers)
            {
                //string lotNumber = lotNo.Trim().ToUpper();
                string lotNumber = lotNo;

                //判定是否存在批次记录。
                if (this.LotDataEngine.IsExists(lotNumber))
                {
                    result.Code = 1002;
                    result.Message = string.Format("批次（{0}）已存在。", lotNumber);
                    return result;
                }

                //生成操作事务主键。
                string transactionKey = Guid.NewGuid().ToString();
                p.TransactionKeys.Add(lotNumber, transactionKey);

                #region //新增批次记录。
                Lot obj = new Lot()
                {
                    Key = lotNumber,
                    ContainerNo = null,
                    CreateTime = now,
                    Creator = p.Creator,
                    DeletedFlag = false,
                    Description = p.Remark,
                    Editor = p.Creator,
                    EditTime = now,
                    EquipmentCode = null,
                    Grade = null,
                    HoldFlag = false,
                    InitialQuantity = p.LotQuantity,
                    Quantity = p.LotQuantity,
                    IsMainLot = true,
                    LineCode = p.LineCode,
                    LocationName = wo.LocationName,
                    LotType = p.LotType,
                    MaterialCode = wo.MaterialCode,
                    OperateComputer = p.OperateComputer,
                    OrderNumber = p.OrderNumber,
                    OriginalOrderNumber = p.OrderNumber,
                    PackageFlag = false,
                    PreLineCode = null,
                    Priority = EnumLotPriority.Normal,
                    RepairFlag = 0,
                    Reworker = null,
                    ReworkFlag = 0,
                    ReworkTime = null,
                    RouteEnterpriseName = p.RouteEnterpriseName,
                    RouteName = p.RouteName,
                    RouteStepName = p.RouteStepName,
                    ShippedFlag = false,
                    SplitFlag = false,
                    StartProcessTime = null,
                    StartWaitTime = now,
                    StateFlag = EnumLotState.WaitTrackIn,
                    Status = EnumObjectStatus.Available
                    //Attr1 = lsmd.Attr1,
                    //Attr2 = lsmd.Attr2,
                    //Attr3 = lsmd.Attr3,
                    //Attr4 = lsmd.Attr4,
                    //Attr5 = lsmd.Attr5                   
                };
                if (isCreateLotForConsumedCell && lsmd != null)
                {
                    obj.Attr1 = lsmd.Attr1;
                    obj.Attr2 = lsmd.Attr2;
                    obj.Attr3 = lsmd.Attr3;
                    obj.Attr4 = lsmd.Attr4;
                    obj.Attr5 = lsmd.Attr5;
                }
                this.LotDataEngine.Insert(obj);
                #endregion

                if (isCreateLotForConsumedCell)
                {
                    #region//新增批次BOM
                    LotBOM lotbomObj = new LotBOM()
                    {
                        Key = new LotBOMKey()
                        {
                            LotNumber = lotNumber,
                            MaterialLot = p.RawMaterialLot,
                            ItemNo = 1
                        },
                        TransactionKey = transactionKey,
                        LineStoreName = p.LineStoreName,
                        Qty = p.RawQuantity,
                        MaterialCode = p.RawMaterialCode,
                        MaterialName = m != null ? m.Name : string.Empty,
                        SupplierCode = lsmd.SupplierCode,
                        SupplierName = s != null ? s.Name : string.Empty,
                        RouteEnterpriseName = p.RouteEnterpriseName,
                        RouteName = p.RouteName,
                        RouteStepName = p.RouteStepName,
                        EquipmentCode = null,
                        LineCode = null,
                        MaterialFrom = EnumMaterialFrom.LineStore,
                        Creator = p.Creator,
                        CreateTime = now,
                        Editor = p.Creator,
                        EditTime = now
                    };
                    this.LotBOMDataEngine.Insert(lotbomObj);
                    #endregion
                }

                #region//记录操作历史。
                LotTransaction transObj = new LotTransaction()
                {
                    Key = transactionKey,
                    Activity = EnumLotActivity.Create,
                    CreateTime = now,
                    Creator = p.Creator,
                    Description = p.Remark,
                    Editor = p.Creator,
                    EditTime = now,
                    InQuantity = 0,
                    LotNumber = lotNumber,
                    LocationName = wo.LocationName,
                    LineCode = p.LineCode,
                    OperateComputer = p.OperateComputer,
                    OrderNumber = p.OrderNumber,
                    OutQuantity = p.LotQuantity,
                    RouteEnterpriseName = p.RouteEnterpriseName,
                    RouteName = p.RouteName,
                    RouteStepName = p.RouteStepName,
                    ShiftName = p.ShiftName,
                    UndoFlag = false,
                    UndoTransactionKey = null
                };
                this.LotTransactionDataEngine.Insert(transObj);
                #endregion

                #region //有附加的批次属性数据。
                if (p.Attributes != null && p.Attributes.ContainsKey(lotNumber))
                {
                    foreach (TransactionParameter tp in p.Attributes[lotNumber])
                    {
                        LotAttribute lotAttributeObj = new LotAttribute()
                        {
                            Key = new LotAttributeKey()
                            {
                                AttributeName = tp.Name,
                                LotNumber = lotNumber,
                            },
                            AttributeValue = tp.Value,
                            Editor = p.Creator,
                            EditTime = now
                        };
                        this.LotAttributeDataEngine.Insert(lotAttributeObj);
                    }
                }
                #endregion

                #region //有附加参数记录附加参数数据。
                if (p.Paramters != null && p.Paramters.ContainsKey(lotNumber))
                {
                    foreach (TransactionParameter tp in p.Paramters[lotNumber])
                    {
                        LotTransactionParameter lotParamObj = new LotTransactionParameter()
                        {
                            Key = new LotTransactionParameterKey()
                            {
                                TransactionKey = transactionKey,
                                ParameterName = tp.Name,
                                ItemNo = tp.Index,
                            },
                            ParameterValue = tp.Value,
                            Editor = p.Creator,
                            EditTime = now
                        };
                        this.LotTransactionParameterDataEngine.Insert(lotParamObj);
                    }
                }
                #endregion
            }
            return result;
        }
    }
}

