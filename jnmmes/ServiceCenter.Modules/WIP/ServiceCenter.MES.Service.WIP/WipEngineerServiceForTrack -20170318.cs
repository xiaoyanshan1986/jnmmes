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
using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.EMS;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.BaseData;
using System.Data;
using System.Data.Common;

namespace ServiceCenter.MES.Service.WIP
{
    public partial class WipEngineerService
    {
        /// <summary> 批次进站操作 </summary>
        /// <param name="p">批次进站参数</param>
        /// <returns></returns>
        public MethodReturnResult TrackInLot(TrackInParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();
            ISession session;

            try
            {
                //创建事物Session
                session = this.SessionFactory.OpenSession();

                //调用批次进站操作
                result = TrackInLot(p, session, true);

                if (result.Code > 0)
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLogError("TrackInLot>", ex);
                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }

            return result;
        }

        /// <summary> 批次进站操作前检查及进站 </summary>
        /// <param name="p">批次进站参数</param>
        /// <param name="session">连接事物</param>
        /// <param name="executedWithTransaction">事物是否提交 TRUE - 在方法内提交 FALSE - 方法内不提交事物</param>
        /// <returns></returns>
        public MethodReturnResult TrackInLot(TrackInParameter p, ISession session, bool executedWithTransaction)
        {
            MethodReturnResult result = new MethodReturnResult();
            MethodReturnResult resultLotLock = new MethodReturnResult();

            double dCancelMaxQuantity = 0;          //设备最大处理数量            
            Equipment equipment;                    //设备对象
            List<Lot> lstLot = new List<Lot>();     //批次对象列表
            bool isLotStateLock = false;            //批次状态锁定

            try
            {
                #region 基本属性判断

                //判断线别代码是否为空
                if (string.IsNullOrEmpty(p.LineCode))
                {
                    result.Code = 1001;
                    result.Message = string.Format("{0} {1}"
                                                    , "线别代码"
                                                    , StringResource.ParameterIsNull);
                    return result;
                }

                //判断工序名称是否为空
                if (string.IsNullOrEmpty(p.RouteOperationName))
                {
                    result.Code = 1001;
                    result.Message = string.Format("{0} {1}"
                                                    , "工序名称"
                                                    , StringResource.ParameterIsNull);
                    return result;
                }

                //判断批次号是否为空
                if (p.LotNumbers == null || p.LotNumbers.Count == 0)
                {
                    result.Code = 1001;
                    result.Message = string.Format("{0} {1}"
                                                    , "批次号"
                                                    , StringResource.ParameterIsNull);
                    return result;
                }

                //获取线别车间。
                string locationName = string.Empty;
                ProductionLine pl = this.ProductionLineDataEngine.Get(p.LineCode);
                if (pl != null)
                {
                    //根据线别所在区域，获取车间名称。
                    Location loc = this.LocationDataEngine.Get(pl.LocationName);
                    locationName = loc.ParentLocationName ?? string.Empty;
                }

                //循环处理批次列表。
                foreach (string lotNumber in p.LotNumbers)
                {
                    //取得批次信息。
                    Lot lot = this.LotDataEngine.Get(lotNumber);

                    //判定是否存在批次记录。
                    if (lot == null || lot.Status == EnumObjectStatus.Disabled)
                    {
                        result.Code = 1002;
                        result.Message = string.Format("批次（{0}）不存在。", lotNumber);
                        return result;
                    }

                    //批次在处理中。
                    if (lot.LotState == 1)
                    {
                        result.Code = 1003;
                        result.Message = string.Format("批次（{0}）正在处理锁定中！", lotNumber);
                        return result;
                    }

                    //批次已经完成。
                    if (lot.StateFlag == EnumLotState.Finished)
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

                    //批次目前非等待进站状态。
                    if (lot.StateFlag != EnumLotState.WaitTrackIn)
                    {
                        result.Code = 1006;
                        result.Message = string.Format("批次（{0}）目前状态（{1}）,非({2})状态。"
                                                                , lotNumber
                                                                , lot.StateFlag.GetDisplayName()
                                                                , EnumLotState.WaitTrackIn.GetDisplayName());
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

                    //校验进站QTime
                    result = CheckQTime("IN", lot, p);

                    if (result.Code > 0)
                    {
                        return result;
                    }

                    //加入LOT数组列表
                    lstLot.Add(lot);
                }

                #endregion

                #region 校验进站条件
                result = TrackInBusinessCheck(p, lstLot);
                if (result.Code > 0)
                {
                    return result;
                }
                #endregion

                #region 判断设备状态
                if (string.IsNullOrEmpty(p.EquipmentCode) == false)
                {
                    //获取设备数据
                    equipment = this.EquipmentDataEngine.Get(p.EquipmentCode ?? string.Empty);
                    if (equipment != null)
                    {
                        //如果设备状态不是“待产” 和 “运行”
                        if (equipment.StateName.ToUpper() != EnumEquipmentStateType.Lost.ToString().ToUpper()
                            && equipment.StateName.ToUpper() != EnumEquipmentStateType.Run.ToString().ToUpper()
                            && equipment.StateName.ToUpper() != EnumEquipmentStateType.Test.ToString().ToUpper())
                        {
                            result.Code = 1200;
                            result.Message = String.Format("设备（{0}）状态（{1}）不可用。", equipment.Key, equipment.StateName);
                            return result;
                        }

                        //取得设备最大加工数量(0 - 默认不进行数量限制)
                        if (equipment.MaxQuantity == null)
                        {
                            dCancelMaxQuantity = 0;
                        }
                        else
                        {
                            dCancelMaxQuantity = Convert.ToDouble(equipment.MaxQuantity.ToString());
                        }

                        //加工数量大于0时需要进行加工数量控制
                        if (dCancelMaxQuantity > 0)
                        {
                            if (dCancelMaxQuantity == 1)
                            {
                                if (equipment.StateName.ToUpper() == EnumEquipmentStateType.Run.ToString().ToUpper())
                                {
                                    result.Code = 1500;
                                    result.Message = String.Format("超过设备（{0}）允许加工数量（{1}）！", equipment.Key, dCancelMaxQuantity);

                                    return result;
                                }
                            }
                            else
                            {
                                //根据设备编码获取当前加工批次数据 
                                PagingConfig cfg = new PagingConfig()
                                {
                                    PageSize = 0,
                                    PageNo = 0,
                                    IsPaging = true,
                                    Where = string.Format("EquipmentCode='{0}' AND State={1} "
                                                            , p.EquipmentCode
                                                            , Convert.ToInt32(EnumLotTransactionEquipmentState.Start)
                                                            )
                                };

                                //取得批次加工清单
                                IList<LotTransactionEquipment> lst = this.LotTransactionEquipmentDataEngine.Get(cfg);

                                //判断是否超出加工能力
                                if ((cfg.Records + p.LotNumbers.Count) > dCancelMaxQuantity)
                                {
                                    result.Code = 1600;
                                    result.Message = String.Format("超过设备（{0}）允许加工数量（{1}）！", equipment.Key, dCancelMaxQuantity);

                                    return result;
                                }
                            }
                        }
                    }
                }
                else
                {
                    result.Code = 1700;
                    result.Message = String.Format("设备代码为空！");

                    return result;
                }
                #endregion

                #region 检验并锁定批次
                result = SetLotStateForLock(lstLot, true);
                if (result.Code > 0)
                {
                    return result;
                }

                isLotStateLock = true;
                #endregion

                //进站处理
                result = ExecuteTrackInLot("C", p, equipment, lstLot, session, executedWithTransaction);

                if (result.Code > 0)
                {
                    //取消批次锁定
                    if (isLotStateLock)
                    {
                        resultLotLock = SetLotStateForLock(lstLot, false);

                        if (resultLotLock.Code != 0)
                        {
                            result.Message = result.Message + resultLotLock.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();

                //解锁批次
                if (isLotStateLock)
                {
                    resultLotLock = SetLotStateForLock(lstLot, false);

                    if (resultLotLock.Code != 0)
                    {
                        result.Message = result.Message + resultLotLock.Message;
                    }
                }
            }

            return result;
        }

        /// <summary> 批次进站操作及进站 </summary>
        /// <param name="workModel">执行类型 C - 标准过站 A - 自动过站操作</param>
        /// <param name="p">批次进站参数</param>
        /// <param name="equipment">设备</param>
        /// <param name="lstLot">批次列表</param>
        /// <param name="session">事物连接</param>
        /// <param name="executedWithTransaction">事物是否提交 TRUE - 在方法内提交 FALSE - 方法内不提交事物</param>
        /// <returns></returns>
        public MethodReturnResult ExecuteTrackInLot(string workModel, TrackInParameter p, Equipment equipment, List<Lot> lstLot, ISession session, bool executedWithTransaction)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            MethodReturnResult resultequipment = new MethodReturnResult();

            DateTime now = DateTime.Now.AddMilliseconds(-10);                //当前时间在当前时间减10毫秒

            #region 定义相应的事物数组
            List<LotTransactionEquipment> lstLotTransactionEquipmentDataEngineForInsert = new List<LotTransactionEquipment>();
            List<LotTransaction> lstLotTransactionForInsert = new List<LotTransaction>();
            List<LotTransactionHistory> lstLotTransactionHistoryForInsert = new List<LotTransactionHistory>();
            List<LotTransactionParameter> lstLotTransParamForInsert = new List<LotTransactionParameter>();

            List<IVTestData> lstIVTestDataForUpdate = new List<IVTestData>();
            #endregion

            Equipment equipmentBack = new Equipment();                                      //设备开始状态保存对象，用于失败回退
            bool isAutoTrackOut = false;                                                    //是否自动出站
            bool isEquipmentChangeState = false;                                            //是否改变设备状态
            ITransaction transaction = null;

            try
            {
                //取得事物主键
                string transactionKey = Guid.NewGuid().ToString();

                #region 锁定设备状态
                //当自动进站时无设备,不进行设备判断
                if (workModel == "C")
                {
                    //由其它状态切换为RUN状态
                    if (equipment.StateName.ToUpper() != EnumEquipmentStateType.Run.ToString().ToUpper())
                    {
                        //复制并保存当前设备状态，用于失败是回滚
                        equipmentBack = equipment.Clone() as Equipment;

                        //修改设备属性
                        equipment.StateName = EnumEquipmentStateType.Run.ToString();        //需改状态为运行状态                    
                        equipment.EditTime = now;                                           //编辑时间
                        equipment.Editor = p.Creator;                                       //编辑人

                        //处理并发问题，在入站前首先改变设备状态
                        resultequipment = SetEquipmentState(equipment);

                        if (resultequipment.Code > 0)
                        {
                            resultequipment.Message = "设备状态设置失败！错误信息：" + resultequipment.Message;
                            result.Message += resultequipment.Message;

                            return result;
                        }

                        isEquipmentChangeState = true;

                        #region 设置设置加工状态

                        #endregion
                    }
                }
                #endregion

                //创建事物对象            
                if (executedWithTransaction == true)
                {
                    transaction = session.BeginTransaction();
                }

                #region 处理批次列表
                foreach (Lot lot in lstLot)
                {
                    #region 批次数据处理
                    //设置锁定状态
                    lot.LotState = 0;

                    //1.记录批次历史状态
                    LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lot);

                    lstLotTransactionHistoryForInsert.Add(lotHistory);

                    //2.更新批次记录
                    lot.StartProcessTime = now;                  //开始处理时间
                    lot.EquipmentCode = p.EquipmentCode;         //设备代码
                    lot.LineCode = p.LineCode;                   //线别代码
                    lot.OperateComputer = p.OperateComputer;     //操作电脑
                    lot.PreLineCode = lot.LineCode;              //原线别代码
                    lot.StateFlag = EnumLotState.WaitTrackOut;   //批次状态
                    lot.Editor = p.Creator;                      //编辑人
                    lot.EditTime = now;                          //编辑时间

                    //3.记录批次设备加工历史数据                
                    LotTransactionEquipment transEquipment = new LotTransactionEquipment()
                    {
                        Key = transactionKey,                     //事物主键
                        EndTransactionKey = null,                 //结束事物主键
                        CreateTime = now,                         //创建时间
                        Creator = p.Creator,                      //创建人
                        Editor = p.Creator,                       //编辑人
                        EditTime = now,                           //编辑时间
                        EndTime = null,                           //结束时间
                        EquipmentCode = p.EquipmentCode,          //设备代码
                        LotNumber = lot.Key,                      //组件批次号
                        Quantity = lot.Quantity,                  //数量
                        StartTime = now,                          //开始时间
                        State = EnumLotTransactionEquipmentState.Start //事务状态
                    };

                    lstLotTransactionEquipmentDataEngineForInsert.Add(transEquipment);

                    //4.记录操作事务数据
                    LotTransaction transObj = new LotTransaction()
                    {
                        Key = transactionKey,                               //事物主键     
                        Activity = EnumLotActivity.TrackIn,                 //批次状态  
                        CreateTime = now,                                   //创建时间
                        Creator = p.Creator,                                //创建人
                        Description = p.Remark,                             //描述
                        Editor = p.Creator,                                 //编辑人
                        EditTime = now,                                     //编辑时间
                        InQuantity = lot.Quantity,                          //数量
                        LotNumber = lot.Key,                                //组件批次号
                        LocationName = lot.LocationName,                    //车间
                        LineCode = lot.LineCode,                            //线别
                        OperateComputer = p.OperateComputer,                //操作电脑
                        OrderNumber = lot.OrderNumber,                      //工单
                        OutQuantity = lot.Quantity,                         //出站数量
                        RouteEnterpriseName = lot.RouteEnterpriseName,      //工艺流程组
                        RouteName = lot.RouteName,                          //工艺流程
                        RouteStepName = lot.RouteStepName,                  //工序名称
                        ShiftName = p.ShiftName,                            //班别
                        UndoFlag = false,                                   //撤销标识
                        UndoTransactionKey = null,                          //撤销主键
                        Grade = lot.Grade,                                  //等级
                        Color = lot.Color,                                  //花色
                        Attr1 = lot.Attr1,                                  //批次属性1
                        Attr2 = lot.Attr2,                                  //批次属性2
                        Attr3 = lot.Attr3,                                  //批次属性3
                        Attr4 = lot.Attr4,                                  //批次属性4
                        Attr5 = lot.Attr5,                                  //批次属性5
                        OriginalOrderNumber = lot.OriginalOrderNumber       //原始工单
                    };

                    lstLotTransactionForInsert.Add(transObj);
                    #endregion

                    #region 设置IV数据无效
                    bool isAllowIVTestData = false;

                    //获取设置IV数据无效属性
                    RouteStepAttribute rsa = this.RouteStepAttributeDataEngine.Get(new RouteStepAttributeKey()
                    {
                        RouteName = lot.RouteName,
                        RouteStepName = lot.RouteStepName,
                        AttributeName = "IsAllowIVTestData"
                    });

                    if (rsa != null)
                    {
                        bool.TryParse(rsa.Value, out isAllowIVTestData);
                    }

                    //设置IV数据无效
                    if (isAllowIVTestData == true)
                    {
                        //判断IV测试数据是否存在。
                        PagingConfig cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            IsPaging = false,
                            Where = string.Format("Key.LotNumber='{0}' AND IsDefault=1", lot.Key)
                        };

                        IList<IVTestData> lstTestData = this.IVTestDataDataEngine.Get(cfg);

                        foreach (IVTestData ivTestData in lstTestData)
                        {
                            //IVTestData testData = ivTestData.Clone() as IVTestData;
                            //testData.IsDefault = false;
                            //testData.Editor = p.Creator;
                            //testData.EditTime = DateTime.Now;
                            //lstIVTestDataForUpdate.Add(testData);

                            ivTestData.IsDefault = false;               //默认标识
                            ivTestData.Editor = p.Creator;              //编辑人
                            ivTestData.EditTime = now;                  //编辑时间

                            lstIVTestDataForUpdate.Add(ivTestData);
                        }
                    }

                    #endregion

                    #region 处理附加参数记录
                    if (p.Paramters != null && p.Paramters.ContainsKey(lot.Key))
                    {
                        foreach (TransactionParameter tp in p.Paramters[lot.Key])
                        {
                            //判断批次属性是否存在

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

                            lstLotTransParamForInsert.Add(lotParamObj);
                        }
                    }
                    #endregion

                    #region 判断是否自动出站
                    //自动进站不能自动出站判断
                    if (workModel == "C")
                    {
                        //获取工步属性设置是否自动出站。
                        rsa = this.RouteStepAttributeDataEngine.Get(new RouteStepAttributeKey()
                        {
                            RouteName = lot.RouteName,                  //批次所处工艺流程
                            RouteStepName = p.RouteOperationName,            //设备工序
                            AttributeName = "IsAutoTrackOut"            //自动出站属性名称
                        }, session);

                        if (rsa != null)
                        {
                            bool.TryParse(rsa.Value, out isAutoTrackOut);
                        }
                        else
                        {
                            isAutoTrackOut = false;
                        }

                        //自动出站处理
                        if (isAutoTrackOut == true)
                        {
                            //初始化参数
                            TrackOutParameter trackOutParameters = new TrackOutParameter();

                            trackOutParameters.IsFinished = true;                               //工序已结束
                            trackOutParameters.RouteName = p.RouteName;                         //工艺流程
                            trackOutParameters.RouteOperationName = p.RouteOperationName;       //工步名称
                            trackOutParameters.LineCode = lot.LineCode;                         //线别代码
                            trackOutParameters.EquipmentCode = p.EquipmentCode;                 //设备代码
                            trackOutParameters.LotNumbers = p.LotNumbers;                       //批次号
                            trackOutParameters.LineCode = p.LineCode;                           //线别代码
                            trackOutParameters.Color = "";                                      //颜色                   
                            trackOutParameters.Grade = "";                                      //等级
                            trackOutParameters.ScrapReasonCodes = null;                         //报废原因代码
                            trackOutParameters.DefectReasonCodes = null;                        //不良原因代码
                            trackOutParameters.CheckBarcodes = null;                            //检验条码
                            trackOutParameters.Creator = p.Creator;                             //创建人
                            trackOutParameters.OperateComputer = p.OperateComputer;             //操纵客户端
                            trackOutParameters.Paramters = p.Paramters;                         //参数清单（待议！！！）
                            trackOutParameters.MaterialParamters = p.MaterialParamters;         //物料清单

                            //调用出站操作
                            result = AutoTrackOutLot(trackOutParameters, equipment, lstLot, session);

                            if (result.Code > 0)
                            {
                                #region 设备状态回滚处理
                                if (isEquipmentChangeState == true)
                                {
                                    resultequipment = SetEquipmentState(equipmentBack);

                                    if (resultequipment.Code > 0)
                                    {
                                        resultequipment.Message = "设备状态回滚失败！错误信息：" + resultequipment.Message;
                                        result.Message += resultequipment.Message;

                                        return result;
                                    }
                                }
                                #endregion

                                return result;
                            }

                            //取得返回事物主键
                            p.TransactionKeys = trackOutParameters.TransactionKeys;

                            //事物完成时间
                            p.TransENDTime = trackOutParameters.TransENDTime;

                            //p.TransactionKeys = new Dictionary<string, string>();
                            //foreach (Lot plot in lstLot)
                            //{
                            //    p.TransactionKeys.Add(plot.Key, trackOutParameters.TransactionKeys[plot.Key]);
                            //}
                        }
                    }
                    #endregion
                }
                #endregion

                #region 开始事物处理

                #region 更新批次LOT 的信息
                if (workModel == "C")
                {
                    //更新批次基本信息
                    foreach (Lot lot in lstLot)
                    {
                        this.LotDataEngine.Update(lot, session);
                    }
                }

                //更新批次LotTransaction信息
                foreach (LotTransaction lotTransaction in lstLotTransactionForInsert)
                {
                    this.LotTransactionDataEngine.Insert(lotTransaction, session);
                }

                //更新批次TransactionHistory信息
                foreach (LotTransactionHistory lotTransactionHistory in lstLotTransactionHistoryForInsert)
                {
                    this.LotTransactionHistoryDataEngine.Insert(lotTransactionHistory, session);
                }

                //新增批次属性
                foreach (LotTransactionParameter lotTransParam in lstLotTransParamForInsert)
                {
                    this.LotTransactionParameterDataEngine.Update(lotTransParam, session);
                }

                #endregion

                #region 更新设备基本信息（更新2017年3月9日19:07:25）
                if (workModel == "C")
                {
                    if (isEquipmentChangeState == true)
                    {
                        //自动出站时取得出站的事物主键
                        string sNexttransactionKey = "";
                        DateTime EndTransTime = now;

                        if (p.TransactionKeys != null && p.TransactionKeys.Count > 0)
                        {
                            sNexttransactionKey = p.TransactionKeys[lstLot[0].Key];

                            //使用设备编辑时间传递事务完成时间
                            EndTransTime = p.TransENDTime;
                        }

                        //新增设备状态事件数据
                        result = EquipmentChangeState(equipment, equipmentBack.StateName, session, transactionKey, sNexttransactionKey, true, EndTransTime);

                        //Insert设备Transaction信息
                        foreach (LotTransactionEquipment lotTransactionEquipment in lstLotTransactionEquipmentDataEngineForInsert)
                        {
                            this.LotTransactionEquipmentDataEngine.Insert(lotTransactionEquipment, session);
                        }
                    }
                }                
                #endregion

                #region 更新IV数据，IsDefault=0
                foreach (IVTestData ivTestData in lstIVTestDataForUpdate)
                {
                    this.IVTestDataDataEngine.Update(ivTestData, session);
                }
                #endregion

                if (executedWithTransaction == true)
                {
                    transaction.Commit();
                    //transaction.Rollback();
                    session.Close();
                }

                #endregion

                //返回自动进站参数
                if (workModel == "A")
                {
                    //取得返回事物主键
                    p.TransactionKeys = new Dictionary<string, string>();
                    foreach (Lot plot in lstLot)
                    {
                        p.TransactionKeys.Add(plot.Key, transactionKey);
                    }

                    //事物结束时间
                    p.TransENDTime = now;
                }
            }
            catch (Exception err)
            {
                if (executedWithTransaction == true)
                {
                    transaction.Rollback();

                    #region 设备状态回滚处理
                    if (isEquipmentChangeState == true)
                    {
                        resultequipment = SetEquipmentState(equipmentBack);

                        if (resultequipment.Code > 0)
                        {
                            resultequipment.Message = "设备状态回滚失败！错误信息：" + resultequipment.Message;
                            result.Message += resultequipment.Message;

                            return result;
                        }

                    }
                    #endregion

                    session.Close();
                }

                result.Code = 1000;
                result.Message += string.Format(StringResource.Error, err.Message);
                result.Detail = err.ToString();
                return result;
            }

            return result;
        }

        /// <summary> 进站业务数据检验 </summary>
        /// <param name="p">批次出站参数</param>
        /// <param name="lstLot">批次列表</param>        
        /// <returns></returns>
        public MethodReturnResult TrackInBusinessCheck(TrackInParameter p, List<Lot> lstLot)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            try
            {
                RouteStepAttribute rs;          //工步属性对象

                //进站业务信息校验
                foreach (Lot lot in lstLot)
                {
                    #region 1. 校验安规测试数据
                    bool isCheck = false;

                    rs = this.RouteStepAttributeDataEngine.Get(new RouteStepAttributeKey()
                    {
                        RouteName = p.RouteName,
                        RouteStepName = p.RouteOperationName,
                        AttributeName = "IsCheckVIRTestData"
                    });

                    if (rs != null
                        && bool.TryParse(rs.Value, out isCheck)
                        && isCheck)
                    {
                        isCheck = true;
                    }
                    else
                    {
                        isCheck = false;
                    }

                    //需要检查IV测试数据。
                    if (isCheck == true)
                    {
                        #region 1.1 校验安规测试数据
                        //取得最新三次安规测试数据（同时三个类型）
                        PagingConfig cfg = new PagingConfig()
                        {
                            Where = string.Format("Key.LotNumber='{0}'", lot.Key),
                            OrderBy = "Key.TestTime Desc",
                            IsPaging = true,
                            PageSize = 3,
                            PageNo = 0
                        };

                        IList<VIRTestData> lstVIRTestData = this.VIRTestDataDataEngine.Get(cfg);

                        //判断安规测试记录是否存在
                        if (lstVIRTestData.Count == 0)
                        {
                            result.Code = 2000;
                            result.Message = string.Format("批次（{0}）安规测试数据不存在！", lot.Key);
                            return result;
                        }

                        //判断是否有多条默认值
                        if (lstVIRTestData.Count < 3)
                        {
                            result.Code = 2100;
                            result.Message = string.Format("批次（{0}）安规测试数据不完整，请重测！", lot.Key);
                            return result;
                        }

                        //判断安规测试是否通过
                        string[] lstTestType = new string[3] { "GND", "IR", "DCW" };    //三个测试类型
                        bool isPass = false;                                            //是否通过

                        //判断类型是否通过
                        for (int i = 0; i < lstTestType.Length; i++)
                        {
                            isPass = false;

                            foreach (VIRTestData virTestData in lstVIRTestData)
                            {
                                if (virTestData.Key.TestType.Trim() == lstTestType[i] && virTestData.TestResult.Trim() == "PASS")
                                {
                                    isPass = true;

                                    break;
                                }
                            }

                            if (isPass == false)
                            {
                                result.Code = 2100;
                                result.Message = string.Format("批次（{0}）安规测试" + lstTestType[i] + "数据未通过，请重测！", lot.Key);
                                return result;
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLogError("TrackInBusinessCheck>", ex);

                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }

            return result;
        }




        /// <summary> 批次出站操作 </summary>
        /// <param name="p">批次出站参数</param>
        /// <returns></returns>
        public MethodReturnResult TrackOutLot(TrackOutParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();
            ISession session;

            try
            {
                session = this.SessionFactory.OpenSession();

                result = TrackOutLot(p, session, true);

                if (result.Code > 0)
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLogError("TrackOutLot>", ex);
                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }

            return result;
        }

        /// <summary> 出站接口方法 </summary>
        /// <param name="p">批次出站参数</param>
        /// <param name="equipment">设备</param>
        /// <param name="lstLot">批次列表</param>
        /// <param name="session">事物连接</param>
        /// <returns></returns>
        public MethodReturnResult TrackOutLot(TrackOutParameter p, ISession session, bool executedWithTransaction)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            MethodReturnResult resultLotLock = new MethodReturnResult();

            List<LotTransaction> lstLotTransactionForInsert = new List<LotTransaction>();                           //作业事物对象列表
            List<LotTransactionHistory> lstLotTransactionHistoryForInsert = new List<LotTransactionHistory>();      //批次加工历史事物对象列表

            DateTime now = DateTime.Now.AddMilliseconds(1); //出站时在原有时间上增加一毫秒
            Equipment equipment = null;                     //设备对象
            List<Lot> lstLot = new List<Lot>();             //批次对象列表
            ITransaction transaction = null;                //事物对象
            bool isLotStateLock = false;            //批次状态锁定

            RouteStepAttribute rsa = null;

            try
            {
                #region 创建批次列表
                foreach (string lotNumber in p.LotNumbers)
                {
                    //取得批次信息。
                    Lot lot = this.LotDataEngine.Get(lotNumber);

                    //判定是否存在批次记录。
                    if (lot == null || lot.Status == EnumObjectStatus.Disabled)
                    {
                        result.Code = 1002;
                        result.Message = string.Format("批次（{0}）不存在。", lotNumber);
                        return result;
                    }

                    //当非自动出站时判断批次是否锁定
                    if (executedWithTransaction)
                    {
                        if (lot.LotState == 1)
                        {
                            result.Code = 1003;
                            result.Message = string.Format("批次（{0}）正在处理锁定中！", lotNumber);

                            return result;
                        }
                    }

                    //加入LOT数组列表
                    lstLot.Add(lot);
                }
                #endregion

                #region 创建设备对象
                if (string.IsNullOrEmpty(p.EquipmentCode) == false)
                {
                    //获取设备数据
                    equipment = this.EquipmentDataEngine.Get(p.EquipmentCode ?? string.Empty);
                    if (equipment == null)
                    {
                        result.Code = 1700;
                        result.Message = String.Format("设备（{0}）信息读取失败！", p.EquipmentCode);
                        return result;
                    }
                }
                else
                {
                    result.Code = 1700;
                    result.Message = String.Format("设备代码为空。");
                    return result;
                }
                #endregion

                #region 当非自动出站时，锁定批次
                if (executedWithTransaction)
                {
                    result = SetLotStateForLock(lstLot, true);
                    if (result.Code > 0)
                    {
                        return result;
                    }

                    isLotStateLock = true;
                }
                #endregion

                #region 校验出站条件
                //1.检验基础出站条件
                result = TrackOutBaseCheck("C", p, equipment, lstLot);
                if (result.Code > 0)
                {
                    //取消批次锁定
                    if (isLotStateLock)
                    {
                        resultLotLock = SetLotStateForLock(lstLot, false);

                        if (resultLotLock.Code > 0)
                        {
                            result.Message = result.Message + resultLotLock.Message;
                        }
                    }

                    return result;
                }

                //2.检验业务出站条件
                result = TrackOutBusinessCheck("C", p, lstLot);
                if (result.Code > 0)
                {
                    //取消批次锁定
                    if (isLotStateLock)
                    {
                        resultLotLock = SetLotStateForLock(lstLot, false);

                        if (resultLotLock.Code > 0)
                        {
                            result.Message = result.Message + resultLotLock.Message;
                        }
                    }

                    return result;
                }
                #endregion

                #region 事物处理
                //取得事物主键
                string transactionKey = Guid.NewGuid().ToString();

                //开始事物对象    
                if (executedWithTransaction == true)
                {
                    transaction = session.BeginTransaction();
                }

                //标准逻辑处理
                result = ExecuteTrackOutBaseFounction("C", p, equipment, lstLot, session, transactionKey);
                if (result.Code > 0)
                {
                    //取消批次锁定
                    if (isLotStateLock)
                    {
                        resultLotLock = SetLotStateForLock(lstLot, false);

                        if (resultLotLock.Code > 0)
                        {
                            result.Message = result.Message + resultLotLock.Message;
                        }
                    }

                    return result;
                }

                //业务逻辑处理
                result = ExecuteTrackOutBusinessFounction("C", p, equipment, lstLot, session, transactionKey);
                if (result.Code > 0)
                {
                    //取消批次锁定
                    if (isLotStateLock)
                    {
                        resultLotLock = SetLotStateForLock(lstLot, false);

                        if (resultLotLock.Code > 0)
                        {
                            result.Message = result.Message + resultLotLock.Message;
                        }
                    }

                    return result;
                }

                foreach (Lot lot in lstLot)
                {
                    //取消批次锁定状态
                    lot.LotState = 0;

                    #region 1.记录批次历史信息
                    LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lot);

                    lstLotTransactionHistoryForInsert.Add(lotHistory);
                    #endregion

                    #region 2.批次属性设置
                    if (p.Remark != null && p.Remark.Length > 0)
                    {
                        lot.Description = p.Remark;                 //备注
                    }

                    if (p.Grade != null && p.Grade.Length > 0)
                    {
                        lot.Grade = p.Grade;                        //批次等级
                    }

                    if (p.Color != null && p.Color.Length > 0)
                    {
                        lot.Color = p.Color;                        //批次花色
                    }
                    #endregion

                    lot.Editor = p.Operator;                        //编辑人
                    lot.EditTime = now;                             //编辑日期

                    #region 3.批次加工事务对象
                    EnumLotActivity lotActivity = EnumLotActivity.TrackOut;

                    //当批次暂停时加工历史状态设置为暂停
                    if (lot.HoldFlag == true)
                    {
                        lotActivity = EnumLotActivity.Hold;
                    }

                    //记录操作历史数据
                    LotTransaction transObj = new LotTransaction()
                    {
                        Key = transactionKey,                               //事物主键     
                        Activity = lotActivity,                             //批次状态                        
                        Description = p.Remark,                             //描述                        
                        InQuantity = lot.Quantity,                          //操作前数量
                        OutQuantity = lot.Quantity,                         //操作后数量
                        LotNumber = lot.Key,                                //组件批次号
                        LocationName = lot.LocationName,                    //车间
                        LineCode = lot.LineCode,                            //线别
                        OperateComputer = p.OperateComputer,                //操作电脑
                        OrderNumber = lot.OrderNumber,                      //工单                        
                        RouteEnterpriseName = lot.RouteEnterpriseName,      //工艺流程组
                        RouteName = lot.RouteName,                          //工艺流程
                        RouteStepName = p.RouteOperationName,               //工序名称
                        ShiftName = p.ShiftName,                            //班别
                        UndoFlag = false,                                   //撤销标识
                        UndoTransactionKey = null,                          //撤销主键
                        Grade = lot.Grade,                                  //等级
                        Color = lot.Color,                                  //花色
                        Attr1 = lot.Attr1,                                  //批次属性1
                        Attr2 = lot.Attr2,                                  //批次属性2
                        Attr3 = lot.Attr3,                                  //批次属性3
                        Attr4 = lot.Attr4,                                  //批次属性4
                        Attr5 = lot.Attr5,                                  //批次属性5
                        OriginalOrderNumber = lot.OriginalOrderNumber,      //原始工单
                        CreateTime = now,                                   //创建时间
                        Creator = p.Creator,                                //创建人
                        Editor = p.Creator,                                 //编辑人
                        EditTime = now                                      //编辑时间
                    };

                    lstLotTransactionForInsert.Add(transObj);

                    //LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lot);

                    //lotHistory.RouteStepName = p.RouteOperationName;       //工序名称
                    //lotHistory.StateFlag = EnumLotState.WaitTrackIn;       //进站状态

                    //lstLotTransactionHistoryForInsert.Add(lotHistory);
                    #endregion
                }

                //新增加工信息
                foreach (LotTransaction lotTransaction in lstLotTransactionForInsert)
                {
                    this.LotTransactionDataEngine.Insert(lotTransaction, session);
                }

                //新增批次加工信息
                foreach (LotTransactionHistory lotTransactionHistory in lstLotTransactionHistoryForInsert)
                {
                    this.LotTransactionHistoryDataEngine.Insert(lotTransactionHistory, session);
                }

                #region 判断是否自动进站
                bool isAutoTrackIn = false;
                //Equipment equipmentBack = null;
                //bool isEquipmentChangeState = false;
                //MethodReturnResult resultequipment = new MethodReturnResult()
                //{
                //    Code = 0
                //};

                foreach (Lot lot in lstLot)
                {
                    //获取工步属性设置是否自动进站。
                    rsa = this.RouteStepAttributeDataEngine.Get(new RouteStepAttributeKey()
                    {
                        RouteName = lot.RouteName,                  //批次下一个工艺流程
                        RouteStepName = lot.RouteStepName,          //设备下一个工序
                        AttributeName = "IsAutoTrackIn"             //自动进站属性名称
                    }, session);

                    if (rsa != null)
                    {
                        bool.TryParse(rsa.Value, out isAutoTrackIn);
                    }
                    else
                    {
                        isAutoTrackIn = false;
                    }

                    //自动进站处理？？？
                    if (isAutoTrackIn == true)
                    {
                        //初始化参数
                        TrackInParameter trackInParameters = new TrackInParameter();

                        trackInParameters.RouteName = lot.RouteName;                    //工艺流程
                        trackInParameters.RouteOperationName = lot.RouteStepName;       //工步名称
                        trackInParameters.LineCode = lot.LineCode;                      //线别代码
                        trackInParameters.EquipmentCode = "";                           //设备代码(自动入站为空)
                        trackInParameters.LotNumbers = p.LotNumbers;                    //批次号                        
                        trackInParameters.Creator = p.Creator;                          //创建人
                        trackInParameters.OperateComputer = p.OperateComputer;          //操纵客户端
                        trackInParameters.MaterialParamters = p.MaterialParamters;      //物料清单

                        //调用进站操作
                        result = ExecuteTrackInLot("A", trackInParameters, equipment, lstLot, session, false);

                        //result = TrackInLot(trackInParameters, session, false);
                        //result = AutoTrackInLot(trackInParameters, equipment, lstLot, session,ref isEquipmentChangeState, ref equipmentBack);

                        if (result.Code > 0)
                        {
                            //#region 设备状态回滚处理
                            //if (isEquipmentChangeState == true)
                            //{
                            //    resultequipment = EquipmentChangeState(equipmentBack);

                            //    if (resultequipment.Code > 0)
                            //    {
                            //        resultequipment.Message = "设备状态回滚失败！错误信息：" + resultequipment.Message;
                            //        result.Message += resultequipment.Message;

                            //        return result;
                            //    }
                            //}
                            //#endregion

                            //取消批次锁定
                            if (isLotStateLock)
                            {
                                resultLotLock = SetLotStateForLock(lstLot, false);

                                if (resultLotLock.Code > 0)
                                {
                                    result.Message = result.Message + resultLotLock.Message;
                                }
                            }

                            return result;
                        }

                        //取得返回事物主键
                        p.TransactionKeys = trackInParameters.TransactionKeys;

                        //事物结束时间
                        p.TransENDTime = trackInParameters.TransENDTime;

                        //p.TransactionKeys = new Dictionary<string, string>();
                        //foreach (Lot plot in lstLot)
                        //{
                        //    p.TransactionKeys.Add(plot.Key, trackInParameters.TransactionKeys[plot.Key]);
                        //}
                    }
                }
                #endregion

                //批次信息
                foreach (Lot lot in lstLot)
                {
                    this.LotDataEngine.Update(lot, session);
                }

                //过站事物提交
                if (executedWithTransaction == true)
                {
                    transaction.Commit();
                    //transaction.Rollback();
                    session.Close();
                }
                #endregion
            }

            catch (Exception ex)
            {
                if (executedWithTransaction == true)
                {
                    transaction.Rollback();
                    session.Close();

                    //取消批次锁定
                    if (isLotStateLock)
                    {
                        resultLotLock = SetLotStateForLock(lstLot, false);

                        if (resultLotLock.Code > 0)
                        {
                            result.Message = result.Message + resultLotLock.Message;
                        }
                    }
                }

                LogHelper.WriteLogError("TrackOutLot>", ex);

                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }

            return result;
        }

        /// <summary> 自动出站接口方法 </summary>
        /// <param name="p">批次出站参数</param>
        /// <param name="equipment">设备</param>
        /// <param name="lstLot">批次列表</param>
        /// <param name="session">事物连接</param>
        /// <returns></returns>
        public MethodReturnResult AutoTrackOutLot(TrackOutParameter p, Equipment equipment, List<Lot> lstLot, ISession session)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            List<LotTransaction> lstLotTransactionForInsert = new List<LotTransaction>();                           //作业事物对象列表
            List<LotTransactionHistory> lstLotTransactionHistoryForInsert = new List<LotTransactionHistory>();      //批次加工历史事物对象列表
            List<Lot> lstOldLot = new List<Lot>();

            DateTime now = DateTime.Now.AddMilliseconds(20);
            string sRouteAttributeValue = "";
            bool bRouteAttributeValue = false;

            try
            {
                //复制设备对象，避免数据修改
                Equipment equipmentTrackOut = equipment.Clone() as Equipment;

                #region 校验出站条件
                //1.检验基础出站条件
                result = TrackOutBaseCheck("A", p, equipmentTrackOut, lstLot);
                if (result.Code > 0)
                {
                    return result;
                }

                //2.检验业务出站条件
                result = TrackOutBusinessCheck("A", p, lstLot);
                if (result.Code > 0)
                {
                    return result;
                }
                #endregion

                #region 取得出站属性
                //1.物料自动扣料属性
                sRouteAttributeValue = GetRouteStepAttribute(lstLot[0].RouteName, lstLot[0].RouteStepName, "isAutoDeductMaterial");

                if (sRouteAttributeValue != "")
                {
                    bool.TryParse(sRouteAttributeValue, out bRouteAttributeValue);
                }
                else
                {
                    bRouteAttributeValue = false;
                }

                p.AutoDeductMaterial = bRouteAttributeValue;        //自动扣料属性

                #endregion

                #region 事物处理
                //记录原批次状态
                foreach (Lot item in lstLot)
                {
                    lstOldLot.Add(item.Clone() as Lot);
                }

                //取得事物主键
                string transactionKey = Guid.NewGuid().ToString();

                //标准逻辑处理
                result = ExecuteTrackOutBaseFounction("A", p, equipmentTrackOut, lstLot, session, transactionKey);
                if (result.Code > 0)
                {
                    return result;
                }

                //业务逻辑处理
                result = ExecuteTrackOutBusinessFounction("A", p, equipmentTrackOut, lstLot, session, transactionKey);
                if (result.Code > 0)
                {
                    return result;
                }

                foreach (Lot lot in lstLot)
                {
                    #region 1.记录批次操作前状态
                    LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lstOldLot[lstLot.IndexOf(lot, 0)]);

                    lstLotTransactionHistoryForInsert.Add(lotHistory);
                    #endregion

                    #region 2.批次属性设置
                    if (p.Remark != null && p.Remark.Length > 0)
                    {
                        lot.Description = p.Remark;                 //备注
                    }

                    if (p.Grade != null && p.Grade.Length > 0)
                    {
                        lot.Grade = p.Grade;                        //批次等级
                    }

                    if (p.Color != null && p.Color.Length > 0)
                    {
                        lot.Color = p.Color;                        //批次花色
                    }
                    #endregion

                    #region 3.批次加工事务对象
                    //记录操作历史数据
                    LotTransaction transObj = new LotTransaction()
                    {
                        Key = transactionKey,                               //事物主键     
                        Activity = EnumLotActivity.TrackOut,                //批次状态                        
                        Description = p.Remark,                             //描述                        
                        InQuantity = lot.Quantity,                          //操作前数量
                        OutQuantity = lot.Quantity,                         //操作后数量
                        LotNumber = lot.Key,                                //组件批次号
                        LocationName = lot.LocationName,                    //车间
                        LineCode = lot.LineCode,                            //线别
                        OperateComputer = p.OperateComputer,                //操作电脑
                        OrderNumber = lot.OrderNumber,                      //工单                        
                        RouteEnterpriseName = lot.RouteEnterpriseName,      //工艺流程组
                        RouteName = lot.RouteName,                          //工艺流程
                        RouteStepName = p.RouteOperationName,               //工序名称
                        ShiftName = p.ShiftName,                            //班别
                        UndoFlag = false,                                   //撤销标识
                        UndoTransactionKey = null,                          //撤销主键
                        Grade = lot.Grade,                                  //等级
                        Color = lot.Color,                                  //花色
                        Attr1 = lot.Attr1,                                  //批次属性1
                        Attr2 = lot.Attr2,                                  //批次属性2
                        Attr3 = lot.Attr3,                                  //批次属性3
                        Attr4 = lot.Attr4,                                  //批次属性4
                        Attr5 = lot.Attr5,                                  //批次属性5
                        OriginalOrderNumber = lot.OriginalOrderNumber,      //原始工单
                        CreateTime = now,                                   //创建时间
                        Creator = p.Creator,                                //创建人
                        Editor = p.Creator,                                 //编辑人
                        EditTime = now                                      //编辑时间
                    };

                    lstLotTransactionForInsert.Add(transObj);
                    #endregion
                }

                //批次信息
                //foreach (Lot lot in lstLot)
                //{
                //    this.LotDataEngine.Update(lot, session);
                //}

                //新增加工信息
                foreach (LotTransaction lotTransaction in lstLotTransactionForInsert)
                {
                    this.LotTransactionDataEngine.Insert(lotTransaction, session);
                }

                //新增批次加工信息
                foreach (LotTransactionHistory lotTransactionHistory in lstLotTransactionHistoryForInsert)
                {
                    this.LotTransactionHistoryDataEngine.Insert(lotTransactionHistory, session);
                }
                #endregion

                //取得返回事物主键
                p.TransactionKeys = new Dictionary<string, string>();
                foreach (Lot plot in lstLot)
                {
                    p.TransactionKeys.Add(plot.Key, transactionKey);
                }

                //事物结束时间
                p.TransENDTime = now;
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary> 出站基础条件检验 </summary>
        /// <param name="workModel">执行类型 C - 标准过站 A - 自动过站操作</param>
        /// <param name="p">批次出站参数</param>
        /// <param name="equipment">设备</param>
        /// <param name="lstLot">批次列表</param>        
        /// <returns></returns>
        public MethodReturnResult TrackOutBaseCheck(string workModel, TrackParameter p, Equipment equipment, List<Lot> lstLot)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            try
            {
                #region 出站基础信息校验(自动过站不需要进行校验)
                if (workModel == "C")
                {
                    if (string.IsNullOrEmpty(p.RouteOperationName))
                    {
                        result.Code = 1001;
                        result.Message = string.Format("{0} {1}"
                                                        , "工序名称"
                                                        , StringResource.ParameterIsNull);
                        return result;
                    }

                    if (string.IsNullOrEmpty(p.LineCode))
                    {
                        result.Code = 1001;
                        result.Message = string.Format("{0} {1}"
                                                        , "线别代码"
                                                        , StringResource.ParameterIsNull);
                        return result;
                    }

                    if (p.LotNumbers == null || p.LotNumbers.Count == 0)
                    {
                        result.Code = 1001;
                        result.Message = string.Format("{0} {1}"
                                                        , "批次列表对象为空"
                                                        , StringResource.ParameterIsNull);
                        return result;
                    }

                    //校验批次信息
                    foreach (Lot lot in lstLot)
                    {
                        //若进站时未选择设备，判断设备是否选择
                        if (string.IsNullOrEmpty(lot.EquipmentCode) && string.IsNullOrEmpty(p.EquipmentCode))
                        {
                            result.Code = 1009;
                            result.Message = string.Format("请选择批次（{0}）加工设备。", lot.Key);
                            return result;
                        }

                        //进站未选择设备，设置当前设备为批次设备
                        if (string.IsNullOrEmpty(lot.EquipmentCode))
                        {
                            lot.EquipmentCode = p.EquipmentCode;
                        }

                        //批次已经完成。
                        if (lot.StateFlag == EnumLotState.Finished)
                        {
                            result.Code = 1003;
                            result.Message = string.Format("批次（{0}）已完成。", lot.Key);
                            return result;
                        }

                        //批次已结束
                        if (lot.DeletedFlag == true)
                        {
                            result.Code = 1004;
                            result.Message = string.Format("批次（{0}）已结束。", lot.Key);
                            return result;
                        }

                        //批次已暂停
                        if (lot.HoldFlag == true)
                        {
                            result.Code = 1005;
                            result.Message = string.Format("批次（{0}）已暂停。", lot.Key);
                            return result;
                        }

                        //批次目前非等待出站状态。
                        if (lot.StateFlag != EnumLotState.WaitTrackOut)
                        {
                            result.Code = 1006;
                            result.Message = string.Format("批次（{0}）目前状态（{1}）,非({2})状态!"
                                                            , lot.Key
                                                            , lot.StateFlag.GetDisplayName()
                                                            , EnumLotState.WaitTrackOut.GetDisplayName());
                            return result;
                        }

                        //检查批次所在工序是否处于指定工序
                        if (lot.RouteStepName != p.RouteOperationName)
                        {
                            result.Code = 1007;
                            result.Message = string.Format("批次（{0}）目前在（{1}）工序，不能在({2})工序操作!"
                                                            , lot.Key
                                                            , lot.RouteStepName
                                                            , p.RouteOperationName);
                            return result;
                        }

                        //检查批次是否在当前设备
                        //if (lot.EquipmentCode != equipment.Key)
                        //{
                        //    result.Code = 1008;
                        //    result.Message = string.Format("批次（{0}）当前在({1})车间的{2}设备中加工，不能在({3})设备上操作。"
                        //                                    , lot.Key
                        //                                    , lot.LocationName
                        //                                    , lot.EquipmentCode
                        //                                    , equipment.Key);
                        //    return result;
                        //}

                        ////检查批次与当前设备是否同车间
                        //if (lot.LocationName != equipment.LocationName)
                        //{
                        //    result.Code = 1008;
                        //    result.Message = string.Format("批次（{0}）当前在({1})车间，与({2})设备所属车间({3})不同!"
                        //                                    , lot.Key
                        //                                    , lot.LocationName
                        //                                    , equipment.Key
                        //                                    , equipment.LocationName);
                        //    return result;
                        //}

                        //校验过站QTime
                        result = CheckQTime("OUT", lot, p);

                        if (result.Code > 0)
                        {
                            return result;
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.WriteLogError("TrackOutBaseCheck>", ex);

                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary> 出站业务数据检验 </summary>
        /// <param name="workModel">执行类型 C - 标准过站 A - 自动过站操作</param>
        /// <param name="p">批次出站参数</param>
        /// <param name="lstLot">批次列表</param>        
        /// <returns></returns>
        public MethodReturnResult TrackOutBusinessCheck(string workModel, TrackOutParameter p, List<Lot> lstLot)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            try
            {
                RouteStepAttribute rs;          //工步属性对象
                WorkOrderRule wor = null;       //工单规则
                string strCalibrationNo = "";   //校准板代码

                //出站业务信息校验
                foreach (Lot lot in lstLot)
                {
                    #region 1. 校验IV测试数据
                    bool isCheck = false;
                    rs = this.RouteStepAttributeDataEngine.Get(new RouteStepAttributeKey()
                    {
                        RouteName = p.RouteName,
                        RouteStepName = p.RouteOperationName,
                        AttributeName = "IsCheckIVTestData"
                    });

                    if (rs != null
                        && bool.TryParse(rs.Value, out isCheck)
                        && isCheck)
                    {
                        isCheck = true;
                    }
                    else
                    {
                        isCheck = false;
                    }

                    //需要检查IV测试数据。
                    if (isCheck == true)
                    {
                        #region 1.1 校验IV测试数据
                        //PagingConfig cfg = new PagingConfig()
                        //{
                        //    PageNo = 0,
                        //    PageSize = 1,
                        //    Where = string.Format("Key.LotNumber='{0}' AND IsDefault=1", lot.Key),
                        //    OrderBy = "Key.TestTime Desc"
                        //};

                        PagingConfig cfg = new PagingConfig()
                        {
                            Where = string.Format("Key.LotNumber='{0}' AND IsDefault=1", lot.Key),
                            IsPaging = false
                        };

                        IList<IVTestData> lstTestData = this.IVTestDataDataEngine.Get(cfg);

                        //判断测试记录是否存在
                        if (lstTestData.Count == 0)
                        {
                            result.Code = 2000;
                            result.Message = string.Format("批次（{0}）IV测试数据不存在！", lot.Key);
                            return result;
                        }

                        //判断是否有多条默认值
                        if (lstTestData.Count > 1)
                        {
                            result.Code = 2100;
                            result.Message = string.Format("批次（{0}）IV测试数据存在多条异常，请重测！", lot.Key);
                            return result;
                        }

                        //判断IV测试仪是否使用全角字符
                        if (lstTestData[0].Key.LotNumber.ToUpper() != lot.Key.ToUpper())
                        {
                            result.Code = 2200;
                            result.Message = string.Format("批次（{0}）在IV测试数据中为全角字符，请修改输入法格式，重新测试。", lot.Key);
                            return result;
                        }
                        #endregion

                        #region 1.2 检查校准板规则
                        //取得校准板（前11位）
                        strCalibrationNo = "";

                        if (lstTestData[0].CalibrationNo.Trim().ToUpper().Length >= 11)
                        {
                            strCalibrationNo = lstTestData[0].CalibrationNo.Substring(0, 11);
                        }
                        else
                        {
                            strCalibrationNo = lstTestData[0].CalibrationNo;
                        }

                        //IV测试数据使用的校准板线别是否正确
                        PagingConfig CalibrationLinecfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format("Key.CalibrationPlateID = '{0}'" +
                                                  " and Key.LocationName = '{1}'" +
                                                  " and Key.LineCode = '{2}'",
                                                  strCalibrationNo,                         //校准板代码
                                                  lot.LocationName,                         //车间
                                                  p.LineCode)                               //线别
                        };

                        IList<CalibrationPlateLine> calibrationPlateLineList = this.CalibrationPlateLineDataEngine.Get(CalibrationLinecfg);

                        if (calibrationPlateLineList == null || calibrationPlateLineList.Count == 0)
                        {
                            result.Code = 2002;
                            result.Message = string.Format("批次（{0}）使用的校准板（{1}）与工艺设置的校准板不匹配，请确认后重新测试IV数据。"
                                                            , lot.Key
                                                            , lstTestData[0].CalibrationNo
                                                            );
                            return result;
                        }
                        #endregion

                        #region 1.3 检查校准周期
                        //获取工单规则。
                        wor = this.WorkOrderRuleDataEngine.Get(new WorkOrderRuleKey()
                        {
                            OrderNumber = lot.OrderNumber,
                            MaterialCode = lot.MaterialCode
                        });

                        //工单没有设置规则。
                        if (wor == null)
                        {
                            result.Code = 2001;
                            result.Message = string.Format("工单（{0}:{1}）规则没有维护，请确认。", lot.OrderNumber, lot.MaterialCode);
                            return result;
                        }

                        //IV测试数据的校准时间是否满足校准周期要求
                        DateTime calibrateTime = lstTestData[0].CalibrateTime ?? DateTime.MinValue;
                        double calibrateCycle = (lstTestData[0].Key.TestTime - calibrateTime).TotalMinutes;
                        if (calibrateCycle > wor.CalibrationCycle)
                        {
                            result.Code = 2002;
                            result.Message = string.Format("批次（{0}）校准时间（{1:yyyy-MM-dd HH:mm:ss}）超过工单规则设置的校准周期（{2}）分钟，请确认。"
                                                            , lot.Key
                                                            , lstTestData[0].CalibrateTime
                                                            , wor.CalibrationCycle);
                            return result;
                        }
                        #endregion
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLogError("TrackOutBusinessCheck>", ex);

                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary> 执行出站基础功能 </summary>
        /// <param name="workModel">执行类型 C - 标准过站 A - 自动过站操作</param>
        /// <param name="p">批次出站参数</param>
        /// <param name="equipment">设备对象</param>
        /// <param name="lstLot">批次列表</param>
        /// <param name="session">连接事物</param>
        /// <returns></returns>
        public MethodReturnResult ExecuteTrackOutBaseFounction(string workModel, TrackOutParameter p, Equipment equipment, List<Lot> lstLot, ISession session, string transactionKey)
        {
            DateTime now = DateTime.Now.AddMilliseconds(20);
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            List<LotTransaction> lstLotTransactionForInsert = new List<LotTransaction>();                           //作业事物对象列表
            List<LotTransactionHistory> lstLotTransactionHistoryForInsert = new List<LotTransactionHistory>();      //批次加工历史事物对象列表
            List<LotAttribute> lstLotAttributeForInsert = new List<LotAttribute>();                                 //批次属性新增对象
            List<LotAttribute> lstLotAttributeForUpdate = new List<LotAttribute>();                                 //批次属性更新对象

            List<LotTransactionDefect> lstLotTransactionDefectForInsert = new List<LotTransactionDefect>();         //批次不良事物对象列表
            List<LotTransactionScrap> lstLotTransactionScrapForInsert = new List<LotTransactionScrap>();            //批次报废事物对象列表

            List<LotTransactionEquipment> lstTransactionEquipmentForUpdate = new List<LotTransactionEquipment>();   //设备加工事物更新对象列表
            List<LotTransactionEquipment> lstTransactionEquipmentForInsert = new List<LotTransactionEquipment>();   //设备加工事物新增对象列表

            //List<EquipmentStateEvent> lstEquipmentStateEventtForInsert = new List<EquipmentStateEvent>();           //设备加工状态记录对象列表
            bool isEquipmentChangeState = false;                                                                    //是否改变设备状态
            string sFromStateName = "";                                                                             //设备来源状态

            try
            {
                PagingConfig cfg = null;                            //查询条件
                //EquipmentChangeState ecsToLost = null;              //获取设备当前状态->LOST的状态切换数据

                foreach (Lot lot in lstLot)
                {
                    #region 1.批次不良数据
                    if (p.DefectReasonCodes != null && p.DefectReasonCodes.ContainsKey(lot.Key))
                    {
                        foreach (DefectReasonCodeParameter rcp in p.DefectReasonCodes[lot.Key])
                        {
                            //建立批次不良对象
                            LotTransactionDefect lotDefect = new LotTransactionDefect()
                            {
                                Key = new LotTransactionDefectKey()
                                {
                                    TransactionKey = transactionKey,                       //事物主键
                                    ReasonCodeCategoryName = rcp.ReasonCodeCategoryName,   //不良原因组名称
                                    ReasonCodeName = rcp.ReasonCodeName                    //不良原因名称
                                },
                                Quantity = rcp.Quantity,                                   //数量
                                ResponsiblePerson = rcp.ResponsiblePerson,                 //责任人
                                RouteOperationName = rcp.RouteOperationName,               //责任工序名称
                                Description = rcp.Description,                             //描述
                                Editor = p.Creator,                                        //编辑人
                                EditTime = now,                                            //编辑时间
                            };

                            //不良原因对象列表
                            lstLotTransactionDefectForInsert.Add(lotDefect);
                        }
                    }
                    #endregion

                    #region 2.批次报废数据
                    if (p.ScrapReasonCodes != null && p.ScrapReasonCodes.ContainsKey(lot.Key))
                    {
                        foreach (ScrapReasonCodeParameter rcp in p.ScrapReasonCodes[lot.Key])
                        {
                            //校验报废数量
                            if (lot.Quantity < rcp.Quantity)
                            {
                                result.Code = 1006;
                                result.Message = string.Format("批次（{0}）数量（{1}）不满足报废数量。"
                                                                , lot.Key
                                                                , lot.Quantity);
                                return result;
                            }

                            //创建报废对象
                            LotTransactionScrap lotScrap = new LotTransactionScrap()
                            {
                                Key = new LotTransactionScrapKey()
                                {
                                    TransactionKey = transactionKey,                        //事物主键
                                    ReasonCodeCategoryName = rcp.ReasonCodeCategoryName,    //代码组名称
                                    ReasonCodeName = rcp.ReasonCodeName                     //原因代码名称
                                },
                                Quantity = rcp.Quantity,                                    //数量
                                ResponsiblePerson = rcp.ResponsiblePerson,                  //责任人
                                RouteOperationName = rcp.RouteOperationName,                //责任工序
                                Description = rcp.Description,                              //描述
                                Editor = p.Creator,                                         //编辑人
                                EditTime = now,                                             //编辑时间
                            };

                            lot.Quantity -= rcp.Quantity;                                   //更新批次数量

                            lstLotTransactionScrapForInsert.Add(lotScrap);
                        }
                    }
                    #endregion

                    #region 3.设备信息、设备Event、设备Transaction

                    #region 3.1 更新批次设备加工历史数据
                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("LotNumber='{0}' AND EquipmentCode='{1}' AND State='{2}'",
                                                lot.Key,
                                                lot.EquipmentCode,
                                                Convert.ToInt32(EnumLotTransactionEquipmentState.Start))
                    };
                    IList<LotTransactionEquipment> lstLotTransactionEquipment = this.LotTransactionEquipmentDataEngine.Get(cfg, session);

                    if (lstLotTransactionEquipment.Count > 0)      //存在设备加工历史纪录
                        foreach (LotTransactionEquipment lotTransactionEquipment in lstLotTransactionEquipment)
                        {
                            lotTransactionEquipment.EndTransactionKey = transactionKey;                                     //事物主键
                            lotTransactionEquipment.EditTime = now;                                                         //编辑时间
                            lotTransactionEquipment.Editor = p.Creator;                                                     //编辑人
                            lotTransactionEquipment.EndTime = now;                                                          //加工结束时间
                            lotTransactionEquipment.State = EnumLotTransactionEquipmentState.End;                           //批次设备加工状态--结束

                            //创建批次加工历史纪录对象列表
                            lstTransactionEquipmentForUpdate.Add(lotTransactionEquipment);
                        }
                    else
                    {
                        LotTransactionEquipment newLotTransEquipment = new LotTransactionEquipment()
                        {
                            Key = transactionKey,                           //事物主键
                            EndTransactionKey = transactionKey,             //结束事物主键
                            CreateTime = now,                               //创建时间
                            Creator = p.Creator,                            //创建人
                            Editor = p.Creator,                             //编辑人
                            EditTime = now,                                 //编辑时间
                            EndTime = now,                                  //结束时间
                            EquipmentCode = p.EquipmentCode,                //设备代码
                            LotNumber = lot.Key,                            //组件批次号
                            Quantity = lot.Quantity,                        //批次数量
                            StartTime = lot.StartProcessTime,               //批次开始处理时间
                            State = EnumLotTransactionEquipmentState.End    //批次设备加工状态--结束
                        };

                        lstTransactionEquipmentForInsert.Add(newLotTransEquipment);
                    }
                    #endregion

                    #region 3.2 设置设备状态（当设备中无加工产品时变更）
                    //检验设备是否还有加工批次
                    //根据设备编码获取当前加工批次数据 

                    cfg = new PagingConfig()
                    {
                        PageSize = 1,
                        PageNo = 0,
                        OrderBy = "#*#",
                        Where = string.Format("EquipmentCode = '{0}' AND STATE = '{1}' "
                                                , equipment.Key
                                                , Convert.ToInt32(EnumLotTransactionEquipmentState.Start)
                                                )
                    };
                    
                    //cfg = new PagingConfig()
                    //{
                    //    PageSize = 0,
                    //    PageNo = 0,
                    //    IsPaging = true,
                    //    Where = string.Format("EquipmentCode = '{0}' AND STATE = '{1}' "
                    //                            , equipment.Key
                    //                            , Convert.ToInt32(EnumLotTransactionEquipmentState.Start)
                    //                            )
                    //};

                    IList<LotTransactionEquipment> lst = this.LotTransactionEquipmentDataEngine.Get(cfg, session);

                    if (lst.Count <= 1)
                    //if (cfg.Records <= 1)
                    {
                        //新增设备状态事件
                        ////获取设备当前状态->LOST的状态切换数据
                        //ecsToLost = this.EquipmentChangeStateDataEngine.Get(equipment.StateName, EnumEquipmentStateType.Lost.ToString(), session);

                        //新增设备状态事件数据
                        //EquipmentStateEvent newEquipmentStateEvent = new EquipmentStateEvent()
                        //{
                        //    Key = Guid.NewGuid().ToString(),                                //主键
                        //    EquipmentCode = equipment.Key,                                  //设备代码
                        //    EquipmentChangeStateName = "",                                  //设备状态切换名称
                        //    //EquipmentChangeStateName = ecsToLost.Key,                       //设备状态切换名称
                        //    EquipmentFromStateName = equipment.StateName,                   //来源状态
                        //    EquipmentToStateName = EnumEquipmentStateType.Lost.ToString(),  //目标状态
                        //    Description = p.Remark,                                         //描述
                        //    IsCurrent = true,                                               //是否当前记录
                        //    CreateTime = now,                                               //创建时间
                        //    Creator = p.Creator,                                            //创建人
                        //    EditTime = now,                                                 //编辑时间
                        //    Editor = p.Creator                                              //编辑人                    
                        //};

                        //lstEquipmentStateEventtForInsert.Add(newEquipmentStateEvent);

                        sFromStateName = equipment.StateName;

                        //修改设备属性
                        equipment.StateName = EnumEquipmentStateType.Lost.ToString();       //需改状态为空闲状态
                        equipment.ChangeStateName = "";                                     //状态切换矩阵代码
                        equipment.EditTime = now;                                           //编辑时间
                        equipment.Editor = p.Creator;                                       //编辑人

                        isEquipmentChangeState = true;
                    }
                    #endregion
                    #endregion

                    #region 4.批次扣料
                    result = DeductMaterial(EnumDataCollectionAction.TrackOut, p, equipment, lstLot, session, transactionKey);

                    if (result.Code > 0)
                    {
                        return result;
                    }
                    #endregion

                    #region 5.批次属性设置
                    if (p.Attributes != null && p.Attributes.ContainsKey(lot.Key))
                    {
                        foreach (TransactionParameter tp in p.Attributes[lot.Key])
                        {
                            LotAttribute lotAttribute = null;

                            //判断批次属性是否存在，若存在进行更新(批次返工或返修时)
                            LotAttributeKey lotAtrriKey = new LotAttributeKey()
                            {
                                AttributeName = tp.Name,
                                LotNumber = lot.Key
                            };

                            if (lot.ReworkFlag != 0 || lot.RepairFlag != 0)
                            {
                                lotAttribute = this.LotAttributeDataEngine.Get(lotAtrriKey);
                            }

                            if (lotAttribute != null)
                            {
                                //更新批次属性
                                lotAttribute.AttributeValue = tp.Value;
                                lotAttribute.Editor = p.Creator;
                                lotAttribute.EditTime = now;

                                lstLotAttributeForUpdate.Add(lotAttribute);
                            }
                            else
                            {
                                //新增批次属性
                                lotAttribute = new LotAttribute()
                                {
                                    Key = lotAtrriKey,
                                    AttributeValue = tp.Value,
                                    Editor = p.Creator,
                                    EditTime = now
                                };

                                lstLotAttributeForInsert.Add(lotAttribute);
                            }
                        }
                    }
                    #endregion

                    #region 6.批次加工历史
                    ////记录操作历史数据
                    //LotTransaction transObj = new LotTransaction()
                    //{
                    //    Key = transactionKey,                               //事物主键     
                    //    Activity = EnumLotActivity.TrackOut,                //批次状态                        
                    //    Description = p.Remark,                             //描述                        
                    //    InQuantity = lot.Quantity,                          //操作前数量
                    //    OutQuantity = lot.Quantity,                         //操作后数量
                    //    LotNumber = lot.Key,                                //组件批次号
                    //    LocationName = lot.LocationName,                    //车间
                    //    LineCode = lot.LineCode,                            //线别
                    //    OperateComputer = p.OperateComputer,                //操作电脑
                    //    OrderNumber = lot.OrderNumber,                      //工单                        
                    //    RouteEnterpriseName = lot.RouteEnterpriseName,      //工艺流程组
                    //    RouteName = lot.RouteName,                          //工艺流程
                    //    RouteStepName = lot.RouteStepName,                  //工序名称
                    //    ShiftName = p.ShiftName,                            //班别
                    //    UndoFlag = false,                                   //撤销标识
                    //    UndoTransactionKey = null,                          //撤销主键
                    //    Grade = lot.Grade,                                  //等级
                    //    Color = lot.Color,                                  //花色
                    //    Attr1 = lot.Attr1,                                  //批次属性1
                    //    Attr2 = lot.Attr2,                                  //批次属性2
                    //    Attr3 = lot.Attr3,                                  //批次属性3
                    //    Attr4 = lot.Attr4,                                  //批次属性4
                    //    Attr5 = lot.Attr5,                                  //批次属性5
                    //    OriginalOrderNumber = lot.OriginalOrderNumber,      //原始工单
                    //    CreateTime = now,                                   //创建时间
                    //    Creator = p.Creator,                                //创建人
                    //    Editor = p.Creator,                                 //编辑人
                    //    EditTime = now                                      //编辑时间
                    //};
                    //lstLotTransactionForInsert.Add(transObj);

                    //LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lot);

                    //lotHistory.StateFlag = EnumLotState.WaitTrackOut;

                    //lstLotTransactionHistoryForInsert.Add(lotHistory);
                    #endregion

                    #region 7.获取下一个工步
                    //取得当前工步的序列号
                    RouteStepKey rsKey = new RouteStepKey()
                    {
                        RouteName = lot.RouteName,
                        RouteStepName = lot.RouteStepName
                    };
                    RouteStep rsObj = this.RouteStepDataEngine.Get(rsKey, session);
                    if (rsObj == null)
                    {
                        result.Code = 2001;
                        result.Message = string.Format("批次（{0}）所在工艺流程（{1}）不存在。"
                                                        , lot.Key
                                                        , rsKey);
                        return result;
                    }

                    //取得下一个工步代码
                    cfg = new PagingConfig()
                    {
                        PageNo = 0,
                        PageSize = 1,
                        Where = string.Format(@"Key.RouteName = '{0}'
                                            AND SortSeq = '{1}'"
                                                , rsObj.Key.RouteName
                                                , rsObj.SortSeq + 1),
                        OrderBy = "SortSeq"
                    };

                    IList<RouteStep> lstRouteStep = this.RouteStepDataEngine.Get(cfg, session);
                    if (lstRouteStep.Count > 0)
                    {
                        //设置批次下一步工步属性
                        lot.RouteStepName = lstRouteStep[0].Key.RouteStepName;  //下一站工步名称
                    }

                    //批次状态等待进站
                    lot.StateFlag = EnumLotState.WaitTrackIn;
                    #endregion
                }

                #region 开始事物处理

                #region 更新批次基本信息
                ////批次信息
                //foreach (Lot lot in lstLot)
                //{
                //    this.LotDataEngine.Update(lot, session);
                //}

                ////新增加工信息
                //foreach (LotTransaction lotTransaction in lstLotTransactionForInsert)
                //{
                //    this.LotTransactionDataEngine.Insert(lotTransaction, session);
                //}

                ////新增批次加工信息
                //foreach (LotTransactionHistory lotTransactionHistory in lstLotTransactionHistoryForInsert)
                //{
                //    this.LotTransactionHistoryDataEngine.Insert(lotTransactionHistory, session);
                //}

                //更新批次LotTransactionStepData信息
                //foreach (LotTransactionStep lotTransactionStep in lstLotTransactionStepDataEngineForInsert)
                //{
                //    this.LotTransactionStepDataEngine.Insert(lotTransactionStep, db);
                //}

                ////更新批次LotTransactionCheckDataEngine信息
                //foreach (LotTransactionCheck lotTransactionCheck in lstLotTransactionCheckForInsert)
                //{
                //    this.LotTransactionCheckDataEngine.Insert(lotTransactionCheck, db);
                //}
                #endregion

                #region 批次不良
                foreach (LotTransactionDefect lotTransactionDefect in lstLotTransactionDefectForInsert)
                {
                    LotTransactionDefectDataEngine.Update(lotTransactionDefect, session);
                }
                #endregion

                #region 批次报废
                foreach (LotTransactionScrap lotTransactionScrap in lstLotTransactionScrapForInsert)
                {
                    LotTransactionScrapDataEngine.Update(lotTransactionScrap, session);
                }
                #endregion

                #region 设备信息 , 设备的Event ,设备的Transaction
                //LotTransactionEquipment ,Equipment ,EquipmentStateEvent
                foreach (LotTransactionEquipment lotTransactionEquipmentUpdate in lstTransactionEquipmentForUpdate)
                {
                    LotTransactionEquipmentDataEngine.Update(lotTransactionEquipmentUpdate, session);
                }

                foreach (LotTransactionEquipment lotTransactionEquipmentInsert in lstTransactionEquipmentForInsert)
                {
                    LotTransactionEquipmentDataEngine.Insert(lotTransactionEquipmentInsert, session);
                }

                this.EquipmentDataEngine.Update(equipment, session);

                //设备的Event
                if (isEquipmentChangeState == true)
                {
                    DateTime EndTransTime = now;

                    if (workModel == "C")
                    {
                        //自动出站时取得出站的事物主键
                        string sNexttransactionKey = "";

                        if (p.TransactionKeys != null && p.TransactionKeys.Count > 0)
                        {
                            sNexttransactionKey = p.TransactionKeys[lstLot[0].Key];
                        }

                        //新增设备状态事件数据
                        result = EquipmentChangeState(equipment, sFromStateName, session, transactionKey, sNexttransactionKey, true, EndTransTime);
                    }
                    else
                    {
                        result = EquipmentChangeState(equipment, sFromStateName, session, transactionKey, "", false, EndTransTime);
                    }
                }

                //foreach (EquipmentStateEvent equipmentStateEvent in lstEquipmentStateEventtForInsert)
                //{
                //    EquipmentStateEventDataEngine.Insert(equipmentStateEvent, session);
                //    //this.ExecuteAddEquipmentStateEvent(equipmentStateEvent, session, true);
                //}
                #endregion

                #region 批次属性
                foreach (LotAttribute lotAttributeForInsert in lstLotAttributeForInsert)
                {
                    LotAttributeDataEngine.Insert(lotAttributeForInsert, session);
                }

                foreach (LotAttribute lotAttribute in lstLotAttributeForUpdate)
                {
                    LotAttributeDataEngine.Update(lotAttribute, session);
                }
                #endregion

                #endregion
            }
            catch (Exception err)
            {
                LogHelper.WriteLogError("ExecuteTrackOutBaseFounction>", err);

                //if (executedWithTransaction == false)
                //{
                //    transaction.Rollback();
                //    db.Close();
                //}
                result.Code = 1000;
                result.Message += string.Format(StringResource.Error, err.Message);
                result.Detail = err.ToString();
                return result;
            }

            return result;
        }

        /// <summary> 出站业务逻辑处理</summary>
        /// <param name="workModel">执行类型 C - 标准过站 A - 自动过站操作</param>
        /// <param name="p">批次出站参数</param>
        /// <param name="equipment">设备对象</param>
        /// <param name="lstLot">批次列表</param>
        /// <param name="session">连接参数</param>
        /// <param name="transactionKey">事物主键</param>
        /// <returns></returns>
        public MethodReturnResult ExecuteTrackOutBusinessFounction(string workModel, TrackOutParameter p, Equipment equipment, List<Lot> lstLot, ISession session, string transactionKey)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            List<IVTestData> lstIVTestDataForUpdate = new List<IVTestData>();
            List<LotTransactionCheck> lstLotTransactionCheckForInsert = new List<LotTransactionCheck>();

            DateTime now = DateTime.Now.AddMilliseconds(20);

            try
            {
                bool isExecutePowerset = false;

                foreach (Lot lot in lstLot)
                {

                    #region 获取工步属性数据。
                    RouteStepAttributeKey key = new RouteStepAttributeKey()
                    {
                        RouteName = p.RouteName,                //工艺流程名称
                        RouteStepName = p.RouteOperationName,   //工步名称
                        AttributeName = "IsExecutePowerset"     //属性名称
                    };
                    RouteStepAttribute rsa = this.RouteStepAttributeDataEngine.Get(key, session);

                    if (rsa != null)
                    {
                        bool.TryParse(rsa.Value, out isExecutePowerset);
                    }

                    //是否进行分档
                    if (isExecutePowerset == true)
                    {
                        //判断IV数据是否存在
                        PagingConfig cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format("Key.LotNumber='{0}' AND IsDefault=1", lot.Key),
                            OrderBy = "Key.TestTime Desc"
                        };

                        IList<IVTestData> lstTestData = this.IVTestDataDataEngine.Get(cfg, session);

                        //IV数据不存在
                        if (lstTestData.Count == 0)
                        {
                            result.Code = 2000;
                            result.Message = string.Format("批次（{0}）IV测试数据不存在！", lot.Key);

                            return result;
                        }

                        //存在多条有效IV数据
                        if (lstTestData.Count > 1)
                        {
                            result.Code = 2001;
                            result.Message = string.Format("批次（{0}）IV测试数据异常，存在多条有效记录，请重测！", lot.Key);

                            return result;
                        }

                        //IV数据衰减
                        IVTestData ivTestUpdate = lstTestData[0];

                        result = IVTestDescy(ivTestUpdate, lot);
                        if (result.Code > 0)
                        {
                            return result;
                        }

                        //功率分档
                        result = ProductLevelPower(ivTestUpdate, lot);
                        if (result.Code > 0)
                        {
                            return result;
                        }
                        ivTestUpdate.Editor = p.Creator;             //编辑人
                        ivTestUpdate.EditTime = DateTime.Now;        //编辑时间

                        //加入IV测试更新对象列表
                        lstIVTestDataForUpdate.Add(ivTestUpdate);

                    }
                    #endregion

                    #region 新增检验数据
                    if (string.IsNullOrEmpty(p.Color) == false
                        || string.IsNullOrEmpty(p.Grade) == false
                        || (p.CheckBarcodes != null && p.CheckBarcodes.ContainsKey(lot.Key)))
                    {
                        LotTransactionCheck ltcObj = new LotTransactionCheck()
                        {
                            Key = transactionKey,
                            Editor = p.Creator,
                            EditTime = now
                        };
                        ltcObj.Color = p.Color;
                        ltcObj.Grade = p.Grade;

                        if (p.CheckBarcodes != null && p.CheckBarcodes.ContainsKey(lot.Key))
                        {
                            IList<string> lstBarcode = p.CheckBarcodes[lot.Key];
                            ltcObj.Barcode1 = lstBarcode.Count > 0 ? lstBarcode[0] : null;
                            ltcObj.Barcode2 = lstBarcode.Count > 1 ? lstBarcode[1] : null;
                            ltcObj.Barcode3 = lstBarcode.Count > 2 ? lstBarcode[2] : null;
                            ltcObj.Barcode4 = lstBarcode.Count > 3 ? lstBarcode[3] : null;
                            ltcObj.Barcode5 = lstBarcode.Count > 4 ? lstBarcode[4] : null;
                        }

                        lstLotTransactionCheckForInsert.Add(ltcObj);

                    }
                    #endregion
                }

                #region 开始事物处理
                //IV测试更新对象
                foreach (IVTestData ivtest in lstIVTestDataForUpdate)
                {
                    this.IVTestDataDataEngine.Update(ivtest, session);
                }

                #endregion
            }
            catch (Exception err)
            {
                LogHelper.WriteLogError("ExecuteTrackOutBusinessFounction>", err);

                result.Code = 1000;
                result.Message += string.Format(StringResource.Error, err.Message);
                result.Detail = err.ToString();
                return result;
            }

            return result;
        }




        /// <summary> 立即修改当前设备状态并提交 </summary>
        /// <param name="equipmentBack">设备对象</param>
        /// <returns></returns>
        public MethodReturnResult SetEquipmentState(Equipment equipment)
        {
            MethodReturnResult result = new MethodReturnResult();
            ITransaction transactioneqp = null;
            ISession session = null;

            try
            {
                session = this.SessionFactory.OpenSession();
                transactioneqp = session.BeginTransaction();

                //更新设备Transaction信息
                this.EquipmentDataEngine.Update(equipment, session);

                transactioneqp.Commit();
                session.Close();

                result.Code = 0;
            }
            catch (Exception ex)
            {
                transactioneqp.Rollback();
                session.Close();

                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }

            return result;
        }

        /// <summary> 修改设备状态 </summary>
        /// <param name="equipment">设备对象</param>
        /// <param name="sFromStateName">设备开始状态</param>
        /// <param name="session">事物对象</param>
        /// <param name="transactionKey">创建事物主键</param>
        /// <param name="nextTransactionKey">下一个事物主键</param>
        /// <returns></returns>
        /// 
        /// <summary>
        /// 修改设备状态
        /// </summary>
        /// <param name="equipment">设备对象</param>
        /// <param name="sFromStateName">设备设置状态</param>
        /// <param name="session">事物对象</param>
        /// <param name="transactionKey">业务事物主键</param>
        /// <param name="nextTransactionKey">下一个业务事物主键</param>
        /// <param name="isGetPrevious"></param>
        /// <param name="endTransTime">结束时间</param>
        /// <returns></returns>
        public MethodReturnResult EquipmentChangeState(Equipment equipment, string sFromStateName, ISession session, string transactionKey, string nextTransactionKey, bool isGetPrevious, DateTime endTransTime)
        {
            MethodReturnResult result = new MethodReturnResult();
            IList<EquipmentStateEvent> lsEquipmentStateEvent = new List<EquipmentStateEvent>();

            try
            {
                if (isGetPrevious == true)
                {
                    //取得设备最后状态对象
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = true,
                        PageNo = 0,
                        PageSize = 1,
                        Where = string.Format("EquipmentCode = '{0}' and IsCurrent = 1",
                                               equipment.Key),
                        OrderBy = "CreateTime DESC"
                    };

                    lsEquipmentStateEvent = this.EquipmentStateEventDataEngine.Get(cfg);

                    if (lsEquipmentStateEvent != null && lsEquipmentStateEvent.Count > 0)
                    {
                        lsEquipmentStateEvent[0].IsCurrent = false;                     //当前事物标识
                        lsEquipmentStateEvent[0].EndEventKey = transactionKey;          //结束事件事物主键
                        //lsEquipmentStateEvent[0].EndTime = equipment.EditTime;          //事件结束时间
                        //lsEquipmentStateEvent[0].EditTime = equipment.EditTime;         //编辑时间    

                        lsEquipmentStateEvent[0].EndTime = endTransTime;                //事件结束时间
                        lsEquipmentStateEvent[0].EditTime = endTransTime;               //编辑时间
                        lsEquipmentStateEvent[0].Editor = equipment.Editor;             //编辑人
                    }
                }

                //新增设备状态事件数据
                EquipmentStateEvent newEquipmentStateEvent = new EquipmentStateEvent()
                {
                    Key = transactionKey,                                           //主键
                    EquipmentCode = equipment.Key,                                  //设备代码
                    EquipmentChangeStateName = "",                                  //设备状态切换名称
                    EquipmentFromStateName = sFromStateName,                        //来源状态
                    EquipmentToStateName = equipment.StateName,                     //目标状态
                    StartTime = equipment.EditTime,                                 //事件开始时间
                    EndTime = endTransTime,                                         //事件结束时间
                    EndEventKey = nextTransactionKey,                               //结束事物
                    Description = "",                                               //描述
                    IsCurrent = true,                                               //当前记录
                    CreateTime = equipment.EditTime,                                //创建时间
                    Creator = equipment.Editor,                                     //创建人
                    EditTime = endTransTime,                                        //编辑时间
                    Editor = equipment.Editor                                       //编辑人                    
                };

                //开始事务处理                
                //历史设备状态事件更新
                foreach (EquipmentStateEvent preEquipmentStateEvent in lsEquipmentStateEvent)
                {
                    this.EquipmentStateEventDataEngine.Update(preEquipmentStateEvent, session);
                }

                //新增设备状态事件
                this.EquipmentStateEventDataEngine.Insert(newEquipmentStateEvent, session);
            }
            catch (Exception err)
            {
                result.Code = 1005;
                result.Message = err.Message;
            }

            return result;
        }

        /// <summary> 检查QTime规则 </summary>
        /// <param name="moveType">操作类型IN - MoveIN OUT - MoveOUT</param>
        /// <param name="lot">批次对象</param>
        /// <param name="p">参数对象</param>
        /// <returns> 0 - 成功 -1 -失败 > 0 异常</returns>
        public MethodReturnResult CheckQTime(string moveType, Lot lot, TrackParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            bool isCheckQTime = true;                                   //是否校验QTime
            double dTimeQTime = 0;                                      //QTime时间
            IList<RouteStepAttribute> lstRouteStepAttributeValues;      //工步属性列表
            string sIsCheckQTime = "";                                  //QTime判断标识属性名称（ true - 控制 false - 不控制）
            string sQTimeControlTypeName = "";                          //QTime类型属性名称（ IN - 时间之内 OUT - 大于时间）
            string sQTimeAttrTimeName = "";                             //QTime控制时间属性名称            
            string sQTimeControlOperationName = "";                     //QTime控制操作属性名称（QTime 控制操作 N:终止操作 H:Hold批次）

            bool isCheckPass = true;                                    //是否通过校验QTime

            try
            {
                #region 校验进站QTime
                //创建规则条件根据工艺流程及工序
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@" Key.RouteName='{0}' and Key.RouteStepName='{1}'"
                        , p.RouteName, p.RouteOperationName)

                };

                //取得工步规则列表
                lstRouteStepAttributeValues = this.RouteStepAttributeDataEngine.Get(cfg);

                //存在工步规则
                if (lstRouteStepAttributeValues != null && lstRouteStepAttributeValues.Count > 0)
                {
                    if (moveType == "IN")       //入站操作
                    {
                        sIsCheckQTime = "IsCheckFixCycleBeforeTrackIn";                     //QTime判断标识属性名称（ true - 控制 false - 不控制）
                        sQTimeControlTypeName = "TrackInQTimeControlType";                  //QTime类型属性名称（ IN - 时间之内 OUT - 大于时间）
                        sQTimeAttrTimeName = "FixCycleTimeForTrackIn";                      //QTime控制时间属性名称            
                        sQTimeControlOperationName = "TrackInQTimeControlOperation";        //QTime控制操作属性名称（QTime 控制操作 N:终止操作 H:Hold批次）
                    }
                    else                        //出站操作
                    {
                        sIsCheckQTime = "IsCheckFixCycleBeforeTrackOut";                    //QTime判断标识属性名称（ true - 控制 false - 不控制）
                        sQTimeControlTypeName = "TrackOutQTimeControlType";                 //QTime类型属性名称（ IN - 时间之内 OUT - 大于时间）
                        sQTimeAttrTimeName = "FixCycleTimeForTrackOut";                     //QTime控制时间属性名称            
                        sQTimeControlOperationName = "TrackOutQTimeControlOperation";       //QTime控制操作属性名称（QTime 控制操作 N:终止操作 H:Hold批次）
                    }

                    //取得是否校验QTime属性
                    string val = getAttributeValueFromList(lstRouteStepAttributeValues, sIsCheckQTime);

                    if (Boolean.TryParse(val, out isCheckQTime) == false)
                    {
                        isCheckQTime = false;
                    }

                    //进行QTime校验
                    if (isCheckQTime == true)
                    {
                        //取得QTime控制时间
                        val = getAttributeValueFromList(lstRouteStepAttributeValues, sQTimeAttrTimeName);
                        if (double.TryParse(val, out dTimeQTime) == false)
                        {
                            dTimeQTime = 0;
                        }

                        //取得控制类型
                        val = getAttributeValueFromList(lstRouteStepAttributeValues, sQTimeControlTypeName);
                        if (val == "")      //未设置控制类型时默认为必须耗时控制
                        {
                            val = "OUT";
                        }

                        //判断是否满足条件
                        if (val == "IN")
                        {
                            if (dTimeQTime >= (System.DateTime.Now - lot.EditTime.Value).TotalMinutes)
                            {
                                isCheckPass = true;
                            }
                            else
                            {
                                isCheckPass = false;

                                result.Message = string.Format("批次等待时间({0})分钟未通过时间校验,时间需控制在{1}分钟之内！",
                                                                (System.DateTime.Now - lot.EditTime.Value).TotalMinutes.ToString("0.0"),
                                                                dTimeQTime);
                            }
                        }
                        else
                        {
                            if (dTimeQTime <= (System.DateTime.Now - lot.EditTime.Value).TotalMinutes)
                            {
                                isCheckPass = true;
                            }
                            else
                            {
                                isCheckPass = false;

                                result.Message = string.Format("批次等待时间({0})分钟未通过时间校验,等待时间未达到{1}分钟以上！",
                                                                (System.DateTime.Now - lot.EditTime.Value).TotalMinutes.ToString("0.0"),
                                                                dTimeQTime);
                            }
                        }

                        //进行控制操作
                        if (isCheckPass == false)
                        {
                            //取得控制类型
                            val = getAttributeValueFromList(lstRouteStepAttributeValues, sQTimeControlOperationName);

                            if (val == "")      //未设置控制操作时默认为终止操作
                            {
                                val = "N";
                            }

                            //进行控制
                            if (val == "H")
                            {
                                //调用HOLD方法
                                result = LotHold(lot, p);
                                if (result.Code > 0)
                                {
                                    return result;
                                }

                                result.Message += " 批次已暂停！";
                            }

                            result.Code = 2000;
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.WriteLogError("CheckQTime>", ex);
                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }

            return result;
        }

        /// <summary> 批次Hold操作 </summary>
        /// <param name="lot">批次对象</param>
        /// <param name="p">参数对象</param>
        /// <returns></returns>
        public MethodReturnResult LotHold(Lot lot, TrackParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            ISession session = null;
            ITransaction transaction = null;

            try
            {
                string transactionKey = "";

                //取得TransactionKey
                transactionKey = Guid.NewGuid().ToString();

                LotTransaction transObj = new LotTransaction()
                {
                    Key = transactionKey,
                    Activity = EnumLotActivity.Hold,
                    CreateTime = DateTime.Now,
                    Creator = p.Creator,
                    Description = "",
                    Editor = p.Creator,
                    EditTime = DateTime.Now,
                    InQuantity = lot.Quantity,
                    LotNumber = lot.Key,
                    LocationName = lot.LocationName,
                    LineCode = lot.LineCode,
                    OperateComputer = p.OperateComputer,
                    OrderNumber = lot.OrderNumber,
                    OutQuantity = 1,
                    RouteEnterpriseName = lot.RouteEnterpriseName,
                    RouteName = lot.RouteName,
                    RouteStepName = lot.RouteStepName,
                    ShiftName = "",
                    UndoFlag = false,
                    UndoTransactionKey = null,
                    Grade = lot.Grade,
                    Color = lot.Color,
                    Attr1 = lot.Attr1,
                    Attr2 = lot.Attr2,
                    Attr3 = lot.Attr3,
                    Attr4 = lot.Attr4,
                    Attr5 = lot.Attr5,
                    OriginalOrderNumber = lot.OriginalOrderNumber
                };

                //批次属性设置HOLD
                lot.HoldFlag = true;

                //创建事物
                session = this.LotDataEngine.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                this.LotDataEngine.Update(lot, session);

                //更新批次事物LotTransaction（表WIP_TRANSACTION）信息
                this.LotTransactionDataEngine.Insert(transObj, session);

                //事物提交
                transaction.Commit();
                session.Close();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLogError("LotHold>", ex);

                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();

                transaction.Rollback();
                session.Close();
            }

            return result;
        }

        /// <summary> 分档 </summary>
        /// <param name="ivtestDescy"></param>
        /// <param name="lot"></param>
        /// <returns></returns>
        private MethodReturnResult ProductLevelPower(IVTestData ivtestDescy, Lot lot)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            PagingConfig cfg = null;

            try
            {
                #region 功率是否符合工单要求
                //获取工单规则。
                WorkOrderRule wor = this.WorkOrderRuleDataEngine.Get(new WorkOrderRuleKey()
                {
                    OrderNumber = lot.OrderNumber,
                    MaterialCode = lot.MaterialCode
                });

                if (wor != null)
                {
                    ivtestDescy.CoefPM = Math.Round(ivtestDescy.CoefPM, wor.PowerDegree, MidpointRounding.AwayFromZero);
                }
                if (wor != null
                    && (ivtestDescy.CoefPM < wor.MinPower || ivtestDescy.CoefPM > wor.MaxPower))
                {
                    result.Code = 1000;
                    result.Message = string.Format("批次（{0}）功率（{1}）不符合工单（{2}:{3}）功率范围（{4}-{5}）要求!"
                                                    , lot.Key
                                                    , ivtestDescy.CoefPM
                                                    , lot.OrderNumber
                                                    , lot.MaterialCode
                                                    , wor.MinPower
                                                    , wor.MaxPower);

                    return result;
                }

                #endregion

                #region 校验工单规则
                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.OrderNumber='{0}' 
                                                AND Key.MaterialCode='{1}'
                                                AND IsUsed=1"
                                            , lot.OrderNumber
                                            , lot.MaterialCode),
                    OrderBy = "Key"
                };

                //                cfg.IsPaging = false;
                //                cfg.Where = string.Format(@"Key.OrderNumber='{0}' 
                //                                                AND Key.MaterialCode='{1}'
                //                                                AND IsUsed=1"
                //                                            , lot.OrderNumber
                //                                            , lot.MaterialCode);
                //                cfg.OrderBy = "Key";
                IList<WorkOrderControlObject> lstWorkOrderControlObject = this.WorkOrderControlObjectDataEngine.Get(cfg);

                //循环取得工单控制参数
                foreach (WorkOrderControlObject item in lstWorkOrderControlObject)
                {
                    double value = double.MinValue;
                    switch (item.Key.Object)
                    {
                        case EnumPVMTestDataType.PM:
                            value = ivtestDescy.CoefPM;
                            break;
                        case EnumPVMTestDataType.FF:
                            value = ivtestDescy.CoefFF;
                            break;
                        case EnumPVMTestDataType.IPM:
                            value = ivtestDescy.CoefIPM;
                            break;
                        case EnumPVMTestDataType.ISC:
                            value = ivtestDescy.CoefISC;
                            break;
                        case EnumPVMTestDataType.VOC:
                            value = ivtestDescy.CoefVOC;
                            break;
                        case EnumPVMTestDataType.VPM:
                            value = ivtestDescy.CoefVPM;
                            break;
                        case EnumPVMTestDataType.CTM:
                            value = ivtestDescy.CTM;
                            break;
                        default:
                            break;
                    }

                    //控制参数检查。
                    if (value != double.MinValue
                        && CheckControlObject(item.Key.Type, value, item.Value) == false)
                    {
                        result.Code = 1000;
                        result.Message = string.Format("批次（{0}）{1} ({4})不符合工单（{5}:{6}）控制对象（{4}{2}{3}）要求!"
                                                        , lot.Key
                                                        , item.Key.Object.GetDisplayName()
                                                        , item.Key.Type
                                                        , item.Value
                                                        , value
                                                        , lot.OrderNumber
                                                        , lot.MaterialCode);

                        return result;
                    }
                }

                #endregion

                #region 校验电池片效率对应产品参数控制规则
                #region 1.取得电池片批次信息（按照先进先出规则，取得最先上料批次）
                cfg.IsPaging = false;
                cfg.Where = string.Format(@"Key.LotNumber='{0}' 
                                           AND MaterialCode like'11%'"
                                            , lot.Key
                                            );
                cfg.OrderBy = "EditTime asc";

                IList<LotBOM> lotBomList = this.LotBOMDataEngine.Get(cfg);

                LotBOM lotBOM = null;

                if (lotBomList != null && lotBomList.Count > 0)
                {
                    lotBOM = (LotBOM)lotBomList.First().Clone();
                }
                else
                {
                    result.Code = 2000;
                    result.Message = string.Format("批次{0}的电池片用料不存在请核实!", lot.Key);

                    return result;
                }
                #endregion

                #region 2.取得电池片供应商代码
                string sOrderNumber = "";

                //当存在历史工单时，按照历史工单进行电池片信息查询
                if (lot.OriginalOrderNumber == null || lot.OriginalOrderNumber == "")
                {
                    sOrderNumber = lot.OrderNumber;
                }
                else
                {
                    sOrderNumber = lot.OriginalOrderNumber;
                }

                MaterialReceiptDetail receiptDetail = this.getMaterialReceiptDetail(lotBOM.Key.MaterialLot, sOrderNumber);

                if (receiptDetail == null)
                {
                    result.Code = 2000;
                    result.Message = string.Format("电池批次{0}异常!无对应的领料记录！", lotBOM.Key.MaterialLot);

                    return result;
                }

                #endregion

                #region 3.校验不同厂商电池片效率对应产品参数控制规则
                cfg.IsPaging = false;
                cfg.Where = string.Format(@"Key.ProductCode='{0}' 
                                                AND Key.CellEff='{1}' 
                                                AND Key.SupplierCode='{2}' 
                                                AND IsUsed=1"
                                            , lot.MaterialCode
                                            , lot.Attr1
                                            , receiptDetail.SupplierCode
                                            );
                cfg.OrderBy = "Key";

                IList<ProductControlObject> lstProductControlObject = this.ProductControlObjectDataEngine.Get(cfg);

                foreach (ProductControlObject item in lstProductControlObject)
                {
                    //double value = double.MinValue;
                    //switch (item.Key.Object)
                    //{
                    //    case EnumPVMTestDataType.PM:
                    //        value = ivtestDescy.CoefPM;
                    //        break;
                    //    case EnumPVMTestDataType.FF:
                    //        value = ivtestDescy.CoefFF;
                    //        break;
                    //    case EnumPVMTestDataType.IPM:
                    //        value = ivtestDescy.CoefIPM;
                    //        break;
                    //    case EnumPVMTestDataType.ISC:
                    //        value = ivtestDescy.CoefISC;
                    //        break;
                    //    case EnumPVMTestDataType.VOC:
                    //        value = ivtestDescy.CoefVOC;
                    //        break;
                    //    case EnumPVMTestDataType.VPM:
                    //        value = ivtestDescy.CoefVPM;
                    //        break;
                    //    case EnumPVMTestDataType.CTM:
                    //        value = ivtestDescy.CTM;
                    //        break;
                    //    default:
                    //        break;
                    //}

                    double value = double.MinValue;
                    switch (item.Key.Object)
                    {
                        case EnumPVMTestDataType.PM:
                            value = ivtestDescy.PM;
                            break;
                        case EnumPVMTestDataType.FF:
                            value = ivtestDescy.FF;
                            break;
                        case EnumPVMTestDataType.IPM:
                            value = ivtestDescy.IPM;
                            break;
                        case EnumPVMTestDataType.ISC:
                            value = ivtestDescy.ISC;
                            break;
                        case EnumPVMTestDataType.VOC:
                            value = ivtestDescy.VOC;
                            break;
                        case EnumPVMTestDataType.VPM:
                            value = ivtestDescy.VPM;
                            break;
                        case EnumPVMTestDataType.CTM:
                            value = ivtestDescy.CTM;
                            break;
                        default:
                            break;
                    }

                    //控制参数检查。
                    if (value != double.MinValue
                        && CheckControlObject(item.Key.Type, value, item.Value) == false)
                    {
                        //HOLD组件
                        //lot.HoldFlag = true;

                        result.Code = 2000;
                        result.Message = string.Format("批次({0}){1}({4})不符合产品({5}:{6})({7})供应商{1}控制要求:({1}{2}{3})！"
                                                , lot.Key
                                                , item.Key.Object.GetDisplayName()
                                                , item.Key.Type
                                                , item.Value
                                                , value
                                                , lot.OrderNumber
                                                , lot.MaterialCode
                                                , receiptDetail.SupplierCode);

                        return result;
                    }
                }
                #endregion
                #endregion

                #region 分档
                //按照工单分档规则取得相应的档位
                cfg.IsPaging = true;
                cfg.Where = string.Format(@"Key.OrderNumber='{0}' 
                                            AND Key.MaterialCode='{1}'
                                            AND MinValue<='{2}'
                                            AND MaxValue>'{2}'
                                            AND IsUsed=1"
                                        , lot.OrderNumber
                                        , lot.MaterialCode
                                        , ivtestDescy.CoefPM);
                cfg.OrderBy = "Key";

                IList<WorkOrderPowerset> lstWorkOrderPowerset = this.WorkOrderPowersetDataEngine.Get(cfg);
                if (lstWorkOrderPowerset == null || lstWorkOrderPowerset.Count == 0)
                {
                    result.Code = 2000;
                    result.Message = string.Format("批次（{0}）功率({1})不符合工单({2}：{3})分档规则要求！"
                                            , lot.Key
                                            , ivtestDescy.CoefPM
                                            , lot.OrderNumber
                                            , lot.MaterialCode);

                    return result;
                }

                //取得对应分档
                WorkOrderPowerset ps = lstWorkOrderPowerset[0];

                //设置IV测试对象分档属性
                ivtestDescy.PowersetCode = ps.Key.Code;         //分档代码
                ivtestDescy.PowersetItemNo = ps.Key.ItemNo;     //分档项目号

                //需要进行子分档
                if (ps.SubWay != EnumPowersetSubWay.None)
                {
                    double value = double.MinValue;

                    //电流子分档。
                    if (ps.SubWay == EnumPowersetSubWay.ISC)
                    {
                        value = ivtestDescy.CoefISC;
                    }
                    else if (ps.SubWay == EnumPowersetSubWay.VOC)
                    {
                        value = ivtestDescy.CoefVOC;
                    }
                    else if (ps.SubWay == EnumPowersetSubWay.IPM)
                    {
                        value = ivtestDescy.CoefIPM;
                    }
                    else if (ps.SubWay == EnumPowersetSubWay.VPM)
                    {
                        value = ivtestDescy.CoefVPM;
                    }

                    cfg.Where = string.Format(@"Key.OrderNumber='{0}' 
                                            AND Key.MaterialCode='{1}'
                                            AND Key.Code='{3}'
                                            AND Key.ItemNo='{4}'
                                            AND MinValue<='{2}'
                                            AND MaxValue>'{2}'
                                            AND IsUsed=1"
                                        , lot.OrderNumber
                                        , lot.MaterialCode
                                        , value
                                        , ps.Key.Code
                                        , ps.Key.ItemNo);
                    cfg.OrderBy = "Key";

                    IList<WorkOrderPowersetDetail> lstWorkOrderPowersetDetail = this.WorkOrderPowersetDetailDataEngine.Get(cfg);

                    if (lstWorkOrderPowersetDetail == null || lstWorkOrderPowersetDetail.Count == 0)
                    {
                        result.Code = 2000;
                        result.Message = string.Format("批次（{0}）({1})子分档({2})不符合工单（{3})分档规则！"
                                                , lot.Key
                                                , ps.SubWay
                                                , value
                                                , lot.OrderNumber);

                        return result;
                    }

                    //设置子分档属性
                    ivtestDescy.PowersetSubCode = lstWorkOrderPowersetDetail[0].Key.SubCode;

                    #region 分档信息提示
                    if (ps.SubWay == EnumPowersetSubWay.None && string.IsNullOrEmpty(ivtestDescy.PowersetSubCode))
                    {
                        result.Message += string.Format("\r\n批次（{0}）符合工单（{1}:{2}）分档规则<font size='20' color='red'>无子分档</font>要求。 <font size='20' color='red'>电池片效率为：{3}</font>"
                        , lot.Key
                        , lot.OrderNumber
                        , lot.MaterialCode
                        , lot.Attr1
                        );
                    }
                    else if (ps.SubWay != EnumPowersetSubWay.None && !string.IsNullOrEmpty(ivtestDescy.PowersetSubCode))
                    {
                        result.Message += string.Format("\r\n批次（{0}）符合工单（{1}:{2}）分档规则<font size='20' color='red'>({3}-{4})</font>要求。<font size='20' color='red'>电池片效率为：{5}</font>"
                            , lot.Key
                            , lot.OrderNumber
                            , lot.MaterialCode
                            , ps.PowerName
                            , ivtestDescy.PowersetSubCode
                            , lot.Attr1);
                    }

                    #endregion
                }

                #endregion
            }
            catch (Exception err)
            {
                LogHelper.WriteLogError("ProductLevelPower>", err);

                result.Code = 1000;
                result.Message += string.Format(StringResource.Error, err.Message);
                result.Detail = err.ToString();
                return result;
            }

            return result;
        }

        /// <summary> IV数据衰减 </summary>
        /// <param name="ivtest"></param>
        /// <param name="lot"></param>
        /// <returns></returns>
        private MethodReturnResult IVTestDescy(IVTestData ivtest, Lot lot)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            try
            {
                //取得产品代码                 
                if (lot.MaterialCode == "1202020012")
                {
                    //实测功率在315.5---315.9999的组件对其Pmax、Voc、Vpm都乘以0.9985
                    if (ivtest.CoefPM > 315.5 && ivtest.CoefPM < 315.9999)
                    {
                        double rate = 0.9985;
                        ivtest.CoefPM = Math.Round(ivtest.PM * rate, 4);
                        ivtest.CoefVOC = Math.Round(ivtest.VOC * rate, 4);
                        ivtest.CoefVPM = Math.Round(ivtest.VPM * rate, 4);


                    }
                    //实测功率在316----319.99999的组件对其Pmax、Voc、Vpm都乘以0.997
                    else
                    {
                        if (ivtest.CoefPM > 316 && ivtest.CoefPM < 319.99999)
                        {
                            double rate = 0.997;
                            ivtest.CoefPM = Math.Round(ivtest.PM * rate, 4);
                            ivtest.CoefVOC = Math.Round(ivtest.VOC * rate, 4);
                            ivtest.CoefVPM = Math.Round(ivtest.VPM * rate, 4);

                        }
                        //实测功率在320.5----320.9999的组件对其Pmax、Voc、Vpm都乘以0.9985
                        else
                        {
                            if (ivtest.CoefPM > 320.5 && ivtest.CoefPM < 320.9999)
                            {
                                double rate = 0.9985;
                                ivtest.CoefPM = Math.Round(ivtest.PM * rate, 4);
                                ivtest.CoefVOC = Math.Round(ivtest.VOC * rate, 4);
                                ivtest.CoefVPM = Math.Round(ivtest.VPM * rate, 4);

                            }
                            //实测功率在321----325.99999的组件对其Pmax、Voc、Vpm都乘以0.997
                            else
                            {
                                if (ivtest.CoefPM > 321 && ivtest.CoefPM < 325.99999)
                                {
                                    double rate = 0.997;
                                    ivtest.CoefPM = Math.Round(ivtest.PM * rate, 4);
                                    ivtest.CoefVOC = Math.Round(ivtest.VOC * rate, 4);
                                    ivtest.CoefVPM = Math.Round(ivtest.VPM * rate, 4);

                                }
                            }
                        }
                    }
                }
                if (lot.MaterialCode == "1202020020")
                {
                    //实测功率在320.5---321的组件对其Pmax、Voc、Vpm都乘以0.9985
                    if (ivtest.CoefPM > 320.5 && ivtest.CoefPM < 321)
                    {
                        double rate = 0.9985;
                        ivtest.CoefPM = Math.Round(ivtest.PM * rate, 4);
                        ivtest.CoefVOC = Math.Round(ivtest.VOC * rate, 4);
                        ivtest.CoefVPM = Math.Round(ivtest.VPM * rate, 4);


                    }
                    //实测功率在321----325的组件对其Pmax、Voc、Vpm都乘以0.997
                    else
                    {
                        if (ivtest.CoefPM > 321 && ivtest.CoefPM < 325)
                        {
                            double rate = 0.997;
                            ivtest.CoefPM = Math.Round(ivtest.PM * rate, 4);
                            ivtest.CoefVOC = Math.Round(ivtest.VOC * rate, 4);
                            ivtest.CoefVPM = Math.Round(ivtest.VPM * rate, 4);

                        }
                        //实测功率在325----327的组件对其Pmax、Voc、Vpm都乘以0.994
                        else
                        {
                            if (ivtest.CoefPM > 325 && ivtest.CoefPM < 327)
                            {
                                double rate = 0.994;
                                ivtest.CoefPM = Math.Round(ivtest.PM * rate, 4);
                                ivtest.CoefVOC = Math.Round(ivtest.VOC * rate, 4);
                                ivtest.CoefVPM = Math.Round(ivtest.VPM * rate, 4);

                            }
                        }

                    }
                }

                //if (lot.MaterialCode == "1202020013")
                //{
                //    //实测功率在270.5---271的组件对其Pmax、Voc、Vpm都乘以0.9985
                //    if (ivtest.CoefPM > 270.5 && ivtest.CoefPM < 271)
                //    {
                //        double rate = 0.9985;
                //        ivtest.CoefPM = Math.Round(ivtest.PM * rate, 4);
                //        ivtest.CoefVOC = Math.Round(ivtest.VOC * rate, 4);
                //        ivtest.CoefVPM = Math.Round(ivtest.VPM * rate, 4);

                //    }
                //    //实测功率在271----274的组件对其Pmax、Voc、Vpm都乘以0.997
                //    else
                //    {
                //        if (ivtest.CoefPM > 271 && ivtest.CoefPM < 274)
                //        {
                //            double rate = 0.997;
                //            ivtest.CoefPM = Math.Round(ivtest.PM * rate, 4);
                //            ivtest.CoefVOC = Math.Round(ivtest.VOC * rate, 4);
                //            ivtest.CoefVPM = Math.Round(ivtest.VPM * rate, 4);

                //        }
                //    }
                //}

                #region
                if (lot.MaterialCode == "1202020013")
                {
                    //实测功率在270.5---271的组件对其Pmax、Voc、Vpm都乘以0.9985
                    if (ivtest.CoefPM > 270.5 && ivtest.CoefPM < 271)
                    {
                        double rate = 0.9985;
                        ivtest.CoefPM = Math.Round(ivtest.PM * rate, 4);
                        ivtest.CoefVOC = Math.Round(ivtest.VOC * rate, 4);
                        ivtest.CoefVPM = Math.Round(ivtest.VPM * rate, 4);


                    }
                    //实测功率在271----275的组件对其Pmax、Voc、Vpm都乘以0.997
                    else
                    {
                        if (ivtest.CoefPM > 271 && ivtest.CoefPM < 275)
                        {
                            double rate = 0.997;
                            ivtest.CoefPM = Math.Round(ivtest.PM * rate, 4);
                            ivtest.CoefVOC = Math.Round(ivtest.VOC * rate, 4);
                            ivtest.CoefVPM = Math.Round(ivtest.VPM * rate, 4);

                        }
                        else
                        {
                            //实测功率在275.5----276的组件对其Pmax、Voc、Vpm都乘以0.9985
                            if (ivtest.CoefPM > 275.5 && ivtest.CoefPM < 276)
                            {
                                double rate = 0.9985;
                                ivtest.CoefPM = Math.Round(ivtest.PM * rate, 4);
                                ivtest.CoefVOC = Math.Round(ivtest.VOC * rate, 4);
                                ivtest.CoefVPM = Math.Round(ivtest.VPM * rate, 4);
                            }
                            else
                            {
                                //实测功率在276----280的组件对其Pmax、Voc、Vpm都乘以0.997
                                if (ivtest.CoefPM > 276 && ivtest.CoefPM < 280)
                                {
                                    double rate = 0.997;
                                    ivtest.CoefPM = Math.Round(ivtest.PM * rate, 4);
                                    ivtest.CoefVOC = Math.Round(ivtest.VOC * rate, 4);
                                    ivtest.CoefVPM = Math.Round(ivtest.VPM * rate, 4);
                                }

                            }
                        }

                    }
                }
                #endregion

                if (lot.MaterialCode == "1202020021")
                {
                    //实测功率在320.5---321的组件对其Pmax、Voc、Vpm都乘以0.9985
                    if (ivtest.CoefPM > 320.5 && ivtest.CoefPM < 321)
                    {
                        double rate = 0.9985;
                        ivtest.CoefPM = Math.Round(ivtest.PM * rate, 4);
                        ivtest.CoefVOC = Math.Round(ivtest.VOC * rate, 4);
                        ivtest.CoefVPM = Math.Round(ivtest.VPM * rate, 4);


                    }
                    //实测功率在321----325的组件对其Pmax、Voc、Vpm都乘以0.997
                    else
                    {
                        if (ivtest.CoefPM > 321 && ivtest.CoefPM < 325)
                        {
                            double rate = 0.997;
                            ivtest.CoefPM = Math.Round(ivtest.PM * rate, 4);
                            ivtest.CoefVOC = Math.Round(ivtest.VOC * rate, 4);
                            ivtest.CoefVPM = Math.Round(ivtest.VPM * rate, 4);

                        }

                    }
                }

                //if (lot.MaterialCode == "1202020024")
                //{
                //    //实测功率在270.5---271的组件对其Pmax、Voc、Vpm都乘以0.9985
                //    if (ivtest.CoefPM > 270.5 && ivtest.CoefPM < 271)
                //    {
                //        double rate = 0.9985;
                //        ivtest.CoefPM = Math.Round(ivtest.PM * rate, 4);
                //        ivtest.CoefVOC = Math.Round(ivtest.VOC * rate, 4);
                //        ivtest.CoefVPM = Math.Round(ivtest.VPM * rate, 4);


                //    }
                //    //实测功率在271----275的组件对其Pmax、Voc、Vpm都乘以0.997
                //    else
                //    {
                //        if (ivtest.CoefPM > 271 && ivtest.CoefPM < 275)
                //        {
                //            double rate = 0.997;
                //            ivtest.CoefPM = Math.Round(ivtest.PM * rate, 4);
                //            ivtest.CoefVOC = Math.Round(ivtest.VOC * rate, 4);
                //            ivtest.CoefVPM = Math.Round(ivtest.VPM * rate, 4);

                //        }

                //    }
                //}
                #region 1202020024料号衰减
                if (lot.MaterialCode == "1202020024")
                {
                    //实测功率在270.5---271的组件对其Pmax、Voc、Vpm都乘以0.9985
                    if (ivtest.CoefPM > 270.5 && ivtest.CoefPM < 271)
                    {
                        double rate = 0.9985;
                        ivtest.CoefPM = Math.Round(ivtest.PM * rate, 4);
                        ivtest.CoefVOC = Math.Round(ivtest.VOC * rate, 4);
                        ivtest.CoefVPM = Math.Round(ivtest.VPM * rate, 4);


                    }
                    //实测功率在271----275的组件对其Pmax、Voc、Vpm都乘以0.997
                    else
                    {
                        if (ivtest.CoefPM > 271 && ivtest.CoefPM < 275)
                        {
                            double rate = 0.997;
                            ivtest.CoefPM = Math.Round(ivtest.PM * rate, 4);
                            ivtest.CoefVOC = Math.Round(ivtest.VOC * rate, 4);
                            ivtest.CoefVPM = Math.Round(ivtest.VPM * rate, 4);

                        }
                        else
                        {
                            //实测功率在275----275.5的组件对其Pmax、Voc、Vpm都乘以0.9985
                            if (ivtest.CoefPM > 275.5 && ivtest.CoefPM < 276)
                            {
                                double rate = 0.9985;
                                ivtest.CoefPM = Math.Round(ivtest.PM * rate, 4);
                                ivtest.CoefVOC = Math.Round(ivtest.VOC * rate, 4);
                                ivtest.CoefVPM = Math.Round(ivtest.VPM * rate, 4);

                            }
                            else
                            {
                                //实测功率在276----280的组件对其Pmax、Voc、Vpm都乘以0.997
                                if (ivtest.CoefPM > 276 && ivtest.CoefPM < 280)
                                {
                                    double rate = 0.997;
                                    ivtest.CoefPM = Math.Round(ivtest.PM * rate, 4);
                                    ivtest.CoefVOC = Math.Round(ivtest.VOC * rate, 4);
                                    ivtest.CoefVPM = Math.Round(ivtest.VPM * rate, 4);

                                }

                            }

                        }
                    }
                }
                #endregion

            }
            catch (Exception err)
            {
                LogHelper.WriteLogError("IVTestDescy>", err);

                result.Code = 1000;
                result.Message += string.Format(StringResource.Error, err.Message);
                result.Detail = err.ToString();
                return result;
            }

            return result;
        }

        /// <summary> 根据属性名称取得相应的属性值 </summary>
        /// <param name="lstRouteStepAttributeValues"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        private string getAttributeValueFromList(IList<RouteStepAttribute> lstRouteStepAttributeValues, string attributeName)
        {
            string strValue = "";
            RouteStepAttribute obj = null;

            var lng = from stepAttr in lstRouteStepAttributeValues.AsEnumerable()
                      where stepAttr.Key.AttributeName == attributeName
                      select stepAttr;

            if (lng != null && lng.Count() > 0)
            {
                obj = lng.FirstOrDefault();
                strValue = obj.Value;
            }

            return strValue;
        }

        /// <summary> 创建批次用料对象 </summary>
        /// <param name="lot">批次对象</param>
        /// <param name="equipment">设备对象</param>
        /// <param name="materialLoadDetail">上料明细对象列表</param>
        /// <param name="transactionKey">事物主键</param>
        /// <param name="quantity">数量</param>
        /// <returns></returns>
        private LotBOM CreateLotBOMObject(int iitemno, TrackOutParameter p, Lot lot, Equipment equipment, MaterialLoadingDetail materialLoadDetail, string transactionKey, double quantity)
        {
            //创建批次用料记录
            LotBOM lotbomObj = new LotBOM()
            {
                Key = new LotBOMKey()
                {
                    LotNumber = lot.Key,                                            //组件批次号
                    MaterialLot = materialLoadDetail.MaterialLot,                   //物料批次号
                    ItemNo = iitemno                                                //项目号                       
                },
                EquipmentCode = equipment.Key,                                      //设备代码
                LineCode = lot.LineCode,                                            //线别代码
                LineStoreName = materialLoadDetail.LineStoreName,                   //线边仓
                MaterialCode = materialLoadDetail.MaterialCode,                     //物料代码
                LoadingKey = materialLoadDetail.Key.LoadingKey,                     //上料单号
                LoadingItemNo = materialLoadDetail.Key.ItemNo,                      //上料项目号
                MaterialName = "",                                                  //物料名称
                SupplierCode = "",                                                  //供应商代码
                SupplierName = "",                                                  //供应商名称
                RouteEnterpriseName = lot.RouteEnterpriseName,                      //工艺流程组
                RouteName = lot.RouteName,                                          //工艺流程
                RouteStepName = lot.RouteStepName,                                  //工序名称
                TransactionKey = transactionKey,                                    //事物主键
                MaterialFrom = EnumMaterialFrom.Loading,                            //物料来源(设备上料)
                Qty = quantity,                                                     //数量
                Creator = p.Creator,                                                //创建人
                CreateTime = DateTime.Now,                                          //创建时间
                Editor = p.Creator,                                                 //编辑人               
                EditTime = DateTime.Now                                             //编辑时间
            };

            return lotbomObj;
        }

        /// <summary> 扣料处理过程 </summary>
        /// <param name="workModel">操作模式 进站、出站</param>
        /// <param name="p">出站参数对象（后期调整！！！）</param>
        /// <param name="equipment">设备对象</param>
        /// <param name="lstLot">批次对象列表</param>
        /// <param name="session">数据连接事物对象</param>
        /// <param name="transactionKey"></param>
        /// <returns></returns>
        private MethodReturnResult DeductMaterial(EnumDataCollectionAction workModel, TrackOutParameter p, Equipment equipment, List<Lot> lstLot, ISession session, string transactionKey)
        {
            DateTime now = DateTime.Now.AddMilliseconds(20);
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            List<MaterialLoadingDetail> lstMaterialLoadingDetailForUpdate = new List<MaterialLoadingDetail>();      //设备上料明细对象列表
            List<LotBOM> lstLotBOMForInsert = new List<LotBOM>();                                                   //批次扣料明细对象列表

            try
            {
                PagingConfig cfg = null;                    //查询条件
                string sMaterialCode = "";                  //物料代码                
                //string sMaterialLot = "";                   //物料批次号
                decimal dMaterialBOMDeduct = 0;             //物料BOM用量
                decimal dMaterialMinUnit = 0;               //物料最小待扣减数量
                decimal dMaterialAvailable = 0;             //可用数量
                decimal dMaterialQuantity = 0;              //物料待扣减数量
                decimal dMaterialResidue = 0;               //尾料数量
                decimal dLoadingDeduct = 0;                 //上料记录扣减数量
                decimal dCurrentQty = 0;                    //当前上料数量
                //bool isFindMaterial = false;                //是否查找到物料扣料记录
                bool iCellSet = false;                      //电池片信息设置标识

                foreach (Lot lot in lstLot)
                {
                    #region 批次扣料
                    //取得批次扣料列表
                    //                    cfg = new PagingConfig()
                    //                    {
                    //                        IsPaging = false,
                    //                        Where = string.Format(@"Key.RouteName = '{0}' 
                    //                                        AND Key.RouteStepName = '{1}'
                    //                                        AND IsDeleted = 0
                    //                                        AND DCType = {2}",
                    //                                        lot.RouteName,
                    //                                        lot.RouteStepName,
                    //                                        (int)workModel),
                    //                        OrderBy = "ParamIndex"
                    //                    };

                    //临时将出站处理所有物料（请注意！！！后期需要调整）
                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"Key.RouteName = '{0}' 
                                        AND Key.RouteStepName = '{1}'
                                        AND IsDeleted = 0",
                                        lot.RouteName,
                                        lot.RouteStepName),
                        OrderBy = "ParamIndex"
                    };

                    IList<RouteStepParameter> lstRouteStepParameter = this.RouteStepParameterDataEngine.Get(cfg);
                    int lotbomItemNo = 0;           //批次用料项目号

                    //需要进行物料扣减
                    if (lstRouteStepParameter.Count > 0)
                    {
                        #region 取得批次用料最大项目号
                        cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format("Key.LotNumber = '{0}' "
                                                    , lot.Key),
                            OrderBy = "Key.ItemNo Desc"
                        };

                        IList<LotBOM> lstLotBom = this.LotBOMDataEngine.Get(cfg);

                        //记录当前最大批次用料项目号
                        if (lstLotBom.Count > 0)
                        {
                            lotbomItemNo = lstLotBom[0].Key.ItemNo;
                        }
                        else
                        {
                            lotbomItemNo = 0;
                        }
                        #endregion

                        if (p.AutoDeductMaterial == true)
                        {
                            #region 自动扣料
                            foreach (RouteStepParameter routeStepParameter in lstRouteStepParameter)
                            {
                                #region 1.取得物料代码及BOM用量\最小扣料数量、物料代码
                                result = GetMaterialBOMAttribute(routeStepParameter.MaterialType, lot, ref dMaterialBOMDeduct, ref dMaterialMinUnit, ref sMaterialCode);

                                if (result.Code > 0)
                                {
                                    return result;
                                }
                                #endregion

                                #region 2.取得设备上料批次列表(按上料先进先出顺序,包含替代料)

                                cfg = new PagingConfig()
                                {
                                    IsPaging = false,
                                    Where = string.Format(@"OrderNumber = '{0}'
                                                        AND ( MaterialCode = '{1}'
                                                            OR EXISTS(SELECT MaterialCode
                                                                        FROM WorkOrderBOM as wbom
                                                                        WHERE wbom.Key.OrderNumber = '{0}'
                                                                        AND wbom.ReplaceMaterial = '{1}'
                                                                        AND wbom.MaterialCode = self.MaterialCode))
                                                        AND CurrentQty > 0
                                                        AND EXISTS(SELECT Key
                                                                    FROM MaterialLoading as p
                                                                    WHERE p.RouteOperationName = '{2}'
                                                                    AND p.EquipmentCode = '{3}'
                                                                    AND p.Key = self.Key.LoadingKey)",
                                                        lot.OrderNumber,                //工单号
                                                        sMaterialCode,                  //物料编码
                                                        lot.RouteStepName,              //当前工序代码             
                                                        equipment.Key),                 //设备代码
                                    OrderBy = "CreateTime Asc,Key.ItemNo Asc"
                                };

                                //获取上料记录
                                IList<MaterialLoadingDetail> lstMaterialLoadingDetail = this.MaterialLoadingDetailDataEngine.Get(cfg);

                                if (lstMaterialLoadingDetail == null || lstMaterialLoadingDetail.Count == 0)
                                {
                                    result.Code = 2005;

                                    //取得物料对象
                                    string sMaterialName = "";
                                    Material material = this.MaterialDataEngine.Get(sMaterialCode);
                                    if (material != null)
                                    {
                                        sMaterialName = material.Name;
                                    }

                                    result.Message = string.Format("当前设备({0})无物料({1},{2})可用上料批次！请上料后操作。",
                                                                    equipment.Key + "[" + equipment.Name + "]",
                                                                    sMaterialCode,
                                                                    sMaterialName);
                                    return result;
                                }
                                #endregion

                                #region 3.按照上料时间(先进先出)进行扣料

                                dMaterialQuantity = dMaterialBOMDeduct;         //物料待扣减数量

                                foreach (MaterialLoadingDetail materialLoadingDetail in lstMaterialLoadingDetail)
                                {
                                    //icount++;

                                    //初始化属性值                                    
                                    dMaterialResidue = 0;         //取得扣料余数（尾料）
                                    dMaterialAvailable = 0;       //本次用量

                                    dCurrentQty = (decimal)materialLoadingDetail.CurrentQty;

                                    //上料满足要求
                                    if (dCurrentQty >= dMaterialQuantity)
                                    {
                                        dMaterialAvailable = dMaterialQuantity;                     //本次用量
                                        dMaterialQuantity = 0;                                      //物料待扣减数量                                         
                                    }
                                    else
                                    {
                                        //不满足上料要求
                                        if (dMaterialMinUnit > 0)  //有最小扣料单位控制
                                        {
                                            //取得本次扣料余数（尾料）
                                            dMaterialResidue = dCurrentQty % dMaterialMinUnit;

                                            dMaterialAvailable = dCurrentQty - dMaterialResidue;       //本次用量
                                            dMaterialQuantity -= dCurrentQty - dMaterialResidue;       //物料待扣减数量
                                        }
                                        else                        //不进行最小扣料单元控制
                                        {
                                            dMaterialAvailable = dCurrentQty - dMaterialResidue;       //本次用量
                                            dMaterialQuantity -= dCurrentQty;                          //物料待扣减数量                                            
                                        }
                                    }

                                    //设置上料明细属性   
                                    dLoadingDeduct = dCurrentQty - (dMaterialAvailable + dMaterialResidue);                 //上料记录扣减数量

                                    materialLoadingDetail.CurrentQty = (double)dLoadingDeduct;                              //扣减上料数据                                    
                                    materialLoadingDetail.SurplusQty = (double)dMaterialResidue;                            //记录尾料
                                    materialLoadingDetail.Editor = p.Creator;                                               //编辑人
                                    materialLoadingDetail.EditTime = now;                                                   //编辑时间

                                    #region 设置电池片效率、花色
                                    if (iCellSet == false)
                                    {
                                        //检查是否需要获取电池片批次信息
                                        if (string.Compare(routeStepParameter.Key.ParameterName, "电池片批号") == 0)
                                        {
                                            MaterialReceiptDetail mReceiptDetail = this.getMaterialReceiptDetail(materialLoadingDetail.MaterialLot, lot.OrderNumber);
                                            if (mReceiptDetail != null)
                                            {
                                                //判断电池片信息是否未录入
                                                if (mReceiptDetail.Attr1 == null || mReceiptDetail.Attr1 == "")
                                                {
                                                    result.Code = 2005;
                                                    result.Message = string.Format("工单({0})中电池批次({1})效率信息未设置！",
                                                                                    lot.OrderNumber,
                                                                                    materialLoadingDetail.MaterialLot);
                                                    return result;
                                                }

                                                if (mReceiptDetail.Attr2 == null || mReceiptDetail.Attr2 == "")
                                                {
                                                    result.Code = 2005;
                                                    result.Message = string.Format("工单({0})中电池批次({1})花色信息未设置！",
                                                                                    lot.OrderNumber,
                                                                                    materialLoadingDetail.MaterialLot);
                                                    return result;
                                                }

                                                lot.Attr1 = mReceiptDetail.Attr1;           //电池片效率
                                                lot.Attr2 = mReceiptDetail.Attr2;           //电池片花色
                                            }
                                            else
                                            {
                                                result.Code = 2005;
                                                result.Message = string.Format("工单({0})中电池批次({1})信息提取失败！",
                                                                                lot.OrderNumber,
                                                                                materialLoadingDetail.MaterialLot);
                                                return result;
                                            }

                                            iCellSet = true;
                                        }
                                    }
                                    #endregion

                                    //新增上料扣料记录对象列表
                                    lstMaterialLoadingDetailForUpdate.Add(materialLoadingDetail);

                                    //若有效扣料本次扣料数大于0，则创建批次扣料记录
                                    if (dMaterialAvailable > 0)
                                    {
                                        //批次扣料记录
                                        lotbomItemNo++;

                                        LotBOM lotBOM = CreateLotBOMObject(lotbomItemNo, p, lot, equipment, materialLoadingDetail, transactionKey, (double)dMaterialAvailable);

                                        //新增批次扣料记录对象列表
                                        lstLotBOMForInsert.Add(lotBOM);
                                    }

                                    //完成扣料退出循环
                                    if (dMaterialQuantity == 0)
                                    {
                                        break;
                                    }
                                }

                                //上料记录不满足需扣料需求
                                if (dMaterialQuantity > 0)
                                {
                                    result.Code = 2006;

                                    //取得物料对象
                                    string sMaterialName = "";
                                    Material material = this.MaterialDataEngine.Get(sMaterialCode);
                                    if (material != null)
                                    {
                                        sMaterialName = material.Name;
                                    }

                                    result.Message = string.Format("当前设备({0})物料({1},{2})数量{3}不满足BOM需求{4}！请上料后操作。",
                                                                    equipment.Name,
                                                                    sMaterialCode,
                                                                    sMaterialName,
                                                                    dMaterialQuantity,
                                                                    dMaterialBOMDeduct);
                                    return result;
                                }

                                #endregion
                            }

                            #endregion
                        }
                        else
                        {
                            #region 手工扣料

                            //循环进行扣料处理
                            foreach (RouteStepParameter routeStepParameter in lstRouteStepParameter)
                            {
                                if (routeStepParameter.ValidateRule != EnumValidateRule.None)
                                {
                                    #region 1.取得物料代码及BOM用量\最小扣料数量、物料代码
                                    result = GetMaterialBOMAttribute(routeStepParameter.MaterialType, lot, ref dMaterialBOMDeduct, ref dMaterialMinUnit, ref sMaterialCode);

                                    if (result.Code > 0)
                                    {
                                        return result;
                                    }
                                    #endregion

                                    //初始化物料待扣减数量
                                    dMaterialQuantity = dMaterialBOMDeduct;

                                    foreach (MaterialConsumptionParameter tp in p.MaterialParamters[lot.Key])
                                    {
                                        if (routeStepParameter.MaterialType == tp.MaterialCode)
                                        {
                                            #region 1.取得批次上料记录
                                            //获取上料记录
                                            cfg = new PagingConfig()
                                            {
                                                IsPaging = false,
                                                Where = string.Format(@"OrderNumber = '{0}'
                                                        AND MaterialLot = '{4}'
                                                        AND ( MaterialCode = '{1}'
                                                            OR EXISTS(SELECT MaterialCode
                                                                        FROM WorkOrderBOM as wbom
                                                                        WHERE wbom.Key.OrderNumber = '{0}'
                                                                        AND wbom.ReplaceMaterial = '{1}'
                                                                        AND wbom.MaterialCode = self.MaterialCode))
                                                        AND CurrentQty > 0
                                                        AND EXISTS(SELECT Key
                                                                    FROM MaterialLoading as p
                                                                    WHERE p.RouteOperationName = '{2}'
                                                                    AND p.EquipmentCode = '{3}'
                                                                    AND p.Key = self.Key.LoadingKey)",
                                                                    lot.OrderNumber,                //工单号
                                                                    sMaterialCode,                  //物料编码
                                                                    lot.RouteStepName,              //当前工序代码             
                                                                    equipment.Key,                  //设备代码
                                                                    tp.MaterialLot),                //物料批次号
                                                OrderBy = "CreateTime Asc,Key.ItemNo Asc"
                                            };

                                            IList<MaterialLoadingDetail> lstMaterialLoadingDetail = this.MaterialLoadingDetailDataEngine.Get(cfg);

                                            if (lstMaterialLoadingDetail == null || lstMaterialLoadingDetail.Count == 0)
                                            {
                                                result.Code = 2005;
                                                result.Message = string.Format("当前设备({0})物料({1},{2})无上料批次({3})！请确认后操作。",
                                                                                equipment.Key + "[" + equipment.Name + "]",
                                                                                sMaterialCode,
                                                                                routeStepParameter.Key.ParameterName,
                                                                                tp.MaterialLot);
                                                return result;
                                            }
                                            #endregion

                                            #region 2.判断数据合理性
                                            //判断剩余物料是否满足
                                            dCurrentQty = (decimal)lstMaterialLoadingDetail[0].CurrentQty;

                                            if (tp.LoadingQty > dCurrentQty)
                                            {
                                                result.Code = 1000;

                                                //取得物料对象
                                                string sMaterialName = "";
                                                Material material = this.MaterialDataEngine.Get(sMaterialCode);
                                                if (material != null)
                                                {
                                                    sMaterialName = material.Name;
                                                }

                                                result.Message = string.Format("当前设备({0})物料({1},{2})上料批次({3})剩余({4})不满足扣料需求！",
                                                                                equipment.Name,
                                                                                sMaterialCode,
                                                                                sMaterialName,
                                                                                tp.MaterialLot,
                                                                                dCurrentQty);

                                                return result;
                                            }

                                            //判断待扣减数量是否超出BOM量
                                            if (tp.LoadingQty > dMaterialQuantity)
                                            {
                                                result.Code = 1000;

                                                //取得物料对象
                                                string sMaterialName = "";
                                                Material material = this.MaterialDataEngine.Get(sMaterialCode);
                                                if (material != null)
                                                {
                                                    sMaterialName = material.Name;
                                                }

                                                result.Message = string.Format("当前设备({0})物料({1},{2})上料批次({3})累计扣料数量({4})大于BOM用量({5})！",
                                                                                equipment.Key + "[" + equipment.Name + "]",
                                                                                sMaterialCode,
                                                                                sMaterialName,
                                                                                tp.MaterialLot,
                                                                                dMaterialBOMDeduct - dMaterialQuantity + tp.LoadingQty,
                                                                                dMaterialBOMDeduct);
                                                return result;
                                            }

                                            //判断是否小于最小扣料单位
                                            if (dMaterialMinUnit > 0)  //有最小扣料单位控制
                                            {
                                                if (dMaterialMinUnit > tp.LoadingQty)
                                                {
                                                    result.Code = 1000;

                                                    //取得物料对象
                                                    string sMaterialName = "";
                                                    Material material = this.MaterialDataEngine.Get(sMaterialCode);
                                                    if (material != null)
                                                    {
                                                        sMaterialName = material.Name;
                                                    }

                                                    result.Message = string.Format("当前设备({0})物料({1},{2})上料批次({3})扣料数量({4})小于最小扣料单位({5})！",
                                                                                    equipment.Key + "[" + equipment.Name + "]",
                                                                                    sMaterialCode,
                                                                                    sMaterialName,
                                                                                    tp.MaterialLot,
                                                                                   tp.LoadingQty,
                                                                                    dMaterialMinUnit);
                                                    return result;
                                                }
                                            }

                                            #endregion

                                            #region 3.扣料处理
                                            dMaterialAvailable = tp.LoadingQty;         //上料量
                                            dMaterialQuantity -= tp.LoadingQty;         //物料待扣减数量                                            

                                            //设置上料明细属性   
                                            dLoadingDeduct = dCurrentQty - dMaterialAvailable;                          //上料记录扣减数量

                                            lstMaterialLoadingDetail[0].CurrentQty = (double)dLoadingDeduct;            //扣减上料数据                                    
                                            lstMaterialLoadingDetail[0].SurplusQty = (double)dMaterialResidue;          //记录尾料
                                            lstMaterialLoadingDetail[0].Editor = p.Creator;                             //编辑人
                                            lstMaterialLoadingDetail[0].EditTime = now;                                 //编辑时间

                                            //新增上料扣料记录对象列表
                                            lstMaterialLoadingDetailForUpdate.Add(lstMaterialLoadingDetail[0]);

                                            #region 设置电池片效率、花色
                                            if (iCellSet == false)
                                            {
                                                //检查是否需要获取电池片批次信息
                                                if (string.Compare(routeStepParameter.Key.ParameterName, "电池片批号") == 0)
                                                {
                                                    MaterialReceiptDetail mReceiptDetail = this.getMaterialReceiptDetail(lstMaterialLoadingDetail[0].MaterialLot, lot.OrderNumber);
                                                    if (mReceiptDetail != null)
                                                    {
                                                        //判断电池片信息是否未录入
                                                        if (mReceiptDetail.Attr1 == null || mReceiptDetail.Attr1 == "")
                                                        {
                                                            result.Code = 2005;
                                                            result.Message = string.Format("工单({0})中电池批次({1})效率信息未设置！",
                                                                                            lot.OrderNumber,
                                                                                            lstMaterialLoadingDetail[0].MaterialLot);
                                                            return result;
                                                        }

                                                        if (mReceiptDetail.Attr2 == null || mReceiptDetail.Attr2 == "")
                                                        {
                                                            result.Code = 2005;
                                                            result.Message = string.Format("工单({0})中电池批次({1})花色信息未设置！",
                                                                                            lot.OrderNumber,
                                                                                            lstMaterialLoadingDetail[0].MaterialLot);
                                                            return result;
                                                        }

                                                        lot.Attr1 = mReceiptDetail.Attr1;           //电池片效率
                                                        lot.Attr2 = mReceiptDetail.Attr2;           //电池片花色
                                                    }
                                                    else
                                                    {
                                                        result.Code = 2005;
                                                        result.Message = string.Format("工单({0})中电池批次({1})信息提取失败！",
                                                                                        lot.OrderNumber,
                                                                                        lstMaterialLoadingDetail[0].MaterialLot);
                                                        return result;
                                                    }

                                                    iCellSet = true;
                                                }
                                            }
                                            #endregion

                                            //若有效扣料本次扣料数大于0，则创建批次扣料记录
                                            if (dMaterialAvailable > 0)
                                            {
                                                //批次扣料记录
                                                lotbomItemNo++;

                                                LotBOM lotBOM = CreateLotBOMObject(lotbomItemNo, p, lot, equipment, lstMaterialLoadingDetail[0], transactionKey, (double)dMaterialAvailable);

                                                //新增批次扣料记录对象列表
                                                lstLotBOMForInsert.Add(lotBOM);
                                            }
                                            #endregion
                                        }
                                    }

                                    if (dMaterialQuantity > 0)
                                    {
                                        result.Code = 2005;
                                        result.Message = string.Format("当前设备({0})物料({1},{2})数量{3}不满足BOM需求{4}！请上料后操作。",
                                                                    equipment.Name,
                                                                    sMaterialCode,
                                                                    routeStepParameter.Key.ParameterName,
                                                                    dMaterialQuantity,
                                                                    dMaterialBOMDeduct);
                                        return result;
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion
                }

                #region 开始事物处理

                //更新上料明细
                foreach (MaterialLoadingDetail materialLoadingDetail in lstMaterialLoadingDetailForUpdate)
                {
                    this.MaterialLoadingDetailDataEngine.Update(materialLoadingDetail, session);
                }

                //批次扣料记录
                foreach (LotBOM lotBOM in lstLotBOMForInsert)
                {
                    this.LotBOMDataEngine.Insert(lotBOM, session);
                }

                #endregion
            }
            catch (Exception err)
            {
                LogHelper.WriteLogError("ExecuteTrackOutBaseFounction>", err);

                result.Code = 1000;
                result.Message += string.Format(StringResource.Error, err.Message);
                result.Detail = err.ToString();

                return result;
            }

            return result;
        }

        /// <summary>
        /// 取得物料BOM消耗量\最小扣料量、物料代码
        /// </summary>
        /// <param name="materialcode">物料代码</param>
        /// <param name="lot">批次对象</param>
        /// <param name="materialAmount">物料消耗量</param>
        /// <param name="minUnit">物料扣料最小数量</param>
        /// <param name="materialCode">物料代码</param>
        /// <returns>1.Code = 0 -  成功  > 0 - 失败</returns>
        private MethodReturnResult GetMaterialBOMAttribute(string materialcode, Lot lot, ref decimal materialAmount, ref decimal minUnit, ref string materialCode)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            try
            {
                PagingConfig cfg = null;                    //查询条件

                if (materialcode == null || materialcode == "")
                {
                    result.Code = 2000;
                    result.Message = "工序属性设置中物料编码为空！";

                    return result;
                }

                //工序BOM后期待完善（敬请期待！）

                //取得工单BOM
                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.OrderNumber='{0}' 
                                        AND MaterialCode LIKE '{1}%'",
                                        lot.OrderNumber,
                                        materialcode)
                };

                IList<WorkOrderBOM> lstWorkOrderBOM = this.WorkOrderBOMDataEngine.Get(cfg);

                if (lstWorkOrderBOM.Count > 0)
                {
                    //存在多条记录
                    if (lstWorkOrderBOM.Count > 1)
                    {
                        result.Code = 2000;

                        //取得物料对象
                        string sMaterialName = "";
                        Material material = this.MaterialDataEngine.Get(materialcode);
                        if (material != null)
                        {
                            sMaterialName = material.Name;
                        }

                        result.Message = string.Format("工单({0})BOM设置中物料编码({1},{2})存在多次设置！",
                                                        lot.OrderNumber,
                                                        materialcode,
                                                        sMaterialName);

                        return result;
                    }

                    materialAmount = (decimal)lstWorkOrderBOM[0].Qty;       //BOM消耗量
                    materialCode = lstWorkOrderBOM[0].MaterialCode;         //物料编码
                    minUnit = (decimal)lstWorkOrderBOM[0].MinUnit;          //物料扣料最小数量
                }
                else
                {
                    //当未设置工单BOM时取得物料BOM
                    //取得工单BOM
                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"Key.MaterialCode = '{0}' 
                                            AND RawMaterialCode LIKE '{1}'",
                                            lot.MaterialCode,
                                            materialcode)
                    };

                    IList<MaterialBOM> lstMaterialBOM = this.MaterialBOMDataEngine.Get(cfg);

                    if (lstMaterialBOM.Count > 0)
                    {
                        //存在多条记录
                        if (lstWorkOrderBOM.Count > 1)
                        {
                            result.Code = 2000;

                            //取得物料对象
                            string sMaterialName = "";
                            Material material = this.MaterialDataEngine.Get(materialcode);
                            if (material != null)
                            {
                                sMaterialName = material.Name;
                            }

                            result.Message = string.Format("产品({0})BOM设置中物料编码({1},{2})存在多次设置！",
                                                            lot.MaterialCode,
                                                            materialcode,
                                                            sMaterialName);

                            return result;
                        }

                        materialAmount = (decimal)lstMaterialBOM[0].Qty;            //BOM消耗量
                        materialCode = lstMaterialBOM[0].RawMaterialCode;           //物料编码
                        minUnit = (decimal)lstMaterialBOM[0].MinUnit;               //物料扣料最小数量
                    }
                    else
                    {
                        result.Code = 2000;

                        //取得物料对象
                        string sMaterialName = "";
                        Material material = this.MaterialDataEngine.Get(materialcode);
                        if (material != null)
                        {
                            sMaterialName = material.Name;
                        }

                        result.Message = string.Format("工单({0})BOM中未设置物料({1},{2})！",
                                                        lot.OrderNumber,
                                                        materialcode,
                                                        sMaterialName);

                        return result;
                    }
                }

                //工序、工单、产品BOM中未设置BOM用量
                if (materialAmount == 0)
                {
                    result.Code = 2000;

                    //取得物料对象
                    string sMaterialName = "";
                    Material material = this.MaterialDataEngine.Get(materialcode);
                    if (material != null)
                    {
                        sMaterialName = material.Name;
                    }

                    result.Message = string.Format("物料{0},{1}未设置BOM用量！",
                                                    materialcode,
                                                    sMaterialName);

                    return result;
                }
            }
            catch (Exception err)
            {
                LogHelper.WriteLogError("ExecuteTrackOutBaseFounction>", err);

                result.Code = 1000;
                result.Message += string.Format(StringResource.Error, err.Message);
                result.Detail = err.ToString();
                return result;
            }

            return result;
        }

        /// <summary> 取得工序属性值 </summary>
        /// <param name="routeName"></param>
        /// <param name="routeStepName"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        private string GetRouteStepAttribute(string routeName, string routeStepName, string attributeName)
        {
            string result = "";

            //获取工序属性
            IList<RouteStepAttribute> lstRouteStepAttribute = new List<RouteStepAttribute>();

            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format("Key.RouteName = '{0}' AND Key.RouteStepName = '{1}' and Key.AttributeName = '{2}'"
                                        , routeName
                                        , routeStepName
                                        , attributeName)
            };

            lstRouteStepAttribute = this.RouteStepAttributeDataEngine.Get(cfg);

            if (lstRouteStepAttribute.Count > 0)
            {
                result = lstRouteStepAttribute[0].Value;
            }

            return result;
        }

        //public MethodReturnResult SetEquipmentState(Equipment equipment)
        //{
        //    MethodReturnResult result = new MethodReturnResult();
        //    ITransaction transactioneqp = null;
        //    ISession session = null;

        //    try
        //    {
        //        session = this.SessionFactory.OpenSession();
        //        transactioneqp = session.BeginTransaction();

        //        //更新设备Transaction信息
        //        this.EquipmentDataEngine.Update(equipment, session);

        //        transactioneqp.Commit();
        //        session.Close();

        //        result.Code = 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        transactioneqp.Rollback();
        //        session.Close();

        //        result.Code = 1000;
        //        result.Message = string.Format(StringResource.Error, ex.Message);
        //        result.Detail = ex.ToString();
        //    }

        //    return result;
        //}

        /// <summary>
        /// 立即将批次状态置为锁定状态
        /// </summary>
        /// <param name="lots">批次对象列表</param>
        /// <param name="bLock">锁定标识</param>
        /// <returns></returns>
        public MethodReturnResult SetLotStateForLock(List<Lot> lots, bool bLock)
        {
            MethodReturnResult result = new MethodReturnResult();
            ITransaction transactioneqp = null;
            ISession session = null;

            try
            {
                session = this.SessionFactory.OpenSession();
                transactioneqp = session.BeginTransaction();

                ////重新提取批次对象
                //Lot lotcur = this.LotDataEngine.Get(lot.Key);

                ////判断批次是否锁定
                //if (bLock)
                //{
                //    foreach (Lot lot in lots)
                //    {
                //        //重新提取批次对象
                //        Lot lotcur = this.LotDataEngine.Get(lot.Key);

                //        if (lotcur.LotState == 1)
                //        {
                //            result.Code = 3000;
                //            result.Message = string.Format("批次（{0}）在加工中，请稍后操作。"
                //                                            , lot.Key);

                //            return result;
                //        }
                //    }
                //}

                //设置状态
                foreach (Lot lot in lots)
                {
                    //重新提取批次对象
                    Lot lotcur = this.LotDataEngine.Get(lot.Key);

                    if (bLock)
                    {
                        if (lotcur.LotState == 1)
                        {
                            result.Code = 3000;
                            result.Message = string.Format("批次（{0}）在加工中，请稍后操作。"
                                                            , lot.Key);

                            return result;
                        }

                        lotcur.LotState = 1;           //临时使用该标识锁定批次
                    }
                    else
                    {
                        lotcur.LotState = 0;           //临时使用该标识锁定批次
                    }

                    //更新设备Transaction信息
                    this.LotDataEngine.Update(lotcur, session);
                }


                transactioneqp.Commit();
                session.Close();

                result.Code = 0;
            }
            catch (Exception ex)
            {
                transactioneqp.Rollback();
                session.Close();

                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }

            return result;
        }
    }
}