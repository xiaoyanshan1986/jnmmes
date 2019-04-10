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
using ServiceCenter.MES.DataAccess.Interface.BaseData;
using ServiceCenter.MES.Model.BaseData;

namespace ServiceCenter.MES.Service.WIP
{
    
    public partial class LotBinServiceEx 
    {

        /// <summary>
        /// 执行入Bing前检查
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult ChkBin(InBinParameter p)
        {

            LogHelper.WriteLogError(string.Format("ChkBin:Start> Lot_Number:{0}|| Scan_IP:{1}", p.ScanLotNumber, p.ScanIP));

            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0,
                Detail = "24"
            };
            try
            {
                result = ExecuteCheckBinEx(p);
                if (result.Code > 0)
                {
                    result.Detail = "24";
                }
            }
            catch (Exception ex)
            {
                result.Code = 24;
                result.Message = ex.Message;
            }
            LogHelper.WriteLogError(string.Format("ChkBin:End> Lot_Number:{0}|| Scan_IP:{1}||Bin_No:{2}||Message:{3}", p.ScanLotNumber, p.ScanIP, result.Detail, result.Message));

            return result;
        }

        /// <summary>
        /// 入bin操作，成功后返回bin号、包装号
        /// </summary>
        /// <param name="p"></param>
        /// <returns>1.code     -- 执行代码 0 - 成功 其它 - 失败</returns>
        /// <returns>2.Detail   -- 需要入bin号</returns>
        private MethodReturnResult ExecuteCheckBinEx(InBinParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0,           //错误代码 0 - 成功
                Detail = "24"       //默认24bin作为校验失败后组件归集默认bin
            };

            MethodReturnResult resultForSub = new MethodReturnResult()
            {
                Code = 0,
                Detail = "24"
            };


            #region 检查参数信息
            //检查组件批次号
            if (string.IsNullOrEmpty(p.ScanLotNumber) || p.ScanLotNumber == "")
            {
                result.Code = 1002;
                result.Message = string.Format("{0} "
                                                , "组件序列号为空");
                return result;
            }

            //检查读头IP地址
            if (string.IsNullOrEmpty(p.ScanIP) || p.ScanIP == "")
            {
                result.Code = 1001;
                result.Message = string.Format("批次{0}的读头IP地址为空"
                                                , p.ScanLotNumber);
                return result;
            }
            #endregion

            #region 判断LOT信息
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

            //批次代码
            string lotNumber = p.ScanLotNumber;

            //取得批次信息
            Lot lot = this.LotDataEngine.Get(lotNumber);

            //判断批次是否存在。
            if (lot == null || lot.Status == EnumObjectStatus.Disabled)
            {
                result.Code = 1003;
                result.Message = string.Format("批次（{0}）不存在。", lotNumber);
                return result;
            }

            if (lot.PackageFlag == true)
            {
                result.Code = 1003;
                result.Message = string.Format("批次（{0}）已包装，包装号：{0}。", lotNumber,lot.PackageNo);
                return result;
            }

            //批次加工设备
            p.EquipmentCode = lot.EquipmentCode;//？？？？？？？？？？？？

            PagingConfig cfg = new PagingConfig();

            //判断Lot是否已入Package
            IList<PackageBin> lstPackageBin = null;
            PackageBin packageBin = null;

            string strPackageNo = lot.PackageNo;

            if (string.IsNullOrEmpty(strPackageNo) == false && strPackageNo.Length > 0)
            {
                cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.PackageNo='{0}'", strPackageNo)
                };

                lstPackageBin = PackageBinDataEngine.Get(cfg);
                packageBin = lstPackageBin.FirstOrDefault();

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

            //批次已撤销
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
            if (string.IsNullOrEmpty(lot.Grade) || lot.Grade.ToUpper() != "A")
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
            #endregion

            #region //获取系统配置参数表
            bool blPackageChkPriority = false;
            cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format(@"Key.CategoryName='{0}'
                                           AND Key.AttributeName='{1}'"
                                        , "SystemParameters"
                                        , "BinChkPriority"),
                OrderBy = "Key.ItemOrder"
            };
            IList<BaseAttributeValue> lstBaseAttributeValues = BaseAttributeValueDataEngine.Get(cfg);
            if (lstBaseAttributeValues != null && lstBaseAttributeValues.Count > 0)
            {
                BaseAttributeValue obj = lstBaseAttributeValues.FirstOrDefault();
                if (String.IsNullOrEmpty(obj.Value) == false)
                {
                    Boolean.TryParse(obj.Value, out blPackageChkPriority);
                }
            }
            #endregion


            #region //获取批次IV测试数据
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
            #endregion

            #region //获取Lot的BInCode
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
                OrderBy = " Key.BinNo "
            };
            //----------
            IList<BinRule> lstBinRules = this.BinRuleDataEngine.Get(cfg);
            string strBinNo = "";
            if (lstBinRules.Count > 0)
            {
                strBinNo = lstBinRules[0].Key.BinNo;
                p.BinNo = strBinNo;
                result.Detail = strBinNo;

                //---增加包装优先级的判断
                if (blPackageChkPriority && lstBinRules.Count>1)
                {
                    //根据Bin对于的包装数量获取获取正确的Bin号，获取满足BIN的最近一次记录
                    cfg = new PagingConfig()
                    {
                        IsPaging = true,
                        PageSize=1,
                        //Where = string.Format(@"Key.BinNo in('{0}','{1}') And Key.PackageLine ='{2}' AND BinPackaged=0 ", lstBinRules[0].Key.BinNo,
                        // lstBinRules[1].Key.BinNo, p.PackageLine),
                        Where = string.Format(@"Key.BinNo in('{0}','{1}') And Key.PackageLine ='{2}' ", 
                            lstBinRules[0].Key.BinNo,lstBinRules[1].Key.BinNo, p.PackageLine),
                        OrderBy = " EditTime desc "
                    };
                    lstPackageBin = PackageBinDataEngine.Get(cfg);
                    if(lstPackageBin!=null && lstPackageBin.Count>0)
                    {
                        if( lstPackageBin[0].BinPackaged==EnumBinPackaged.UnFinished)
                        { 
                            //判断如果上一次入Bin的组件没有满托，取优先取上一次的
                            strBinNo = lstPackageBin[0].Key.BinNo;
                            p.BinNo = strBinNo;
                            result.Detail = strBinNo;
                        }
                        else
                        {
                            if(lstPackageBin[0].Key.BinNo== strBinNo)
                            { 
                                //如果已满托，取另一个BIN号
                                strBinNo = lstBinRules[1].Key.BinNo;
                                p.BinNo = strBinNo;
                                result.Detail = strBinNo;
                            }
                        }
                    }
                }
            }
            else
            {
                result.Code = 1010;
                result.Message = string.Format("{0} 找不到对应的Bin", lotNumber);
                return result;
            }
            #endregion

            #region //检查是否可以入Package

            //获取Bin 对应的PackageNo
            cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format(@"Key.BinNo='{0}' And Key.PackageLine ='{1}' AND BinPackaged=0 ", p.BinNo, p.PackageLine),
                OrderBy = ""
            };

            lstPackageBin = PackageBinDataEngine.Get(cfg);
            bool blCheckPackageInfo = false;
            string packageNo = "";
            int nToBeBinQuantity = 0;

            if (lstPackageBin != null && lstPackageBin.Count > 0)
            {
                packageBin = lstPackageBin.FirstOrDefault();
                packageNo = packageBin.Key.PackageNo;  
 
                //----------------------------------------------------------
                nToBeBinQuantity = packageBin.BinQty + 1;
                if (nToBeBinQuantity <= packageBin.BinMaxQty)
                {
                    blCheckPackageInfo = true;
                }
            }

            if (blCheckPackageInfo)
            {
                List<string> lstLotNumbers = new List<string>();
                lstLotNumbers.Add(lotNumber);
                PackageParameter pkParam = new PackageParameter()
                {
                    Creator = p.Creator,
                    EquipmentCode = p.EquipmentCode,
                    //IsFinishPackage = model.IsFinishPackage,
                    //IsLastestPackage = model.IsLastestPackage,
                    LineCode = p.LineCode,
                    LotNumbers = lstLotNumbers,
                    OperateComputer = p.OperateComputer,
                    Operator = p.Operator,
                    PackageNo = packageNo,
                    Remark = "",
                    RouteOperationName = p.RouteOperationName
                };

                //若Package Quantity 不满时，判断工单规则是否符合要求      
                resultForSub = CheckPackageForWorkOrderRule(pkParam);
                if (resultForSub.Code > 0)
                {
                    return resultForSub;
                }

                //若Package Quantity 不满时，判断是否可以入Package
                resultForSub = CheckPackageForPowerset(pkParam);
                if (resultForSub.Code > 0)
                {
                    return resultForSub;
                }
            }           
            #endregion

            return result;
        }

    }
}
