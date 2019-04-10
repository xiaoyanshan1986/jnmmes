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
using NHibernate;
using Microsoft.Practices.EnterpriseLibrary.Data;
//using ServiceCenter.Model;
//using System;
//using System.Collections.Generic;
using System.Data;
using System.Data.Common;
//using System.Linq;
//using System.ServiceModel.Activation;
//using System.Text;
//using System.Threading.Tasks;
//using System.Transactions;
using ServiceCenter.Common;
using ServiceCenter.MES.DataAccess.Interface.ERP;
using ServiceCenter.MES.Model.ERP;
using ServiceCenter.MES.Service.Contract.ERP;
using System.Data.SqlClient;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.ZPVM;

namespace ServiceCenter.MES.Service.WIP
{
    // 实现批次返工单服务契约
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LotReworkService : ILotReworkContract, ILotReworkCheck, ILotRework
    {
        #region 定义数据库实例

        protected Database _db;

        #endregion

        #region 构造函数
        public LotReworkService()
        {
            this.RegisterCheckInstance(this);
            this.RegisterExecutedInstance(this);
            this._db = DatabaseFactory.CreateDatabase();
        }

        #endregion

        #region 定义数据访问对象

        //柜动作日志数据访问对象
        public IChestLogDataEngine ChestLogDataEngine { get; set; }

        // 批次数据访问类
        public ILotDataEngine LotDataEngine { get; set; }

        // 虚拟BIN数据访问类
        public IPackageCornerDetailDataEngine PackageCornerDetailDataEngine { get; set; }

        // 批次操作数据访问类
        public ILotTransactionDataEngine LotTransactionDataEngine { get; set; }

        // 批次历史数据访问类
        public ILotTransactionHistoryDataEngine LotTransactionHistoryDataEngine { get; set; }

        // 批次附加参数数据访问类
        public ILotTransactionParameterDataEngine LotTransactionParameterDataEngine { get; set; }

        // 包装数据访问对象
        public IPackageDataEngine PackageDataEngine { get; set; }

        // 包装明细数据访问对象
        public IPackageDetailDataEngine PackageDetailDataEngine { get; set; }

        // 工艺流程工序属性数据访问对象
        public IRouteStepAttributeDataEngine RouteStepAttributeDataEngine { get; set; }

        // 批次属性数据访问对象
        public ILotAttributeDataEngine LotAttributeDataEngine { get; set; }

        // 柜数据访问对象
        public IChestDataEngine ChestDataEngine { get; set; }

        // 柜明细数据访问对象
        public IChestDetailDataEngine ChestDetailDataEngine { get; set; }

        // Oem批次数据访问对象
        public IOemDataEngine OemDataEngine { get; set; }

        // 工单数据访问对象
        public IWorkOrderDataEngine WorkOrderDataEngine { get; set; }

        // 线边仓物料明细数据访问类
        public ILineStoreMaterialDetailDataEngine LineStoreMaterialDetailDataEngine { get; set; }

        // 工单工艺流程访问类
        public IWorkOrderRouteDataEngine WorkOrderRouteDataEngine { get; set; }

        // 工艺流程工步列表访问类
        public IRouteStepDataEngine RouteStepDataEngine { get; set; }

        #endregion

        #region 定于数据访问对象的列表
        List<LotTransactionPackage> lstLotTransactionPackageForInsert = new List<LotTransactionPackage>();
        #endregion

        #region 定义事件

        // 操作前检查事件
        public event Func<ReworkParameter, MethodReturnResult> CheckEvent;

        // 执行操作时事件
        public event Func<ReworkParameter, MethodReturnResult> ExecutingEvent;

        // 操作执行完成事件
        public event Func<ReworkParameter, MethodReturnResult> ExecutedEvent;

        #endregion

        #region 定义事件清单列表

        // 自定义操作前检查的清单列表
        private IList<ILotReworkCheck> CheckList { get; set; }

        // 自定义执行中操作的清单列表
        private IList<ILotRework> ExecutingList { get; set; }

        // 自定义执行后操作的清单列表
        private IList<ILotRework> ExecutedList { get; set; }

        #endregion

        #region 定义事件操作实例

        // 注册自定义检查的操作实例
        public void RegisterCheckInstance(ILotReworkCheck obj)
        {
            if (this.CheckList == null)
            {
                this.CheckList = new List<ILotReworkCheck>();
            }
            this.CheckList.Add(obj);
        }

        // 注册执行中的自定义操作实例
        public void RegisterExecutingInstance(ILotRework obj)
        {
            if (this.ExecutingList == null)
            {
                this.ExecutingList = new List<ILotRework>();
            }
            this.ExecutingList.Add(obj);
        }

        // 注册执行完成后的自定义操作实例
        public void RegisterExecutedInstance(ILotRework obj)
        {
            if (this.ExecutedList == null)
            {
                this.ExecutedList = new List<ILotRework>();
            }
            this.ExecutedList.Add(obj);
        }

        #endregion

        #region 定义事件执行方法

        // 返工投批前检查
        protected virtual MethodReturnResult OnCheck(PackageReworkParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            StringBuilder sbMessage = new StringBuilder();

            if (this.CheckEvent != null)
            {
                foreach (Func<PackageReworkParameter, MethodReturnResult> d in this.CheckEvent.GetInvocationList())
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
                foreach (ILotReworkCheck d in this.CheckList)
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

        // 操作执行中
        protected virtual MethodReturnResult OnExecuting(PackageReworkParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            StringBuilder sbMessage = new StringBuilder();
            if (this.ExecutingEvent != null)
            {
                foreach (Func<PackageReworkParameter, MethodReturnResult> d in this.ExecutingEvent.GetInvocationList())
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
                foreach (ILotRework d in this.ExecutingList)
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

        // 执行完成
        protected virtual MethodReturnResult OnExecuted(PackageReworkParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            StringBuilder sbMessage = new StringBuilder();
            if (this.ExecutedEvent != null)
            {
                foreach (Func<PackageReworkParameter, MethodReturnResult> d in this.ExecutedEvent.GetInvocationList())
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
                foreach (ILotRework d in this.ExecutedList)
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

        #endregion

        #region 批次返工操作执行方法

        // 批次返工单操作---代码表示：0：成功，其他失败
        MethodReturnResult ILotReworkContract.Rework(PackageReworkParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();
            
            try
            {
                if (p == null)
                {
                    result.Code = 1001;
                    result.Message = StringResource.ParameterIsNull;
                    return result;
                }

                StringBuilder sbMessage = new StringBuilder();

                result = this.OnExecuted(p);
                
                if (result.Code > 0)
                {
                    return result;
                }

                sbMessage.Append(result.Message);

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

        // 操作前检查
        MethodReturnResult ILotReworkCheck.Check(PackageReworkParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

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
            }
            return result;
        }

        // 批次投料操作
        MethodReturnResult ILotRework.Execute(PackageReworkParameter p)
        {            
            MethodReturnResult result = new MethodReturnResult();
            
            try
            {
                List<Lot> lstLotDataEngineForUpdate = new List<Lot>();                                  //批次更新列表
                List<PackageDetail> lstPackageDetailForDelete = new List<PackageDetail>();              //包装明细删除列表
                List<ChestDetail> lstChestDetailForDelete = new List<ChestDetail>();                    //柜明细删除列表
                List<ChestDetail> lstChestDetailForUpdate = new List<ChestDetail>();                    //柜明细更新列表
                List<LotTransaction> lstLotTransInsert = new List<LotTransaction>();                    //批次事物列表
                List<LotTransactionHistory> lstLotTransHisInsert = new List<LotTransactionHistory>();   //批次历史事物列表
                List<OemData> lstOemLotForUpdate = new List<OemData>();                                 //OEM组件事物列表
                List<LotAttribute> lstLotAttributeForDelete = new List<LotAttribute>();
                WorkOrderRoute workOrderRoute = null;
                IList<RouteStep> lstRouteStep = null;
                RouteStep routeStep = null;
                DateTime now = DateTime.Now;
                ChestLog chestLog = null;
                double packageQty = 0;    //托内数量
                double lineStoreQty = 0;  //线边仓内数量

                #region 1.取得托并判断合理性
                Package package = this.PackageDataEngine.Get(p.PackageNo);

                if (package == null)
                {
                    result.Code = 2000;
                    result.Message = string.Format("托号[{0}]不存在！", p.PackageNo);

                    return result;
                }

                if (package.PackageState != EnumPackageState.InFabStore)
                {
                    result.Code = 2000;
                    result.Message = string.Format("托[{0}]状态为[{1}]，非线边仓！",
                                                    p.PackageNo,
                                                    package.PackageState.GetDisplayName());
                    return result;
                }
                packageQty = package.Quantity;
                #endregion               

                #region 2.取得工单信息/工单工艺流程及第一个工步
                //取得工单信息
                WorkOrder workorder = WorkOrderDataEngine.Get(package.OrderNumber);

                if (workorder == null)
                {
                    result.Code = 2002;
                    result.Message = "工单[" + package.OrderNumber + "]不存在！";
                    return result;
                }

                //判断车间是否相同
                if (workorder.LocationName != p.LocationName)
                {
                    result.Code = 2003;
                    result.Message = string.Format("托[{0}]产品属于[{1}]车间！",
                                           p.PackageNo,
                                           workorder.LocationName);

                    return result;
                }

                //取得工单主流程
                PagingConfig cfg = new PagingConfig();
                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.OrderNumber = '{0}' AND IsRework = false", workorder.Key),
                    OrderBy = "Key.ItemNo"
                };

                IList<WorkOrderRoute> lstWorkOrderRoute = WorkOrderRouteDataEngine.Get(cfg);

                if (lstWorkOrderRoute != null && lstWorkOrderRoute.Count > 0)
                {
                    workOrderRoute = lstWorkOrderRoute.First<WorkOrderRoute>();
                }
                else
                {
                    result.Code = 2003;
                    result.Message = string.Format("工单{0}的主流程未设置！", workorder.Key);

                    return result;
                }

                //取得工艺主流程第一个工步               
                cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format(@"Key.RouteName='{0}'",
                                            workOrderRoute.RouteName),
                    OrderBy = "SortSeq"
                };

                lstRouteStep = RouteStepDataEngine.Get(cfg);

                if (lstRouteStep.Count == 0)
                {
                    result.Code = 1000;
                    result.Message = string.Format("工艺流程{0}的工艺工步未设置！", workOrderRoute.RouteName);

                    return result;
                }

                routeStep = lstRouteStep.First<RouteStep>();
                #endregion

                #region 3.取得线边仓对应记录
                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.OrderNumber = '{0}' and Key.MaterialLot = '{1}' and CurrentQty>0",
                                           workorder.Key,
                                           p.PackageNo)
                };

                //根据线边仓明细记录主键取得记录数据
                IList<LineStoreMaterialDetail> lstLineStoreMaterialDetail = LineStoreMaterialDetailDataEngine.Get(cfg);

                if (lstLineStoreMaterialDetail.Count > 1)
                {
                    result.Code = 2005;
                    result.Message = string.Format("托[{0}]线边仓存在多条记录！",
                                                    p.PackageNo);

                    return result;
                }

                LineStoreMaterialDetail lsmd = lstLineStoreMaterialDetail.FirstOrDefault();
                if (lsmd == null)
                {
                    result.Code = 2003;
                    result.Message = string.Format("托[{0}]线边仓记录不存在！",
                                                    p.PackageNo);

                    return result;
                }
                lineStoreQty = lsmd.CurrentQty;
                if (!p.IsLot)
                {
                    if (lineStoreQty != packageQty)
                    {
                        result.Code = 2003;
                        result.Message = string.Format("托号{0}线边仓领料数量{1}与MES系统入库数量{2}不一致,请确认托内数量,如果确实不一致,请选择按批次投料！！！",
                                                        p.PackageNo, lineStoreQty, packageQty);

                        return result;
                    }
                }
                else
                {
                    if (lineStoreQty > packageQty)
                    {
                        result.Code = 2003;
                        result.Message = string.Format("托号{0}线边仓领料数量{1}大于MES系统入库数量{2},请确认托内数量！！！",
                                                        p.PackageNo, lineStoreQty, packageQty);

                        return result;
                    }
                }
                #endregion
                
                #region 4.处理批次数据
                
                #region 4.1提取包装明细对象列表
                //取得包装明细
                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.PackageNo = '{0}'",
                                           p.PackageNo)
                };                

                //提取全部包装明细数据
                IList<PackageDetail> lstPackageDetail = PackageDetailDataEngine.Get(cfg);
                //当明细记录为零时提取归档数据
                if (lstPackageDetail.Count == 0)
                {
                    #region 4.1.1.提取归档数据
                    REbackdataParameter param = new REbackdataParameter()
                    {
                        PackageNo = p.PackageNo,
                        ReType = 2,
                        IsDelete = 1
                    };

                    result = GetREbackdata(param);

                    if (result.Code > 0)
                    {
                        return result;
                    }
                    #endregion

                    //重新提取包装明细数据
                    lstPackageDetail = PackageDetailDataEngine.Get(cfg);
                }

                if (lstPackageDetail.Count == 0)
                {
                    result.Code = 2006;
                    result.Message = string.Format("托[{0}]中批次记录为0！'",
                                                    p.PackageNo);

                    return result;
                }
                #endregion

                #region 4.2循环处理批次对象
                string transactionKey = "";
    
                //循环处理批次信息
                foreach(PackageDetail packageDetail in lstPackageDetail)
                {
                    OemData oemLot = this.OemDataEngine.Get(packageDetail.Key.ObjectNumber);
                    if (oemLot != null)
                    {
                        #region oem组件
                        if (p.IsLot)
                        {
                            if (p.LotNumber.Trim() == oemLot.Key)
                            {
                                //保留托号
                                p.RetainPackageNo = true;

                                //处理线边仓数据记录
                                lsmd.CurrentQty -= 1;                   //当前数量
                                lsmd.Editor = p.Operator;               //编辑人
                                lsmd.EditTime = now;                    //编辑时间
                                lsmd.LoadingQty += 1;                   //上料数量

                                oemLot.Status = EnumOemStatus.Import;
                                oemLot.OrderNumber = workorder.Key;
                                oemLot.PackageNo = "";
                                oemLot.Editor = p.Operator;
                                oemLot.EditTime = now;

                                lstOemLotForUpdate.Add(oemLot);
                                lstPackageDetailForDelete.Add(packageDetail);
                            }                           
                        }
                        else
                        {
                            //处理线边仓数据记录
                            lsmd.CurrentQty -= 1;                   //当前数量
                            lsmd.Editor = p.Operator;               //编辑人
                            lsmd.EditTime = now;                    //编辑时间
                            lsmd.LoadingQty += 1;                   //上料数量

                            oemLot.Status = EnumOemStatus.Import;
                            oemLot.OrderNumber = workorder.Key;
                            oemLot.PackageNo = "";
                            oemLot.Editor = p.Operator;
                            oemLot.EditTime = now;

                            lstOemLotForUpdate.Add(oemLot);
                        }                       
                        
                        #endregion                           
                    }
                    else
                    {
                        #region 自制组件
                        //取得批次对象
                        Lot lot = this.LotDataEngine.Get(packageDetail.Key.ObjectNumber);
                        if (p.IsLot)
                        {
                            if (p.LotNumber.Trim() == lot.Key.Trim())
                            {
                                //保留托号
                                p.RetainPackageNo = true;

                                //处理线边仓数据记录
                                lsmd.CurrentQty -= 1;                   //当前数量
                                lsmd.Editor = p.Operator;               //编辑人
                                lsmd.EditTime = now;                    //编辑时间
                                lsmd.LoadingQty += 1;                   //上料数量

                                #region 包装明细批次处理
                                //生成操作事务主键。
                                transactionKey = Guid.NewGuid().ToString();

                                //创建批次历史事物
                                LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lot);
                                lstLotTransHisInsert.Add(lotHistory);

                                //记录操作事物数据
                                LotTransaction transLot = new LotTransaction()
                                {
                                    Key = transactionKey,                               //事物主键 
                                    //Activity = EnumLotActivity.TrackIn,                 //批次状态
                                    Activity = EnumLotActivity.TrackOut,                //批次状态  
                                    CreateTime = now,                                   //创建时间
                                    Creator = p.Operator,                               //创建人
                                    Description = "",                                   //描述
                                    Editor = p.Operator,                                //编辑人
                                    EditTime = now,                                     //编辑时间
                                    InQuantity = lot.Quantity,                          //数量
                                    LotNumber = lot.Key,                                //组件批次号
                                    LocationName = p.LocationName,                      //车间
                                    LineCode = p.LineCode,                              //线别
                                    OperateComputer = p.OperateComputer,                //操作电脑
                                    OrderNumber = workorder.Key,                        //工单
                                    OutQuantity = lot.Quantity,                         //出站数量
                                    RouteEnterpriseName = "",                           //工艺流程组
                                    RouteName = routeStep.Key.RouteName,                //工艺流程
                                    RouteStepName = "线边仓",                           //工序名称
                                    //RouteStepName = routeStep.Key.RouteStepName,        //工序名称
                                    ShiftName = "",                                     //班别
                                    UndoFlag = false,                                   //撤销标识
                                    UndoTransactionKey = "",                            //撤销主键
                                    Grade = lot.Grade,                                  //等级
                                    Color = lot.Color,                                  //花色
                                    Attr1 = lot.Attr1,                                  //批次属性1
                                    Attr2 = lot.Attr2,                                  //批次属性2
                                    Attr3 = "",                                         //批次属性3
                                    Attr4 = "",                                         //批次属性4
                                    Attr5 = "",                                         //批次属性5
                                    OriginalOrderNumber = lot.OrderNumber               //原始工单
                                };

                                //增加事物列表
                                lstLotTransInsert.Add(transLot);

                                //更新批次属性
                                lot.PreLineCode = lot.LineCode;
                                lot.RouteEnterpriseName = "";                       //工艺流程组（是否废止？）        
                                lot.RouteName = routeStep.Key.RouteName;            //工艺流程
                                lot.RouteStepName = routeStep.Key.RouteStepName;    //工步
                                lot.StateFlag = EnumLotState.WaitTrackIn;           //批次状态（等待进站）
                                lot.EquipmentCode = "";                             //设备代码
                                lot.StartWaitTime = now;                            //开始等待时间
                                lot.StartProcessTime = now;                         //开始处理时间
                                lot.Editor = p.Operator;                            //编辑人
                                lot.EditTime = now;                                 //编辑日期
                                lot.OrderNumber = workorder.Key;
                                lot.MaterialCode = workorder.MaterialCode;
                                lot.PackageFlag = false;
                                lot.PackageNo = "";
                                lot.ReworkFlag = lot.ReworkFlag + 1;
                                lot.OperateComputer = p.OperateComputer;
                                lot.LocationName = p.LocationName;
                                lot.LineCode = p.LineCode;

                                lstLotDataEngineForUpdate.Add(lot);

                                lstPackageDetailForDelete.Add(packageDetail);

                                #region //清除批次IV图片属性或层压机台号属性
                                //1.判断返修工艺流程中工序过站是否需要层压机台号
                                PagingConfig cfgOfStepAttr = new PagingConfig()
                                {
                                    Where = string.Format("Key.RouteName = '{0}' AND Key.AttributeName = 'isGetLayerEquipment'", workOrderRoute.RouteName)
                                };
                                IList<RouteStepAttribute> lstRouteStepAttribute = RouteStepAttributeDataEngine.Get(cfgOfStepAttr);
                                if (lstRouteStepAttribute != null && lstRouteStepAttribute.Count > 0)
                                {
                                    #region 清除批次层压机台号属性
                                    //获取批次层压机台号
                                    LotAttributeKey lotAttributeKey = new LotAttributeKey()
                                    {
                                        LotNumber = lot.Key,
                                        AttributeName = "LayerEquipmentNo"
                                    };
                                    LotAttribute lotAttribute = this.LotAttributeDataEngine.Get(lotAttributeKey);
                                    if (lotAttribute != null)
                                    {
                                        //this.LotAttributeDataEngine.Delete(lotAttributeKey);
                                        lstLotAttributeForDelete.Add(lotAttribute);
                                    }
                                    #endregion
                                }
                                //2.获取返修工艺流程工序并判断是否包含功率测试站
                                RouteStepKey routeStepKey = new RouteStepKey()
                                {
                                    RouteName = workOrderRoute.RouteName,
                                    RouteStepName = "功率测试"
                                };
                                RouteStep routeStepOfIV = this.RouteStepDataEngine.Get(routeStepKey);
                                if (routeStepOfIV != null)
                                {
                                    #region 清除批次IV图片属性
                                    //获取批次IV图片属性
                                    LotAttributeKey lotAttributeKey = new LotAttributeKey()
                                    {
                                        LotNumber = lot.Key,
                                        AttributeName = "IVImagePath"
                                    };
                                    LotAttribute lotAttribute = this.LotAttributeDataEngine.Get(lotAttributeKey);
                                    if (lotAttribute != null)
                                    {
                                        //this.LotAttributeDataEngine.Delete(lotAttributeKey);
                                        lstLotAttributeForDelete.Add(lotAttribute);
                                    }
                                    #endregion
                                }
                                #endregion

                                #endregion
                            }
                        }
                        else
                        {
                            //处理线边仓数据记录
                            lsmd.CurrentQty -= 1;                   //当前数量
                            lsmd.Editor = p.Operator;               //编辑人
                            lsmd.EditTime = now;                    //编辑时间
                            lsmd.LoadingQty += 1;                   //上料数量

                            #region 包装明细批次处理
                            //生成操作事务主键。
                            transactionKey = Guid.NewGuid().ToString();

                            //创建批次历史事物
                            LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lot);
                            lstLotTransHisInsert.Add(lotHistory);

                            //记录操作事物数据
                            LotTransaction transLot = new LotTransaction()
                            {
                                Key = transactionKey,                               //事物主键 
                                //Activity = EnumLotActivity.TrackIn,                 //批次状态
                                Activity = EnumLotActivity.TrackOut,                //批次状态  
                                CreateTime = now,                                   //创建时间
                                Creator = p.Operator,                               //创建人
                                Description = "",                                   //描述
                                Editor = p.Operator,                                //编辑人
                                EditTime = now,                                     //编辑时间
                                InQuantity = lot.Quantity,                          //数量
                                LotNumber = lot.Key,                                //组件批次号
                                LocationName = p.LocationName,                      //车间
                                LineCode = p.LineCode,                              //线别
                                OperateComputer = p.OperateComputer,                //操作电脑
                                OrderNumber = workorder.Key,                        //工单
                                OutQuantity = lot.Quantity,                         //出站数量
                                RouteEnterpriseName = "",                           //工艺流程组
                                RouteName = routeStep.Key.RouteName,                //工艺流程
                                RouteStepName = "线边仓",                           //工序名称
                                //RouteStepName = routeStep.Key.RouteStepName,      //工序名称
                                ShiftName = "",                                     //班别
                                UndoFlag = false,                                   //撤销标识
                                UndoTransactionKey = "",                            //撤销主键
                                Grade = lot.Grade,                                  //等级
                                Color = lot.Color,                                  //花色
                                Attr1 = lot.Attr1,                                  //批次属性1
                                Attr2 = lot.Attr2,                                  //批次属性2
                                Attr3 = "",                                         //批次属性3
                                Attr4 = "",                                         //批次属性4
                                Attr5 = "",                                         //批次属性5
                                OriginalOrderNumber = lot.OrderNumber               //原始工单
                            };

                            //增加事物列表
                            lstLotTransInsert.Add(transLot);

                            //更新批次属性
                            lot.PreLineCode = lot.LineCode;
                            lot.RouteEnterpriseName = "";                       //工艺流程组（是否废止？）        
                            lot.RouteName = routeStep.Key.RouteName;            //工艺流程
                            lot.RouteStepName = routeStep.Key.RouteStepName;    //工步
                            lot.StateFlag = EnumLotState.WaitTrackIn;           //批次状态（等待进站）
                            lot.EquipmentCode = "";                             //设备代码
                            lot.StartWaitTime = now;                            //开始等待时间
                            lot.StartProcessTime = now;                         //开始处理时间
                            lot.Editor = p.Operator;                            //编辑人
                            lot.EditTime = now;                                 //编辑日期
                            lot.OrderNumber = workorder.Key;
                            lot.MaterialCode = workorder.MaterialCode;
                            lot.PackageFlag = false;
                            lot.PackageNo = "";
                            lot.ReworkFlag = lot.ReworkFlag + 1;
                            lot.OperateComputer = p.OperateComputer;
                            lot.LocationName = p.LocationName;
                            lot.LineCode = p.LineCode;

                            lstLotDataEngineForUpdate.Add(lot);

                            #region //清除批次IV图片属性或层压机台号属性
                            //1.判断返修工艺流程中工序过站是否需要层压机台号
                            PagingConfig cfgOfStepAttr = new PagingConfig()
                            {
                                Where = string.Format("Key.RouteName = '{0}' AND Key.AttributeName = 'isGetLayerEquipment'", workOrderRoute.RouteName)
                            };
                            IList<RouteStepAttribute> lstRouteStepAttribute = RouteStepAttributeDataEngine.Get(cfgOfStepAttr);
                            if (lstRouteStepAttribute != null && lstRouteStepAttribute.Count > 0)
                            {
                                #region 清除批次层压机台号属性
                                //获取批次层压机台号
                                LotAttributeKey lotAttributeKey = new LotAttributeKey()
                                {
                                    LotNumber = lot.Key,
                                    AttributeName = "LayerEquipmentNo"
                                };
                                LotAttribute lotAttribute = this.LotAttributeDataEngine.Get(lotAttributeKey);
                                if (lotAttribute != null)
                                {
                                    //this.LotAttributeDataEngine.Delete(lotAttributeKey);
                                    lstLotAttributeForDelete.Add(lotAttribute);
                                }
                                #endregion
                            }
                            //2.获取返修工艺流程工序并判断是否包含功率测试站
                            RouteStepKey routeStepKey = new RouteStepKey()
                            {
                                RouteName = workOrderRoute.RouteName,
                                RouteStepName = "功率测试"
                            };
                            RouteStep routeStepOfIV = this.RouteStepDataEngine.Get(routeStepKey);
                            if (routeStepOfIV != null)
                            {
                                #region 清除批次IV图片属性
                                //获取批次IV图片属性
                                LotAttributeKey lotAttributeKey = new LotAttributeKey()
                                {
                                    LotNumber = lot.Key,
                                    AttributeName = "IVImagePath"
                                };
                                LotAttribute lotAttribute = this.LotAttributeDataEngine.Get(lotAttributeKey);
                                if (lotAttribute != null)
                                {
                                    //this.LotAttributeDataEngine.Delete(lotAttributeKey);
                                    lstLotAttributeForDelete.Add(lotAttribute);
                                }
                                #endregion
                            }
                            #endregion

                            #endregion
                        }
                        #endregion
                    }                   
                }
                #endregion

                //批次不属于托
                if (lstLotTransInsert.Count == 0)
                {
                    if (lstOemLotForUpdate.Count == 0)
                    {
                        result.Code = 2006;
                        result.Message = string.Format("批次[{0}]不属于托[{1}]！'",
                                                        p.LotNumber,
                                                        p.PackageNo);
                        return result;
                    }                   
                }

                #endregion

                #region 5.更新托和柜信息
                if (p.IsLot)
                {        
                    //按批次投料后保留托号但不可再用
                    package.Quantity -= (lstLotDataEngineForUpdate.Count + lstOemLotForUpdate.Count);
                    package.Editor = p.Operator;
                    package.EditTime = now;
                    package.ToWarehousePerson = "";
                    package.ToWarehouseTime = null;                    
                    if (package.Quantity == 0)
                    {
                        package.InOrder = 0;
                        package.PackageState = EnumPackageState.Packaging;                       
                        package.OrderNumber = "";
                        package.MaterialCode = "";
                        package.Location = "";
                        package.LineCode = "";
                        package.PowerName = "";
                        package.PowerSubCode = "";
                        package.Grade = "";
                        package.Color = "";
                    }
                }
                else
                {
                    package.InOrder = 0;
                    package.PackageState = EnumPackageState.Packaging;
                    package.OrderNumber = "";
                    package.MaterialCode = "";
                    package.Quantity -= (lstLotDataEngineForUpdate.Count + lstOemLotForUpdate.Count);
                    package.Editor = p.Operator;
                    package.EditTime = now;
                    package.ToWarehousePerson = "";
                    package.ToWarehouseTime = null;
                    package.Location = "";
                    package.LineCode = "";
                    package.PowerName = "";
                    package.PowerSubCode = "";
                    package.Grade = "";
                    package.Color = "";
                }

                #region 5.1 取得托所在柜明细
                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.ObjectNumber = '{0}'",
                                           p.PackageNo)
                };
                IList<ChestDetail> lstChestDetail = this.ChestDetailDataEngine.Get(cfg);
                
                Chest chest = new Chest();                
                if (lstChestDetail != null && lstChestDetail.Count > 0)
                {
                    
                    chest = this.ChestDataEngine.Get(lstChestDetail[0].Key.ChestNo.ToString());
                    if (chest != null)
                    {
                        cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format("Key.ChestNo = '{0}'",chest.Key)
                        };
                        lstChestDetail = this.ChestDetailDataEngine.Get(cfg);

                        ChestDetail chestDetail0 = null;
                        //校验托号是否在柜明细项中
                        var lstPackageInChestDetail = (from item in lstChestDetail
                                                       where item.Key.ObjectNumber == p.PackageNo
                                                       select item);
                        if (lstPackageInChestDetail == null || lstPackageInChestDetail.Count() == 0)
                        {
                            result.Code = 1003;
                            result.Message = string.Format("柜号（{0}）明细项中不存在托号（{1}）。", chest.Key, p.PackageNo);
                            return result;
                        }
                        else
                        {
                            chestDetail0 = lstPackageInChestDetail.FirstOrDefault();
                        }

                        lstChestDetailForDelete.Add(chestDetail0);

                        //移除出柜的托号
                        foreach (ChestDetail item in lstChestDetailForDelete)
                        {
                            bool iii = lstChestDetail.Remove(item);
                        }

                        //重新定义Item次序
                        int itemNo = 0;
                        foreach (ChestDetail chestDetailObj in lstChestDetail)
                        {
                            itemNo++;
                            if (chestDetailObj.ItemNo == itemNo)
                            {
                                continue;
                            }
                            ChestDetail chestDetailObjUpdate = chestDetailObj.Clone() as ChestDetail;
                            chestDetailObjUpdate.ItemNo = itemNo;
                            lstChestDetailForUpdate.Add(chestDetailObjUpdate);
                        }

                        if (package.Quantity == 0)
                        {
                            chest.Quantity -= 1;
                            chest.ChestState = EnumChestState.InFabStore;
                            chest.Editor = p.Operator;
                            chest.EditTime = now;
                        }                      
                    }

                    #region 5.1.1.记录托号投料出柜日志
                    ChestLogKey chestKey = new ChestLogKey()
                    {
                        ChestNo = chest.Key,
                        PackageNo = package.Key,
                        CreateTime = now,
                        ChestActivity = EnumChestActivity.InFabStore
                    };
                    chestLog = new ChestLog()
                    {
                        Key = chestKey,
                        Creator = p.Operator,
                        ModelType = 0
                    };
                    #endregion
                }

                #endregion

                #endregion
                
                #region 6.事物处理
                ISession session = this.LotDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();

                try
                {
                    //6.1更新批次基本信息
                    foreach (Lot lot in lstLotDataEngineForUpdate)
                    {
                        this.LotDataEngine.Update(lot, session);
                    }

                    //6.2更新批次LotTransaction信息
                    foreach (LotTransaction lotTransaction in lstLotTransInsert)
                    {
                        this.LotTransactionDataEngine.Insert(lotTransaction, session);
                    }

                    //6.3更新批次TransactionHistory信息
                    foreach (LotTransactionHistory lotTransactionHistory in lstLotTransHisInsert)
                    {
                        this.LotTransactionHistoryDataEngine.Insert(lotTransactionHistory, session);
                    }

                    //6.4处理托明细
                    if (lstPackageDetailForDelete.Count == 0)
                    {
                        //删除全托明细
                        foreach (PackageDetail packageDetail in lstPackageDetail)
                        {
                            this.PackageDetailDataEngine.Delete(packageDetail.Key, session);
                            cfg = new PagingConfig()
                            {
                                Where = string.Format("Key.LotNumber='{0}'", packageDetail.Key.ObjectNumber),
                                IsPaging = false
                            };
                            IList<PackageCornerDetail> lstPackageCornerDetail = this.PackageCornerDetailDataEngine.Get(cfg);
                            if (lstPackageCornerDetail.Count > 0)
                            {
                                PackageCornerDetailKey key=lstPackageCornerDetail.FirstOrDefault().Key;
                                this.PackageCornerDetailDataEngine.Delete(key, session);
                            }
                        }
                    }
                    else
                    {
                        //删除批次号对应托明细
                        foreach (PackageDetail packageDetail in lstPackageDetailForDelete)
                        {
                            this.PackageDetailDataEngine.Delete(packageDetail.Key, session);
                            cfg = new PagingConfig()
                            {
                                Where = string.Format("Key.LotNumber='{0}'", packageDetail.Key.ObjectNumber),
                                IsPaging = false
                            };
                            IList<PackageCornerDetail> lstPackageCornerDetail = this.PackageCornerDetailDataEngine.Get(cfg);
                            if (lstPackageCornerDetail.Count > 0)
                            {
                                PackageCornerDetailKey key = lstPackageCornerDetail.FirstOrDefault().Key;
                                this.PackageCornerDetailDataEngine.Delete(key, session);
                            }
                        }
                    }

                    //6.5处理托信息
                    if (p.RetainPackageNo)
                    {
                        package.ContainerNo = null;
                        //保留托号
                        PackageDataEngine.Update(package, session);                        
                    }
                    else
                    {
                        //删除托号
                        PackageDataEngine.Delete(package.Key, session);                       
                    }  

                    //6.6更新线边仓记录
                    LineStoreMaterialDetailDataEngine.Update(lsmd, session);

                    //6.7更新OEM组件信息
                    foreach (OemData oemdata in lstOemLotForUpdate)
                    {
                        this.OemDataEngine.Update(oemdata, session);
                    }

                    //6.8更新LotAttribute信息
                    foreach (LotAttribute lotAttribute in lstLotAttributeForDelete)
                    {
                        this.LotAttributeDataEngine.Delete(lotAttribute.Key, session);
                    }

                    //6.9删除柜明细信息
                    if (lstChestDetailForDelete.Count > 0)
                    {                        
                        foreach (ChestDetail chestDetail in lstChestDetailForDelete)
                        {
                            this.ChestDetailDataEngine.Delete(chestDetail.Key, session);
                        }
                        //6.10更新柜信息
                        ChestDataEngine.Update(chest, session);
                    }   
                
                    //6.10更新柜明细信息
                    if (lstChestDetailForUpdate.Count > 0)
                    {
                        foreach (ChestDetail chestDetail in lstChestDetailForUpdate)
                        {
                            this.ChestDetailDataEngine.Update(chestDetail, session);
                        }
                    }

                    //6.11新增托号投料出柜日志
                    if (chestLog != null)
                    {                        
                        this.ChestLogDataEngine.Insert(chestLog, session);
                    }                   

                    //提交事务
                    transaction.Commit();
                    session.Close();

                    //返回投料信息
                    result.Message = string.Format("投入工单：[{0}]，投入工步：[{1}]，工艺流程：[{2}]",
                                                   workorder.Key,
                                                   routeStep.Key.RouteStepName,
                                                   routeStep.Key.RouteName);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    session.Close();

                    result.Code = 2000;
                    result.Message = string.Format(StringResource.Error, ex.Message);
                }
                #endregion
            }
            catch (Exception ex)
            {                
                result.Code = 2000;
                result.Message = string.Format(StringResource.Error, ex.Message);
            }

            return result;            
        }

        /// <summary>
        /// 提取托号相关数据
        /// </summary>
        /// <param name="p">
        /// 1.提取（WIP_PACKAGE）表到当前库{p.ReType = 1,p.IsDelete = 0}
        /// 2.提取其他归档表数据到当前库，并删除从归档库{p.ReType = 2,p.IsDelete = 1}
        /// </param>
        /// <returns></returns>
        public MethodReturnResult GetREbackdata(REbackdataParameter p)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                if (!string.IsNullOrEmpty(p.PackageNo))
                {                    
                    using (DbConnection con = this._db.CreateConnection())
                    {
                        DbCommand cmd = con.CreateCommand();
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.CommandText = "sp_BK_ReBackData";

                        this._db.AddInParameter(cmd, "PackageNo", DbType.String, p.PackageNo);
                        this._db.AddInParameter(cmd, "ReType", DbType.Int32, p.ReType);
                        this._db.AddInParameter(cmd, "IsDelete", DbType.Int32, p.IsDelete);
                        cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));
                        cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

                        SqlParameter parReturn = new SqlParameter("@return", SqlDbType.Int);
                        parReturn.Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add(parReturn);

                        this._db.ExecuteNonQuery(cmd);

                        int i = (int)cmd.Parameters["@return"].Value;

                        if (i == -1)
                        {
                            strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();

                            result.Code = 2000;
                            result.Message = strErrorMessage;
                            result.Detail = strErrorMessage;
                        }
                    }                    
                }
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
