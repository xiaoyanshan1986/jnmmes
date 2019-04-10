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
using ServiceCenter.Common;
using System.ServiceModel.Activation;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.Model.EMS;
using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.Common.DataAccess.NHibernate;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Service.Contract.ZPVM;
using ServiceCenter.MES.Service.Class.COMMON;

namespace ServiceCenter.MES.Service.WIP
{
    // 实现批次包装服务契约。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public partial class LotPackageService : ILotPackageContract, ILotPackageCheck, ILotPackage, IPackageNoGenerate
    {
        #region 构造函数
        public LotPackageService()
        {
            this.RegisterCheckInstance(this);
            this.RegisterExecutedInstance(this);
            this.PackageNoGenerate = this;
        }

        #endregion

        #region 定义数据访问对象

        //产品编码成柜参数数据访问对象
        public IMaterialChestParameterDataEngine MaterialChestParameterDataEngine { get; set; }

        // 批次IV测试数据访问对象。
        public IIVTestDataDataEngine IVTestDataDataEngine { get; set; }

        // 工单等级包装规则数据访问对象。
        public IWorkOrderGradeDataEngine WorkOrderGradeDataEngine { get; set; }

        //工单分档规则数据访问对象
        public IWorkOrderPowersetDataEngine WorkOrderPowersetDataEngine { get; set; }

        // 批次数据访问对象。
        public ILotDataEngine LotDataEngine { get; set; }

        // 批次操作数据访问对象(批次加工历史)。
        public ILotTransactionDataEngine LotTransactionDataEngine { get; set; }

        // 批次历史数据访问对象。
        public ILotTransactionHistoryDataEngine LotTransactionHistoryDataEngine { get; set; }

        // 批次附加参数数据访问对象。
        public ILotTransactionParameterDataEngine LotTransactionParameterDataEngine { get; set; }

        // 批次包装操作数据访问对象。
        public ILotTransactionPackageDataEngine LotTransactionPackageDataEngine { get; set; }

        // 生产线数据访问对象。
        public IProductionLineDataEngine ProductionLineDataEngine { get; set; }

        // 区域数据访问对象。
        public ILocationDataEngine LocationDataEngine { get; set; }

        // 工序属性数据访问对象。
        public IRouteOperationAttributeDataEngine RouteOperationAttributeDataEngine { get; set; }

        // 包装数据访问对象。
        public IPackageDataEngine PackageDataEngine { get; set; }

        // 包装明细数据访问对象。
        public IPackageDetailDataEngine PackageDetailDataEngine { get; set; }

        // 包装号生成对象。
        public IPackageNoGenerate PackageNoGenerate { get; set; }

        // 批次用料数据访问对象。
        public ILotBOMDataEngine LotBOMDataEngine { get; set; }

        //混工单组规则数据访问对象
        public IWorkOrderGroupDetailDataEngine WorkOrderGroupDetailDataEngine { get; set; }

        // 基础数据值数据访问对象。
        public IBaseAttributeValueDataEngine BaseAttributeValueDataEngine { get; set; }

        // 工单属性数据访问对象
        public IWorkOrderAttributeDataEngine WorkOrderAttributeDataEngine { get; set; }

        // Oem批次数据访问对象
        public IOemDataEngine OemDataEngine { get; set; }

        // 工单数据访问对象
        public IWorkOrderDataEngine WorkOrderDataEngine { get; set; }

        #endregion

        #region 定于数据访问对象的列表
        List<Lot> lstLotDataEngineForUpdate = new List<Lot>();
        List<OemData> lstOemDataEngineForUpdate = new List<OemData>();
        List<LotTransaction> lstLotTransactionForInsert = new List<LotTransaction>();
        List<LotTransactionHistory> lstLotTransactionHistoryForInsert = new List<LotTransactionHistory>();
        List<LotTransactionParameter> lstLotTransactionParameterDataEngineForInsert = new List<LotTransactionParameter>();
        List<LotTransactionStep> lstLotTransactionStepDataEngineForInsert = new List<LotTransactionStep>();
        List<LotTransactionEquipment> lstLotTransactionEquipmentForUpdate = new List<LotTransactionEquipment>();
        List<LotTransactionEquipment> lstLotTransactionEquipmentForInsert = new List<LotTransactionEquipment>();
        List<Equipment> lstEquipmentForUpdate = new List<Equipment>();
        List<EquipmentStateEvent> lstEquipmentStateEventForInsert = new List<EquipmentStateEvent>();
        List<Package> lstPackageDataForUpdate = new List<Package>();
        List<Package> lstPackageDataForInsert = new List<Package>();
        List<PackageDetail> lstPackageDetailForInsert = new List<PackageDetail>();
        List<PackageDetail> lstPackageDetailForUpdate = new List<PackageDetail>();
        List<PackageDetail> lstPackageDetailForDelete = new List<PackageDetail>();
        List<PackageCornerDetail> lstPackageCornerDetailForDelete = new List<PackageCornerDetail>();
        List<PackageBin> lstPackageBinForUpdate = new List<PackageBin>();
        List<PackageCorner> lstPackageCornerForUpdate = new List<PackageCorner>();
        List<LotTransactionPackage> lstLotTransactionPackageForInsert = new List<LotTransactionPackage>();
        #endregion

        #region 定义事件

        // 操作前检查事件。
        public event Func<PackageParameter, MethodReturnResult> CheckEvent;

        // 执行操作时事件。
        public event Func<PackageParameter, MethodReturnResult> ExecutingEvent;

        // 操作执行完成事件。
        public event Func<PackageParameter, MethodReturnResult> ExecutedEvent;

        #endregion

        #region 定义事件清单列表

        // 自定义操作前检查的清单列表。
        private IList<ILotPackageCheck> CheckList { get; set; }

        // 自定义执行中操作的清单列表。
        private IList<ILotPackage> ExecutingList { get; set; }

        // 自定义执行后操作的清单列表。
        private IList<ILotPackage> ExecutedList { get; set; }

        #endregion

        #region 定义事件操作实例

        // 注册自定义检查的操作实例。
        public void RegisterCheckInstance(ILotPackageCheck obj)
        {
            if (this.CheckList == null)
            {
                this.CheckList = new List<ILotPackageCheck>();
            }
            this.CheckList.Add(obj);
        }

        // 注册执行中的自定义操作实例。
        public void RegisterExecutingInstance(ILotPackage obj)
        {
            if (this.ExecutingList == null)
            {
                this.ExecutingList = new List<ILotPackage>();
            }
            this.ExecutingList.Add(obj);
        }

        // 注册执行完成后的自定义操作实例。
        public void RegisterExecutedInstance(ILotPackage obj)
        {
            if (this.ExecutedList == null)
            {
                this.ExecutedList = new List<ILotPackage>();
            }
            this.ExecutedList.Add(obj);
        }

        #endregion

        #region 定义事件执行方法

        // 操作前检查。
        protected virtual MethodReturnResult OnCheck(PackageParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (this.CheckEvent != null)
            {
                foreach (Func<PackageParameter, MethodReturnResult> d in this.CheckEvent.GetInvocationList())
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
                foreach (ILotPackageCheck d in this.CheckList)
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

        // 操作执行中。
        protected virtual MethodReturnResult OnExecuting(PackageParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (this.ExecutingEvent != null)
            {
                foreach (Func<PackageParameter, MethodReturnResult> d in this.ExecutingEvent.GetInvocationList())
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
                foreach (ILotPackage d in this.ExecutingList)
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

        // 执行完成。
        protected virtual MethodReturnResult OnExecuted(PackageParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (this.ExecutedEvent != null)
            {
                foreach (Func<PackageParameter, MethodReturnResult> d in this.ExecutedEvent.GetInvocationList())
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
                foreach (ILotPackage d in this.ExecutedList)
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

        #endregion

        #region 批次包装操作执行方法

        //批次包装操作---0：成功，其他失败
        //手动包装--设置托号表头属性
        MethodReturnResult ILotPackageContract.Package(PackageParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();
            MethodReturnResult<string> resultPackageNo = new MethodReturnResult<string>(); //新托号
            ISession session = null;
            ITransaction transaction = null;

            if (p.LotNumbers.Count == 0 && p.LotNumbers.ToString() != "" && p.LotNumbers.ToString() != null)
            {
                result.Code = 2002;
                result.Message = string.Format("批次为空！");

                return result;
            }
            string[] lotNumbers = p.LotNumbers[0].ToString().Split(',');
            for (int i = 0; i < lotNumbers.Length; i++)
            {
                #region 批次入托
                try
                {
                    string strPackageNo = "";
                    PagingConfig cfg = null;
                    IList<LotBOM> lstLotBom = null;                             //批次BOM列表
                    string strSupplierCodeForLot = "";                          //批次电池片供应商
                    DateTime now = DateTime.Now;                                //当前时间
                    Package package = null;
                    int iFullpackageQty = 0;
                    PackageDetail packageDetail = null;                         //托包装明细对象
                    string transactionKey = "";                                 //操作事务主键
                    LotTransaction lotTrans = null;                             //批次事物对象
                    LotTransactionHistory lotTransHistory = null;               //批次历史事物对象
                    LotTransactionEquipment lotTransactionEquipment = null;     //加工设备事物对象
                    bool isNewPackage = false;                                  //托是否为新建
                    Lot lotCurr = null;                                         //自产组件
                    OemData oemData = null;                                     //Oem组件
                    Lot lot = null;                                             
                    WorkOrder oemWorkOrder = null;                              //Oem组件工单信息
                    IList<IVTestData> lstLotIVTestData = null;                  //自制组件IV测试数据
                    IList<WorkOrderPowerset> lstWorkOrderPowerset = null;       //自制组件工单分档规则
                    string locationName = string.Empty;                         //界面所选线别所在车间
                    ProductionLine line = new ProductionLine();                 //线别
                    Location location = new Location();                         //区域

                    if (p == null)
                    {
                        result.Code = 2001;
                        result.Message = StringResource.ParameterIsNull;
                        return result;
                    }

                    #region 1.检查工序是否是包装工序
                    bool isPackageOperation = false;
                    RouteOperationAttribute roAttr = this.RouteOperationAttributeDataEngine.Get(new RouteOperationAttributeKey()
                    {
                        RouteOperationName = p.RouteOperationName,
                        AttributeName = "IsPackageOperation"
                    });

                    //如果没有设置为包装工序，则直接返回。
                    if (roAttr == null || !bool.TryParse(roAttr.Value, out isPackageOperation) || isPackageOperation == false)
                    {
                        result.Code = 1009;
                        result.Message = string.Format("{0} 非包装工序，请确认。"
                                                        , p.RouteOperationName);
                        return result;
                    }
                    #endregion

                    #region 2.取得组件批次信息并判断
                    
                    #region 2.0 取得界面所选线别所在车间
                    line = this.ProductionLineDataEngine.Get(p.LineCode);
                    if (line == null)
                    {
                        result.Code = 2005;
                        result.Message = string.Format("产线[{0}]不存在！",
                                                        p.LineCode);
                        return result;
                    }
                    //根据线别所在区域，获取车间名称。
                    location = this.LocationDataEngine.Get(line.LocationName);

                    if (location == null)
                    {
                        result.Code = 2005;
                        result.Message = string.Format("产线[{0}]对应区域[{1}]不存在！",
                                                        p.LineCode,
                                                        line.LocationName);
                        return result;
                    }

                    locationName = location.ParentLocationName ?? string.Empty;
                    #endregion

                    //判断是否为OEM组件
                    oemData = this.OemDataEngine.Get(lotNumbers[i]);
                    if (oemData != null)
                    {
                        #region 2.1 取得OEM组件批次信息并判断
                        if (oemData.Status != EnumOemStatus.Import)
                        {
                            if (oemData.PackageNo != null && oemData.PackageNo != "")
                            {
                                result.Code = 2005;
                                result.Message = string.Format("批次[{0}]已在托[{1}]中！",
                                                                oemData.Key.ToString(),
                                                                oemData.PackageNo.ToString());
                                return result;
                            }
                            result.Code = 2005;
                            result.Message = string.Format("批次状态为【{0}】状态！", oemData.Status.GetDisplayName());
                            return result;
                        }                        
                        #endregion
                    }
                    else
                    {
                        #region 2.2 取得自制组件批次信息并判断
                        //取得批次信息
                        lotCurr = this.LotDataEngine.Get(lotNumbers[i]);

                        if (lotCurr == null)
                        {
                            result.Code = 2003;
                            result.Message = string.Format("批次[{0}]不存在！", lotNumbers[i]);

                            return result;
                        }

                        lot = lotCurr.Clone() as Lot;

                        //判断批次是否存在。
                        if (lot == null || lot.Status == EnumObjectStatus.Disabled)
                        {
                            result.Code = 2003;
                            result.Message = string.Format("批次：（{0}）不存在！", lotNumbers[i]);
                            return result;
                        }

                        //批次已撤销
                        if (lot.DeletedFlag == true)
                        {
                            result.Code = 2004;
                            result.Message = string.Format("批次：（{0}）已删除！", lot.Key);
                            return result;
                        }

                        //批次已暂停
                        if (lot.HoldFlag == true)
                        {
                            result.Code = 2005;
                            result.Message = string.Format("批次：（{0}）已暂停！", lot.Key);
                            return result;
                        }

                        //批次是否已经包装 lot.PackageFlag = true;
                        if (lot.PackageNo != "" && lot.PackageNo != null)
                        {
                            result.Code = 2005;
                            result.Message = string.Format("批次[{0}]已在托[{1}]中！",
                                                            lot.Key,
                                                            lot.PackageNo);
                            return result;
                        }

                        //批次是否当前工序
                        if (lot.RouteStepName != p.RouteOperationName)
                        {
                            result.Code = 2005;
                            result.Message = string.Format("批次[{0}]在[{1}]工序！",
                                                            lot.Key,
                                                            lot.RouteStepName);
                            return result;
                        }

                        //取得批次IV测试数据
                        cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format("Key.LotNumber = '{0}' AND IsDefault = 1", lot.Key)
                        };

                        IList<IVTestData> lstPackageLotIVTestData = this.IVTestDataDataEngine.Get(cfg);
                        //IV数据不存在
                        if (lstPackageLotIVTestData.Count == 0)
                        {
                            result.Code = 2000;
                            result.Message = string.Format("批次（{0}）IV测试数据不存在！", lot.Key);

                            return result;
                        }

                        //存在多条有效IV数据
                        if (lstPackageLotIVTestData.Count > 1)
                        {
                            result.Code = 2001;
                            result.Message = string.Format("批次（{0}）IV测试数据异常，存在多条有效记录，请重测！", lot.Key);

                            return result;
                        }

                        #region 判断车间是否相同
                        
                        if (lot.LocationName != locationName)
                        {
                            result.Code = 2005;
                            result.Message = string.Format("批次[{0}]在[{1}]车间！",
                                                            lot.Key,
                                                            lot.LocationName);
                            return result;
                        }

                        #endregion

                        //取得批次电池片厂商
                        cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format(@"Key.LotNumber='{0}' and MaterialCode like '{1}%'",
                                                    lot.Key,
                                                    "110"),
                            OrderBy = ""
                        };

                        lstLotBom = this.LotBOMDataEngine.Get(cfg);
                        if (lstLotBom != null && lstLotBom.Count > 0)
                        {
                            //取得批次对应电池片供应商
                            strSupplierCodeForLot = lstLotBom[0].SupplierCode;
                        }

                        //设置批次属性
                        lot.PreLineCode = lot.LineCode;
                        lot.LineCode = p.LineCode;

                        #endregion
                    }
                    #endregion

                    #region 3.取得工单托最大入托数
                    //取得工单设置最大入托数
                    CommonObjectDataEngine<WorkOrderRule, WorkOrderRuleKey> commonObjectDataEngine;
                    commonObjectDataEngine = new CommonObjectDataEngine<WorkOrderRule, WorkOrderRuleKey>(LotDataEngine.SessionFactory);
                    WorkOrderRuleKey workOrderRuleKey;
                    if (oemData != null)
                    {
                        oemWorkOrder = this.WorkOrderDataEngine.Get(oemData.OrderNumber.ToString().Trim().ToUpper());
                        if (oemWorkOrder == null)
                        {
                            result.Code = 2005;
                            result.Message = string.Format("批次[{0}]工单[{1}]不存在！",
                                                            oemData.Key.ToString(),
                                                            oemData.OrderNumber.ToString().Trim().ToUpper());
                            return result;
                        }

                        #region 3.1 判断OEM组件工单所在车间与界面选择是否一致
                        if (oemWorkOrder.LocationName != locationName)
                        {
                            result.Code = 2005;
                            result.Message = string.Format("批次[{0}]工单[{1}]所在车间[{2}]与界面所选线别[{3}]车间[{4}]不一致！",
                                                            oemData.Key.ToString(),
                                                            oemData.OrderNumber.ToString().Trim().ToUpper(),
                                                            oemWorkOrder.LocationName,
                                                            p.LineCode,
                                                            locationName);
                            return result;
                        }

                        #endregion

                        //oem组件工单信息
                        workOrderRuleKey = new WorkOrderRuleKey
                        {
                            OrderNumber = oemWorkOrder.Key,
                            MaterialCode = oemWorkOrder.MaterialCode
                        };
                    }
                    else
                    {
                        workOrderRuleKey = new WorkOrderRuleKey
                        {
                            OrderNumber = lot.OrderNumber,
                            MaterialCode = lot.MaterialCode
                        };
                    }

                    WorkOrderRule workOrderRule = commonObjectDataEngine.Get(workOrderRuleKey);

                    if (workOrderRule != null && workOrderRule.FullPackageQty.ToString() != "" && workOrderRule.FullPackageQty > 0)
                    {
                        int.TryParse(workOrderRule.FullPackageQty.ToString(), out iFullpackageQty);
                    }
                    else
                    {
                        result.Code = 2002;
                        result.Message = string.Format("工单[{0}]最大满托数未设置！", lot.OrderNumber);
                        return result;
                    }
                    #endregion

                    #region 4.取得托信息

                    if (p.PackageNo != "" && p.PackageNo != null)
                    {
                        //取得托对象                    
                        package = this.PackageDataEngine.Get(p.PackageNo);
                    }
                    if (resultPackageNo.Data != null)
                    {
                        if (resultPackageNo.Data.ToString() != "")
                        {
                            //取得托对象                    
                            package = this.PackageDataEngine.Get(resultPackageNo.Data.ToString());
                        }
                    }

                    #region 4.1 取得自制组件批次IV测试数据
                    if (oemData == null)
                    {
                        cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format("Key.LotNumber = '{0}' AND IsDefault = 1", lot.Key)
                        };
                        lstLotIVTestData = this.IVTestDataDataEngine.Get(cfg);
                        if (lstLotIVTestData != null && lstLotIVTestData.Count > 0)
                        {
                            #region 取得批次工单分档规则
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format("Key.OrderNumber = '{0}' AND Key.Code='{1}' AND Key.ItemNo = '{2}'", lot.OrderNumber, lstLotIVTestData[0].PowersetCode, lstLotIVTestData[0].PowersetItemNo)
                            };
                            lstWorkOrderPowerset = this.WorkOrderPowersetDataEngine.Get(cfg);
                            if (lstWorkOrderPowerset == null || lstWorkOrderPowerset.Count <= 0)
                            {
                                result.Code = 3001;
                                result.Message = string.Format("批次[{0}]所在工单[{1}]分档规则[{3}-{4}]不存在！",
                                    lot.Key, lot.OrderNumber, lstLotIVTestData[0].PowersetCode, lstLotIVTestData[0].PowersetItemNo);

                                return result;
                            }
                            #endregion
                        }
                        else
                        {
                            result.Code = 3001;
                            result.Message = string.Format("提取批次[{0}]测试数据失败！", lot.Key);

                            return result;
                        }
                    }
                    #endregion

                    #region 4.2 托号存在
                    if (package != null)
                    {
                        #region 4.2.0 托号所在车间是否与界面所选一致
                        if (package.Location != null && package.Location != "")
                        {
                            if (!package.IsLastPackage && !p.IsLastestPackage)
                            {
                                if (locationName != package.Location)
                                {
                                    result.Code = 2005;
                                    result.Message = string.Format("托号[{0}]所在车间[{1}]与界面所选线别[{2}]所在车间[{3}]不一致！",
                                                                    package.Key,
                                                                    package.Location,
                                                                    p.LineCode,
                                                                    locationName);
                                    return result;
                                }
                            }                           
                        }
                        #endregion

                        #region 4.2.1 包装是否包装中状态

                        if (package.PackageState != EnumPackageState.Packaging)
                        {
                            result.Code = 2006;
                            result.Message = string.Format("托：[{0}]状态[{1}]非包装中状态！", strPackageNo, package.PackageState.GetDisplayName());
                            return result;
                        }

                        #endregion                       

                        #region 4.2.2 处理空托
                        if (package.Quantity == 0)
                        {
                            string packagePowName = "";
                            if (package.Key.Substring(0, 2) == "64" || package.Key.Substring(0, 2) == "05")
                            {
                                packagePowName = package.Key.Trim().Substring(6, 3); //协鑫托号功率档
                            }
                            else
                            {
                                if (package.PowerName != null && package.PowerName != "")
                                {
                                    packagePowName = package.PowerName.Substring(0, 3);//常规托号功率档
                                }                                
                            }
                            
                            #region 4.2.2.1 如果托号功率档不为空,检验批次功率档与托号功率档是否一致
                            if (packagePowName != "" && packagePowName != null)
                            {
                                if (oemData != null)
                                {
                                    #region OEM组件校验批次功率档与托号功率档是否一致

                                    if (packagePowName != oemData.PnName.Substring(0, 3))
                                    {
                                        result.Code = 3001;
                                        result.Message = string.Format("批次[{0}]档位[{1}]与托号[{2}]档位[{3}]不一致！",
                                           oemData.Key, oemData.PnName.Substring(0, 3), package.Key, packagePowName);
                                        return result;
                                    }

                                    #endregion
                                    
                                }
                                else
                                {
                                    #region 自制组件校验批次功率档与托号功率档是否一致
                                    lstWorkOrderPowerset = this.WorkOrderPowersetDataEngine.Get(cfg);
                                    if (lstWorkOrderPowerset != null && lstWorkOrderPowerset.Count > 0)
                                    {
                                        if (packagePowName != lstWorkOrderPowerset[0].PowerName.Substring(0, 3))
                                        {
                                            result.Code = 3001;
                                            result.Message = string.Format("批次[{0}]档位[{1}]与托号[{2}]档位[{3}]不一致！",
                                                lot.Key, lstWorkOrderPowerset[0].PowerName.Substring(0, 3), package.Key, packagePowName);

                                            return result;
                                        }
                                    }
                                    #endregion
                                }
                            }
                            #endregion

                            #region 4.2.2.2 设置托号各属性值
                            if (oemData != null)
                            {
                                package.OrderNumber = oemWorkOrder.Key;           //工单号
                                package.MaterialCode = oemWorkOrder.MaterialCode; //物料代码
                                package.IsLastPackage = p.IsLastestPackage;       //设置尾包状态
                                package.SupplierCode = strSupplierCodeForLot;     //电池片供应商编号
                                package.Color = oemData.Color;                    //花色
                                package.Grade = oemData.Grade;                    //等级
                                package.PowerName = oemData.PnName;               //分档名称
                                package.PowerSubCode = oemData.PsSubCode;         //子分挡代码
                                package.LineCode = p.LineCode;                    //线别
                                package.Location = locationName;                  //车间
                            }
                            else
                            {
                                package.PowerSubCode = lstLotIVTestData[0].PowersetSubCode; //子分挡代码
                                package.PowerName = lstWorkOrderPowerset[0].PowerName;      //分档名称
                                package.OrderNumber = lot.OrderNumber;                      //工单号
                                package.MaterialCode = lot.MaterialCode;                    //物料代码
                                package.IsLastPackage = p.IsLastestPackage;                 //设置尾包状态
                                package.SupplierCode = strSupplierCodeForLot;               //电池片供应商编号
                                package.Color = lot.Color;                                  //花色
                                package.Grade = lot.Grade;                                  //等级
                                package.LineCode = p.LineCode;                              //线别
                                package.Location = locationName;                            //车间
                            }
                            #endregion
                        }
                        #endregion

                        #region 4.2.3 非尾托和空托判断控制条件
                        if (!package.IsLastPackage && package.Quantity > 0)
                        {
                            #region 界面未勾选尾包
                            if (p.IsLastestPackage == false)
                            {
                                bool isPackageLimitedForWorkOrder = false;

                                if (oemData != null)
                                {
                                    #region 2.托工单与批次工单混托控制(OEM)
                                    //包装号里的工单若跟批次的工单相同，则不需要判断属性
                                    if (string.Compare(oemWorkOrder.Key, package.OrderNumber, true) != 0)
                                    {
                                        #region OEM组件混托控制
                                        //批次工单混工单属性
                                        WorkOrderAttribute lotWorkOrderAttribute = this.WorkOrderAttributeDataEngine.Get(new WorkOrderAttributeKey()
                                        {
                                            OrderNumber = oemWorkOrder.Key,
                                            AttributeName = "PackageLimited"
                                        });

                                        //未设置默认为允许混工单(false)
                                        if (lotWorkOrderAttribute == null || !bool.TryParse(lotWorkOrderAttribute.AttributeValue, out isPackageLimitedForWorkOrder))
                                        {
                                            isPackageLimitedForWorkOrder = false;
                                        }

                                        if (isPackageLimitedForWorkOrder == true)
                                        {
                                            result.Code = 2007;
                                            result.Message = string.Format("批次：[{0}]所在工单[{1}]不允许混工单！", oemData.Key, oemWorkOrder.Key);

                                            return result;
                                        }

                                        //托工单混托属性
                                        WorkOrderAttribute packageWorkOrderAttribute = this.WorkOrderAttributeDataEngine.Get(new WorkOrderAttributeKey()
                                        {
                                            OrderNumber = package.OrderNumber,
                                            AttributeName = "PackageLimited"
                                        });

                                        //未设置默认为允许混工单(false)
                                        if (packageWorkOrderAttribute == null || !bool.TryParse(packageWorkOrderAttribute.AttributeValue, out isPackageLimitedForWorkOrder))
                                        {
                                            isPackageLimitedForWorkOrder = false;
                                        }

                                        if (isPackageLimitedForWorkOrder == true)
                                        {
                                            result.Code = 2008;
                                            result.Message = string.Format("托：[{0}]所在工单[{1}]不允许混工单！", package.Key, package.OrderNumber);
                                            return result;
                                        }
                                        #endregion

                                        #region 判断要包装OEM批次的工单是否设置混工单组
                                        cfg = new PagingConfig()
                                        {
                                            Where = string.Format(@"Key.OrderNumber = '{0}'", oemWorkOrder.Key)
                                        };
                                        IList<WorkOrderGroupDetail> lstLotWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                                        //批次所在工单设置了混工单组
                                        if (lstLotWorkOrderGroupDetail != null && lstLotWorkOrderGroupDetail.Count > 0)
                                        {
                                            #region 判断要托工单是否设置混工单组
                                            cfg = new PagingConfig()
                                            {
                                                Where = string.Format(@"Key.OrderNumber = '{0}'", package.OrderNumber)
                                            };
                                            IList<WorkOrderGroupDetail> lstPackageWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                                            //托工单设置了混工单组
                                            if (lstPackageWorkOrderGroupDetail != null && lstPackageWorkOrderGroupDetail.Count > 0)
                                            {
                                                if (lstLotWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString() != lstPackageWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString())
                                                {
                                                    result.Code = 2008;
                                                    result.Message = string.Format("托：（{0}）所在工单（{1} 设置的混工单组（{2}）与入托批次（{3}）所在工单（{4}）设置的混工单组（{5}）不一致！",
                                                                                    package.Key, package.OrderNumber, lstPackageWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString(),
                                                                                    oemData.Key, oemWorkOrder.Key, lstLotWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString());
                                                    return result;
                                                }
                                            }
                                            //托工单没设混工单组
                                            else
                                            {
                                                result.Code = 2008;
                                                result.Message = string.Format("托：（{0}）所在工单（{1} 未设置混工单组规则，但要入托批次（{2}）所在工单（{3}）设置了混工单组！",
                                                                                package.Key, package.OrderNumber, oemData.Key, oemWorkOrder.Key);
                                                return result;
                                            }
                                            #endregion
                                        }
                                        //批次所在工单没设混工单组
                                        else
                                        {
                                            #region 判断要托工单是否设置混工单组
                                            cfg = new PagingConfig()
                                            {
                                                Where = string.Format(@"Key.OrderNumber = '{0}'", package.OrderNumber)
                                            };
                                            IList<WorkOrderGroupDetail> lstPackageWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                                            //托工单设置了混工单组
                                            if (lstPackageWorkOrderGroupDetail != null && lstPackageWorkOrderGroupDetail.Count > 0)
                                            {
                                                result.Code = 2008;
                                                result.Message = string.Format("托：（{0}）所在工单（{1} 已设置混工单组规则，但要入托批次（{2}）所在工单（{3}）未设置混工单组！",
                                                                                package.Key, package.OrderNumber, oemData.Key, oemWorkOrder.Key);
                                                return result;
                                            }
                                            #endregion
                                        }
                                        #endregion
                                    }
                                    #endregion

                                    #region 3.判断产品是否一致，必须一致方可入托（OEM）
                                    if (package.MaterialCode != oemWorkOrder.MaterialCode && package.MaterialCode != "")
                                    {
                                        result.Code = 2009;
                                        result.Message = string.Format("包装物料[{0}]与批次物料[{1}]不一致！",
                                                                        package.MaterialCode,
                                                                        oemWorkOrder.MaterialCode);
                                        return result;
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region 2.托工单与批次工单混托控制（自制）
                                    //包装号里的工单若跟批次的工单相同，则不需要判断属性
                                    if (string.Compare(lot.OrderNumber, package.OrderNumber, true) != 0)
                                    {
                                        #region 自制组件混托控制
                                        //批次工单混工单属性
                                        WorkOrderAttribute lotWorkOrderAttribute = this.WorkOrderAttributeDataEngine.Get(new WorkOrderAttributeKey()
                                        {
                                            OrderNumber = lot.OrderNumber,
                                            AttributeName = "PackageLimited"
                                        });

                                        //未设置默认为允许混工单(false)
                                        if (lotWorkOrderAttribute == null || !bool.TryParse(lotWorkOrderAttribute.AttributeValue, out isPackageLimitedForWorkOrder))
                                        {
                                            isPackageLimitedForWorkOrder = false;
                                        }

                                        if (isPackageLimitedForWorkOrder == true)
                                        {
                                            result.Code = 2007;
                                            result.Message = string.Format("批次：[{0}]所在工单[{1}]不允许混工单！", lot.Key, lot.OrderNumber);

                                            return result;
                                        }

                                        //托工单混托属性
                                        WorkOrderAttribute packageWorkOrderAttribute = this.WorkOrderAttributeDataEngine.Get(new WorkOrderAttributeKey()
                                        {
                                            OrderNumber = package.OrderNumber,
                                            AttributeName = "PackageLimited"
                                        });

                                        //未设置默认为允许混工单(false)
                                        if (packageWorkOrderAttribute == null || !bool.TryParse(packageWorkOrderAttribute.AttributeValue, out isPackageLimitedForWorkOrder))
                                        {
                                            isPackageLimitedForWorkOrder = false;
                                        }

                                        if (isPackageLimitedForWorkOrder == true)
                                        {
                                            result.Code = 2008;
                                            result.Message = string.Format("托：[{0}]所在工单[{1}]不允许混工单！", package.Key, package.OrderNumber);
                                            return result;
                                        }
                                        #endregion

                                        #region 判断要包装批次的工单是否设置混工单组
                                        cfg = new PagingConfig()
                                        {
                                            Where = string.Format(@"Key.OrderNumber = '{0}'", lot.OrderNumber)
                                        };
                                        IList<WorkOrderGroupDetail> lstLotWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                                        //批次所在工单设置了混工单组
                                        if (lstLotWorkOrderGroupDetail != null && lstLotWorkOrderGroupDetail.Count > 0)
                                        {
                                            #region 判断要托工单是否设置混工单组
                                            cfg = new PagingConfig()
                                            {
                                                Where = string.Format(@"Key.OrderNumber = '{0}'", package.OrderNumber)
                                            };
                                            IList<WorkOrderGroupDetail> lstPackageWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                                            //托工单设置了混工单组
                                            if (lstPackageWorkOrderGroupDetail != null && lstPackageWorkOrderGroupDetail.Count > 0)
                                            {
                                                if (lstLotWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString() != lstPackageWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString())
                                                {
                                                    result.Code = 2008;
                                                    result.Message = string.Format("托：（{0}）所在工单（{1} 设置的混工单组（{2}）与入托批次（{3}）所在工单（{4}）设置的混工单组（{5}）不一致！",
                                                                                    package.Key, package.OrderNumber, lstPackageWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString(),
                                                                                    lot.Key, lot.OrderNumber, lstLotWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString());
                                                    return result;
                                                }
                                            }
                                            //托工单没设混工单组
                                            else
                                            {
                                                result.Code = 2008;
                                                result.Message = string.Format("托：（{0}）所在工单（{1} 未设置混工单组规则，但要入托批次（{2}）所在工单（{3}）设置了混工单组！",
                                                                                package.Key, package.OrderNumber, lot.Key, lot.OrderNumber);
                                                return result;
                                            }
                                            #endregion
                                        }
                                        //批次所在工单没设混工单组
                                        else
                                        {
                                            #region 判断要托工单是否设置混工单组
                                            cfg = new PagingConfig()
                                            {
                                                Where = string.Format(@"Key.OrderNumber = '{0}'", package.OrderNumber)
                                            };
                                            IList<WorkOrderGroupDetail> lstPackageWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                                            //托工单设置了混工单组
                                            if (lstPackageWorkOrderGroupDetail != null && lstPackageWorkOrderGroupDetail.Count > 0)
                                            {
                                                result.Code = 2008;
                                                result.Message = string.Format("托：（{0}）所在工单（{1} 已设置混工单组规则，但要入托批次（{2}）所在工单（{3}）未设置混工单组！",
                                                                                package.Key, package.OrderNumber, lot.Key, lot.OrderNumber);
                                                return result;
                                            }
                                            #endregion
                                        }
                                        #endregion
                                    }
                                    #endregion

                                    #region 3.判断产品是否一致，必须一致方可入托（自制）
                                    if (package.MaterialCode != lot.MaterialCode && package.MaterialCode != "")
                                    {
                                        result.Code = 2009;
                                        result.Message = string.Format("包装物料[{0}]与批次物料[{1}]不一致！",
                                                                        package.MaterialCode,
                                                                        lot.MaterialCode);
                                        return result;
                                    }
                                    #endregion
                                }

                                #region 4.检查电池片供应商是否可以混装
                                //取得是否允许不同的电池片供应商混装
                                cfg = new PagingConfig()
                                {
                                    IsPaging = false,
                                    Where = string.Format(@"Key.CategoryName='{0}' 
                                    AND Key.AttributeName='{1}'"
                                                        , "SystemParameters"
                                                        , "PackageChkMaterialSupplier"),
                                    OrderBy = "Key.ItemOrder"
                                };

                                //取得电池片混包标志
                                bool blChkSupplierCode = false;
                                IList<BaseAttributeValue> lstBaseAttributeValues = BaseAttributeValueDataEngine.Get(cfg);

                                if (lstBaseAttributeValues != null && lstBaseAttributeValues.Count > 0)
                                {
                                    if (String.IsNullOrEmpty(lstBaseAttributeValues[0].Value) != true)
                                    {
                                        Boolean.TryParse(lstBaseAttributeValues[0].Value, out blChkSupplierCode);
                                    }
                                }

                                if (blChkSupplierCode)      //需要进行供应商的检测
                                {
                                    //判断电池片供应商是否一致
                                    if (string.Compare(package.SupplierCode, strSupplierCodeForLot, true) != 0)
                                    {
                                        if (oemData != null)
                                        {
                                            result.Code = 2010;
                                            result.Message = string.Format("托[{0}]电池片供应商[{1}]与批次[{2}]电池片供应商[{3}]不一致。",
                                                                            package.Key,
                                                                            package.SupplierCode,
                                                                            oemData.Key,
                                                                            strSupplierCodeForLot);
                                            return result;
                                        }
                                        else
                                        {
                                            result.Code = 2010;
                                            result.Message = string.Format("托[{0}]电池片供应商[{1}]与批次[{2}]电池片供应商[{3}]不一致。",
                                                                            package.Key,
                                                                            package.SupplierCode,
                                                                            lot.Key,
                                                                            strSupplierCodeForLot);
                                            return result;
                                        }
                                    }
                                }
                                #endregion

                                #region 5.校验等级、花色、分档、电流规则
                                if (oemData != null)
                                {
                                    result = CheckLotInPackage(oemData, oemWorkOrder, package);
                                }
                                else
                                {
                                    result = CheckLotInPackage(lot, package);
                                }

                                if (result.Code > 0)
                                {
                                    return result;
                                }
                                #endregion
                            }
                            #endregion

                            #region 界面勾选尾包
                            else
                            {
                                package.IsLastPackage = true;
                            }
                            #endregion
                        }
                        #endregion

                        #region 4.2.4 尾托控制
                        if (package.IsLastPackage)
                        {
                            cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format(@"Key.CategoryName='{0}' 
                                    AND Key.AttributeName='{1}'"
                                                    , "LastPackageControlParameters"
                                                    , "ByMaterialCode"),
                                OrderBy = "Key.ItemOrder"
                            };

                            //取得物料编码控制标志
                            bool byMaterialCode = false;
                            IList<BaseAttributeValue> lstBaseAttributeValues = BaseAttributeValueDataEngine.Get(cfg);
                            if (lstBaseAttributeValues != null && lstBaseAttributeValues.Count > 0)
                            {
                                if (String.IsNullOrEmpty(lstBaseAttributeValues[0].Value) != true)
                                {
                                    Boolean.TryParse(lstBaseAttributeValues[0].Value, out byMaterialCode);
                                }
                            }
                            if (byMaterialCode)   //需要检验物料编码一致
                            {
                                if (oemData != null)
                                {
                                    #region 判断产品是否一致，必须一致方可入托（OEM）
                                    if (package.MaterialCode != oemWorkOrder.MaterialCode && package.MaterialCode != "")
                                    {
                                        result.Code = 2009;
                                        result.Message = string.Format("包装物料[{0}]与批次物料[{1}]不一致！",
                                                                        package.MaterialCode,
                                                                        oemWorkOrder.MaterialCode);
                                        return result;
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region 判断产品是否一致，必须一致方可入托（自制）
                                    if (package.MaterialCode != lot.MaterialCode && package.MaterialCode != "")
                                    {
                                        result.Code = 2009;
                                        result.Message = string.Format("包装物料[{0}]与批次物料[{1}]不一致！",
                                                                        package.MaterialCode,
                                                                        lot.MaterialCode);
                                        return result;
                                    }
                                    #endregion
                                }
                            }
                        }
                        #endregion

                        //设置托包装属性
                        package.Quantity += 1;                      //包装数量                    
                        package.Editor = p.Operator;                //编辑人
                        package.EditTime = now;                     //编辑日期                     
                    }
                    #endregion

                    #region 4.3 托号不存在
                    else
                    {
                        #region 4.3.1 创建新托对象

                        #region 4.3.1.1 生成托号
                        if (p.PackageNo == "" || p.PackageNo == null)
                        {
                            #region 创建新托号
                            if (oemData != null || oemWorkOrder != null)
                            {
                                resultPackageNo = PackageNoGenerate.CreatePackageNo(oemData, oemWorkOrder);
                            }
                            else
                            {
                                resultPackageNo = PackageNoGenerate.CreatePackageNo(lot.Key);
                            }

                            if (resultPackageNo.Code > 0)
                            {
                                result.Code = resultPackageNo.Code;
                                result.Message = resultPackageNo.Message;

                                return result;
                            }

                            strPackageNo = resultPackageNo.Data;
                            #endregion
                        }
                        else
                        {
                            #region 使用界面托号
                            if (p.PackageNo.Length <= 12)
                            {
                                strPackageNo = p.PackageNo;
                            }
                            else
                            {
                                result.Code = 1000;
                                result.Message = string.Format("托号：[{0}]长度大于12！", p.PackageNo);

                                return result;
                            }
                            #endregion
                        }
                        #endregion

                        #region 4.3.1.2 生成托对象
                        if (oemWorkOrder != null)
                        {
                            package = new Package()
                            {
                                Key = strPackageNo,                                      //主键托号
                                ContainerNo = "",                                        //容器号
                                OrderNumber = oemWorkOrder.Key,                          //工单号
                                PackageState = EnumPackageState.Packaging,               //包装状态
                                MaterialCode = oemWorkOrder.MaterialCode,                //物料代码
                                IsLastPackage = p.IsLastestPackage,                      //是否尾包
                                Quantity = 1,                                            //包装数量
                                PackageType = EnumPackageType.Packet,                    //包装类型 Packet=0 ---按包 Box=1 --- 按箱
                                PackageMixedType = EnumPackageMixedType.UnMixedType,     //包装最大类型UnMixedType=0 ---按包 MixedType=1 --- 按箱
                                Description = "",                                        //描述
                                Checker = null,                                          //检验人
                                CheckTime = null,                                        //检验时间
                                ToWarehousePerson = null,                                //入库人
                                ToWarehouseTime = null,                                  //入库时间
                                ShipmentPerson = null,                                   //出货人
                                ShipmentTime = null,                                     //出货时间
                                Creator = p.Operator,                                    //创建人
                                CreateTime = now,                                        //创建时间                        
                                Editor = p.Operator,                                     //编辑人             
                                EditTime = now,                                          //编辑时间 
                                SupplierCode = strSupplierCodeForLot,                    //电池片供应商编号 
                                Color = oemData.Color,                                   //花色
                                Grade = oemData.Grade,                                   //等级
                                PowerName = oemData.PnName,                              //分档名称
                                PowerSubCode = oemData.PsSubCode,                        //子分挡代码
                                LineCode = p.LineCode,                                   //线别
                                Location = locationName                                  //车间
                            };
                        }
                        else
                        {
                            package = new Package()
                            {
                                Key = strPackageNo,                                     //主键托号
                                ContainerNo = "",                                       //容器号
                                OrderNumber = lot.OrderNumber,                          //工单号
                                PackageState = EnumPackageState.Packaging,              //包装状态
                                MaterialCode = lot.MaterialCode,                        //物料代码
                                IsLastPackage = p.IsLastestPackage,                     //是否尾包
                                Quantity = 1,                                           //包装数量
                                PackageType = EnumPackageType.Packet,                   //包装类型 Packet=0 ---按包 Box=1 --- 按箱
                                PackageMixedType = EnumPackageMixedType.UnMixedType,    //包装最大类型UnMixedType=0 ---按包 MixedType=1 --- 按箱
                                Description = "",                                       //描述
                                Checker = null,                                         //检验人
                                CheckTime = null,                                       //检验时间
                                ToWarehousePerson = null,                               //入库人
                                ToWarehouseTime = null,                                 //入库时间
                                ShipmentPerson = null,                                  //出货人
                                ShipmentTime = null,                                    //出货时间
                                Creator = p.Operator,                                   //创建人
                                CreateTime = now,                                       //创建时间                        
                                Editor = p.Operator,                                    //编辑人             
                                EditTime = now,                                         //编辑时间 
                                SupplierCode = strSupplierCodeForLot,                   //电池片供应商编号
                                Color = lot.Color,                                      //花色
                                Grade = lot.Grade,                                      //等级
                                LineCode = p.LineCode,                                  //线别
                                PowerSubCode = lstLotIVTestData[0].PowersetSubCode,     //子分挡代码
                                PowerName = lstWorkOrderPowerset[0].PowerName,          //分档名称
                                Location = locationName                                 //车间
                            };
                        }
                        isNewPackage = true;                                            //托为新建标志
                        #endregion

                        #endregion
                    }
                    #endregion

                    //判断托状态
                    if (iFullpackageQty > package.Quantity)
                    {
                        package.PackageState = EnumPackageState.Packaging;
                    }
                    else
                    {
                        package.PackageState = EnumPackageState.Packaged;
                    }

                    if (oemData != null)
                    {
                        oemData.PackageNo = package.Key;
                        oemData.Status = EnumOemStatus.Packaged;
                        oemData.Editor = p.Operator;
                        oemData.EditTime = now;
                    }
                    else
                    {
                        lot.PackageNo = package.Key;
                        lot.PackageFlag = true;
                    }
                    #endregion

                    #region 5.创建托包装明细对象
                    if (oemData != null || oemWorkOrder != null)
                    {
                        packageDetail = new PackageDetail()
                        {
                            Key = new PackageDetailKey()
                            {
                                PackageNo = package.Key,                //托号
                                ObjectNumber = oemData.Key,             //批次代码
                                ObjectType = EnumPackageObjectType.Lot  //Lot=0 批次 Packet=1 小包
                            },
                            ItemNo = Convert.ToInt32(package.Quantity), //入托项目号（入托顺序）
                            Creator = p.Operator,                       //创建人
                            CreateTime = now,                           //创建时间                   
                            MaterialCode = oemWorkOrder.MaterialCode,   //物料编码
                            OrderNumber = oemWorkOrder.Key              //工单代码
                        };
                    }
                    else
                    {
                        packageDetail = new PackageDetail()
                        {
                            Key = new PackageDetailKey()
                            {
                                PackageNo = package.Key,                //托号
                                ObjectNumber = lot.Key,                 //批次代码
                                ObjectType = EnumPackageObjectType.Lot  //Lot=0 批次 Packet=1 小包
                            },
                            ItemNo = Convert.ToInt32(package.Quantity), //入托项目号（入托顺序）
                            Creator = p.Operator,                       //创建人
                            CreateTime = now,                           //创建时间                   
                            MaterialCode = lot.MaterialCode,            //物料编码
                            OrderNumber = lot.OrderNumber               //工单代码
                        };
                    }

                    #endregion

                    #region 6.创建操作事物
                    //生成操作事务主键
                    transactionKey = Guid.NewGuid().ToString();

                    if (oemData == null)
                    {
                        #region 批次事物对象
                        //批次事物对象LotTransaction（表WIP_TRANSACTION）
                        lotTrans = new LotTransaction()
                        {
                            Key = transactionKey,                   //事物主键
                            Activity = EnumLotActivity.Package,     //批次操作类型（包装）                    
                            InQuantity = lot.Quantity,              //操作数量
                            LotNumber = lot.Key,                    //批次代码                    
                            OrderNumber = lot.OrderNumber,          //工单号
                            OutQuantity = lot.Quantity,             //操作后数量
                            LocationName = lot.LocationName,        //生产车间
                            LineCode = p.LineCode,                  //生产线
                            RouteEnterpriseName = lot.RouteEnterpriseName,  //工艺流程组
                            RouteName = lot.RouteName,              //工艺流程
                            RouteStepName = lot.RouteStepName,      //工步
                            ShiftName = "",                         //班别
                            UndoFlag = false,                       //撤销标识
                            UndoTransactionKey = null,              //撤销记录主键
                            Description = "",                       //备注
                            OperateComputer = p.OperateComputer,    //操作客户端
                            Creator = p.Operator,                   //创建人
                            CreateTime = now,                       //创建时间                  
                            Editor = p.Operator,                    //编辑人
                            EditTime = now                          //编辑时间
                        };

                        //新增批次事物历史记录TransactionHistory（表WIP_TRANSACTION_LOT）  
                        lotTransHistory = new LotTransactionHistory(transactionKey, lotCurr);

                        lot.EquipmentCode = p.EquipmentCode;
                        #endregion

                        #region 创建设备加工事物对象
                        //设备         
                        if (!string.IsNullOrEmpty(lot.EquipmentCode))
                        {
                            //更新批次设备加工历史数据
                            cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format("LotNumber='{0}' AND EquipmentCode='{1}' AND State=0",
                                                        lot.Key,
                                                        lot.EquipmentCode),
                                OrderBy = " EditTime desc"
                            };

                            IList<LotTransactionEquipment> lstLotTransactionEquipment = this.LotTransactionEquipmentDataEngine.Get(cfg);

                            if (lstLotTransactionEquipment != null && lstLotTransactionEquipment.Count > 0)
                            {
                                //取得设备加工历史对象
                                lotTransactionEquipment = lstLotTransactionEquipment.FirstOrDefault();

                                lotTransactionEquipment.EndTransactionKey = transactionKey;             //加工结束历史事物主键
                                lotTransactionEquipment.Editor = p.EquipmentCode;                       //编辑人（设备代码）
                                lotTransactionEquipment.EditTime = now;                                 //编辑时间
                                lotTransactionEquipment.State = EnumLotTransactionEquipmentState.End;   //事物状态（1 - 结束）                        
                            }
                            else
                            {
                                lotTransactionEquipment = new LotTransactionEquipment
                                {
                                    Key = transactionKey,                           //加工开始历史事物主键
                                    EndTransactionKey = transactionKey,             //加工结束历史事物主键
                                    EquipmentCode = p.EquipmentCode,                //设备代码
                                    LotNumber = lot.Key,                            //加工批次
                                    Quantity = 1,                                   //加工数量（默认1后期优化）
                                    StartTime = now,                                //加工开始时间
                                    EndTime = now,                                  //加工结束时间
                                    Creator = p.EquipmentCode,                      //创建人（设备代码）
                                    CreateTime = now,                               //创建时间
                                    Editor = p.EquipmentCode,                       //编辑人（设备代码）
                                    EditTime = now,                                 //编辑时间
                                    State = EnumLotTransactionEquipmentState.End    //事物状态（1 - 结束）
                                };

                                //isNewEquipmentTran = true;                          //设备事物对象状态-NEW
                            }
                        }
                        #endregion
                    }
                    #endregion

                    #region 7.开始事物处理
                    session = this.LotDataEngine.SessionFactory.OpenSession();
                    transaction = session.BeginTransaction();

                    if (oemData == null)
                    {
                        #region 1.更新批次基本信息
                        this.LotDataEngine.Update(lot, session);

                        //更新批次事物LotTransaction（表WIP_TRANSACTION）信息
                        this.LotTransactionDataEngine.Insert(lotTrans, session);

                        //更新批次历史事物TransactionHistory（表WIP_TRANSACTION_LOT）信息
                        this.LotTransactionHistoryDataEngine.Insert(lotTransHistory, session);

                        #endregion
                    }
                    else
                    {
                        #region 1.更新OEM_DATA表中批次的包装号/状态/编辑人/编辑时间
                        this.OemDataEngine.Update(oemData, session);
                        #endregion
                    }

                    #region 2.包装数据
                    if (isNewPackage)        //新托号
                    {
                        this.PackageDataEngine.Insert(package, session);
                    }
                    else
                    {
                        this.PackageDataEngine.Update(package, session);
                    }

                    //2.1.包装明细数据
                    this.PackageDetailDataEngine.Insert(packageDetail, session);

                    //2.2.包装事物
                    //this.LotTransactionPackageDataEngine.Insert(transPackageObj, session);
                    #endregion

                    transaction.Commit();
                    session.Close();
                    #endregion

                    //返回包装信息
                    result.ObjectNo = package.Key;
                    result.Detail = package.PackageState.GetHashCode().ToString();
                }
                catch (Exception ex)
                {
                    //事务回滚
                    transaction.Rollback();
                    session.Close();
                    result.Code = 1000;
                    result.Message = string.Format(StringResource.Error, ex.Message);
                    result.Detail = ex.ToString();
                }

                #endregion
            }
            return result;
        }

        // 批次完成包装操作---0：成功，其他失败
        MethodReturnResult ILotPackageContract.FinishPackage(PackageParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();            
            ISession session = null;
            ITransaction transaction = null;
            try
            {
                string strPackageNo = "";
                DateTime now = DateTime.Now;                        //当前时间
                Package package = null;
                MethodReturnResult<Package> haveAttrPackage = null;
                MethodReturnResult<string> chestNo = null;

                if (p == null)
                {
                    result.Code = 2001;
                    result.Message = StringResource.ParameterIsNull;
                    return result;
                }

                #region 1.检查工序是否是包装工序
                bool isPackageOperation = false;
                RouteOperationAttribute roAttr = this.RouteOperationAttributeDataEngine.Get(new RouteOperationAttributeKey()
                {
                    RouteOperationName = p.RouteOperationName,
                    AttributeName = "IsPackageOperation"
                });

                //如果没有设置为包装工序，则直接返回。
                if (roAttr == null
                    || !bool.TryParse(roAttr.Value, out isPackageOperation)
                    || isPackageOperation == false)
                {
                    result.Code = 1009;
                    result.Message = string.Format("{0} 非包装工序，请确认。"
                                                    , p.RouteOperationName);
                    return result;
                }
                #endregion

                #region 2.更新托信息

                #region 2.1.取得托信息
                //取得托信息
                package = this.PackageDataEngine.Get(p.PackageNo);

                //判断包装号是否存在。
                if (package == null)
                {
                    result.Code = 2005;
                    result.Message = string.Format("托：[{0}]不存在！", strPackageNo);
                    return result;
                }

                //包装是否包装状态
                if (package.PackageState != EnumPackageState.Packaging)
                {
                    result.Code = 2006;
                    result.Message = string.Format("托：[{0}]状态[{1}]非包装中状态！", strPackageNo, package.PackageState.GetDisplayName());
                    return result;
                }
                #endregion
                
                #region 2.2.更新托属性
                package.PackageState = EnumPackageState.Packaged;    //包装完成 
                package.IsLastPackage = p.IsLastestPackage;          //界面勾选尾包   
                package.Editor = p.Operator;                         //编辑人
                package.EditTime = now;                              //编辑日期 
                #endregion

                #endregion

                #region 3.开始事物处理
                session = this.LotDataEngine.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                #region 包装数据

                this.PackageDataEngine.Update(package, session);

                #endregion

                transaction.Commit();
                session.Close();
                #endregion

                #region 4.自动成柜
                //ISessionFactory SessionFactory = this.PackageDataEngine.SessionFactory;
                //PackageInChestService packageInChestService = new PackageInChestService(SessionFactory);
                using (PackageInChestCommom packageInChestService = new PackageInChestCommom())
                {
                    //获取带属性的托号
                    haveAttrPackage = new MethodReturnResult<Package>();
                    haveAttrPackage = packageInChestService.GetAttrOfPackage(package);
                    MaterialChestParameter mcp = null;
                    mcp = this.MaterialChestParameterDataEngine.Get(haveAttrPackage.Data.MaterialCode);
                    if (mcp != null && mcp.IsPackagedChest)
                    {
                        //获取最佳柜号
                        chestNo = new MethodReturnResult<string>();
                        chestNo = packageInChestService.GetChestNo(package.Key, "", false, false);
                        if (chestNo.Code > 0)
                        {
                            result.Code = chestNo.Code;
                            result.Message = chestNo.Message;
                            return result;
                        }
                        //获取最佳柜号的信息明细
                        MethodReturnResult<Chest> mChest = packageInChestService.Get(chestNo.Data);
                        Chest chest = null;
                        if (mChest.Code > 0 && mChest.Data != null)
                        {
                            chest = mChest.Data;
                        }

                        //获取满柜数量
                        int chestFullQty = 0;
                        chestFullQty = mcp.FullChestQty;
                        if (chestFullQty == 0)
                        {
                            result.Code = 2006;
                            result.Message = string.Format("托号内产品编码【{0}】设置的满柜数量为0，请联系成柜规则设定人员修改。", haveAttrPackage.Data.MaterialCode);
                            return result;
                        }

                        //获取当前数量
                        int currentQty = 0;
                        if (chest != null)
                        {
                            currentQty = Convert.ToInt32(chest.Quantity) + 1;
                        }
                        //生成成柜参数
                        ChestParameter chestParameter = new ChestParameter()
                        {
                            Editor = p.Operator,
                            ChestNo = chestNo.Data,
                            IsLastestPackageInChest = false,
                            ChestFullQty = chestFullQty,
                            StoreLocation = "",
                            PackageNo = haveAttrPackage.Data.Key,
                            isManual = false,
                            ModelType = 0
                        };
                        if (currentQty == chestFullQty)
                        {
                            chestParameter.IsFinishPackageInChest = true;
                        }
                        //成柜
                        result = packageInChestService.Chest(chestParameter);
                    }                   
                }                
                #endregion

                if (result.Code == 0)
                {
                    //返回包装信息
                    result.ObjectNo = package.Key;
                    result.Detail = package.PackageState.GetHashCode().ToString();
                    if (haveAttrPackage != null || chestNo != null)
                    {
                        result.Message = string.Format("托号【{0}】自动入柜到【{1}】", haveAttrPackage.Data.Key, chestNo.Data);
                    }                    
                }               
            }
            catch (Exception ex)
            {
                //事务回滚
                transaction.Rollback();
                session.Close();
                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        // 包装结束后，包整体出站
        MethodReturnResult ILotPackageContract.TrackOutPackage(PackageParameter p)
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
                result = this.ExecuteTrackOutPackage(p);
                if (result.Code > 0)
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        // 拆包操作
        MethodReturnResult ILotPackageContract.UnPackage(PackageParameter p)
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
                result = this.ExecuteUnPackage(p);
                if (result.Code > 0)
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        // 操作前检查。
        MethodReturnResult ILotPackageCheck.Check(PackageParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (string.IsNullOrEmpty(p.LineCode))
            {
                result.Code = 1001;
                result.Message = string.Format("{0} {1}"
                                                , "线别代码"
                                                , StringResource.ParameterIsNull);
                return result;
            }

            //if (string.IsNullOrEmpty(p.EquipmentCode))
            //{
            //    result.Code = 1001;
            //    result.Message = string.Format("{0} {1}"
            //                                    , "设备代码"
            //                                    , StringResource.ParameterIsNull);
            //    return result;
            //}

            if (string.IsNullOrEmpty(p.RouteOperationName))
            {
                result.Code = 1001;
                result.Message = string.Format("{0} {1}"
                                                , "工序名称"
                                                , StringResource.ParameterIsNull);
                return result;
            }

            if (p.IsFinishPackage == false
                && (p.LotNumbers == null || p.LotNumbers.Count == 0))
            {
                result.Code = 1001;
                result.Message = string.Format("{0} {1}"
                                                , "批次号"
                                                , StringResource.ParameterIsNull);
                return result;
            }

            if (string.IsNullOrEmpty(p.PackageNo))
            {
                result.Code = 1001;
                result.Message = string.Format("{0} {1}"
                                                , "包装号"
                                                , StringResource.ParameterIsNull);
                return result;
            }
            //检查工序是否是包装工序。
            bool isPackageOperation = false;
            RouteOperationAttribute roAttr = this.RouteOperationAttributeDataEngine.Get(new RouteOperationAttributeKey()
            {
                RouteOperationName = p.RouteOperationName,
                AttributeName = "IsPackageOperation"
            });

            //如果没有设置为包装工序，则直接返回。
            if (roAttr == null
                || !bool.TryParse(roAttr.Value, out isPackageOperation)
                || isPackageOperation == false)
            {
                result.Code = 1009;
                result.Message = string.Format("{0} 非包装工序，请确认。"
                                                , p.RouteOperationName);
                return result;
            }
            //获取最近一次批次BOM信息，跟当前的LOT进行匹配。若当前

            //获取线别车间。
            string locationName = string.Empty;
            ProductionLine pl = this.ProductionLineDataEngine.Get(p.LineCode);
            if (pl != null)
            {
                //根据线别所在区域，获取车间名称。
                Location loc = this.LocationDataEngine.Get(pl.LocationName);
                locationName = loc.ParentLocationName ?? string.Empty;
            }

            #region //判断是否是自动包装线的包装号
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format(@"Key.PackageNo='{0}' and BinPackaged='0' "
                                        , p.PackageNo)
            };

            IList<PackageBin> lstPackageBin = PackageBinDataEngine.Get(cfg);
            if (lstPackageBin != null && lstPackageBin.Count > 0)
            {
                result.Code = 1003;
                result.Message = string.Format("包装号（{0}）为自动包装未满托，请用手动入Bin功能进行包装。", p.PackageNo);
                return result;
            }
            lstPackageBin = null;
            #endregion

            //循环批次。
            foreach (string lotNumber in p.LotNumbers)
            {
                Lot lot = this.LotDataEngine.Get(lotNumber);
                //判定是否存在批次记录。
                if (lot == null || lot.Status == EnumObjectStatus.Disabled)
                {
                    result.Code = 1002;
                    result.Message = string.Format("批次（{0}）不存在。", lotNumber);
                    return result;
                }
                //批次需要已进站 
                if (lot.StateFlag == EnumLotState.WaitTrackIn)
                {
                    result.Code = 1003;
                    result.Message = string.Format("批次（{0}）还未进工序（{1}），请先做进站作业。", lotNumber, lot.RouteStepName);
                    return result;
                }

                //批次已完成。
                if (lot.StateFlag != EnumLotState.Finished && lot.StateFlag != EnumLotState.WaitTrackOut)
                {
                    result.Code = 1003;
                    result.Message = string.Format("批次（{0}）已完成。", lotNumber);
                    return result;
                }

                //批次已结束
                if (lot.DeletedFlag == true)
                {
                    result.Code = 1004;
                    result.Message = string.Format("批次（{0}）已结束。", lotNumber);
                    return result;
                }
                //批次已暂停
                if (lot.HoldFlag == true)
                {
                    result.Code = 1005;
                    result.Message = string.Format("批次（{0}）已暂停。", lotNumber);
                    return result;
                }
                //批次已包装。
                if (lot.PackageFlag == true)
                {
                    result.Code = 1006;
                    result.Message = string.Format("批次（{0}）已包装。", lotNumber);
                    return result;
                }
                //检查批次所在工序是否处于指定工序
                if (lot.RouteStepName != p.RouteOperationName)
                {
                    result.Code = 1007;
                    result.Message = string.Format("批次（{0}）目前在（{1}）工序，不能在({2})操作。"
                                                    , lotNumber
                                                    , lot.RouteStepName
                                                    , p.RouteOperationName);
                    return result;
                }
                //检查批次车间和线别车间是否匹配。
                if (lot.LocationName != locationName)
                {
                    result.Code = 1008;
                    result.Message = string.Format("批次（{0}）属于({1})车间，不能在({2})车间线别上操作。"
                                                    , lotNumber
                                                    , lot.LocationName
                                                    , locationName);
                    return result;
                }
            }
            return result;
        }

        // 执行操作。
        MethodReturnResult ILotPackage.Execute(PackageParameter p)
        {
            DateTime now = DateTime.Now;
            string strNewPackageNo = "";
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            p.TransactionKeys = new Dictionary<string, string>();
            strNewPackageNo = p.PackageNo;

            #region define List of DataEngine
            lstLotDataEngineForUpdate = new List<Lot>();
            lstLotTransactionForInsert = new List<LotTransaction>();
            lstLotTransactionHistoryForInsert = new List<LotTransactionHistory>();
            lstLotTransactionParameterDataEngineForInsert = new List<LotTransactionParameter>();
            lstLotTransactionStepDataEngineForInsert = new List<LotTransactionStep>();

            //LotTransactionEquipment ,Equipment ,EquipmentStateEvent
            lstLotTransactionEquipmentForUpdate = new List<LotTransactionEquipment>();
            lstLotTransactionEquipmentForInsert = new List<LotTransactionEquipment>();

            lstEquipmentForUpdate = new List<Equipment>();
            lstEquipmentStateEventForInsert = new List<EquipmentStateEvent>();

            //Package
            lstPackageDataForUpdate = new List<Package>();
            lstPackageDataForInsert = new List<Package>();
            lstPackageDetailForInsert = new List<PackageDetail>();
            lstLotTransactionPackageForInsert = new List<LotTransactionPackage>();
            #endregion


            bool isPackageLimitedForPWorkOrder = false;

            Package packageObj = this.PackageDataEngine.Get(p.PackageNo);
            Package packageUpdate = null;
            //更新包装数据。
            if (packageObj != null)
            {
                packageUpdate = packageObj.Clone() as Package;
                packageUpdate.Editor = p.Creator;
                packageUpdate.EditTime = now;
                packageUpdate.IsLastPackage = p.IsLastestPackage;
                if (p.IsFinishPackage)
                {
                    packageUpdate.PackageState = EnumPackageState.Packaged;
                }

                #region //获取批次工单的包装属性

                WorkOrderAttribute workOrderAttribute = this.WorkOrderAttributeDataEngine.Get(new WorkOrderAttributeKey()
                {
                    OrderNumber = packageUpdate.OrderNumber,
                    AttributeName = "PackageLimited"
                });

                //如果没有设置为包装工序，则直接返回。
                if (workOrderAttribute == null
                    || !bool.TryParse(workOrderAttribute.AttributeValue, out isPackageLimitedForPWorkOrder)
                   )
                {
                    isPackageLimitedForPWorkOrder = false;
                }
                #endregion
            }

            PagingConfig cfg = null;
            string strSupplierCodeForLot = "";
            //string strSupplierCodeForPackage = "";
            Lot lotForCreatePackageNo = null;
            #region //循环批次
            foreach (string lotNumber in p.LotNumbers)
            {
                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.ObjectNumber='{0}'", lotNumber),
                    OrderBy = ""
                };
                IList<PackageDetail> lstObj = this.PackageDetailDataEngine.Get(cfg);
                if (lstObj != null && lstObj.Count > 0)
                {
                    result.Code = 2001;
                    result.Message = string.Format("批次（{0}）已存在在托号（{1}）中!"
                                                    , lotNumber
                                                    , lstObj[0].Key.PackageNo);
                    return result;
                }
                lstObj = null;

                Lot lot = this.LotDataEngine.Get(lotNumber);

                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.LotNumber='{0}' and SupplierCode in ('{1}','{2}') and MaterialName like '%{3}%'", lotNumber, "010022", "010035", "电池片"),
                    OrderBy = ""
                };

                IList<LotBOM> lstLotBom = this.LotBOMDataEngine.Get(cfg);
                if (lstLotBom != null && lstLotBom.Count > 0)
                {
                    LotBOM lotBom = lstLotBom.FirstOrDefault();
                    strSupplierCodeForLot = lotBom.SupplierCode;
                }
                //生成操作事务主键。
                string transactionKey = Guid.NewGuid().ToString();
                p.TransactionKeys.Add(lotNumber, transactionKey);

                //更新批次记录。
                Lot lotUpdate = lot.Clone() as Lot;
                lotUpdate.PackageFlag = true;
                lotUpdate.PackageNo = p.PackageNo;
                lotUpdate.OperateComputer = p.OperateComputer;
                lotUpdate.Editor = p.Creator;
                lotUpdate.EditTime = now;

                #region //记录包装数据。
                if (packageObj == null)
                {
                    lotForCreatePackageNo = lot;
                    packageObj = new Package()
                    {
                        CreateTime = now,
                        Creator = p.Creator,
                        Checker = null,
                        CheckTime = null,
                        ContainerNo = null,
                        Description = p.Remark,
                        Editor = p.Creator,
                        EditTime = now,
                        IsLastPackage = p.IsLastestPackage,
                        Key = p.PackageNo,
                        MaterialCode = lot.MaterialCode,
                        OrderNumber = lot.OrderNumber,
                        PackageState = p.IsFinishPackage ? EnumPackageState.Packaged : EnumPackageState.Packaging,
                        PackageType = EnumPackageType.Packet,
                        Quantity = lot.Quantity,
                        ShipmentPerson = null,
                        ShipmentTime = null,
                        ToWarehousePerson = null,
                        ToWarehouseTime = null,
                        SupplierCode = strSupplierCodeForLot,
                        PackageMixedType = EnumPackageMixedType.UnMixedType
                    };
                    lstPackageDataForInsert.Add(packageObj);
                    //this.PackageDataEngine.Insert(packageObj);
                }
                else
                {
                    //更新包装数据。
                    if (packageUpdate == null)
                    {
                        packageUpdate = packageObj.Clone() as Package;
                    }
                    if (packageUpdate.Quantity == 0)
                    {
                        packageUpdate.OrderNumber = lot.OrderNumber;
                        packageUpdate.MaterialCode = lot.MaterialCode;
                    }

                    //判断工单是否可以混托
                    if (isPackageLimitedForPWorkOrder)
                    {
                        if (string.Compare(packageUpdate.OrderNumber, lot.OrderNumber, true) != 0)
                        {
                            result.Code = 1012;
                            result.Message = string.Format("托{0}所属工单号（{1}）不能混装别的工单号{2}中."
                                                            , packageUpdate.Key
                                                            , packageUpdate.OrderNumber
                                                            , lot.OrderNumber);
                            return result;
                        }
                    }


                    #region //获取批次工单的包装属性
                    bool isPackageLimited = false;
                    if (string.Compare(lot.OrderNumber, packageUpdate.OrderNumber, true) != 0)
                    {
                        //包装号里的工单若跟批次的工单相同，则不需要判断属性
                        WorkOrderAttribute workOrderAttribute = this.WorkOrderAttributeDataEngine.Get(new WorkOrderAttributeKey()
                        {
                            OrderNumber = lot.OrderNumber,
                            AttributeName = "PackageLimited"
                        });

                        //如果没有设置为包装工序，则直接返回。
                        if (workOrderAttribute == null
                            || !bool.TryParse(workOrderAttribute.AttributeValue, out isPackageLimited)
                           )
                        {
                            isPackageLimited = false;
                        }
                    }
                    #endregion

                    //检查批次是否不能混工单
                    if (isPackageLimited)
                    {
                        if (string.Compare(packageUpdate.OrderNumber, lot.OrderNumber, true) != 0)
                        {
                            result.Code = 1012;
                            result.Message = string.Format("批次{0}所属工单号（{1}）不能混装到此托的工单号{2}中."
                                                            , lotNumber
                                                            , lot.OrderNumber
                                                            , packageUpdate.OrderNumber
                                                            );
                            return result;
                        }
                    }

                    #region 判断要包装批次的工单是否设置混工单组
                    cfg = new PagingConfig()
                    {
                        Where = string.Format(@"Key.OrderNumber = '{0}'", lot.OrderNumber)
                    };
                    IList<WorkOrderGroupDetail> lstLotWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                    //批次所在工单设置了混工单组
                    if (lstLotWorkOrderGroupDetail != null && lstLotWorkOrderGroupDetail.Count > 0)
                    {
                        #region 判断要托工单是否设置混工单组
                        cfg = new PagingConfig()
                        {
                            Where = string.Format(@"Key.OrderNumber = '{0}'", packageUpdate.OrderNumber)
                        };
                        IList<WorkOrderGroupDetail> lstPackageWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                        //托工单设置了混工单组
                        if (lstPackageWorkOrderGroupDetail != null && lstPackageWorkOrderGroupDetail.Count > 0)
                        {
                            if (lstLotWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString() != lstPackageWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString())
                            {
                                result.Code = 0;
                                result.Message = string.Format("托：（{0}）所在工单（{1} 设置的混工单组（{2}）与入托批次（{3}）所在工单（{4}）设置的混工单组（{5}）不一致！",
                                                                packageUpdate.Key, packageUpdate.OrderNumber, lstPackageWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString(),
                                                                lot.Key, lot.OrderNumber, lstLotWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString());
                                return result;
                            }
                        }
                        //托工单没设混工单组
                        else
                        {
                            result.Code = 0;
                            result.Message = string.Format("托：（{0}）所在工单（{1} 未设置混工单组规则，但要入托批次（{2}）所在工单（{3}）设置了混工单组！",
                                                            packageUpdate.Key, packageUpdate.OrderNumber, lot.Key, lot.OrderNumber);
                            return result;
                        }
                        #endregion
                    }
                    //批次所在工单没设混工单组
                    else
                    {
                        #region 判断要托工单是否设置混工单组
                        cfg = new PagingConfig()
                        {
                            Where = string.Format(@"Key.OrderNumber = '{0}'", packageUpdate.OrderNumber)
                        };
                        IList<WorkOrderGroupDetail> lstPackageWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                        //托工单设置了混工单组
                        if (lstPackageWorkOrderGroupDetail != null && lstPackageWorkOrderGroupDetail.Count > 0)
                        {
                            result.Code = 0;
                            result.Message = string.Format("托：（{0}）所在工单（{1} 已设置混工单组规则，但要入托批次（{2}）所在工单（{3}）未设置混工单组！",
                                                            packageUpdate.Key, packageUpdate.OrderNumber, lot.Key, lot.OrderNumber);
                            return result;
                        }
                        #endregion
                    }
                    #endregion

                    if (String.IsNullOrEmpty(packageUpdate.SupplierCode) == true || packageUpdate.SupplierCode == "")
                    {
                        packageUpdate.SupplierCode = strSupplierCodeForLot;
                    }
                    else
                    {
                        bool blChkSupplierCode = false;
                        //检查 packageNO
                        if (String.IsNullOrEmpty(strSupplierCodeForLot) == false && strSupplierCodeForLot != "")
                        {
                            //BaseAttributeValueDataEngine

                            cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format(@"Key.CategoryName='{0}'
                                           AND Key.AttributeName='{1}'"
                                                        , "SystemParameters"
                                                        , "PackageChkMaterialSupplier"),
                                OrderBy = "Key.ItemOrder"
                            };
                            IList<BaseAttributeValue> lstBaseAttributeValues = BaseAttributeValueDataEngine.Get(cfg);
                            if (lstBaseAttributeValues != null && lstBaseAttributeValues.Count > 0)
                            {
                                BaseAttributeValue obj = lstBaseAttributeValues.FirstOrDefault();
                                if (String.IsNullOrEmpty(obj.Value) == false)
                                {
                                    Boolean.TryParse(obj.Value, out blChkSupplierCode);
                                }
                            }
                            if (blChkSupplierCode)
                            {
                                //系统判断是否需要进行供应商的检测
                                if (string.Compare(packageUpdate.SupplierCode, strSupplierCodeForLot, true) != 0)
                                {
                                    result.Code = 1010;
                                    result.Message = string.Format("此托已存在电池片供应商编号为（{2}）。批次（{0}）所用的电池片供应商编号为（{1}），不能混装入托。"
                                                                    , lotNumber
                                                                    , strSupplierCodeForLot
                                                                    , packageUpdate.SupplierCode);
                                    return result;
                                }
                            }
                        }
                    }

                    packageUpdate.Quantity += lot.Quantity;
                }
                //记录包装明细数据。
                int itemNo = 1;
                cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.PackageNo='{0}'", p.PackageNo),
                    OrderBy = "ItemNo Desc"
                };

                IList<PackageDetail> lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);
                if (lstPackageDetail != null && lstPackageDetail.Count > 0)
                {
                    itemNo = lstPackageDetail[0].ItemNo + 1;
                }

                PackageDetail packageDetail = new PackageDetail()
                {
                    Key = new PackageDetailKey()
                    {
                        PackageNo = p.PackageNo,
                        ObjectNumber = lot.Key,
                        ObjectType = EnumPackageObjectType.Lot
                    },
                    ItemNo = itemNo,
                    CreateTime = now,
                    Creator = p.Creator,
                    MaterialCode = lot.MaterialCode,
                    OrderNumber = lot.OrderNumber
                };
                //this.PackageDetailDataEngine.Insert(packageDetail);
                lstPackageDetailForInsert.Add(packageDetail);
                #endregion

                LotTransaction transObj = null;
                LotTransactionHistory lotHistory = null;

                if (lotUpdate.StateFlag == EnumLotState.WaitTrackIn)
                {
                    #region //LOT FOR TrackIn
                    /*
                    lotUpdate.StartProcessTime = now;
                    lotUpdate.EquipmentCode = p.EquipmentCode;
                    lotUpdate.LineCode = p.LineCode;
                    lotUpdate.OperateComputer = p.OperateComputer;
                    lotUpdate.PreLineCode = lot.LineCode;
                    lotUpdate.StateFlag = EnumLotState.WaitTrackOut;


                    //记录批次设备加工历史数据
                    if (!string.IsNullOrEmpty(p.EquipmentCode))
                    {
                        LotTransactionEquipment transEquipment = new LotTransactionEquipment()
                        {
                            Key = Guid.NewGuid().ToString(),
                            EndTransactionKey = null,
                            CreateTime = now,
                            Creator = p.Creator,
                            Editor = p.Creator,
                            EditTime = now,
                            EndTime = null,
                            EquipmentCode = p.EquipmentCode,
                            LotNumber = lotNumber,
                            Quantity = lot.Quantity,
                            StartTime = now,
                            State = EnumLotTransactionEquipmentState.Start
                        };
                        lstLotTransactionEquipmentForInsert.Add(transEquipment);
                        //this.LotTransactionEquipmentDataEngine.Insert(transEquipment);
                    }

                    #region //更新设备状态
                    bool blEquipmentContinue = false;
                    if (string.IsNullOrEmpty(p.EquipmentCode))
                    {
                        blEquipmentContinue = false;
                    }
                    Equipment e = null;
                    if (blEquipmentContinue)
                    {
                        //获取设备数据。
                        e = this.EquipmentDataEngine.Get(p.EquipmentCode);
                        if (e == null)
                        {
                            blEquipmentContinue = false;
                        }
                    }
                    EquipmentState es = null;
                    if (blEquipmentContinue)
                    {
                        //获取设备当前状态。
                        es = this.EquipmentStateDataEngine.Get(e.StateName);
                        if (es == null)
                        {
                            blEquipmentContinue = false;
                        }
                    }
                    #region //blEquipmentContinue =true
                    EquipmentState runState = null;
                    if (blEquipmentContinue)
                    {
                        //获取设备RUN的主键
                        runState = this.EquipmentStateDataEngine.Get("RUN");

                        //获取设备当前状态->RUN的状态切换数据。
                        EquipmentChangeState ecsToRun = this.EquipmentChangeStateDataEngine.Get(es.Key, runState.Key);
                        if (ecsToRun != null)
                        {
                            //更新父设备状态。
                            if (!string.IsNullOrEmpty(e.ParentEquipmentCode))
                            {
                                Equipment ep = this.EquipmentDataEngine.Get(e.ParentEquipmentCode);
                                if (ep != null)
                                {
                                    Equipment epUpdate = ep.Clone() as Equipment;
                                    //更新设备状态。
                                    epUpdate.StateName = runState.Key;
                                    epUpdate.ChangeStateName = ecsToRun.Key;

                                    lstEquipmentForUpdate.Add(epUpdate);
                                    //this.EquipmentDataEngine.Update(epUpdate);

                                    //新增设备状态事件数据
                                    EquipmentStateEvent newStateEvent = new EquipmentStateEvent()
                                    {
                                        Key = Guid.NewGuid().ToString(),
                                        CreateTime = now,
                                        Creator = p.Creator,
                                        Description = p.Remark,
                                        Editor = p.Creator,
                                        EditTime = now,
                                        EquipmentChangeStateName = ecsToRun.Key,
                                        EquipmentCode = e.ParentEquipmentCode,
                                        EquipmentFromStateName = es.Key,
                                        EquipmentToStateName = runState.Key,
                                        IsCurrent = true
                                    };
                                    lstEquipmentStateEventForInsert.Add(newStateEvent);
                                    //this.EquipmentStateEventDataEngine.Insert(newStateEvent);
                                }
                            }

                            //更新设备状态。
                            Equipment eUpdate = e.Clone() as Equipment;
                            eUpdate.StateName = runState.Key;
                            eUpdate.ChangeStateName = ecsToRun.Key;
                            lstEquipmentForUpdate.Add(eUpdate);
                            //this.EquipmentDataEngine.Update(eUpdate);


                            //新增设备状态事件数据
                            EquipmentStateEvent stateEvent = new EquipmentStateEvent()
                            {
                                Key = Guid.NewGuid().ToString(),
                                CreateTime = now,
                                Creator = p.Creator,
                                Description = p.Remark,
                                Editor = p.Creator,
                                EditTime = now,
                                EquipmentChangeStateName = ecsToRun.Key,
                                EquipmentCode = e.Key,
                                EquipmentFromStateName = es.Key,
                                EquipmentToStateName = runState.Key,
                                IsCurrent = true
                            };
                            lstEquipmentStateEventForInsert.Add(stateEvent);
                            // this.EquipmentStateEventDataEngine.Insert(stateEvent);
                        }
                    }
                    #endregion

                    #endregion

                    #region//记录TrackIn操作历史。
                    transactionKey = Guid.NewGuid().ToString();
                    transObj = new LotTransaction()
                    {
                        Key = transactionKey,
                        Activity = EnumLotActivity.TrackIn,
                        CreateTime = now,
                        Creator = p.Creator,
                        Description = p.Remark,
                        Editor = p.Creator,
                        EditTime = now,
                        InQuantity = lot.Quantity,
                        LotNumber = lotNumber,
                        OperateComputer = p.OperateComputer,
                        OrderNumber = lot.OrderNumber,
                        OutQuantity = lotUpdate.Quantity,
                        RouteEnterpriseName = lot.RouteEnterpriseName,
                        RouteName = lot.RouteName,
                        RouteStepName = lot.RouteStepName,
                        ShiftName = p.ShiftName,
                        UndoFlag = false,
                        UndoTransactionKey = null
                    };
                    lstLotTransactionForInsert.Add(transObj);
                    //this.LotTransactionDataEngine.Insert(transObj);
                    //新增批次历史记录。
                    lotHistory = new LotTransactionHistory(transactionKey, lot);

                    lstLotTransactionHistoryForInsert.Add(lotHistory);
                    //this.LotTransactionHistoryDataEngine.Insert(lotHistory);
                    #endregion
                    */
                    #endregion //End of LOT FOR TrackIn
                }

                //批次处于等待出站状态。
                if (lot.StateFlag == EnumLotState.WaitTrackOut)
                {
                    #region //获取 Lot信息，下一道工序


                    //根据批次当前工艺工步获取下一个工步
                    RouteStepKey rsKey = new RouteStepKey()
                    {
                        RouteName = lot.RouteName,
                        RouteStepName = lot.RouteStepName
                    };
                    RouteStep rsObj = this.RouteStepDataEngine.Get(rsKey);
                    if (rsObj == null)
                    {
                        result.Code = 2001;
                        result.Message = string.Format("批次（{0}）所在工艺流程（{1}）不存在。"
                                                        , lotNumber
                                                        , rsKey);
                        return result;
                    }
                    cfg = new PagingConfig()
                    {
                        PageNo = 0,
                        PageSize = 1,
                        Where = string.Format(@"Key.RouteName='{0}'
                                        AND SortSeq>='{1}'"
                                                , rsObj.Key.RouteName
                                                , rsObj.SortSeq + 1),
                        OrderBy = "SortSeq"
                    };
                    IList<RouteStep> lstRouteStep = this.RouteStepDataEngine.Get(cfg);
                    if (lstRouteStep.Count == 0)
                    {
                        //获取下一个工艺流程。
                        RouteEnterpriseDetail reDetail = this.RouteEnterpriseDetailDataEngine.Get(new RouteEnterpriseDetailKey()
                        {
                            RouteEnterpriseName = lot.RouteEnterpriseName,
                            RouteName = lot.RouteName
                        });


                        if (reDetail != null)
                        {
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key.RouteEnterpriseName='{0}'
                                                AND ItemNo='{1}'"
                                                        , reDetail.Key.RouteEnterpriseName
                                                        , reDetail.ItemNo + 1),
                                OrderBy = "ItemNo"
                            };
                            IList<RouteEnterpriseDetail> lstRouteEnterpriseDetail = this.RouteEnterpriseDetailDataEngine.Get(cfg);
                            if (lstRouteEnterpriseDetail.Count > 0)
                            {
                                //获取下一工艺流程的第一个工艺工步。
                                reDetail = lstRouteEnterpriseDetail[0];
                                cfg = new PagingConfig()
                                {
                                    PageNo = 0,
                                    PageSize = 1,
                                    Where = string.Format(@"Key.RouteName='{0}'"
                                                            , reDetail.Key.RouteName),
                                    OrderBy = "SortSeq"
                                };
                                lstRouteStep = this.RouteStepDataEngine.Get(cfg);
                            }
                        }
                    }
                    bool isFinish = true;
                    string toRouteEnterpriseName = lot.RouteEnterpriseName;
                    string toRouteName = lot.RouteName;
                    string toRouteStepName = lot.RouteStepName;
                    if (lstRouteStep != null && lstRouteStep.Count > 0)
                    {
                        isFinish = false;
                        toRouteName = lstRouteStep[0].Key.RouteName;
                        toRouteStepName = lstRouteStep[0].Key.RouteStepName;
                    }

                    //更新批次记录。
                    if (isFinish)
                    {
                        lotUpdate.StartWaitTime = null;
                    }
                    else
                    {
                        lotUpdate.StartWaitTime = now;
                    }
                    lotUpdate.StartProcessTime = null;
                    lotUpdate.StateFlag = EnumLotState.WaitTrackIn;
                    lotUpdate.RouteEnterpriseName = toRouteEnterpriseName;
                    lotUpdate.RouteName = toRouteName;
                    lotUpdate.RouteStepName = toRouteStepName;
                    lotUpdate.OperateComputer = p.OperateComputer;
                    lotUpdate.PreLineCode = lot.LineCode;
                    lotUpdate.LineCode = p.LineCode;
                    lotUpdate.Editor = p.Creator;
                    lotUpdate.EditTime = now;
                    lotUpdate.EquipmentCode = null;

                    #endregion

                    #region //更新设备信息 , 设备的Event ,设备的Transaction

                    //bool blLogEquipment = true;
                    string equipmentCode = lot.EquipmentCode;
                    //进站时没有选择设备，则设置出站时批次为当前设备。
                    if (string.IsNullOrEmpty(equipmentCode))
                    {
                        equipmentCode = p.EquipmentCode;
                    }

                    //进站时已经确定设备。              
                    if (!string.IsNullOrEmpty(lot.EquipmentCode))
                    {
                        //更新批次设备加工历史数据。
                        cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format("LotNumber='{0}' AND EquipmentCode='{1}' AND State=0",
                                                    lotNumber,
                                                    lot.EquipmentCode)
                        };
                        IList<LotTransactionEquipment> lstLotTransactionEquipment = this.LotTransactionEquipmentDataEngine.Get(cfg);
                        foreach (LotTransactionEquipment lotTransactionEquipment in lstLotTransactionEquipment)
                        {
                            LotTransactionEquipment itemUpdate = lotTransactionEquipment.Clone() as LotTransactionEquipment;
                            itemUpdate.EndTransactionKey = transactionKey;
                            itemUpdate.EditTime = now;
                            itemUpdate.Editor = p.Creator;
                            itemUpdate.EndTime = now;
                            itemUpdate.State = EnumLotTransactionEquipmentState.End;
                            lstLotTransactionEquipmentForUpdate.Add(itemUpdate);
                            //this.LotTransactionEquipmentDataEngine.Update(itemUpdate, session);
                        }
                    }
                    else
                    {//记录批次设备加工历史数据
                        LotTransactionEquipment transEquipment = new LotTransactionEquipment()
                        {
                            Key = transactionKey,
                            EndTransactionKey = transactionKey,
                            CreateTime = now,
                            Creator = p.Creator,
                            Editor = p.Creator,
                            EditTime = now,
                            EndTime = now,
                            EquipmentCode = p.EquipmentCode,
                            LotNumber = lotNumber,
                            Quantity = lot.Quantity,
                            StartTime = lot.StartProcessTime,
                            State = EnumLotTransactionEquipmentState.End
                        };
                        lstLotTransactionEquipmentForInsert.Add(transEquipment);
                        //this.LotTransactionEquipmentDataEngine.Insert(transEquipment, session);
                    }

                    #region 设备状态信息更新
                    /*
                        //如果出站也没有选择设备，直接返回。                       
                        if (string.IsNullOrEmpty(equipmentCode) == false)
                        {
                            //获取设备数据
                            Equipment e = this.EquipmentDataEngine.Get(equipmentCode ?? string.Empty);
                            if (e != null)
                            {
                                //获取设备当前状态。
                                EquipmentState es = this.EquipmentStateDataEngine.Get(e.StateName ?? string.Empty);
                                if (es != null)
                                {
                                    //获取设备LOST的主键
                                    EquipmentState lostState = this.EquipmentStateDataEngine.Get("LOST");
                                    //获取设备当前状态->LOST的状态切换数据。
                                    EquipmentChangeState ecsToLost = this.EquipmentChangeStateDataEngine.Get(es.Key, lostState.Key);

                                    if (ecsToLost != null)
                                    {
                                        //根据设备编码获取当前加工批次数据。
                                        cfg = new PagingConfig()
                                        {
                                            PageSize = 1,
                                            PageNo = 0,
                                            Where = string.Format("EquipmentCode='{0}' AND STATE='{1}'"
                                                                    , equipmentCode
                                                                    , Convert.ToInt32(EnumLotTransactionEquipmentState.Start))
                                        };
                                        IList<LotTransactionEquipment> lst = this.LotTransactionEquipmentDataEngine.Get(cfg);
                                        if (lst.Count == 0)//设备当前加工批次>0，则直接返回。
                                        {
                                            #region Change EqupmentState
                                            //更新父设备状态。
                                            if (!string.IsNullOrEmpty(e.ParentEquipmentCode))
                                            {
                                                Equipment ep = this.EquipmentDataEngine.Get(e.ParentEquipmentCode);
                                                if (ep != null)
                                                {
                                                    Equipment epUpdate = ep.Clone() as Equipment;
                                                    //更新设备状态。
                                                    epUpdate.StateName = lostState.Key;
                                                    epUpdate.ChangeStateName = ecsToLost.Key;
                                                    //this.EquipmentDataEngine.Update(epUpdate);
                                                    lstEquipmentForUpdate.Add(epUpdate);
                                                    //新增设备状态事件数据
                                                    EquipmentStateEvent newStateEvent = new EquipmentStateEvent()
                                                    {
                                                        Key = Guid.NewGuid().ToString(),
                                                        CreateTime = now,
                                                        Creator = p.Creator,
                                                        Description = p.Remark,
                                                        Editor = p.Creator,
                                                        EditTime = now,
                                                        EquipmentChangeStateName = ecsToLost.Key,
                                                        EquipmentCode = e.ParentEquipmentCode,
                                                        EquipmentFromStateName = es.Key,
                                                        EquipmentToStateName = lostState.Key,
                                                        IsCurrent = true
                                                    };
                                                    //this.EquipmentStateEventDataEngine.Insert(newStateEvent);
                                                    lstEquipmentStateEventForInsert.Add(newStateEvent);
                                                }
                                            }
                                            //更新设备状态。
                                            Equipment eUpdate = e.Clone() as Equipment;
                                            eUpdate.StateName = lostState.Key;
                                            eUpdate.ChangeStateName = ecsToLost.Key;
                                            lstEquipmentForUpdate.Add(eUpdate);
                                            //this.EquipmentDataEngine.Update(eUpdate);

                                            //新增设备状态事件数据
                                            EquipmentStateEvent stateEvent = new EquipmentStateEvent()
                                            {
                                                Key = Guid.NewGuid().ToString(),
                                                CreateTime = now,
                                                Creator = p.Creator,
                                                Description = p.Remark,
                                                Editor = p.Creator,
                                                EditTime = now,
                                                EquipmentChangeStateName = ecsToLost.Key,
                                                EquipmentCode = e.Key,
                                                EquipmentFromStateName = es.Key,
                                                EquipmentToStateName = lostState.Key,
                                                IsCurrent = true
                                            };
                                            //this.EquipmentStateEventDataEngine.Insert(stateEvent);
                                            lstEquipmentStateEventForInsert.Add(stateEvent);
                                            #endregion
                                        }
                                    }
                                }
                            }
                        }*/
                    #endregion

                    #endregion

                    #region//记录操作历史。
                    transObj = new LotTransaction()
                    {
                        Key = transactionKey,
                        Activity = EnumLotActivity.TrackOut,
                        CreateTime = now,
                        Creator = p.Creator,
                        Description = p.Remark,
                        Editor = p.Creator,
                        EditTime = now,
                        InQuantity = lot.Quantity,
                        LotNumber = lotNumber,
                        OperateComputer = p.OperateComputer,
                        OrderNumber = lot.OrderNumber,
                        OutQuantity = lotUpdate.Quantity,
                        RouteEnterpriseName = lot.RouteEnterpriseName,
                        RouteName = lot.RouteName,
                        RouteStepName = lot.RouteStepName,
                        ShiftName = p.ShiftName,
                        UndoFlag = false,
                        UndoTransactionKey = null
                    };
                    lstLotTransactionForInsert.Add(transObj);
                    //this.LotTransactionDataEngine.Insert(transObj, session);

                    //新增批次历史记录。
                    lotHistory = new LotTransactionHistory(transactionKey, lot);
                    lstLotTransactionHistoryForInsert.Add(lotHistory);
                    //this.LotTransactionHistoryDataEngine.Insert(lotHistory, session);

                    //新增工艺下一步记录。
                    LotTransactionStep nextStep = new LotTransactionStep()
                    {
                        Key = transactionKey,
                        ToRouteEnterpriseName = toRouteEnterpriseName,
                        ToRouteName = toRouteName,
                        ToRouteStepName = toRouteStepName,
                        Editor = p.Creator,
                        EditTime = now
                    };
                    lstLotTransactionStepDataEngineForInsert.Add(nextStep);
                    //this.LotTransactionStepDataEngine.Insert(nextStep, session);
                    #endregion
                }
                lstLotDataEngineForUpdate.Add(lotUpdate);

                #region//记录Package操作历史。
                transactionKey = Guid.NewGuid().ToString();
                now = System.DateTime.Now;
                transObj = new LotTransaction()
                {
                    Key = transactionKey,
                    Activity = EnumLotActivity.Package,
                    CreateTime = now,
                    Creator = p.Creator,
                    Description = p.Remark,
                    Editor = p.Creator,
                    EditTime = now,
                    InQuantity = lot.Quantity,
                    LotNumber = lotNumber,
                    OperateComputer = p.OperateComputer,
                    OrderNumber = lot.OrderNumber,
                    OutQuantity = lotUpdate.Quantity,
                    RouteEnterpriseName = lot.RouteEnterpriseName,
                    RouteName = lot.RouteName,
                    RouteStepName = lot.RouteStepName,
                    ShiftName = p.ShiftName,
                    UndoFlag = false,
                    UndoTransactionKey = null
                };
                //暂时忽略打包的事物,事物设置为撤销
                lstLotTransactionForInsert.Add(transObj);
                //this.LotTransactionDataEngine.Insert(transObj);
                //新增批次历史记录。
                lotHistory = new LotTransactionHistory(transactionKey, lot);
                lotHistory.StateFlag = EnumLotState.WaitTrackOut;
                lstLotTransactionHistoryForInsert.Add(lotHistory);
                //this.LotTransactionHistoryDataEngine.Insert(lotHistory);

                LotTransactionPackage transPackage = new LotTransactionPackage()
                {
                    Key = transactionKey,
                    PackageNo = p.PackageNo,
                    Editor = p.Creator,
                    EditTime = now
                };
                lstLotTransactionPackageForInsert.Add(transPackage);
                //this.LotTransactionPackageDataEngine.Insert(transPackage);
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
                        lstLotTransactionParameterDataEngineForInsert.Add(lotParamObj);
                        //this.LotTransactionParameterDataEngine.Insert(lotParamObj);
                    }
                }
                #endregion
            }
            #endregion

            if (packageUpdate != null)
            {
                lstPackageDataForUpdate.Add(packageUpdate);
            }


            #region //若已满包，需要进行出站处理
            if (p.IsFinishPackage)
            {



            }
            #endregion

            ISession session = this.LotDataEngine.SessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();
            try
            {

                #region //开始事物处理

                #region //更新Package基本信息
                foreach (Package package in lstPackageDataForUpdate)
                {
                    this.PackageDataEngine.Update(package, session);
                }

                foreach (Package package in lstPackageDataForInsert)
                {
                    bool blContinue = false;
                    if (PackageDataEngine.IsExists(package.Key, session) == true)
                    {
                        blContinue = true;
                        while (blContinue)
                        {
                            strNewPackageNo = GenerateEx(lotForCreatePackageNo.Key, session);
                            blContinue = PackageDataEngine.IsExists(strNewPackageNo, session);
                        }
                        package.Key = strNewPackageNo;
                    }
                    this.PackageDataEngine.Insert(package, session);
                }

                foreach (PackageDetail packageDetail in lstPackageDetailForInsert)
                {
                    PackageDetailKey Key = new PackageDetailKey()
                    {
                        PackageNo = strNewPackageNo,
                        ObjectNumber = packageDetail.Key.ObjectNumber,
                        ObjectType = EnumPackageObjectType.Lot
                    };
                    packageDetail.Key = Key;
                    this.PackageDetailDataEngine.Insert(packageDetail, session);
                }

                foreach (LotTransactionPackage lotTransactionPackage in lstLotTransactionPackageForInsert)
                {
                    lotTransactionPackage.PackageNo = strNewPackageNo;
                    this.LotTransactionPackageDataEngine.Insert(lotTransactionPackage, session);
                }
                #endregion

                #region 更新批次LOT 的信息
                //更新批次基本信息
                foreach (Lot lot in lstLotDataEngineForUpdate)
                {
                    lot.PackageNo = strNewPackageNo;
                    this.LotDataEngine.Update(lot, session);
                }

                //更新批次LotTransaction信息,包装及拆包不需要撤销
                foreach (LotTransaction lotTransaction in lstLotTransactionForInsert)
                {

                    this.LotTransactionDataEngine.Insert(lotTransaction, session);
                }

                //更新批次TransactionHistory信息
                foreach (LotTransactionHistory lotTransactionHistory in lstLotTransactionHistoryForInsert)
                {
                    lotTransactionHistory.PackageNo = strNewPackageNo;
                    this.LotTransactionHistoryDataEngine.Insert(lotTransactionHistory, session);
                }

                //LotTransactionParameter
                foreach (LotTransactionParameter lotTransactionParameter in lstLotTransactionParameterDataEngineForInsert)
                {
                    this.LotTransactionParameterDataEngine.Insert(lotTransactionParameter, session);
                }
                #endregion

                #region //更新设备信息 , 设备的Event ,设备的Transaction
                //LotTransactionEquipment ,Equipment ,EquipmentStateEvent

                foreach (LotTransactionEquipment lotTransactionEquipment in lstLotTransactionEquipmentForUpdate)
                {
                    this.LotTransactionEquipmentDataEngine.Update(lotTransactionEquipment, session);
                }

                foreach (LotTransactionEquipment lotTransactionEquipment in lstLotTransactionEquipmentForInsert)
                {
                    this.LotTransactionEquipmentDataEngine.Insert(lotTransactionEquipment, session);
                }

                foreach (Equipment equipment in lstEquipmentForUpdate)
                {
                    this.EquipmentDataEngine.Update(equipment, session);
                }

                foreach (EquipmentStateEvent equipmentStateEvent in lstEquipmentStateEventForInsert)
                {
                    this.EquipmentStateEventDataEngine.Insert(equipmentStateEvent, session);
                }
                #endregion
                result.ObjectNo = strNewPackageNo;

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

        // 自动生成包装号。
        public string Generate(string lotNumber, bool isLastestPackage)
        {
            Lot obj = this.LotDataEngine.Get(lotNumber);
            if (obj.PackageFlag == false)
            {
                string prefixPackageNo = string.Format("{0}-{1}", obj.OrderNumber, isLastestPackage ? "L" : string.Empty);
                int seqNo = 1;
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format(@"Key LIKE '{0}%'"
                                            , prefixPackageNo),
                    OrderBy = "Key DESC"
                };

                IList<Package> lstPackage = this.PackageDataEngine.Get(cfg);
                if (lstPackage.Count > 0)
                {
                    string maxSeqNo = lstPackage[0].Key.Replace(prefixPackageNo, "");
                    if (int.TryParse(maxSeqNo, out seqNo))
                    {
                        seqNo = seqNo + 1;
                    }
                }
                return string.Format("{0}{1}", prefixPackageNo, isLastestPackage ? seqNo.ToString("000") : seqNo.ToString("0000"));
            }
            else
            {
                return obj.PackageNo;
            }
        }

        // 自动生成包装号。
        MethodReturnResult<string> ILotPackageContract.Generate(string lotNumber, bool isLastestPackage)
        {
            MethodReturnResult<string> result = new MethodReturnResult<string>();

            result = this.PackageNoGenerate.CreatePackageNo(lotNumber);

            return result;
        }

        // 自动生成包装号。
        public string GenerateEx(string lotNumber, ISession session)
        {
            Lot obj = this.LotDataEngine.Get(lotNumber, session);

            if (obj.PackageFlag == false)
            {
                string prefixPackageNo = string.Empty;
                if (obj.OrderNumber.Length > 8)
                {
                    prefixPackageNo = obj.OrderNumber.Substring(obj.OrderNumber.Length - 8, 8);
                }

                int seqNo = 1;
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format(@"Key LIKE '{0}%'"
                                            , prefixPackageNo),
                    OrderBy = "Key DESC"
                };

                IList<Package> lstPackage = this.PackageDataEngine.Get(cfg, session);
                if (lstPackage.Count > 0)
                {
                    if (lstPackage[0].Key.Length < 12 || lstPackage[0].Key.Length > 13)
                    {
                        return "";
                    }
                    string maxSeqNo = lstPackage[0].Key.Replace(prefixPackageNo, "");
                    if (int.TryParse(maxSeqNo, out seqNo))
                    {
                        seqNo = seqNo + 1;
                    }
                }
                return string.Format("{0}{1}", prefixPackageNo, seqNo.ToString("0000"));
            }
            else
            {
                return obj.PackageNo;
            }
        }

        // 自制组件检验批次是否满足入托规则
        private MethodReturnResult CheckLotInPackage(Lot lot, Package package)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                bool packageMixPowerset = false;        //是否允许混档位
                bool packageMixSubPowerset = false;     //是否允许混子档位包装
                bool packageMixColor = false;           //是否允许混花包装
                string packageGroup = string.Empty;     //包装组
                bool lotMixPowerset = false;            //是否允许混档位
                bool lotMixSubPowerset = false;         //是否允许混子档位包装
                bool lotMixColor = false;               //是否允许混花包装
                string lotGroup = string.Empty;         //包装组

                #region 1.取得托信息

                #region 1.1.获取托批次信息（花色、等级）
                //明细中的第一条记录。
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.PackageNo = '{0}' AND ItemNo = 1", package.Key)
                };

                IList<PackageDetail> lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);

                if (lstPackageDetail == null || lstPackageDetail.Count == 0)
                {
                    ////空托不进行判断
                    //package.OrderNumber = lot.OrderNumber;
                    //package.MaterialCode = lot.MaterialCode;
                    //return result;

                    result.Code = 3001;
                    result.Message = string.Format("托号[{0}]首块批次明细不存在！",package.Key);
                    return result;
                }

                //获取托第一个批次信息
                Lot packageLot = this.LotDataEngine.Get(lstPackageDetail[0].Key.ObjectNumber);
                WorkOrderGrade workOrderGrade = new WorkOrderGrade();

                #region 首块批次为OEM组件
                if (packageLot == null)
                {
                    OemData packageLotOfOem = this.OemDataEngine.Get(lstPackageDetail[0].Key.ObjectNumber);
                    //首块批次为OEM组件
                    if (packageLotOfOem != null)
                    {
                        //result.Message = string.Format("托号[{0}]第一块批次[{1}]为OEM代加工组件,在非尾包情况下，自产组件[{2}]不可与之混包！", package.Key,lstPackageDetail[0].Key.ObjectNumber,lot.Key);
                        WorkOrder oemWorkOrderOfFirstLot = this.WorkOrderDataEngine.Get(packageLotOfOem.OrderNumber.ToString().Trim().ToUpper());
                        if (oemWorkOrderOfFirstLot == null)
                        {
                            result.Code = 3001;
                            result.Message = string.Format("批次[{0}]工单[{1}]不存在！",
                                                            packageLotOfOem.Key.ToString(),
                                                            packageLotOfOem.OrderNumber.ToString().Trim().ToUpper());
                            return result;
                        }

                        #region 1.获取托第一个批次分档代码及分档项目号（根据第一个入托批次）
                        cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format("PowerName='{0}' AND Key.OrderNumber='{1}'", packageLotOfOem.PnName, oemWorkOrderOfFirstLot.Key)
                        };
                        IList<WorkOrderPowerset> lstWorkOrderPowersetOfFirstLot = this.WorkOrderPowersetDataEngine.Get(cfg);
                        if (lstWorkOrderPowersetOfFirstLot == null || lstWorkOrderPowersetOfFirstLot.Count == 0)
                        {
                            result.Code = 3001;
                            result.Message = string.Format("获取批次[{0}]所在工单[{1}]功率档[{2}]对应的分档数据失败！", packageLotOfOem.Key, oemWorkOrderOfFirstLot.Key, packageLotOfOem.PnName);

                            return result;
                        }
                        #endregion

                        #region 2.获取托第一个批次包装规则
                        //托工单包装规则
                        workOrderGrade = this.WorkOrderGradeDataEngine.Get(new WorkOrderGradeKey()
                        {
                            OrderNumber = oemWorkOrderOfFirstLot.Key,
                            MaterialCode = oemWorkOrderOfFirstLot.MaterialCode,
                            Grade = packageLotOfOem.Grade
                        });

                        if (workOrderGrade != null)
                        {
                            packageMixPowerset = workOrderGrade.MixPowerset;
                            packageMixSubPowerset = workOrderGrade.MixSubPowerset;
                            packageMixColor = workOrderGrade.MixColor;
                            packageGroup = workOrderGrade.PackageGroup;
                        }
                        #endregion

                        #region 3.获取将入托批次分档及电流档
                        //取得IV测试数据
                        cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format("Key.LotNumber = '{0}' AND IsDefault = 1", lot.Key)
                        };

                        IList<IVTestData> lstLotIVTestData = this.IVTestDataDataEngine.Get(cfg);

                        if (lstLotIVTestData == null || lstLotIVTestData.Count == 0)
                        {
                            result.Code = 3001;
                            result.Message = string.Format("提取批次[{0}]测试数据失败！", lot.Key);

                            return result;
                        }
                        #endregion

                        #region 4.获取将入托批次包装规则
                        //托工单包装规则
                        workOrderGrade = this.WorkOrderGradeDataEngine.Get(new WorkOrderGradeKey()
                        {
                            OrderNumber = lot.OrderNumber,
                            MaterialCode = lot.MaterialCode,
                            Grade = lot.Grade
                        });

                        if (workOrderGrade != null)
                        {
                            lotMixPowerset = workOrderGrade.MixPowerset;
                            lotMixSubPowerset = workOrderGrade.MixSubPowerset;
                            lotMixColor = workOrderGrade.MixColor;
                            lotGroup = workOrderGrade.PackageGroup;
                        }
                        #endregion

                        #region 5.规则判断
                        //判断包装等级组是否一致
                        if (packageGroup != lotGroup)
                        {
                            result.Code = 3002;
                            result.Message = string.Format("提取托[{0}]等级包装组[{1}]与批次[{2}]等级包装组[{3}]不一致！",
                                                            package.Key,
                                                            packageGroup,
                                                            lot.Key,
                                                            lotGroup);

                            return result;
                        }

                        //花色控制
                        if (lot.Color != packageLotOfOem.Color && (!packageMixColor || !lotMixColor))
                        {
                            result.Code = 3003;
                            result.Message = string.Format("托[{0}]花色[{1}]与批次[{2}]花色[{3}]不一致！",
                                                            package.Key,
                                                            packageLotOfOem.Color,
                                                            lot.Key,
                                                            lot.Color);

                            return result;
                        }

                        //混功率档控制
                        if ((lstWorkOrderPowersetOfFirstLot[0].Key.Code != lstLotIVTestData[0].PowersetCode || lstWorkOrderPowersetOfFirstLot[0].Key.ItemNo != lstLotIVTestData[0].PowersetItemNo)
                             && (!packageMixPowerset || !lotMixPowerset))
                        {
                            result.Code = 3003;
                            result.Message = string.Format("托[{0}]分档[{1}]与批次[{2}]分档[{3}]不一致！",
                                                            package.Key,
                                                            lstWorkOrderPowersetOfFirstLot[0].Key.Code + ":" + lstWorkOrderPowersetOfFirstLot[0].Key.ItemNo,
                                                            lot.Key,
                                                            lstLotIVTestData[0].PowersetCode + ":" + lstLotIVTestData[0].PowersetItemNo);

                            return result;
                        }

                        //混电流档控制
                        if (packageLotOfOem.PsSubCode != lstLotIVTestData[0].PowersetSubCode && (!packageMixSubPowerset || !lotMixSubPowerset))
                        {
                            result.Code = 3003;
                            result.Message = string.Format("托[{0}]电流分档[{1}]与批次[{2}]电流分档[{3}]不一致！",
                                                            package.Key,
                                                            packageLotOfOem.PsSubCode,
                                                            lot.Key,
                                                            lstLotIVTestData[0].PowersetSubCode);

                            return result;
                        }
                        #endregion

                    }
                    else
                    {
                        result.Code = 3002;
                        result.Message = string.Format("托号[{0}]第一块批次[{1}]不存在！", package.Key, lstPackageDetail[0].Key.ObjectNumber);
                    }
                    return result;
                }
                #endregion

                #endregion

                #region 1.2.获取分档及电流档
                //取得IV测试数据
                cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber = '{0}' AND IsDefault = 1", packageLot.Key)
                };

                IList<IVTestData> lstPackageLotIVTestData = this.IVTestDataDataEngine.Get(cfg);

                if (lstPackageLotIVTestData == null || lstPackageLotIVTestData.Count == 0)
                {
                    result.Code = 3001;
                    result.Message = string.Format("提取托[{0}]中批次[{1}]测试数据失败！", package.Key, packageLot.Key);

                    return result;
                }
                #endregion

                #region 1.3.获取托包装规则（根据第一个入托批次）
                //托工单包装规则
                workOrderGrade = this.WorkOrderGradeDataEngine.Get(new WorkOrderGradeKey()
                {
                    OrderNumber = packageLot.OrderNumber,
                    MaterialCode = packageLot.MaterialCode,
                    Grade = packageLot.Grade
                });

                if (workOrderGrade != null)
                {
                    packageMixPowerset = workOrderGrade.MixPowerset;
                    packageMixSubPowerset = workOrderGrade.MixSubPowerset;
                    packageMixColor = workOrderGrade.MixColor;
                    packageGroup = workOrderGrade.PackageGroup;
                }
                #endregion

                #endregion

                #region 2.取得批次测试信息

                #region 2.1.获取批次IV测试数据
                cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber = '{0}' AND IsDefault = 1", lot.Key)
                };

                IList<IVTestData> lstLotTestData = this.IVTestDataDataEngine.Get(cfg);

                if (lstLotTestData == null || lstLotTestData.Count == 0)
                {
                    result.Code = 3002;
                    result.Message = string.Format("提取批次[{0}]测试数据失败！", lot.Key);
                    return result;
                }
                #endregion

                #region 2.2.获取批次包装规则
                //托工单包装规则
                workOrderGrade = this.WorkOrderGradeDataEngine.Get(new WorkOrderGradeKey()
                {
                    OrderNumber = lot.OrderNumber,
                    MaterialCode = lot.MaterialCode,
                    Grade = lot.Grade
                });

                if (workOrderGrade != null)
                {
                    lotMixPowerset = workOrderGrade.MixPowerset;
                    lotMixSubPowerset = workOrderGrade.MixSubPowerset;
                    lotMixColor = workOrderGrade.MixColor;
                    lotGroup = workOrderGrade.PackageGroup;
                }
                #endregion

                #endregion

                #region 3.规则判断
                //判断包装等级组是否一致
                if (packageGroup != lotGroup)
                {
                    result.Code = 3002;
                    result.Message = string.Format("提取托[{0}]等级包装组[{1}]与批次[{2}]等级包装组[{3}]不一致！",
                                                    package.Key,
                                                    packageGroup,
                                                    lot.Key,
                                                    lotGroup);

                    return result;
                }

                //花色控制
                if (lot.Color != packageLot.Color && (!packageMixColor || !lotMixColor))
                {
                    result.Code = 3003;
                    result.Message = string.Format("托[{0}]花色[{1}]与批次[{2}]花色[{3}]不一致！",
                                                    package.Key,
                                                    packageLot.Color,
                                                    lot.Key,
                                                    lot.Color);

                    return result;
                }

                //混档控制
                if ((lstLotTestData[0].PowersetCode != lstPackageLotIVTestData[0].PowersetCode || lstLotTestData[0].PowersetItemNo != lstPackageLotIVTestData[0].PowersetItemNo)
                    && (!packageMixPowerset || !lotMixPowerset))
                {
                    result.Code = 3003;
                    result.Message = string.Format("托[{0}]分档[{1}]与批次[{2}]分档[{3}]不一致！",
                                                    package.Key,
                                                    lstPackageLotIVTestData[0].PowersetCode + ":" + lstPackageLotIVTestData[0].PowersetItemNo,
                                                    lot.Key,
                                                    lstLotTestData[0].PowersetCode + ":" + lstLotTestData[0].PowersetItemNo);

                    return result;
                }

                //混电流档控制
                if (lstLotTestData[0].PowersetSubCode != lstPackageLotIVTestData[0].PowersetSubCode && (!packageMixSubPowerset || !lotMixSubPowerset))
                {
                    result.Code = 3003;
                    result.Message = string.Format("托[{0}]电流分档[{1}]与批次[{2}]电流分档[{3}]不一致！",
                                                    package.Key,
                                                    lstPackageLotIVTestData[0].PowersetSubCode,
                                                    lot.Key,
                                                    lstLotTestData[0].PowersetSubCode);

                    return result;
                }

                #endregion
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return result;
        }

        // OEM组件检验批次是否满足入托规则
        private MethodReturnResult CheckLotInPackage(OemData oemData, WorkOrder oemWorkOrder, Package package)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                bool packageMixPowerset = false;        //是否允许混档位
                bool packageMixSubPowerset = false;     //是否允许混子档位包装
                bool packageMixColor = false;           //是否允许混花包装
                string packageGroup = string.Empty;     //包装组
                bool lotMixPowerset = false;            //是否允许混档位
                bool lotMixSubPowerset = false;         //是否允许混子档位包装
                bool lotMixColor = false;               //是否允许混花包装
                string lotGroup = string.Empty;         //包装组

                #region 1.获取托第一个批次信息（花色、等级、功率、分档）
                //明细中的第一条记录。
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.PackageNo = '{0}' AND ItemNo = 1", package.Key)
                };

                IList<PackageDetail> lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);

                if (lstPackageDetail == null || lstPackageDetail.Count == 0)
                {
                    ////空托不进行判断
                    //package.OrderNumber = oemWorkOrder.Key;
                    //package.MaterialCode = oemWorkOrder.MaterialCode;
                    //return result;

                    result.Code = 3001;
                    result.Message = string.Format("托号[{0}]首块批次明细不存在！", package.Key);
                    return result;
                }

                //获取托第一个批次信息
                OemData packageLotOfOem = this.OemDataEngine.Get(lstPackageDetail[0].Key.ObjectNumber);
                WorkOrderGrade workOrderGrade = new WorkOrderGrade();

                #region 托内首块组件为自制组件
                if (packageLotOfOem == null)
                {
                    Lot packageLot = this.LotDataEngine.Get(lstPackageDetail[0].Key.ObjectNumber);

                    if (packageLot != null)
                    {
                        //result.Message = string.Format("托号[{0}]第一块批次[{1}]为自产组件,在非尾包情况下，OEM组件[{2}]不可与之混包！", package.Key, lstPackageDetail[0].Key.ObjectNumber, oemData.Key);

                        #region 1.获取第一块批次分档及电流档
                        //取得IV测试数据
                        cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format("Key.LotNumber = '{0}' AND IsDefault = 1", packageLot.Key)
                        };

                        IList<IVTestData> lstPackageLotIVTestData = this.IVTestDataDataEngine.Get(cfg);

                        if (lstPackageLotIVTestData == null || lstPackageLotIVTestData.Count == 0)
                        {
                            result.Code = 3001;
                            result.Message = string.Format("提取托[{0}]中批次[{1}]测试数据失败！", package.Key, packageLot.Key);

                            return result;
                        }
                        #endregion

                        #region 2.获取托第一块批次包装规则（根据第一个入托批次）
                        //托工单包装规则
                        workOrderGrade = this.WorkOrderGradeDataEngine.Get(new WorkOrderGradeKey()
                        {
                            OrderNumber = packageLot.OrderNumber,
                            MaterialCode = packageLot.MaterialCode,
                            Grade = packageLot.Grade
                        });

                        if (workOrderGrade != null)
                        {
                            packageMixPowerset = workOrderGrade.MixPowerset;
                            packageMixSubPowerset = workOrderGrade.MixSubPowerset;
                            packageMixColor = workOrderGrade.MixColor;
                            packageGroup = workOrderGrade.PackageGroup;
                        }
                        #endregion

                        #region 3.获取将入托批次分档代码及分档项目号
                        cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format("PowerName='{0}' AND Key.OrderNumber='{1}'", oemData.PnName, oemWorkOrder.Key)
                        };
                        IList<WorkOrderPowerset> lstWorkOrderPowerset = this.WorkOrderPowersetDataEngine.Get(cfg);
                        if (lstWorkOrderPowerset == null || lstWorkOrderPowerset.Count == 0)
                        {
                            result.Code = 3001;
                            result.Message = string.Format("获取批次[{0}]所在工单[{1}]功率档[{2}]对应的分档数据失败！", oemData.Key, oemWorkOrder.Key, oemData.PnName);

                            return result;
                        }
                        #endregion

                        #region 4.获取将入托批次包装规则
                        //托工单包装规则
                        workOrderGrade = this.WorkOrderGradeDataEngine.Get(new WorkOrderGradeKey()
                        {
                            OrderNumber = oemWorkOrder.Key,
                            MaterialCode = oemWorkOrder.MaterialCode,
                            Grade = oemData.Grade
                        });

                        if (workOrderGrade != null)
                        {
                            lotMixPowerset = workOrderGrade.MixPowerset;
                            lotMixSubPowerset = workOrderGrade.MixSubPowerset;
                            lotMixColor = workOrderGrade.MixColor;
                            lotGroup = workOrderGrade.PackageGroup;
                        }
                        #endregion

                        #region 5.规则判断
                        //判断包装等级组是否一致
                        if (packageGroup != lotGroup)
                        {
                            result.Code = 3002;
                            result.Message = string.Format("提取托[{0}]等级包装组[{1}]与批次[{2}]等级包装组[{3}]不一致！",
                                                            package.Key,
                                                            packageGroup,
                                                            oemData.Key,
                                                            lotGroup);

                            return result;
                        }

                        //花色控制
                        if (oemData.Color != packageLot.Color && (!packageMixColor || !lotMixColor))
                        {
                            result.Code = 3003;
                            result.Message = string.Format("托[{0}]花色[{1}]与批次[{2}]花色[{3}]不一致！",
                                                            package.Key,
                                                            packageLotOfOem.Color,
                                                            oemData.Key,
                                                            oemData.Color);

                            return result;
                        }

                        //混功率档控制
                        if ((lstWorkOrderPowerset[0].Key.Code != lstPackageLotIVTestData[0].PowersetCode || lstWorkOrderPowerset[0].Key.ItemNo != lstPackageLotIVTestData[0].PowersetItemNo)
                             && (!packageMixPowerset || !lotMixPowerset))
                        {
                            result.Code = 3003;
                            result.Message = string.Format("托[{0}]分档[{1}]与批次[{2}]分档[{3}]不一致！",
                                                            package.Key,
                                                            lstPackageLotIVTestData[0].PowersetCode + ":" + lstPackageLotIVTestData[0].PowersetItemNo,
                                                            oemData.Key,
                                                            lstWorkOrderPowerset[0].Key.Code + ":" + lstWorkOrderPowerset[0].Key.ItemNo);

                            return result;
                        }

                        //混电流档控制
                        if (oemData.PsSubCode != lstPackageLotIVTestData[0].PowersetSubCode && (!packageMixSubPowerset || !lotMixSubPowerset))
                        {
                            result.Code = 3003;
                            result.Message = string.Format("托[{0}]电流分档[{1}]与批次[{2}]电流分档[{3}]不一致！",
                                                            package.Key,
                                                            lstPackageLotIVTestData[0].PowersetSubCode,
                                                            oemData.Key,
                                                            oemData.PsSubCode);

                            return result;
                        }
                        #endregion
                    }
                    else
                    {
                        result.Code = 3001;
                        result.Message = string.Format("托号[{0}]第一块批次[{1}]不存在！", package.Key, lstPackageDetail[0].Key.ObjectNumber);
                    }
                    return result;
                }

                WorkOrder oemWorkOrderOfFirstLot = this.WorkOrderDataEngine.Get(packageLotOfOem.OrderNumber.ToString().Trim().ToUpper());
                if (oemWorkOrderOfFirstLot == null)
                {
                    result.Code = 3001;
                    result.Message = string.Format("批次[{0}]工单[{1}]不存在！",
                                                    packageLotOfOem.Key.ToString(),
                                                    packageLotOfOem.OrderNumber.ToString().Trim().ToUpper());
                    return result;
                }
                #endregion

                #endregion

                #region 2.获取托第一块批次分档代码及分档项目号
                cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("PowerName='{0}' AND Key.OrderNumber='{1}'", packageLotOfOem.PnName, oemWorkOrderOfFirstLot.Key)
                };
                IList<WorkOrderPowerset> lstWorkOrderPowersetOfFirstLot = this.WorkOrderPowersetDataEngine.Get(cfg);
                if (lstWorkOrderPowersetOfFirstLot == null || lstWorkOrderPowersetOfFirstLot.Count == 0)
                {
                    result.Code = 3001;
                    result.Message = string.Format("获取批次[{0}]所在工单[{1}]功率档[{2}]对应的分档数据失败！", packageLotOfOem.Key, oemWorkOrderOfFirstLot.Key, packageLotOfOem.PnName);

                    return result;
                }
                #endregion

                #region 3.获取托包装规则（根据第一个入托批次）
                //托工单包装规则
                workOrderGrade = this.WorkOrderGradeDataEngine.Get(new WorkOrderGradeKey()
                {
                    OrderNumber = oemWorkOrderOfFirstLot.Key,
                    MaterialCode = oemWorkOrderOfFirstLot.MaterialCode,
                    Grade = packageLotOfOem.Grade
                });

                if (workOrderGrade != null)
                {
                    packageMixPowerset = workOrderGrade.MixPowerset;
                    packageMixSubPowerset = workOrderGrade.MixSubPowerset;
                    packageMixColor = workOrderGrade.MixColor;
                    packageGroup = workOrderGrade.PackageGroup;
                }
                #endregion

                #region 4.获取将入托批次分档代码及分档项目号
                cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("PowerName='{0}' AND Key.OrderNumber='{1}'", oemData.PnName, oemWorkOrder.Key)
                };
                IList<WorkOrderPowerset> lstWorkOrderPowersetOfOemLot = this.WorkOrderPowersetDataEngine.Get(cfg);
                if (lstWorkOrderPowersetOfOemLot == null || lstWorkOrderPowersetOfOemLot.Count == 0)
                {
                    result.Code = 3001;
                    result.Message = string.Format("获取批次[{0}]所在工单[{1}]功率档[{2}]对应的分档数据失败！", oemData.Key, oemWorkOrder.Key, oemData.PnName);

                    return result;
                }
                #endregion

                #region 5.获取将入托批次包装规则
                //托工单包装规则
                workOrderGrade = this.WorkOrderGradeDataEngine.Get(new WorkOrderGradeKey()
                {
                    OrderNumber = oemWorkOrder.Key,
                    MaterialCode = oemWorkOrder.MaterialCode,
                    Grade = oemData.Grade
                });

                if (workOrderGrade != null)
                {
                    lotMixPowerset = workOrderGrade.MixPowerset;
                    lotMixSubPowerset = workOrderGrade.MixSubPowerset;
                    lotMixColor = workOrderGrade.MixColor;
                    lotGroup = workOrderGrade.PackageGroup;
                }
                #endregion

                #region 6.规则判断
                //判断包装等级组是否一致
                if (packageGroup != lotGroup)
                {
                    result.Code = 3002;
                    result.Message = string.Format("提取托[{0}]等级包装组[{1}]与批次[{2}]等级包装组[{3}]不一致！",
                                                    package.Key,
                                                    packageGroup,
                                                    oemData.Key,
                                                    lotGroup);

                    return result;
                }

                //花色控制
                if (oemData.Color != packageLotOfOem.Color && (!packageMixColor || !lotMixColor))
                {
                    result.Code = 3003;
                    result.Message = string.Format("托[{0}]花色[{1}]与批次[{2}]花色[{3}]不一致！",
                                                    package.Key,
                                                    packageLotOfOem.Color,
                                                    oemData.Key,
                                                    oemData.Color);

                    return result;
                }

                //混功率档控制
                //if (oemData.PnName != packageLotOfOem.PnName)
                //{
                //    result.Code = 3003;
                //    result.Message = string.Format("托[{0}]分档[{1}]与批次[{2}]分档[{3}]不一致！",
                //                                    package.Key,
                //                                    packageLotOfOem.PnName,
                //                                    oemData.Key,
                //                                    oemData.PnName);

                //    return result;
                //}
                if ((lstWorkOrderPowersetOfOemLot[0].Key.Code != lstWorkOrderPowersetOfFirstLot[0].Key.Code || lstWorkOrderPowersetOfOemLot[0].Key.ItemNo != lstWorkOrderPowersetOfFirstLot[0].Key.ItemNo)
                             && (!packageMixPowerset || !lotMixPowerset))
                {
                    result.Code = 3003;
                    result.Message = string.Format("托[{0}]分档[{1}]与批次[{2}]分档[{3}]不一致！",
                                                    package.Key,
                                                    lstWorkOrderPowersetOfFirstLot[0].Key.Code + ":" + lstWorkOrderPowersetOfFirstLot[0].Key.ItemNo,
                                                    oemData.Key,
                                                    lstWorkOrderPowersetOfOemLot[0].Key.Code + ":" + lstWorkOrderPowersetOfOemLot[0].Key.ItemNo);

                    return result;
                }

                //混电流档控制
                if (oemData.PsSubCode != packageLotOfOem.PsSubCode && (!packageMixSubPowerset || !lotMixSubPowerset))
                {
                    result.Code = 3003;
                    result.Message = string.Format("托[{0}]电流分档[{1}]与批次[{2}]电流分档[{3}]不一致！",
                                                    package.Key,
                                                    packageLotOfOem.PsSubCode,
                                                    oemData.Key,
                                                    oemData.PsSubCode);

                    return result;
                }
                #endregion

            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return result;
        }

        // 自制组件根据批次号创建新托号
        public MethodReturnResult<string> CreatePackageNo(string lotNumber)
        {
            MethodReturnResult<string> result = new MethodReturnResult<string>();

            try
            {
                result.Data = "";
            }
            catch (Exception ex)
            {
                result.Code = 2000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }

            return result;
        }

        // OEM组件根据批次号创建新托号
        public MethodReturnResult<string> CreatePackageNo(OemData oemData, WorkOrder oemWorkOrder)
        {
            MethodReturnResult<string> result = new MethodReturnResult<string>();

            try
            {
                result.Data = "";
            }
            catch (Exception ex)
            {
                result.Code = 2000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }

            return result;
        }

        #endregion  
    }
}
