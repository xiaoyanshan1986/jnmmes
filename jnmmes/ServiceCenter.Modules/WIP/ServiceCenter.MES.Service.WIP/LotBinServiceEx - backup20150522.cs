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
using ServiceCenter.Common.DataAccess.NHibernate;
using System.ServiceModel.Activation;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.Model.EMS;
using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.EMS;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Model.PPM;
using System.Data;
using ServiceCenter.MES.Model.BaseData;

namespace ServiceCenter.MES.Service.WIP
{
    /// <summary>
    /// 实现批次包装服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public partial class LotBinServiceEx : ILotBinContract,ILotBinCheck,ILotBin
    {

        #region define List of DataEngine
        List<Lot> lstLotDataEngineForUpdate = new List<Lot>();
        List<LotTransaction> lstLotTransactionForInsert = new List<LotTransaction>();
        List<LotTransactionHistory> lstLotTransactionHistoryForInsert = new List<LotTransactionHistory>();
        List<LotTransactionParameter> lstLotTransactionParameterDataEngineForInsert = new List<LotTransactionParameter>();
        List<LotTransactionStep> lstLotTransactionStepDataEngineForInsert = new List<LotTransactionStep>();
        
        //LotTransactionEquipment ,Equipment ,EquipmentStateEvent
        List<LotTransactionEquipment> lstLotTransactionEquipmentForUpdate = new List<LotTransactionEquipment>();
        List<LotTransactionEquipment> lstLotTransactionEquipmentForInsert = new List<LotTransactionEquipment>();

        List<Equipment> lstEquipmentForUpdate = new List<Equipment>();
        List<EquipmentStateEvent> lstEquipmentStateEventForInsert = new List<EquipmentStateEvent>();

        //Package
        List<Package> lstPackageDataForUpdate = new List<Package>();
        List<Package> lstPackageDataForInsert = new List<Package>();

        List<PackageBin> lstPackageBinForUpdate = new List<PackageBin>();
        List<PackageBin> lstPackageBinForInsert = new List<PackageBin>();
        List<PackageDetail> lstPackageDetailForInsert = new List<PackageDetail>();
        List<PackageDetail> lstPackageDetailForUpdate = new List<PackageDetail>();
        List<PackageDetail> lstPackageDetailForDelete = new List<PackageDetail>();

        List<LotTransactionPackage> lstLotTransactionPackageForInsert = new List<LotTransactionPackage>();
        #endregion

        /// <summary>
        /// 操作前检查事件。
        /// </summary>
        public event Func<InBinParameter, MethodReturnResult> CheckEvent;
        /// <summary>
        /// 执行操作时事件。
        /// </summary>
        public event Func<InBinParameter, MethodReturnResult> ExecutingEvent;
        /// <summary>
        /// 操作执行完成事件。
        /// </summary>
        public event Func<InBinParameter, MethodReturnResult> ExecutedEvent;

        /// <summary>
        /// 自定义操作前检查的清单列表。
        /// </summary>
        private IList<ILotBinCheck> CheckList { get; set; }
        /// <summary>
        /// 自定义执行中操作的清单列表。
        /// </summary>
        private IList<ILotBin> ExecutingList { get; set; }
        /// <summary>
        /// 自定义执行后操作的清单列表。
        /// </summary>
        private IList<ILotBin> ExecutedList { get; set; }


        /// <summary>
        /// 注册自定义检查的操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterCheckInstance(ILotBinCheck obj)
        {
            if (this.CheckList == null)
            {
                this.CheckList = new List<ILotBinCheck>();
            }
            this.CheckList.Add(obj);
        }
        /// <summary>
        /// 注册执行中的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutingInstance(ILotBin obj)
        {
            if (this.ExecutingList == null)
            {
                this.ExecutingList = new List<ILotBin>();
            }
            this.ExecutingList.Add(obj);
        }

        /// <summary>
        /// 注册执行完成后的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutedInstance(ILotBin obj)
        {
            if (this.ExecutedList == null)
            {
                this.ExecutedList = new List<ILotBin>();
            }
            this.ExecutedList.Add(obj);
        }

        #region //Define DataAccessEngine
       
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
        ///  批次包装操作数据访问类。
        /// </summary>
        public ILotTransactionPackageDataEngine LotTransactionPackageDataEngine
        {
            get;
            set;
        }


        /// <summary>
        /// 生产线数据访问对象。
        /// </summary>
        public IProductionLineDataEngine ProductionLineDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 区域数据访问对象。
        /// </summary>
        public ILocationDataEngine LocationDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 工序属性数据访问对象。
        /// </summary>
        public IRouteOperationAttributeDataEngine RouteOperationAttributeDataEngine
        {
            get;
            set;
        }

      

        /// <summary>
        /// 包装号生成对象。
        /// </summary>
        public IPackageNoGenerate PackageNoGenerate
        {
            get;
            set;
        }


        /// <summary>
        /// 包装明细数据访问对象。
        /// </summary>
        public ILotBOMDataEngine LotBOMDataEngine
        {
            get;
            set;
        }

  

        /// <summary>
        /// 设备状态事件数据访问类。
        /// </summary>
        public IBinDataEngine BinDataEngine
        {
            get;
            set;
        }


        /// <summary>
        /// 设备状态事件数据访问类。
        /// </summary>
        public IBinRuleDataEngine BinRuleDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 设备状态事件数据访问类。
        /// </summary>
        public IScanDataEngine ScanDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 设备状态事件数据访问类。
        /// </summary>
        public IPackageBinDataEngine PackageBinDataEngine
        {
            get;
            set;
        }


        public IWorkOrderAttributeDataEngine WorkOrderAttributeDataEngine
        {
            get;
            set;
        }


        public IColorTestDataDataEngine ColorTestDataDataEngine
        {
            get;
            set;
        }

       

        #endregion

        /// <summary>
        /// 操作前检查。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected virtual MethodReturnResult OnCheck(InBinParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (this.CheckEvent != null)
            {
                foreach (Func<InBinParameter, MethodReturnResult> d in this.CheckEvent.GetInvocationList())
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
                foreach (ILotBinCheck d in this.CheckList)
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
        protected virtual MethodReturnResult OnExecuting(InBinParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (this.ExecutingEvent != null)
            {
                foreach (Func<InBinParameter, MethodReturnResult> d in this.ExecutingEvent.GetInvocationList())
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
                foreach (ILotBin d in this.ExecutingList)
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
        protected virtual MethodReturnResult OnExecuted(InBinParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (this.ExecutedEvent != null)
            {
                foreach (Func<InBinParameter, MethodReturnResult> d in this.ExecutedEvent.GetInvocationList())
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
                foreach (ILotBin d in this.ExecutedList)
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
        public LotBinServiceEx()
        {
            this.RegisterCheckInstance(this);
            this.RegisterExecutedInstance(this);
            //this.PackageNoGenerate = this;

        }


        /// <summary>
        /// 批次包装操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>
        /// 代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult ILotBinContract.InBin(InBinParameter p)
        {

            LogHelper.WriteLogError(string.Format("InBin:Start> Lot_Number:{0}|| Scan_IP:{1}||Bin_No:{2}", p.ScanLotNumber, p.ScanIP,p.BinNo));

            MethodReturnResult result = new MethodReturnResult();
            if (p == null)
            {
                result.Code = 24;
                result.Detail = "24";
                result.Message = "Input parameter is null";
                return result;
            }
            try
            {
                if (string.IsNullOrEmpty(p.BinNo))
                {
                    result.Code = 0;
                    result.Message = string.Format("批次{0}的Bin号为空", p.BinNo);
                    return result;
                }
                if (p.BinNo == "24")
                {
                    result.Code = 0;
                    result.Message = string.Format("批次{0}的传入Bin号为24,忽略此批次号", p.ScanLotNumber);
                    return result;
                }

                if (string.IsNullOrEmpty(p.ScanLotNumber) || p.ScanLotNumber.Trim().Length == 0)
                {
                    result.Code = 0;
                    result.Message = string.Format("批次号为空", p.ScanLotNumber);
                    return result;
                }
                
                //操作前检查。
                result = this.OnCheck(p);
                if (result.Code > 0)
                {
                    return result;
                }
                
                result = this.OnExecuted(p);
                if (result.Code > 0)
                {
                    result.Detail = "24";
                    return result;
                }              
            }
            catch (Exception ex)
            {
                result.Code = 24;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = "24";
            }
            LogHelper.WriteLogError(string.Format("InBin:End> Lot_Number:{0}|| Scan_IP:{1}||Bin_No:{2}||Message:{3}", p.ScanLotNumber, p.ScanIP, p.BinNo,result.Message));

            return result;
        }

        /*
        /// <summary>
        /// 批次包装操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>
        /// 代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult ILotBinContract.ChkBin(InBinParameter p)
        {

            LogHelper.WriteLogError(string.Format("ChkBin:Start> Lot_Number:{0}|| Scan_IP:{1}", p.ScanLotNumber, p.ScanIP));

            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0,
                Detail = "24"
            };

            try
            {
                //result = this.OnCheck(p);
                //if (result.Code > 0)
                //{
                //    if (result.Message != "##OK##")
                //    {
                //        result.Detail = "24";
                //    }
                //    return result;
                //}

                result = CheckLotForBin(p);
                if(result.Code>0)
                {
                    result.Detail = "24";
                }
            }
            catch (Exception ex)
            {
                result.Code = 24;
                result.Message = ex.Message;
            }
            LogHelper.WriteLogError(string.Format("ChkBin:End> Lot_Number:{0}|| Scan_IP:{1}||Bin_No:{2}||Message:{3}", p.ScanLotNumber, p.ScanIP, result.Detail,result.Message));

            return result;
        }
        */

        /// <summary>
        /// 组件路径检查。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        MethodReturnResult ILotBinContract.PathCheck(InBinParameter p)
        {

            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (string.IsNullOrEmpty(p.ScanLotNumber))
            {
                result.Code = 1002;
                result.Message = string.Format("{0} {1}"
                                                , "组件序列号为空"
                                                , StringResource.ParameterIsNull);
                return result;
            }

            string lotNumber = p.ScanLotNumber;
            Lot lot = this.LotDataEngine.Get(lotNumber);
            p.EquipmentCode = lot.EquipmentCode;

            //判定是否存在批次记录。
            if (lot == null || lot.Status == EnumObjectStatus.Disabled)
            {
                result.Code = 1002;
                result.Message = string.Format("批次（{0}）不存在。", lotNumber);
                return result;
            }
            //包装线
            p.PackageLine = lot.LineCode;


            //批次已完成。
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
                result.Message = string.Format("批次（{0}）已删除。", lotNumber);
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

            //得到Lot正确的Bin信息
            //获取IV测试数据。
            PagingConfig cfg = new PagingConfig()
            {
                PageNo = 0,
                PageSize = 1,
                Where = string.Format("Key.LotNumber='{0}' AND IsDefault=1", lotNumber)
            };
            IList<IVTestData> lstTestData = this.IVTestDataDataEngine.Get(cfg);
            //检查批次特性和包装特性是否匹配。
            string powersetCode = string.Empty;
            int powersetCodeItemNo = -1;
            string powersetSubCode = string.Empty;
            if (lstTestData.Count > 0)
            {
                powersetCode = lstTestData[0].PowersetCode;
                powersetCodeItemNo = lstTestData[0].PowersetItemNo ?? -1;
                powersetSubCode = lstTestData[0].PowersetSubCode;
            }
            else
            {
                result.Code = 1010;
                result.Message = string.Format("{0} 不存在IV测试数据。"
                                                , lotNumber);
                return result;
            }

            //Lot的颜色
            cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format("Key.LotNumber ='{0}'", lotNumber),
                OrderBy = "Key.InspectTime desc "

            };
            string color = lot.Color;
            IList<ColorTestData> lstColorTestData = ColorTestDataDataEngine.Get(cfg);
            if (lstColorTestData != null && lstColorTestData.Count > 0)
            {
                ColorTestData colorTestData = lstColorTestData.FirstOrDefault();
                color = colorTestData.InspctResult;
            }

            //获取对应的Bin信息
            cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format(@"Key.PackageLine='{0}' AND Key.PsCode='{1}' AND Key.PsItemNo='{2}'  
                      AND Key.PsSubCode='{3}' AND Key.Color='{4}'
                      AND ( Key.WorkOrderNumber='{5}' or Key.WorkOrderNumber='{6}')",
                   p.PackageLine, powersetCode, powersetCodeItemNo, powersetSubCode, color, lot.OrderNumber, "*"),
                OrderBy = " Key.WorkOrderNumber "

            };
            //
            IList<BinRule> lstBinRules = this.BinRuleDataEngine.Get(cfg);
            string strBinNo = "";
            if (lstBinRules.Count > 0)
            {
                strBinNo = lstBinRules[0].Key.BinNo;
              //  p.BinNo = strBinNo;
                result.Code = int.Parse(strBinNo);
                result.Message = string.Format("{0}对应的Bin号{1}", lotNumber, strBinNo);
                return result;
            }
            else
            {
                result.Code = 1010;
                result.Message = string.Format("{0} 找不到对应的Bin", lotNumber);
                return result;
            }
            return result;
        }
  
        /// <summary>
        /// 操作前检查。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        MethodReturnResult ILotBinCheck.Check(InBinParameter p)
        {
            
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (string.IsNullOrEmpty(p.ScanLotNumber) || p.ScanLotNumber == "")
            {
                result.Code = 1002;
                result.Message = string.Format("{0}"
                                                , "组件序列号为空");
                result.Detail = "24";
                return result;
            }
             
            //if (string.IsNullOrEmpty(p.BinNo))
            //{
            //    result.Code = 1001;
            //    result.Message = string.Format("批次{0}的Bin号为空", p.BinNo);
            //    return result;
            //}

            if (p.BinNo=="24")
            {
                result.Code = 1001;
                result.Message = string.Format("批次{0}的传入Bin号为24,忽略此批次号", p.ScanLotNumber);
                return result;
            }

            if (string.IsNullOrEmpty(p.ScanIP))
            {
                result.Code = 1001;
                result.Message = string.Format("批次{0}的扫描读头的IP地址为空"
                                                , p.ScanLotNumber);
                result.Detail = "29";
                return result;
            }

            //操作前检查读头信息
            // Scan 读头的代码换成设备的
            //Scan scan = this.ScanDataEngine.Get(p.ScanNo);
            //Scan scan = this.ScanDataEngine.Get(p.ScanIP);
         
            Equipment equipment = this.EquipmentDataEngine.Get(p.ScanIP);
            if (equipment == null)
            {
                result.Code = 1001;
                result.Message = string.Format("读头编号{0}在数据库中不存在"
                                                , p.ScanIP);
                result.Detail = "24";
                return result;
            }
            //设置读头的包装线
            p.PackageLine = equipment.LineCode;

            
            string lotNumber = p.ScanLotNumber;
            Lot lot = this.LotDataEngine.Get(lotNumber);
                        
            //判定是否存在批次记录。
            if (lot == null || lot.Status == EnumObjectStatus.Disabled)
            {
                result.Code = 1002;
                result.Message = string.Format("批次（{0}）不存在。", lotNumber);
                result.Detail = "24";
                return result;
            }
            p.EquipmentCode = lot.EquipmentCode;

            PagingConfig cfg = new PagingConfig();
            //判断Lot是否已入Package
            string strPackageNo = lot.PackageNo;
            if (string.IsNullOrEmpty( strPackageNo)==false && strPackageNo.Length >0)
            {
                cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.PackageNo='{0}'" , strPackageNo)
                };
                IList<PackageBin> lstPackageBin = PackageBinDataEngine.Get(cfg);
                PackageBin packageBin = lstPackageBin.FirstOrDefault();
                if(packageBin!=null)
                {
                    string strBinNO = packageBin.Key.BinNo;
                    int code = 24;
                    if (int.TryParse(strBinNO,out code))
                    {
                        result.Code = code;
                        result.Message = "##OK##";
                        result.Detail = strBinNO;
                        return result;
                    }
                }
            }

            //批次需要已进站 
            if (lot.StateFlag == EnumLotState.WaitTrackIn)
            {
                result.Code = 1003;
                result.Message = string.Format("批次（{0}）还未进工序（{1}），请先做进站作业。", lotNumber, lot.RouteStepName);
                return result;
            }

            //批次已完成。
            //if (lot.StateFlag == EnumLotState.Finished)
            //{
            //    result.Code = 1003;
            //    result.Message = string.Format("批次（{0}）已完成。", lotNumber);
            //    return result;
            //}

            //批次已结束
            if (lot.DeletedFlag == true)
            {
                result.Code = 1004;
                result.Message = string.Format("批次（{0}）已删除。", lotNumber);
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

            //检查工序是否是包装工序。
            bool isPackageOperation = false;
            RouteOperationAttribute roAttr = this.RouteOperationAttributeDataEngine.Get(new RouteOperationAttributeKey()
            {
                RouteOperationName = lot.RouteStepName,
                AttributeName = "IsPackageOperation"
            });

            //如果没有设置为包装工序，则直接返回。
            if (roAttr == null
                || !bool.TryParse(roAttr.Value, out isPackageOperation)
                || isPackageOperation == false)
            {
                result.Code = 1009;
                result.Message = string.Format("{0} 非包装工序，请确认。"
                                                , lot.RouteStepName);
                return result;
            }

            //得到Lot正确的Bin信息

            //获取IV测试数据。
            cfg = new PagingConfig()
            {
                PageNo = 0,
                PageSize = 1,
                Where = string.Format("Key.LotNumber='{0}' AND IsDefault=1", lotNumber)
            };
            IList<IVTestData> lstTestData = this.IVTestDataDataEngine.Get(cfg);
            //检查批次特性和包装特性是否匹配。
            string powersetCode = string.Empty;
            int powersetCodeItemNo = -1;
            string powersetSubCode = string.Empty;
            if (lstTestData.Count > 0)
            {
                powersetCode = lstTestData[0].PowersetCode;
                powersetCodeItemNo = lstTestData[0].PowersetItemNo ?? -1;
                powersetSubCode = lstTestData[0].PowersetSubCode;
            }else
            {
                result.Code = 1010;
                result.Message = string.Format("{0} 不存在IV测试数据。"
                                                , lotNumber);
                return result;
            }

            //Lot的颜色
            string color = lot.Color;
           
            //获取对应的Bin信息
            cfg = new PagingConfig()
            {
                IsPaging=false,
                Where = string.Format(@"Key.PackageLine='{0}' AND Key.PsCode='{1}' AND Key.PsItemNo='{2}'  
                      AND Key.PsSubCode='{3}' AND Key.Color='{4}'
                      AND ( Key.WorkOrderNumber='{5}' or Key.WorkOrderNumber='{6}')", 
                   p.PackageLine, powersetCode, powersetCodeItemNo, powersetSubCode,color,lot.OrderNumber,"*"),
                OrderBy = " Key.WorkOrderNumber "
            };
            //
            IList<BinRule> lstBinRules = this.BinRuleDataEngine.Get(cfg);
            string strBinNo = "";
            if (lstBinRules.Count>0)
            {
                strBinNo=lstBinRules[0].Key.BinNo;
                if(string.Compare(p.BinNo,strBinNo,true)!=0)
                {
                    result.Message = string.Format("批次{0} 应该入{1}Bin ，机器臂入{2}Bin", lotNumber,strBinNo,p.BinNo);
                    LogHelper.WriteLogError(result.Message);
                }
                //p.BinNo = strBinNo;
            }
            else
            {
                result.Message = string.Format("批次{0} 应该入异常Bin ，机器臂入{2}Bin", lotNumber, strBinNo, p.BinNo);
                LogHelper.WriteLogError(result.Message);

                return result;
            }

            #region //Common Remark

            /*
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
               if (lot.PackageFlag==true)
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
           }*/
            #endregion

           
            return result;
        }
        /// <summary>
        /// 执行操作。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        MethodReturnResult ILotBin.Execute(InBinParameter p)
        {
            
            DateTime now = DateTime.Now;
            PagingConfig cfg = null;
       
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0,
                Detail = p.BinNo
            };
            string lotNumber = p.ScanLotNumber;
            string strNewPackageNo = "";
            int nToBeBinQuantity = 0;

            cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format(@"Key.ObjectNumber='{0}' ", lotNumber),
            };
            IList<PackageDetail> lstObj = this.PackageDetailDataEngine.Get(cfg);
            if (lstObj != null && lstObj.Count > 0)
            {
                result.Code = 1011;
                result.Message = string.Format("批次（{0}）已存在在托号（{1}）中!"
                                                , lotNumber
                                                , lstObj[0].Key.PackageNo);
                return result;
            }
            lstObj = null;
            Lot lot = this.LotDataEngine.Get(lotNumber);

            //检查BIN对应的PackageNo 
            cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format(@"Key.BinNo='{0}' And Key.PackageLine ='{1}' AND BinPackaged=0 ", p.BinNo, p.PackageLine),
                OrderBy = ""
            };
            IList<PackageBin> lstPackageBin = PackageBinDataEngine.Get(cfg);
            string packageNo = "";
            bool blCreatePackageFlag = false;
            bool blPackageFinished = false;
            int nBinQty = 0;
            int nBinQtyMax = 29;
            PackageBin packageBin = null;
            PackageBin packageBinForUpdate = null;
            PackageBin packageBinForInsert = null;
            if (lstPackageBin != null && lstPackageBin.Count>0)
            {
                packageBin  = lstPackageBin.FirstOrDefault();
                packageNo   = packageBin.Key.PackageNo;
                nBinQty     = packageBin.BinQty;
                p.PackageNo = packageNo;
                //----------------------------------------------------------
                packageBinForUpdate = packageBin.Clone() as PackageBin;
                packageBinForUpdate.BinQty = packageBinForUpdate.BinQty + 1;
                packageBinForUpdate.EditTime = now;
                nToBeBinQuantity = packageBinForUpdate.BinQty;

                if (packageBin.BinMaxQty == packageBinForUpdate.BinQty)
                {
                    packageBinForUpdate.BinPackaged = EnumBinPackaged.Finished;
                    p.IsFinishPackage = true;
                    blPackageFinished = true;
                }
            }
            else
            {
                //Generate new PackageNo
                blCreatePackageFlag = true;
            }

            if(blCreatePackageFlag)
            {
                packageNo = PackageNoGenerate.Generate(p.ScanLotNumber, false);
                strNewPackageNo = packageNo;
                //packageNo = Generate(p.ScanLotNumber, false);
                p.PackageNo = packageNo;

                nBinQty = 1;
                nBinQtyMax = 29;
                nToBeBinQuantity = 1;

                CommonObjectDataEngine<WorkOrderRule, WorkOrderRuleKey> commonObjectDataEngine =
                        new CommonObjectDataEngine<WorkOrderRule, WorkOrderRuleKey>(LotDataEngine.SessionFactory);
                WorkOrderRuleKey workOrderRuleKey = new WorkOrderRuleKey
                {
                    OrderNumber = lot.OrderNumber,
                    MaterialCode = lot.MaterialCode
                };

                WorkOrderRule workOrderRule = commonObjectDataEngine.Get(workOrderRuleKey);
                if (workOrderRule != null && workOrderRule.FullPackageQty!=null)
                {
                    int.TryParse(workOrderRule.FullPackageQty.ToString(), out nBinQtyMax);
                }

                //WorkOrderAttributeKey workOrderAttributeKey = new WorkOrderAttributeKey
                //{
                //    OrderNumber = lot.OrderNumber,
                //    AttributeName = "FullPackageQuantity"
                //};
                //WorkOrderAttribute  workOrderAttribute =this.WorkOrderAttributeDataEngine.Get(workOrderAttributeKey);
                //if (workOrderAttribute != null && string.IsNullOrEmpty(workOrderAttribute.AttributeValue)==false)
                //{
                //    int.TryParse(workOrderAttribute.AttributeValue, out nBinQtyMax);
                //}

                PackageBinKey packageBinKey = new PackageBinKey
                {
                    PackageLine = p.PackageLine,
                    BinNo =p.BinNo,
                    PackageNo= packageNo                    
                };

                packageBinForInsert = new PackageBin
                {
                    Key = packageBinKey,                    
                    BinQty =1,
                    BinMaxQty = nBinQtyMax,
                    BinPackaged = EnumBinPackaged.UnFinished,
                    BinState =1,
                    CreateTime = now,
                    Creator = p.Creator,
                    Editor = p.Creator,
                    EditTime = now
                };
            }

            p.TransactionKeys = new Dictionary<string, string>();

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
            lstPackageBinForUpdate = new List<PackageBin>();
            lstPackageBinForInsert = new List<PackageBin>();

            lstPackageDetailForInsert = new List<PackageDetail>();
            lstLotTransactionPackageForInsert = new List<LotTransactionPackage>();
            #endregion

            if(blCreatePackageFlag)
            {
                lstPackageBinForInsert.Add(packageBinForInsert);
            }
            if(packageBinForUpdate!=null)
            { 
                lstPackageBinForUpdate.Add(packageBinForUpdate);
            }
            
            Package packageObj = this.PackageDataEngine.Get(p.PackageNo);
            Package packageUpdate = null;
            //更新包装数据。
            if (packageObj != null)
            {
                strNewPackageNo = packageObj.Key;
                packageUpdate = packageObj.Clone() as Package;
                packageUpdate.Editor = p.Creator;
                packageUpdate.EditTime = now;
                packageUpdate.IsLastPackage = p.IsLastestPackage;
                if (p.IsFinishPackage)
                {
                    packageUpdate.PackageState = EnumPackageState.Packaged;
                }
            }

            string strSupplierCodeForLot = "";
            //string strSupplierCodeForPackage = "";
        
            cfg = new PagingConfig()
            {
                IsPaging =false,
                Where = string.Format(@"Key.LotNumber='{0}' and SupplierCode in ('{1}','{2}') and MaterialName like '%{3}%'", lotNumber, "010022", "010035", "电池片"),
                OrderBy = ""
            };

            IList<LotBOM> lstLotBom = this.LotBOMDataEngine.Get(cfg);
            if(lstLotBom!=null && lstLotBom.Count>0)
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

                if (String.IsNullOrEmpty(packageUpdate.SupplierCode)==true ||  packageUpdate.SupplierCode=="")
                {
                    packageUpdate.SupplierCode = strSupplierCodeForLot;
                }
                else
                {
                    bool blChkSupplierCode = false;
                        //检查 packageNO
                    if (String.IsNullOrEmpty(strSupplierCodeForLot)==false && strSupplierCodeForLot!="" )
                    {
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
                            if (string.Compare(packageUpdate.SupplierCode.Trim(), strSupplierCodeForLot.Trim(), true) != 0)
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

            if(lotUpdate.StateFlag == EnumLotState.WaitTrackIn)
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

                bool blLogEquipment = true;
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
                UndoFlag = true,
                UndoTransactionKey = null
            };
            //暂时忽略打包的事物,事物设置为撤销
            lstLotTransactionForInsert.Add(transObj);
            //this.LotTransactionDataEngine.Insert(transObj);
            //新增批次历史记录。
            lotHistory = new LotTransactionHistory(transactionKey, lot);
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
                  

            if (packageUpdate!=null)
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
                            strNewPackageNo = GenerateEx(lot.Key, session);
                            blContinue = PackageDataEngine.IsExists(strNewPackageNo, session);
                        }
                        package.Key = strNewPackageNo;
                    }
                    this.PackageDataEngine.Insert(package, session);
                }

                foreach (PackageBin obj in lstPackageBinForUpdate)
                {
                    this.PackageBinDataEngine.Update(obj, session);
                }

                foreach (PackageBin obj in lstPackageBinForInsert)
                {
                    PackageBinKey packageBinKey = new PackageBinKey
                    {
                        PackageLine = p.PackageLine,
                        BinNo = p.BinNo,
                        PackageNo = strNewPackageNo
                    };
                    obj.Key = packageBinKey;
                    this.PackageBinDataEngine.Insert(obj, session);
                }

                foreach (PackageDetail packageDetail1 in lstPackageDetailForInsert)
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
                foreach (Lot lot1 in lstLotDataEngineForUpdate)
                {
                    lot1.PackageNo = strNewPackageNo;
                    this.LotDataEngine.Update(lot1, session);
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

              
                if(p.IsFinishPackage)
                {
                    result.Message = string.Format("是否满托{0}:Bin编号{1}:当前Bin数量{2}", "1", p.BinNo, nToBeBinQuantity);
                }
                else
                {
                    result.Message = string.Format("是否满托{0}:Bin编号{1}:当前Bin数量{2}", "0", p.BinNo, nToBeBinQuantity);
                }

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
                result.Detail = "24";
                return result;
            }
            return result;
        }

        public MethodReturnResult<IList<PackageBin>> QueryBinListFromPackageLine(string packageLine)
        {
            DateTime now = DateTime.Now;
            PagingConfig cfg = null;

            MethodReturnResult<IList<PackageBin>> result = new MethodReturnResult<IList<PackageBin>>
            {
                Code = 0,
            };

            cfg = new PagingConfig
            {
                IsPaging=false,
                Where = string.Format(" PackageLine ='{0}' ", packageLine),
                OrderBy = "Key "
            };
            IList<Bin> lstBin = BinDataEngine.Get(cfg);

            cfg = new PagingConfig
            {
                IsPaging=false,
                Where = string.Format(" Key.PackageLine ='{0}' AND BinPackaged=0 ", packageLine),
                OrderBy = "Key.BinNo "
            };

            IList<PackageBin> lstPackageBin = PackageBinDataEngine.Get(cfg);
            List<Bin> lstBinToAdd = new List<Bin>();
            foreach (Bin oBin in lstBin)
            {
                var q =
                    from c in lstPackageBin
                    where c.Key.BinNo == oBin.Key
                    select c;

                if (q.Count()==0)
                {
                    lstBinToAdd.Add(oBin);
                }
            }

            if(lstBinToAdd!=null && lstBinToAdd.Count>0)
            {
                foreach(Bin oBin in lstBinToAdd)
                {
                    PackageBinKey packageBinKey = new PackageBinKey
                    {
                        BinNo = oBin.Key,
                        PackageLine = oBin.PackageLine,
                        PackageNo = ""
                    };
                    PackageBin packageBin = new PackageBin();
                    packageBin.Key = packageBinKey;
                    packageBin.BinQty = 0;
                    lstPackageBin.Add(packageBin);
                }
            }
            result.Data = lstPackageBin;
            return result;
        }

        public MethodReturnResult CheckLotForBin(InBinParameter p)
        {

            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0,
                Detail = "24"
            };

            if (string.IsNullOrEmpty(p.ScanLotNumber) || p.ScanLotNumber == "")
            {
                result.Code = 1002;
                result.Message = string.Format("{0} "
                                                , "组件序列号为空");
                return result;
            }

            if (string.IsNullOrEmpty(p.ScanIP))
            {
                result.Code = 1001;
                result.Message = string.Format("批次{0}的读头IP地址为空"
                                                , p.ScanLotNumber);
                return result;
            }

            //操作前检查读头信息
            Equipment equipment = this.EquipmentDataEngine.Get(p.ScanIP);
            if (equipment == null)
            {
                result.Code = 1001;
                result.Message = string.Format("读头编号{0}在数据库中不存在"
                                                , p.ScanIP);
                return result;
            }
            //设置读头的包装线
            p.PackageLine = equipment.LineCode;

            
            string lotNumber = p.ScanLotNumber;
            Lot lot = this.LotDataEngine.Get(lotNumber);

            //判定是否存在批次记录。
            if (lot == null || lot.Status == EnumObjectStatus.Disabled)
            {
                result.Code = 1002;
                result.Message = string.Format("批次（{0}）不存在。", lotNumber);
                return result;
            }
            p.EquipmentCode = lot.EquipmentCode;

            PagingConfig cfg = new PagingConfig();
            //判断Lot是否已入Package
            string strPackageNo = lot.PackageNo;
            if (string.IsNullOrEmpty(strPackageNo) == false && strPackageNo.Length > 0)
            {
                cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.PackageNo='{0}'", strPackageNo)
                };
                IList<PackageBin> lstPackageBin = PackageBinDataEngine.Get(cfg);
                PackageBin packageBin = lstPackageBin.FirstOrDefault();
                if (packageBin != null)
                {
                    string strBinNO = packageBin.Key.BinNo;
                    int code = 24;
                    if (int.TryParse(strBinNO, out code))
                    {
                        result.Code = code;
                        result.Message = "##OK##";
                        result.Detail = strBinNO;
                        return result;
                    }
                }
            }
            //批次需要已进站 
            if (lot.StateFlag == EnumLotState.WaitTrackIn)
            {
                result.Code = 1003;
                result.Message = string.Format("批次（{0}）还未进工序（{1}），请先做进站作业。", lotNumber, lot.RouteStepName);
                return result;
            }

            //批次已完成。
            //if (lot.StateFlag == EnumLotState.Finished)
            //{
            //    result.Code = 1003;
            //    result.Message = string.Format("批次（{0}）已完成。", lotNumber);
            //    return result;
            //}

            //批次已结束
            if (lot.DeletedFlag == true)
            {
                result.Code = 1004;
                result.Message = string.Format("批次（{0}）已删除。", lotNumber);
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

            //判断Lot的等级是否是A级
            if (string.IsNullOrEmpty(lot.Grade) || lot.Grade.ToUpper()!="A")
            {
                result.Code = 1006;
                result.Message = string.Format("批次（{0}）不是A级。", lotNumber);
                return result;
            }

            //检查工序是否是包装工序。
            bool isPackageOperation = false;
            RouteOperationAttribute roAttr = this.RouteOperationAttributeDataEngine.Get(new RouteOperationAttributeKey()
            {
                RouteOperationName = lot.RouteStepName,
                AttributeName = "IsPackageOperation"
            });

            //如果没有设置为包装工序，则直接返回。
            if (roAttr == null
                || !bool.TryParse(roAttr.Value, out isPackageOperation)
                || isPackageOperation == false)
            {
                result.Code = 1009;
                result.Message = string.Format("{0} 非包装工序，请确认。"
                                                , lot.RouteStepName);

                return result;
            }


            //获取IV测试数据。
            cfg = new PagingConfig()
            {
                PageNo = 0,
                PageSize = 1,
                Where = string.Format("Key.LotNumber='{0}' AND IsDefault=1", lotNumber)
            };
            IList<IVTestData> lstTestData = this.IVTestDataDataEngine.Get(cfg);
            //检查批次特性和包装特性是否匹配。
            string powersetCode = string.Empty;
            int powersetCodeItemNo = -1;
            string powersetSubCode = string.Empty;
            if (lstTestData.Count > 0)
            {
                powersetCode = lstTestData[0].PowersetCode;
                powersetCodeItemNo = lstTestData[0].PowersetItemNo ?? -1;
                powersetSubCode = lstTestData[0].PowersetSubCode;
            }
            else
            {
                result.Code = 1010;
                result.Message = string.Format("{0} 不存在IV测试数据。"
                                                , lotNumber);
                return result;
            }

            //Lot的颜色
            string color = lot.Color;

            //获取对应的Bin信息
            cfg = new PagingConfig()
            {
                IsPaging = false,
//                Where = string.Format(@"Key.PackageLine='{0}' AND Key.Grade='{7}' AND Key.PsCode='{1}' AND Key.PsItemNo='{2}'  
//                      AND Key.PsSubCode='{3}' AND Key.Color='{4}'
//                      AND ( Key.WorkOrderNumber='{5}' or Key.WorkOrderNumber='{6}')",
//                   p.PackageLine, powersetCode, powersetCodeItemNo, powersetSubCode, color, lot.OrderNumber, "*",lot.Grade),

                Where = string.Format(@"Key.PackageLine='{0}' AND Key.PsCode='{1}' AND Key.PsItemNo='{2}'  
                      AND Key.PsSubCode='{3}' AND Key.Color='{4}'
                      AND ( Key.WorkOrderNumber='{5}' or Key.WorkOrderNumber='{6}')",
                   p.PackageLine, powersetCode, powersetCodeItemNo, powersetSubCode, color, lot.OrderNumber, "*"),
                OrderBy = " Key.WorkOrderNumber "
            };
            //
            IList<BinRule> lstBinRules = this.BinRuleDataEngine.Get(cfg);
            string strBinNo = "";
            if (lstBinRules.Count > 0)
            {
                strBinNo = lstBinRules[0].Key.BinNo;
                p.BinNo = strBinNo;
                result.Detail = strBinNo;
            }
            else
            {
                result.Code = 1010;
                result.Message = string.Format("{0} 找不到对应的Bin", lotNumber);
                return result;
            }
            return result;

        }

        /// <summary>
        /// 自动生成包装号。
        /// </summary>
        /// <param name="lotNumber">批次号。</param>
        /// <param name="isLastestPackage">尾包？</param>
        /// <returns>包装号。</returns>
        public string Generate(string lotNumber, bool isLastestPackage)
        {
            Lot obj = this.LotDataEngine.Get(lotNumber);
            if (obj.PackageFlag == false)
            {
                string prefixPackageNo = string.Format("{0}-{1}", obj.OrderNumber, isLastestPackage?"L":string.Empty);
                int seqNo = 1;
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format(@"Key LIKE '{0}%'"
                                            , prefixPackageNo),
                    OrderBy = "Key DESC"
                };

                IList<Package> lstPackage= this.PackageDataEngine.Get(cfg);
                if (lstPackage.Count > 0)
                {
                    string maxSeqNo = lstPackage[0].Key.Replace(prefixPackageNo, "");
                    if (int.TryParse(maxSeqNo, out seqNo))
                    {
                        seqNo = seqNo + 1;
                    }
                }

                return string.Format("{0}{1}", prefixPackageNo, isLastestPackage?seqNo.ToString("000"):seqNo.ToString("0000"));
            }
            else
            {
                return obj.PackageNo;
            }
        }


        public string GetBinNo(string lotNumber, string scanNo)
        {
            string strBinNo = "";

            
            return strBinNo;
        }


        //MethodReturnResult<string> ILotPackageContract.Generate(string lotNumber, bool isLastestPackage)
        //{
        //    MethodReturnResult<string> result = new MethodReturnResult<string>();
        //    result.Data = this.PackageNoGenerate.Generate(lotNumber, isLastestPackage);
        //    return result;
        //}

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
    }
}
