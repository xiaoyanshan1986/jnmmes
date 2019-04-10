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
using System.Configuration;
using ServiceCenter.MES.Service.Contract.ZPVM;
using ServiceCenter.MES.Service.Class.COMMON;

namespace ServiceCenter.MES.Service.WIP
{    
    public partial class LotBinServiceEx 
    {
        string localName = System.Configuration.ConfigurationSettings.AppSettings["LocalName"];
        string packageType = System.Configuration.ConfigurationSettings.AppSettings["PackageType"];

        //产品编码成柜参数数据访问对象
        public IMaterialChestParameterDataEngine MaterialChestParameterDataEngine { get; set; }

        private MethodReturnResult LotResult(InBinParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (packageType == "31")
            {
                #region 隔块包护角
                result = new MethodReturnResult()
                {
                    Code = 1000,
                    Detail = "24"
                };
                string lotNumber = p.ScanLotNumber;//最新组件的批次号
                PagingConfig cfg = new PagingConfig()
                {
                    Where = string.Format("Key.LotNumber='{0}' and PackageLine='{1}'", lotNumber, p.PackageLine),
                    IsPaging = false
                };
                IList<PackageCornerDetail> lstPackageCornerDetail = this.PackageCornerDetailDataEngine.Get(cfg);
                if (lstPackageCornerDetail.Count != 0 && lstPackageCornerDetail != null)
                {
                    string packageKey = lstPackageCornerDetail[0].Key.PackageKey;
                    PackageCorner packageCorner = this.PackageCornerDataEngine.Get(packageKey);
                    if (packageCorner != null)
                    {
                        result.Code = 0;
                        result.Detail = packageCorner.BinNo;                       //入Bin号
                        result.ObjectNo = lstPackageCornerDetail[0].ItemNo.ToString();     //入托序列号
                    }
                    else
                    {
                        result.Message = string.Format("批次：{0}没对应的虚拟BIN", lotNumber);
                    }

                }
                else
                {
                    result.Message = string.Format("终检出站的批次不包含{0}", lotNumber);
                }
                #endregion
            }
            if (packageType == "30")
            {
                #region 全包护角
                result = new MethodReturnResult()
                {
                    Code = 0,
                    Detail = "24"
                };
                string lotNumber = p.ScanLotNumber;//最新组件的批次号
                PagingConfig cfg = new PagingConfig()
                {
                    Where = string.Format("Key.LotNumber='{0}' and PackageLine='{1}'", lotNumber, p.PackageLine),
                    IsPaging = false
                };
                IList<PackageCornerDetail> lstPackageCornerDetail = this.PackageCornerDetailDataEngine.Get(cfg);
                if (lstPackageCornerDetail.Count != 0 && lstPackageCornerDetail != null)
                {
                    string packageKey = lstPackageCornerDetail[0].Key.PackageKey;
                    PackageCorner packageCorner = this.PackageCornerDataEngine.Get(packageKey);
                    if (packageCorner != null)
                    {
                        result.Code = 0;
                        result.Detail = packageCorner.BinNo;                       //入Bin号
                        result.ObjectNo = lstPackageCornerDetail[0].ItemNo.ToString();     //入托序列号
                    }

                }
                else
                {
                    result.Code = 1012;
                    result.Message = string.Format("终检出站的批次不包含{0}", lotNumber);
                    result.Detail = "24";
                }                
                #endregion
            }
            return result;
        }

        private MethodReturnResult InAbnormalBin(InBinParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (packageType == "30")
            {
                #region 全包护角
                result = new MethodReturnResult()
                {
                    Code = 0
                };
                ISession session = this.LotDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                string strLotNumber = p.ScanLotNumber;
                Lot lot = this.LotDataEngine.Get(strLotNumber);
                //判断批次是否存在。
                if (lot == null || lot.Status == EnumObjectStatus.Disabled)
                {
                    result.Code = 1001;
                    result.Message = string.Format("批次：（{0}）不存在！", strLotNumber);
                    return result;
                }

                //取得工单设置最大入托数
                CommonObjectDataEngine<WorkOrderRule, WorkOrderRuleKey> commonObjectDataEngine;
                commonObjectDataEngine = new CommonObjectDataEngine<WorkOrderRule, WorkOrderRuleKey>(LotDataEngine.SessionFactory);

                WorkOrderRuleKey workOrderRuleKey = new WorkOrderRuleKey
                {
                    OrderNumber = lot.OrderNumber,
                    MaterialCode = lot.MaterialCode
                };

                WorkOrderRule workOrderRule = commonObjectDataEngine.Get(workOrderRuleKey);
                string lineCode = p.LineCode;
                int iQtyMax = 0;

                if (workOrderRule != null && workOrderRule.FullPackageQty != null)
                {
                    int.TryParse(workOrderRule.FullPackageQty.ToString(), out iQtyMax);
                }
                else
                {
                    result.Code = 1002;
                    result.Message = "最大满托数未设置！";
                    return result;
                }
                try
                {
                    ProductionLine productionLine = this.ProductionLineDataEngine.Get(p.PackageLine);
                    if (productionLine != null && productionLine.Attr2 != "" && productionLine.Attr2 != null)
                    {
                        string abnormalBinNo = productionLine.Attr2;
                        PagingConfig cfg = new PagingConfig()
                        {
                            Where = string.Format("Key.BinNo='{0}' and Key.PackageLine='{1}' and (Key.PackageNo is null or Key.PackageNo='')", abnormalBinNo, p.PackageLine),
                            OrderBy = "EditTime desc",
                            IsPaging = false
                        };
                        IList<PackageBin> listPackageBin = this.PackageBinDataEngine.Get(cfg);
                        PackageBin packageBin = null;
                        if (listPackageBin.Count == 0)
                        {
                            PackageBinKey packageBinKey = new PackageBinKey
                            {
                                BinNo = abnormalBinNo,
                                PackageLine = p.PackageLine,
                                PackageNo = ""
                            };
                            packageBin = new PackageBin
                            {
                                Key = packageBinKey,
                                BinMaxQty = iQtyMax,
                                BinQty = 1,
                                BinState = 0,
                                BinPackaged = EnumBinPackaged.UnFinished,
                                CreateTime = DateTime.Now,
                                Creator = p.PackageLine,
                                Editor = p.PackageLine,
                                EditTime = DateTime.Now
                            };
                            this.PackageBinDataEngine.Insert(packageBin, session);

                        }
                        else
                        {
                            packageBin = listPackageBin.FirstOrDefault();
                            if (packageBin.BinPackaged == EnumBinPackaged.Finished)
                            {
                                packageBin.BinQty = 1;
                            }
                            else
                            {
                                packageBin.BinQty = packageBin.BinQty + 1;
                            }
                            if (packageBin.BinQty == packageBin.BinMaxQty)
                            {
                                packageBin.BinPackaged = EnumBinPackaged.Finished;
                            }
                            else
                            {
                                packageBin.BinPackaged = EnumBinPackaged.UnFinished;
                            }
                            packageBin.EditTime = DateTime.Now;
                            packageBin.Editor = p.PackageLine;
                            this.PackageBinDataEngine.Update(packageBin, session);
                        }
                        transaction.Commit();
                        session.Close();
                    }
                    else
                    {
                        result.Code = 1000;
                        result.Detail = "24";
                    }
                }
                catch (Exception ex)
                {
                    result.Code = 1000;
                    result.Message = ex.Message;
                    result.Detail = ex.ToString();
                    transaction.Rollback();
                    session.Close();
                    return result;
                }
                #endregion
            }
            if (packageType == "31")
            {
                #region 隔块包护角
                result = new MethodReturnResult()
                {
                    Code = 0
                };
                ISession session = this.LotDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                string strLotNumber = p.ScanLotNumber;
                Lot lot = this.LotDataEngine.Get(strLotNumber);
                //判断批次是否存在。
                if (lot == null || lot.Status == EnumObjectStatus.Disabled)
                {
                    result.Code = 1001;
                    result.Message = string.Format("批次：（{0}）不存在！", strLotNumber);
                    return result;
                }

                //取得工单设置最大入托数
                CommonObjectDataEngine<WorkOrderRule, WorkOrderRuleKey> commonObjectDataEngine;
                commonObjectDataEngine = new CommonObjectDataEngine<WorkOrderRule, WorkOrderRuleKey>(LotDataEngine.SessionFactory);

                WorkOrderRuleKey workOrderRuleKey = new WorkOrderRuleKey
                {
                    OrderNumber = lot.OrderNumber,
                    MaterialCode = lot.MaterialCode
                };

                WorkOrderRule workOrderRule = commonObjectDataEngine.Get(workOrderRuleKey);
                string lineCode = p.LineCode;
                int iQtyMax = 0;

                if (workOrderRule != null && workOrderRule.FullPackageQty != null)
                {
                    int.TryParse(workOrderRule.FullPackageQty.ToString(), out iQtyMax);
                }
                else
                {
                    result.Code = 1002;
                    result.Message = "最大满托数未设置！";
                    return result;
                }
                try
                {
                    ProductionLine productionLine = this.ProductionLineDataEngine.Get(p.PackageLine);
                    if (productionLine != null && productionLine.Attr2 != "" && productionLine.Attr2 != null)
                    {
                        string abnormalBinNo = productionLine.Attr2;
                        PagingConfig cfg = new PagingConfig()
                        {
                            Where = string.Format("Key.BinNo='{0}' and Key.PackageLine='{1}' and (Key.PackageNo is null or Key.PackageNo='')", abnormalBinNo, p.PackageLine),
                            OrderBy = "EditTime desc",
                            IsPaging = false
                        };
                        IList<PackageBin> listPackageBin = this.PackageBinDataEngine.Get(cfg);
                        PackageBin packageBin = null;
                        if (listPackageBin.Count == 0)
                        {
                            PackageBinKey packageBinKey = new PackageBinKey
                            {
                                BinNo = abnormalBinNo,
                                PackageLine = p.PackageLine,
                                PackageNo = ""
                            };
                            packageBin = new PackageBin
                            {
                                Key = packageBinKey,
                                BinMaxQty = iQtyMax,
                                BinQty = 1,
                                BinState = 0,
                                BinPackaged = EnumBinPackaged.UnFinished,
                                CreateTime = DateTime.Now,
                                Creator = p.PackageLine,
                                Editor = p.PackageLine,
                                EditTime = DateTime.Now
                            };
                            this.PackageBinDataEngine.Insert(packageBin, session);

                        }
                        else
                        {
                            packageBin = listPackageBin.FirstOrDefault();
                            if (packageBin.BinPackaged == EnumBinPackaged.Finished)
                            {
                                packageBin.BinQty = 1;
                            }
                            else
                            {
                                packageBin.BinQty = packageBin.BinQty + 1;
                            }
                            if (packageBin.BinQty == packageBin.BinMaxQty)
                            {
                                packageBin.BinPackaged = EnumBinPackaged.Finished;
                            }
                            else
                            {
                                packageBin.BinPackaged = EnumBinPackaged.UnFinished;
                            }
                            packageBin.EditTime = DateTime.Now;
                            packageBin.Editor = p.PackageLine;
                            this.PackageBinDataEngine.Update(packageBin, session);
                        }
                        transaction.Commit();
                        session.Close();
                    }
                    else
                    {
                        result.Code = 1000;
                        result.Detail = "24";
                    }
                }
                catch (Exception ex)
                {
                    result.Code = 1000;
                    result.Message = ex.Message;
                    result.Detail = ex.ToString();
                    transaction.Rollback();
                    session.Close();
                    return result;
                }
                #endregion
            }
            return result;
        }

        #region 隔块包护角专有方法
        private MethodReturnResult AdjustmentLot(InBinParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0,
            };
            try
            {
                PagingConfig cfg = new PagingConfig()//查找该线别最新入BIN的托号
                {
                    Where = string.Format("Key.PackageLine='{0}' and BinState=0", p.PackageLine),
                    OrderBy = "EditTime desc"
                };
                IList<PackageBin> lstPackageBin = this.PackageBinDataEngine.Get(cfg);
                if (lstPackageBin.Count != 0 && lstPackageBin != null)
                {
                    string packageNo = lstPackageBin[0].Key.PackageNo;
                    cfg = new PagingConfig()//查找该托最新的批次号
                    {
                        Where = string.Format("Key.PackageNo='{0}'", packageNo),
                        OrderBy = "CreateTime desc",
                    };
                    IList<PackageDetail> lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);
                    if (lstPackageDetail.Count != 0 && lstPackageDetail != null)
                    {
                        string lotNumber = lstPackageDetail[0].Key.ObjectNumber;//最新组件的批次号

                        cfg = new PagingConfig()
                        {
                            Where = string.Format("Key.LotNumber='{0}'", lotNumber),
                            IsPaging = false
                        };
                        IList<PackageCornerDetail> lstPackageCornerDetail = this.PackageCornerDetailDataEngine.Get(cfg);
                        if (lstPackageCornerDetail.Count != 0 && lstPackageCornerDetail != null)
                        {
                            string dateTime = lstPackageCornerDetail[0].CreateTime.ToString();//最新批次号在终检创建时间
                            cfg = new PagingConfig()//获得该批次号下一批次号的批次数据
                            {
                                Where = string.Format("CreateTime>'{0}' and PackageLine='{1}'", dateTime, p.PackageLine),
                                OrderBy = "CreateTime asc",
                                IsPaging = false
                            };
                            IList<PackageCornerDetail> lstPackageCornerDetailAll = this.PackageCornerDetailDataEngine.Get(cfg);
                            if (lstPackageCornerDetailAll.Count != 0 && lstPackageCornerDetailAll != null)
                            {
                                string newLotNumber = lstPackageCornerDetailAll[0].Key.LotNumber;//获得最新批次的下一个批次
                                string packageKey = lstPackageCornerDetailAll[0].Key.PackageKey;
                                PackageCorner packageCorner = this.PackageCornerDataEngine.Get(packageKey);
                                if (packageCorner != null)
                                {
                                    result.Code = 0;
                                    result.Message = newLotNumber;
                                    result.Detail = packageCorner.BinNo;                       //入Bin号
                                    result.ObjectNo = lstPackageCornerDetailAll[0].ItemNo.ToString();     //入托序列号
                                    p.ScanLotNumber = newLotNumber;
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                result.Code = 0;
                result.Detail = "24";
                result.Message = ex.Message;
            }

            return result;
        }

        #endregion

        /// <summary>
        /// 检验是否可以入bin操作，成功后返回bin号、及将入Bin位置代码
        /// </summary>
        /// <param name="p">1.ScanLotNumber  -- 组件LOT号 </param>
        ///                 2.ScanNo         -- 读头IP地址
        /// <returns>1.code         -- 执行代码 0 - 成功 其它 - 失败</returns>
        ///          2.Detail       -- 需要入bin号 
        ///          3.ObjectNo     -- 组件的入Bin位置代码
        ///          4.Message      -- 错误信息
        public MethodReturnResult ChkBin(InBinParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (packageType == "31")
            {
                #region 隔块包护角
                //记录日志信息
                LogHelper.WriteLogError(string.Format("ChkBin:Start> Lot_Number:{0}|| Scan_IP:{1}", p.ScanLotNumber, p.ScanIP));

                result = new MethodReturnResult()
                {
                    Code = 0,
                    Detail = "24"
                };

                try
                {
                    //检验是否可以入读头所在线别Bin，若可行返回入Bin代码
                    result = ExecuteCheckBinEx(p);

                    if (result.Code > 1)
                    {
                        result.Code = 0;
                        result.Detail = "24";
                    }
                    else
                    {
                        //Code=1000为组件流水线连续两次扫描同一块组件时智能纠错，直接返回
                        if (result.Code == 1)
                        {
                            result.Code = 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.Code = 1000;
                    result.Detail = "24";
                    result.Message = ex.Message;
                }

                //记录操作日志
                if (result.Code > 0)
                {
                    LogHelper.WriteLogError(string.Format("ChkBin:End> Lot_Number:{0}|| Scan_IP:{1}||Error! Message:{2}",
                                        p.ScanLotNumber,
                                        p.ScanIP,
                                        result.Message));
                }
                else
                {
                    LogHelper.WriteLogError(string.Format("ChkBin:End> Lot_Number:{0}|| Scan_IP:{1}||Bin_No:{2}||InBinOrder:{3}||Message:{4}",
                                        p.ScanLotNumber,
                                        p.ScanIP,
                                        result.Detail,
                                        result.ObjectNo,
                                        result.Message));
                }
                #endregion
            }

            if (packageType == "30")
            {
                #region 全包护角
                if (localName == "K01")
                {
                    //记录日志信息
                    LogHelper.WriteLogError(string.Format("ChkBin:Start> Lot_Number:{0}|| Scan_IP:{1}", p.ScanLotNumber, p.ScanIP));
                }
                if (localName == "G01")
                {
                    //记录日志信息
                    LogHelper.WriteLogError(string.Format("ChkBin:Start> Lot_Number:{0}|| PackageLine:{1}", p.ScanLotNumber, p.PackageLine));
                }


                result = new MethodReturnResult()
                {
                    Code = 0,
                    Detail = "24"
                };

                try
                {
                    //检验是否可以入读头所在线别Bin，若可行返回入Bin代码
                    result = ExecuteCheckBinEx(p);

                    if (result.Code > 1)
                    {
                        result.Code = 0;
                        result.Detail = "24";
                    }
                    else
                    {
                        //Code=1000为组件流水线连续两次扫描同一块组件时智能纠错，直接返回
                        if (result.Code == 1)
                        {
                            result.Code = 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.Code = 1000;
                    result.Detail = "24";
                    result.Message = ex.Message;
                }

                if (localName == "K01")
                {
                    //记录操作日志
                    if (result.Code > 0)
                    {
                        LogHelper.WriteLogError(string.Format("ChkBin:End> Lot_Number:{0}|| Scan_IP:{1}||Error! Message:{2}",
                                            p.ScanLotNumber,
                                            p.ScanIP,
                                            result.Message));
                    }
                    else
                    {
                        LogHelper.WriteLogError(string.Format("ChkBin:End> Lot_Number:{0}|| Scan_IP:{1}||Bin_No:{2}||InBinOrder:{3}||Message:{4}",
                                            p.ScanLotNumber,
                                            p.ScanIP,
                                            result.Detail,
                                            result.ObjectNo,
                                            result.Message));
                    }
                }
                if (localName == "G01")
                {
                    //记录操作日志
                    if (result.Code > 0)
                    {
                        LogHelper.WriteLogError(string.Format("ChkBin:End> Lot_Number:{0}|| PackageLine:{1}||Error! Message:{2}",
                                            p.ScanLotNumber,
                                            p.PackageLine,
                                            result.Message));
                    }
                    else
                    {
                        LogHelper.WriteLogError(string.Format("ChkBin:End> Lot_Number:{0}|| PackageLine:{1}||Bin_No:{2}||InBinOrder:{3}||Message:{4}",
                                            p.ScanLotNumber,
                                            p.PackageLine,
                                            result.Detail,
                                            result.ObjectNo,
                                            result.Message));
                    }
                }
                #endregion
            }
            
            return result;
        }

        /// <summary>
        /// 检验是否可以入bin操作，成功后返回bin号、及将入Bin位置代码
        /// </summary>
        /// <param name="p">1.ScanLotNumber  -- 组件LOT号 </param>
        ///                 2.ScanNo         -- 读头IP地址
        /// <returns>1.code         -- 执行代码 0 - 成功， 1 - 当前Bin最后一块多次入Bin，纠错返回当前Bin及位置号， 其它 - 失败</returns>
        ///          2.Detail       -- 需要入bin号 
        ///          3.ObjectNo     -- 组件的入Bin位置代码
        ///          4.Message      -- 错误信息
        private MethodReturnResult ExecuteCheckBinEx(InBinParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();
            MethodReturnResult resultAbormal = new MethodReturnResult();//记录异常BIN
            MethodReturnResult resultNullLot = new MethodReturnResult();//记录批次号为空时的结果
            MethodReturnResult resultLot = new MethodReturnResult();//记录终检排队的结果数据
            ISession session = this.LotDataEngine.SessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            if (packageType == "31")
            {
                #region 隔块包护角
                try
                {
                    PagingConfig cfg = new PagingConfig();      //参数对象
                    IList<PackageBin> lstPackageBin = null;     //Bin对象列表
                    Equipment equipment = null;                 //设备对象
                    string strBinNo = "";                       //Bin号
                    string strPackageNo = "";                   //包装号
                    int intInBinOrder = 1;                      //入Bin序列号
                    int intNewBinCount = 0;                     //新规则未创建Bin记录数

                    #region  找到该线别的异常BIN号,并判断该组件入异常BIN的顺序

                    ProductionLine productionLine = this.ProductionLineDataEngine.Get(p.PackageLine);
                    if (productionLine != null && productionLine.Attr2 != "" && productionLine.Attr2 != null)
                    {
                        string abnormalBinNo = productionLine.Attr2;
                        cfg = new PagingConfig()
                        {
                            Where = string.Format("Key.BinNo='{0}' and Key.PackageLine='{1}' and (Key.PackageNo='' or Key.PackageNo is null) ", abnormalBinNo, p.PackageLine),
                            OrderBy = "EditTime desc",
                            IsPaging = false
                        };
                        IList<PackageBin> listPackageBin = this.PackageBinDataEngine.Get(cfg);
                        if (listPackageBin.Count == 0)
                        {
                            resultAbormal.ObjectNo = "1";
                        }
                        else
                        {
                            PackageBin packageBin = listPackageBin.FirstOrDefault();
                            if (packageBin.BinPackaged == EnumBinPackaged.Finished)
                            {
                                packageBin.BinQty = 1;
                            }
                            else
                            {
                                packageBin.BinQty = packageBin.BinQty + 1;
                            }
                            resultAbormal.ObjectNo = packageBin.BinQty.ToString();
                        }

                        resultAbormal.Code = 0;
                        resultAbormal.Detail = abnormalBinNo;
                    }
                    else
                    {
                        resultAbormal.Code = 1000;
                        resultAbormal.Detail = "24";
                    }
                    #endregion

                    #region 检查参数信息是否为空
                    //检查组件批次号是否为空
                    if (string.IsNullOrEmpty(p.ScanLotNumber) || p.ScanLotNumber == "")
                    {
                        //resultNullLot = AdjustmentLot(p);//组件序列号为空时，按终检出站队列排序
                        //result.Code = 1001;
                        //result.Detail = "23";
                        //result.ObjectNo = "1";
                        //if (string.IsNullOrEmpty(p.ScanLotNumber) || p.ScanLotNumber == "")
                        //{
                        resultAbormal.Message = "组件序列号为空！";
                        return resultAbormal;
                        //}
                    }

                    //检查读头IP地址是否为空
                    if (string.IsNullOrEmpty(p.ScanIP) || p.ScanIP == "")
                    {

                        resultAbormal.Message = "读头IP地址为空！";
                        return resultAbormal;
                    }
                    #endregion

                    #region 取得并判断批次信息
                    string lotNumber = p.ScanLotNumber;

                    //取得批次信息
                    Lot lot = this.LotDataEngine.Get(lotNumber);

                    //判断批次是否存在。
                    if (lot == null || lot.Status == EnumObjectStatus.Disabled)
                    {
                        resultAbormal.Message = string.Format("批次：（{0}）不存在！", lotNumber);
                        return resultAbormal;
                    }

                    //批次已撤销
                    if (lot.DeletedFlag == true)
                    {
                        resultAbormal.Message = string.Format("批次：（{0}）已删除！", lotNumber);
                        return resultAbormal;
                    }
                    //批次已暂停
                    if (lot.HoldFlag == true)
                    {
                        resultAbormal.Message = string.Format("批次：（{0}）已暂停！", lotNumber);
                        return resultAbormal;
                    }

                    //判断Lot的等级是否是A级
                    if (string.IsNullOrEmpty(lot.Grade) || lot.Grade.ToUpper() != "A")
                    {
                        resultAbormal.Message = string.Format("批次（{0}）等级不是A级。", lotNumber);
                        return resultAbormal;
                    }
                    #endregion

                    #region 检查工序是否是包装工序
                    //取得当前批次所在站别是否包装流程站别属性
                    RouteOperationAttribute roAttr = this.RouteOperationAttributeDataEngine.Get(new RouteOperationAttributeKey()
                    {
                        RouteOperationName = lot.RouteStepName,     //当前批次所在工序
                        AttributeName = "IsPackageOperation"        //包装站属性名
                    });

                    //如果没有设置为包装工序，则直接返回。
                    if (roAttr == null)
                    {
                        resultAbormal.Message = string.Format("产品：({0})在({1})工序，请确认。", lotNumber, lot.RouteStepName);
                        return resultAbormal;
                    }
                    #endregion

                    #region 判断Lot是否已包装并进行数据纠错
                    if (lot.PackageFlag == true)
                    {
                        //智能纠错（仅在当前线上该组件有最后入bin记录）
                        //取得读头设备信息
                        equipment = this.EquipmentDataEngine.Get(p.ScanIP);

                        if (equipment == null)
                        {
                            resultAbormal.Message = string.Format("读头：（{0}）信息未设置！", p.ScanIP);
                            return resultAbormal;
                        }

                        //读头线别与批次线别是否相同
                        if (equipment.LineCode != lot.LineCode)
                        {
                            resultAbormal.Message = string.Format("批次（{0}）已包装！",
                                                        p.ScanLotNumber);
                            return resultAbormal;
                        }

                        //取得包装号是否在当前读头自动包装线并取得对应当前有效bin号
                        cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format("Key.PackageLine='{0}' and Key.PackageNo='{1}' and BinState=0",
                                                lot.LineCode,
                                                lot.PackageNo)
                        };

                        lstPackageBin = PackageBinDataEngine.Get(cfg);

                        if (lstPackageBin == null || lstPackageBin.Count == 0)      //非bin包装号
                        {
                            resultAbormal.Message = string.Format("批次（{0}）所在包装({1})不存在当前包装线（{2}）！",
                                                        lotNumber,
                                                        lot.PackageNo,
                                                        equipment.LineCode);
                            return resultAbormal;
                        }
                        else
                        {
                            //确定是否为最后一个入BIN组件
                            //取得最后一个入托组件
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format("Key.PackageNo='{0}' ",
                                                      lot.PackageNo),
                                OrderBy = " ItemNo desc"
                            };

                            IList<PackageDetail> lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);

                            if (lstPackageDetail != null && lstPackageDetail.Count > 0)
                            {
                                if (lstPackageDetail[0].Key.ObjectNumber == lotNumber)
                                {
                                    //当前组件再次入BIin
                                    resultAbormal.Code = 1;
                                    resultAbormal.Detail = lstPackageBin[0].Key.BinNo;                 //入Bin号
                                    resultAbormal.ObjectNo = lstPackageDetail[0].ItemNo.ToString();    //入托序列号
                                    return resultAbormal;
                                }
                                else
                                {
                                    //非最后入Bin组件
                                    resultAbormal.Message = string.Format("批次({0})已包装！",
                                                                    lotNumber);
                                    return resultAbormal;
                                }
                            }
                            else
                            {
                                resultAbormal.Message = string.Format("包装({0})数据异常！",
                                                                lot.PackageNo);
                                return resultAbormal;
                            }
                        }
                    }
                    #endregion

                    #region 查询是否有锁定的批次号，并解锁

                    cfg = new PagingConfig()
                    {
                        Where = string.Format("LockFlag=1 and PackageLine='{0}'", p.PackageLine),
                        IsPaging = false
                    };
                    IList<PackageCorner> lstPackageCornerLock = this.PackageCornerDataEngine.Get(cfg);
                    if (lstPackageCornerLock.Count > 0 && lstPackageCornerLock != null)
                    {
                        foreach (PackageCorner item in lstPackageCornerLock)
                        {
                            SetPackageStateForLock(item, false);
                        }
                    }
                    #endregion

                    #region 获取批次IV测试数据
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
                        powersetCode = lstTestData[0].PowersetCode;                 //分档组
                        powersetCodeItemNo = lstTestData[0].PowersetItemNo ?? -1;   //分档代码
                        powersetSubCode = lstTestData[0].PowersetSubCode;           //子分档代码
                    }
                    else
                    {
                        resultAbormal.Message = string.Format("批次：（{0}） IV测试数据不存在！", lotNumber);
                        return resultAbormal;
                    }
                    #endregion

                    #region 获取对应的Bin信息
                    //取得读头设备信息
                    equipment = this.EquipmentDataEngine.Get(p.ScanIP);

                    if (equipment == null)
                    {
                        resultAbormal.Message = string.Format("读头：（{0}）信息未设置！", p.ScanIP);
                        return resultAbormal;
                    }

                    //Bin测试规则参数
                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"Key.PackageLine='{0}'  
                                          AND Key.PsCode='{1}' 
                                          AND Key.PsItemNo='{2}'  
                                          AND Key.PsSubCode='{3}' 
                                          AND Key.Color='{4}'
                                          AND ( Key.WorkOrderNumber='{5}' or Key.WorkOrderNumber='{6}')",
                                              equipment.LineCode,
                                              powersetCode,
                                              powersetCodeItemNo,
                                              powersetSubCode,
                                              lot.Color,
                                              lot.OrderNumber,
                                              "*"),
                        OrderBy = " Key.BinNo "
                    };

                    //取得Bin规则清单
                    IList<BinRule> lstBinRules = this.BinRuleDataEngine.Get(cfg);
                    if (lstBinRules.Count > 0)
                    {
                        //遍历所有的节点后优先入未满Bin，在空Bin中选择编辑时间最晚Bin
                        DateTime? dtEditTime = DateTime.Now;
                        bool isInPackage = false;
                        int iCount = 0;

                        for (int i = 0; i < lstBinRules.Count; i++)
                        {
                            //取得包装Bin信息
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format("Key.BinNo='{0}' and Key.PackageLine='{1}'",
                                                        lstBinRules[i].Key.BinNo,
                                                        equipment.LineCode),
                                OrderBy = " EditTime desc"
                            };

                            lstPackageBin = PackageBinDataEngine.Get(cfg);

                            if (lstPackageBin == null || lstPackageBin.Count == 0)  //入Bin记录未生成，在无未满托时优先入新Bin
                            {
                                if (intNewBinCount == 0)
                                {
                                    //取得Bin信息
                                    strBinNo = lstBinRules[i].Key.BinNo;            //规则设置对应Bin号
                                    intInBinOrder = 1;                              //入Bin序列号

                                    //结果计数器
                                    iCount++;
                                }

                                intNewBinCount++;
                            }
                            else
                            {
                                //判断Bin是否包装完成
                                if (lstPackageBin[0].BinPackaged == EnumBinPackaged.Finished)
                                {
                                    //判断高优先级新Bin规则是否存在
                                    if (intNewBinCount == 0)
                                    {
                                        //判断是否是否最晚时间
                                        if (lstPackageBin[0].EditTime <= dtEditTime)
                                        {
                                            //取得Bin信息
                                            strBinNo = lstPackageBin[0].Key.BinNo;              //Bin号
                                            intInBinOrder = 1;                                  //入Bin序列号
                                            dtEditTime = lstPackageBin[0].EditTime;             //最后编辑时间

                                            //结果计数器
                                            iCount++;
                                        }
                                    }
                                }
                                else
                                {
                                    //判断是否满足入托规则 
                                    result = CheckLotInPackageRule(lot, lstPackageBin[0].Key.PackageNo, out isInPackage);
                                    if (result.Code > 0)        //产生错误
                                    {
                                        return resultAbormal;
                                    }
                                    else
                                    {
                                        if (isInPackage)
                                        {
                                            //若为未满Bin优先入未满Bin
                                            //取得Bin信息
                                            strBinNo = lstPackageBin[0].Key.BinNo;          //Bin号
                                            intInBinOrder = int.Parse(result.ObjectNo);     //将入托序列号
                                            strPackageNo = lstPackageBin[0].Key.PackageNo;  //将入托包装号
                                            dtEditTime = lstPackageBin[0].EditTime;         //最后编辑时间

                                            //结果计数器
                                            iCount++;

                                            //退出搜寻
                                            i = lstBinRules.Count;
                                        }
                                        else
                                        {
                                            result.Message = "";
                                        }
                                    }
                                }
                            }
                        }

                        if (iCount == 0)    //未找到符合条件Bin
                        {
                            resultAbormal.Message = string.Format("批次：（{0}） 无对应Bin！", lotNumber);
                            return resultAbormal;
                        }
                    }
                    else
                    {
                        resultAbormal.Message = string.Format("批次：（{0}） 无对应Bin！", lotNumber);
                        return resultAbormal;
                    }
                    #endregion

                    result.Code = 0;
                    result.Detail = strBinNo;                       //入Bin号
                    result.ObjectNo = intInBinOrder.ToString();     //入托序列号

                    p.EquipmentCode = equipment.Key;                //返回设备代码
                    p.LineCode = equipment.LineCode;                //返回线别
                    p.PackageNo = strPackageNo;                     //入托包装号
                    //以下edit by yanshan.xiao
                    if (resultNullLot.Message == p.ScanLotNumber)
                    {
                        if (resultNullLot.ObjectNo == result.ObjectNo && result.Detail == resultNullLot.Detail)
                        {
                            return result;
                        }
                        else
                        {
                            resultAbormal.Message = string.Format("批次为空或者不存在");
                            return resultAbormal;
                        }
                    }
                    resultLot = LotResult(p);
                    List<PackageCornerDetail> lstPackageCornerDetailDelete = new List<PackageCornerDetail>();//删除错位批次号
                    List<PackageCorner> lstPackageCornerUpdate = new List<PackageCorner>();//更新当前BIN号
                    List<PackageCorner> lstPackageCornerDelete = new List<PackageCorner>();//删除当前BIN号
                    if (resultLot.Code == 0)
                    {
                        if (resultLot.ObjectNo == result.ObjectNo && resultLot.Detail == result.Detail)//实物bin和虚拟bin，位置号完全一致
                        {
                            return result;
                        }
                        else//实物bin和虚拟bin或者位置号不一致
                        {
                            string strBinNoSort = "('" + lstBinRules[0].Key.BinNo + "'";
                            for (int i = 1; i < lstBinRules.Count; i++)
                            {
                                strBinNoSort = strBinNoSort + ",'" + lstBinRules[i].Key.BinNo + "'";
                            }
                            strBinNoSort = strBinNoSort + ")";
                            cfg = new PagingConfig()//查找实物BIN最后一个入托的组件批次号
                            {
                                Where = string.Format("Key.BinNo IN {0} and Key.PackageLine='{1}'", strBinNoSort, p.PackageLine),
                                OrderBy = "EditTime desc",
                                IsPaging = false
                            };
                            IList<PackageBin> lstPackageBinCorner = this.PackageBinDataEngine.Get(cfg);//取得当前实物BIN的托号
                            string PackageNo = "";
                            string LastLotNo = "";
                            if (lstPackageBinCorner.Count > 0)
                            {
                                PackageNo = lstPackageBinCorner[0].Key.PackageNo;
                            }
                            if (PackageNo != "")
                            {
                                cfg = new PagingConfig()//获取该实物BIN所在托的组件情况
                                {
                                    Where = string.Format("Key.PackageNo='{0}'", PackageNo),
                                    OrderBy = "ItemNo desc",
                                    IsPaging = false
                                };
                                IList<PackageDetail> lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);//取得当前BIN的最后一个批次号
                                if (lstPackageDetail.Count > 0)
                                {
                                    LastLotNo = lstPackageDetail[0].Key.ObjectNumber;
                                }
                            }
                            if (LastLotNo != "")//批次号不为空时，找到这个批次号的对应虚拟bin的进站时间
                            {
                                cfg = new PagingConfig()
                                {
                                    Where = string.Format("Key.LotNumber='{0}' and PackageLine='{1}'", LastLotNo, p.PackageLine),
                                    IsPaging = false
                                };
                                IList<PackageCornerDetail> lstPackageCornerDetail = this.PackageCornerDetailDataEngine.Get(cfg);
                                if (lstPackageCornerDetail.Count != 0 && lstPackageCornerDetail != null)
                                {
                                    string dtCreateTime = lstPackageCornerDetail[0].CreateTime.ToString();
                                    for (int i = 0; i < lstBinRules.Count; i++)
                                    {
                                        cfg = new PagingConfig()
                                        {
                                            Where = string.Format("PackageLine='{0}' and EditTime>'{1}' and BinNo='{2}'", p.PackageLine, dtCreateTime, lstBinRules[i].Key.BinNo),
                                            IsPaging = false
                                        };
                                        IList<PackageCorner> lstPackageCorner = this.PackageCornerDataEngine.Get(cfg);
                                        foreach (PackageCorner item in lstPackageCorner)
                                        {
                                            string packageKey = item.Key;
                                            PackageCorner packageCorner = this.PackageCornerDataEngine.Get(packageKey);
                                            SetPackageStateForLock(packageCorner, true);//锁定该托号
                                            cfg = new PagingConfig()
                                            {
                                                Where = string.Format("Key.PackageKey='{0}' and PackageLine='{1}' and CreateTime>'{2}'", packageKey, p.PackageLine, dtCreateTime),
                                                IsPaging = false,
                                                OrderBy = "ItemNo asc"
                                            };
                                            IList<PackageCornerDetail> lstPackageCornerDetailMore = this.PackageCornerDetailDataEngine.Get(cfg);
                                            if (lstPackageCornerDetailMore.Count > 0)
                                            {
                                                if (lstPackageCornerDetailMore[0].ItemNo == 1)//判断是否需要更新还是删除该虚拟BIN
                                                {
                                                    lstPackageCornerDelete.Add(item);
                                                }
                                                else
                                                {
                                                    PackageCorner packageCornerClone = item.Clone() as PackageCorner;
                                                    packageCornerClone.BinQty = lstPackageCornerDetailMore[0].ItemNo - 1;
                                                    packageCornerClone.BinPackaged = EnumCornerPackaged.UnFinished;
                                                    lstPackageCornerUpdate.Add(packageCornerClone);
                                                }
                                                foreach (PackageCornerDetail packageCornerDetail in lstPackageCornerDetailMore)
                                                {
                                                    lstPackageCornerDetailDelete.Add(packageCornerDetail);
                                                }
                                            }
                                        }
                                    }

                                    #region 执行事务
                                    foreach (PackageCornerDetail packageCornerDetail in lstPackageCornerDetailDelete)//删除当前时间之后所有虚拟BIN的批次
                                    {
                                        this.PackageCornerDetailDataEngine.Delete(packageCornerDetail.Key, session);
                                        PackageCornerDetailTransaction packageCornerDetailTransaction = new PackageCornerDetailTransaction()
                                        {
                                            Key = packageCornerDetail.Key,
                                            CreateTime = packageCornerDetail.CreateTime,
                                            ItemNo = packageCornerDetail.ItemNo,
                                            Creator = packageCornerDetail.Creator,
                                            MaterialCode = packageCornerDetail.MaterialCode,
                                            OrderNumber = packageCornerDetail.OrderNumber,
                                            PackageFlag = packageCornerDetail.PackageFlag,
                                            PackageLine = packageCornerDetail.PackageLine
                                        };
                                        this.PackageCornerDetailTransactionDataEngine.Insert(packageCornerDetailTransaction, session);
                                    }

                                    foreach (PackageCorner packageCorner in lstPackageCornerDelete)
                                    {
                                        this.PackageCornerDataEngine.Delete(packageCorner.Key, session);
                                    }

                                    foreach (PackageCorner packageCorner in lstPackageCornerUpdate)
                                    {
                                        this.PackageCornerDataEngine.Update(packageCorner, session);

                                    }
                                    transaction.Commit();
                                    session.Close();
                                    foreach (PackageCorner packageCorner in lstPackageCornerUpdate)
                                    {
                                        SetPackageStateForLock(packageCorner, false);//取消锁定该托号
                                    }
                                    #endregion
                                }
                            }

                            resultAbormal.Message = string.Format("虚拟BIN{0}位置为{1}与实物BIN{2}位置为{3}不一致", resultLot.Detail, resultLot.ObjectNo, result.Detail, result.ObjectNo);
                            return resultAbormal;
                        }

                    }
                    else
                    {
                        resultAbormal.Message = resultLot.Message;
                        return resultAbormal;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    session.Close();
                    resultAbormal.Message = ex.Message;
                    return resultAbormal;
                }
                #endregion
            }

            if (packageType == "30")
            {
                #region 全包护角
                try
                {
                    PagingConfig cfg = new PagingConfig();      //参数对象
                    IList<PackageBin> lstPackageBin = null;     //Bin对象列表
                    Equipment equipment = null;                 //设备对象
                    string strBinNo = "";                       //Bin号
                    string strPackageNo = "";                   //包装号
                    int intInBinOrder = 1;                      //入Bin序列号
                    int intNewBinCount = 0;                     //新规则未创建Bin记录数

                    #region  找到该线别的异常BIN号,并判断该组件入异常BIN的顺序

                    ProductionLine productionLine = this.ProductionLineDataEngine.Get(p.PackageLine);
                    if (productionLine != null && productionLine.Attr2 != "" && productionLine.Attr2 != null)
                    {
                        string abnormalBinNo = productionLine.Attr2;
                        cfg = new PagingConfig()
                        {
                            Where = string.Format("Key.BinNo='{0}' and Key.PackageLine='{1}' and (Key.PackageNo='' or Key.PackageNo is null) ", abnormalBinNo, p.PackageLine),
                            OrderBy = "EditTime desc",
                            IsPaging = false
                        };
                        IList<PackageBin> listPackageBin = this.PackageBinDataEngine.Get(cfg);
                        if (listPackageBin.Count == 0)
                        {
                            resultAbormal.ObjectNo = "1";
                        }
                        else
                        {
                            PackageBin packageBin = listPackageBin.FirstOrDefault();
                            if (packageBin.BinPackaged == EnumBinPackaged.Finished)
                            {
                                packageBin.BinQty = 1;
                            }
                            else
                            {
                                packageBin.BinQty = packageBin.BinQty + 1;
                            }
                            resultAbormal.ObjectNo = packageBin.BinQty.ToString();
                        }
                        resultAbormal.Code = 0;
                        resultAbormal.Detail = abnormalBinNo;
                    }
                    else
                    {
                        resultAbormal.Code = 1000;
                        resultAbormal.Detail = "24";
                    }
                    #endregion

                    #region 检查参数信息是否为空
                    //检查组件批次号是否为空
                    if (string.IsNullOrEmpty(p.ScanLotNumber) || p.ScanLotNumber == "")
                    {
                        resultAbormal.Message = "组件序列号为空！";
                        return resultAbormal;
                    }

                    if (localName == "K01")
                    {
                        //检查读头IP地址是否为空
                        if (string.IsNullOrEmpty(p.ScanIP) || p.ScanIP == "")
                        {
                            resultAbormal.Message = "读头IP地址为空！";
                            return resultAbormal;
                        }
                    }
                    #endregion

                    #region 取得并判断批次信息
                    string lotNumber = p.ScanLotNumber;

                    //取得批次信息
                    Lot lot = this.LotDataEngine.Get(lotNumber);

                    //判断批次是否存在。
                    if (lot == null || lot.Status == EnumObjectStatus.Disabled)
                    {
                        resultAbormal.Message = string.Format("批次：（{0}）不存在！", lotNumber);
                        return resultAbormal;
                    }


                    //批次已撤销
                    if (lot.DeletedFlag == true)
                    {
                        resultAbormal.Message = string.Format("批次：（{0}）已删除！", lotNumber);
                        return resultAbormal;
                    }
                    //批次已暂停
                    if (lot.HoldFlag == true)
                    {
                        resultAbormal.Message = string.Format("批次：（{0}）已暂停！", lotNumber);
                        return resultAbormal;
                    }

                    //判断Lot的等级是否是A级
                    if (string.IsNullOrEmpty(lot.Grade) || lot.Grade.ToUpper() != "A")
                    {
                        resultAbormal.Message = string.Format("批次（{0}）等级不是A级。", lotNumber);
                        return resultAbormal;
                    }
                    #endregion

                    #region 检查工序是否是包装工序
                    //取得当前批次所在站别是否包装流程站别属性
                    RouteOperationAttribute roAttr = this.RouteOperationAttributeDataEngine.Get(new RouteOperationAttributeKey()
                    {
                        RouteOperationName = lot.RouteStepName,     //当前批次所在工序
                        AttributeName = "IsPackageOperation"        //包装站属性名
                    });

                    //如果没有设置为包装工序，则直接返回。
                    if (roAttr == null)
                    {
                        resultAbormal.Message = string.Format("产品：({0})在({1})工序，请确认。", lotNumber, lot.RouteStepName);
                        return resultAbormal;
                    }
                    #endregion

                    #region 判断Lot是否已包装并进行数据纠错
                    if (lot.PackageFlag == true)
                    {
                        if (localName == "K01")
                        {
                            //智能纠错（仅在当前线上该组件有最后入bin记录）
                            //取得读头设备信息
                            equipment = this.EquipmentDataEngine.Get(p.ScanIP);

                            if (equipment == null)
                            {
                                resultAbormal.Message = string.Format("读头：（{0}）信息未设置！", p.ScanIP);
                                return resultAbormal;
                            }

                            //读头线别与批次线别是否相同
                            if (equipment.LineCode != lot.LineCode)
                            {
                                resultAbormal.Message = string.Format("批次（{0}）已包装！",
                                                            p.ScanLotNumber);
                                return resultAbormal;
                            }
                        }
                        if (localName == "G01")
                        {
                            //读头线别与批次线别是否相同
                            if (p.PackageLine != lot.LineCode)
                            {
                                resultAbormal.Message = string.Format("批次（{0}）已包装！",
                                                            p.ScanLotNumber);
                                return resultAbormal;
                            }
                        }

                        //取得包装号是否在当前读头自动包装线并取得对应当前有效bin号
                        cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format("Key.PackageLine='{0}' and Key.PackageNo='{1}' and BinState=0",
                                                p.PackageLine,
                                                lot.PackageNo)
                        };

                        lstPackageBin = PackageBinDataEngine.Get(cfg);

                        if (lstPackageBin == null || lstPackageBin.Count == 0)      //非bin包装号
                        {
                            resultAbormal.Message = string.Format("批次（{0}）所在包装({1})不存在当前包装线（{2}）！",
                                                        lotNumber,
                                                        lot.PackageNo,
                                                        p.PackageLine);
                            return resultAbormal;
                        }
                        else
                        {
                            //确定是否为最后一个入BIN组件
                            //取得最后一个入托组件
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format("Key.PackageNo='{0}' ",
                                                      lot.PackageNo),
                                OrderBy = " ItemNo desc"
                            };

                            IList<PackageDetail> lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);

                            if (lstPackageDetail != null && lstPackageDetail.Count > 0)
                            {
                                if (lstPackageDetail[0].Key.ObjectNumber == lotNumber)
                                {
                                    //当前组件再次入BIin
                                    result.Code = 1;
                                    result.Detail = lstPackageBin[0].Key.BinNo;                 //入Bin号
                                    result.ObjectNo = lstPackageDetail[0].ItemNo.ToString();    //入托序列号

                                    return result;
                                }
                                else
                                {
                                    //非最后入Bin组件
                                    resultAbormal.Message = string.Format("批次({0})已包装！",
                                                                    lotNumber);
                                    return resultAbormal;
                                }
                            }
                            else
                            {
                                resultAbormal.Message = string.Format("包装({0})数据异常！",
                                                                lot.PackageNo);
                                return resultAbormal;
                            }
                        }
                    }
                    #endregion

                    #region 获取批次IV测试数据
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
                        powersetCode = lstTestData[0].PowersetCode;                 //分档组
                        powersetCodeItemNo = lstTestData[0].PowersetItemNo ?? -1;   //分档代码
                        powersetSubCode = lstTestData[0].PowersetSubCode;           //子分档代码
                    }
                    else
                    {
                        resultAbormal.Message = string.Format("批次：（{0}） IV测试数据不存在！", lotNumber);
                        return resultAbormal;
                    }
                    #endregion

                    #region 获取对应的Bin信息

                    if (localName == "K01")
                    {
                        //取得读头设备信息
                        equipment = this.EquipmentDataEngine.Get(p.ScanIP);

                        if (equipment == null)
                        {
                            resultAbormal.Message = string.Format("读头：（{0}）信息未设置！", p.ScanIP);
                            return resultAbormal;
                        }
                    }

                    //Bin测试规则参数
                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"Key.PackageLine='{0}'  
                                          AND Key.PsCode='{1}' 
                                          AND Key.PsItemNo='{2}'  
                                          AND Key.PsSubCode='{3}' 
                                          AND Key.Color='{4}'
                                          AND ( Key.WorkOrderNumber='{5}' or Key.WorkOrderNumber='{6}')",
                                              p.PackageLine,
                                              powersetCode,
                                              powersetCodeItemNo,
                                              powersetSubCode,
                                              lot.Color,
                                              lot.OrderNumber,
                                              "*"),
                        OrderBy = " Key.BinNo "
                    };

                    //取得Bin规则清单
                    IList<BinRule> lstBinRules = this.BinRuleDataEngine.Get(cfg);

                    if (lstBinRules.Count > 0)
                    {
                        //遍历所有的节点后优先入未满Bin，在空Bin中选择编辑时间最晚Bin
                        DateTime? dtEditTime = DateTime.Now;
                        bool isInPackage = false;
                        int iCount = 0;

                        for (int i = 0; i < lstBinRules.Count; i++)
                        {
                            //取得包装Bin信息
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format("Key.BinNo='{0}' and Key.PackageLine='{1}'",
                                                        lstBinRules[i].Key.BinNo,
                                                        p.PackageLine),
                                OrderBy = " EditTime desc"
                            };

                            lstPackageBin = PackageBinDataEngine.Get(cfg);

                            if (lstPackageBin == null || lstPackageBin.Count == 0)  //入Bin记录未生成，在无未满托时优先入新Bin
                            {
                                if (intNewBinCount == 0)
                                {
                                    //取得Bin信息
                                    strBinNo = lstBinRules[i].Key.BinNo;            //规则设置对应Bin号
                                    intInBinOrder = 1;                              //入Bin序列号

                                    //结果计数器
                                    iCount++;
                                }

                                intNewBinCount++;
                            }
                            else
                            {
                                //判断Bin是否包装完成
                                if (lstPackageBin[0].BinPackaged == EnumBinPackaged.Finished)
                                {
                                    //判断高优先级新Bin规则是否存在
                                    if (intNewBinCount == 0)
                                    {
                                        //判断是否是否最晚时间
                                        if (lstPackageBin[0].EditTime <= dtEditTime)
                                        {
                                            //取得Bin信息
                                            strBinNo = lstPackageBin[0].Key.BinNo;              //Bin号
                                            intInBinOrder = 1;                                  //入Bin序列号
                                            dtEditTime = lstPackageBin[0].EditTime;             //最后编辑时间

                                            //结果计数器
                                            iCount++;
                                        }
                                    }
                                }
                                else
                                {
                                    //判断是否满足入托规则 
                                    result = CheckLotInPackageRule(lot, lstPackageBin[0].Key.PackageNo, out isInPackage);
                                    if (result.Code > 0)        //产生错误
                                    {
                                        return resultAbormal;
                                    }
                                    else
                                    {
                                        if (isInPackage)
                                        {
                                            //若为未满Bin优先入未满Bin
                                            //取得Bin信息
                                            strBinNo = lstPackageBin[0].Key.BinNo;          //Bin号
                                            intInBinOrder = int.Parse(result.ObjectNo);     //将入托序列号
                                            strPackageNo = lstPackageBin[0].Key.PackageNo;  //将入托包装号
                                            dtEditTime = lstPackageBin[0].EditTime;         //最后编辑时间

                                            //结果计数器
                                            iCount++;

                                            //退出搜寻
                                            i = lstBinRules.Count;
                                        }
                                        else
                                        {
                                            result.Message = "";
                                        }
                                    }
                                }
                            }
                        }

                        if (iCount == 0)    //未找到符合条件Bin
                        {
                            resultAbormal.Message = string.Format("批次：（{0}） 无对应Bin！", lotNumber);
                            return resultAbormal;
                        }
                    }
                    else
                    {
                        resultAbormal.Message = string.Format("批次：（{0}） 无对应Bin！", lotNumber);
                        return resultAbormal;
                    }
                    #endregion

                    result.Code = 0;
                    result.Detail = strBinNo;                       //入Bin号
                    result.ObjectNo = intInBinOrder.ToString();     //入托序列号

                    if (localName == "K01")
                    {
                        p.EquipmentCode = equipment.Key;                //返回设备代码
                    }
                    if (localName == "G01")
                    {
                        p.EquipmentCode = p.PackageLine;                //返回设备代码
                    }
                    p.LineCode = p.PackageLine;                      //返回线别
                    p.PackageNo = strPackageNo;                     //入托包装号
                    return result;
                }
                catch (Exception ex)
                {
                    result.Code = 1000;
                    result.Message = ex.Message;
                    result.Detail = ex.ToString();

                    return result;
                }
                #endregion
            }
            return result;
        }

        /// <summary>
        /// 判断批次是否可以入托(包装)
        /// </summary>
        /// <param name="lotNumber">        批次号</param>
        /// <param name="packageNo">        包装号</param>
        /// <param name="isInPackage">      是否可以入托 （ true - 可以入托 false - 不可以入托）</param>
        /// <returns>1.code        -- 0 - 成功 其它 - 失败</returns>
        ///          2.Message     -- 错误信息
        ///          3.ObjectNo    -- 组件的入Bin位置代码  
        private MethodReturnResult CheckLotInPackageRule( Lot lot, string packageNo, out bool isInPackage )
        {
            MethodReturnResult result = new MethodReturnResult();
            bool isPackageLimitedForWorkOrder = false;
            PagingConfig cfg = null;
            isInPackage = false;

            if (packageType == "31")
            {
                #region 隔块包护角
                try
                {                    
                    //判断批次是否存在。
                    if (lot == null || lot.Status == EnumObjectStatus.Disabled)
                    {
                        result.Code = 1001;
                        result.Message = string.Format("批次：（{0}）不存在！", lot.Key);
                        return result;
                    }

                    //取得包装属性
                    Package packageObj = this.PackageDataEngine.Get(packageNo);

                    //判断包装号是否存在。
                    if (packageObj == null)
                    {
                        result.Code = 1002;
                        result.Message = string.Format("包装号：（{0}）不存在！", packageNo);
                        return result;
                    }

                    //包装是否包装状态
                    if (packageObj.PackageState != EnumPackageState.Packaging)
                    {
                        result.Code = 1000;
                        result.Message = string.Format("包装号：（{0}）已完成包装！", packageNo);
                        return result;
                    }

                    //判断产品是否一致，必须一致方可入托
                    if (packageObj.MaterialCode != lot.MaterialCode && packageObj.MaterialCode != "")
                    {
                        result.Code = 0;
                        result.Message = string.Format("包装物料({0})与批次物料({1})不一致！",
                                                       packageObj.MaterialCode,
                                                       lot.MaterialCode);
                        return result;
                    }

                    #region 判断工单是否一致及是否可以混工单
                    if (lot.OrderNumber != packageObj.OrderNumber)
                    {
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
                            result.Code = 0;
                            result.Message = string.Format("批次：（{0}）所在工单（{1}）不允许混工单！", lot.Key, lot.OrderNumber);

                            return result;
                        }

                        //托工单混托属性
                        WorkOrderAttribute packageWorkOrderAttribute = this.WorkOrderAttributeDataEngine.Get(new WorkOrderAttributeKey()
                        {
                            OrderNumber = packageObj.OrderNumber,
                            AttributeName = "PackageLimited"
                        });

                        //未设置默认为允许混工单(false)
                        if (packageWorkOrderAttribute == null || !bool.TryParse(packageWorkOrderAttribute.AttributeValue, out isPackageLimitedForWorkOrder))
                        {
                            isPackageLimitedForWorkOrder = false;
                        }

                        if (isPackageLimitedForWorkOrder == true)
                        {
                            result.Code = 0;
                            result.Message = string.Format("托：（{0}）所在工单（{1}）不允许混工单！", packageNo, packageObj.OrderNumber);
                            return result;
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
                                Where = string.Format(@"Key.OrderNumber = '{0}'", packageObj.OrderNumber)
                            };
                            IList<WorkOrderGroupDetail> lstPackageWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                            //托工单设置了混工单组
                            if (lstPackageWorkOrderGroupDetail != null && lstPackageWorkOrderGroupDetail.Count > 0)
                            {
                                if (lstLotWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString() != lstPackageWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString())
                                {
                                    result.Code = 0;
                                    result.Message = string.Format("托：（{0}）所在工单（{1} 设置的混工单组（{2}）与入托批次（{3}）所在工单（{4}）设置的混工单组（{5}）不一致！",
                                                                    packageNo, packageObj.OrderNumber, lstPackageWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString(),
                                                                    lot.Key, lot.OrderNumber, lstLotWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString());
                                    return result;
                                }
                            }
                            //托工单没设混工单组
                            else
                            {
                                result.Code = 0;
                                result.Message = string.Format("托：（{0}）所在工单（{1} 未设置混工单组规则，但要入托批次（{2}）所在工单（{3}）设置了混工单组！",
                                                                packageNo, packageObj.OrderNumber, lot.Key, lot.OrderNumber);
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
                                Where = string.Format(@"Key.OrderNumber = '{0}'", packageObj.OrderNumber)
                            };
                            IList<WorkOrderGroupDetail> lstPackageWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                            //托工单设置了混工单组
                            if (lstPackageWorkOrderGroupDetail != null && lstPackageWorkOrderGroupDetail.Count > 0)
                            {
                                result.Code = 0;
                                result.Message = string.Format("托：（{0}）所在工单（{1} 已设置混工单组规则，但要入托批次（{2}）所在工单（{3}）未设置混工单组！",
                                                                packageNo, packageObj.OrderNumber, lot.Key, lot.OrderNumber);
                                return result;
                            }
                            #endregion
                        }
                        #endregion
                    }
                    #endregion

                    #region 检查电池片供应商是否可以混装
                    //取得是否允许不同的电池片供应商是否可以混装
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
                        //取得批次电池片厂商
                        string strSupplierCodeForLot = "";      //批次电池片厂商

                        cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format(@"Key.LotNumber='{0}' and MaterialCode like '%{1}%'",
                                                    lot.Key,
                                                    "110")
                        };

                        IList<LotBOM> lstLotBom = this.LotBOMDataEngine.Get(cfg);
                        if (lstLotBom != null && lstLotBom.Count > 0)
                        {
                            //取得批次对应电池片供应商
                            strSupplierCodeForLot = lstLotBom[0].SupplierCode;
                        }

                        //判断电池片供应商是否一致
                        if (string.Compare(packageObj.SupplierCode, strSupplierCodeForLot, true) != 0)
                        {
                            result.Code = 0;
                            result.Message = string.Format("托（{0}）电池片供应商（{1}）与批次（{2}）电池片供应商（{3}）不一致。",
                                                            packageNo,
                                                            packageObj.SupplierCode,
                                                            lot.Key,
                                                            strSupplierCodeForLot);
                            return result;
                        }
                    }
                    #endregion

                    //设置返回参数
                    isInPackage = true;                                             //可入托标志
                    result.ObjectNo = ((int)packageObj.Quantity + 1).ToString();    //最后包装序列号
                    result.Code = 0;

                    return result;
                }
                catch (Exception ex)
                {
                    result.Code = 1000;
                    result.Message = ex.Message;
                    result.Detail = ex.ToString();

                    isInPackage = false;

                    return result;
                }
                #endregion
            }
            if (packageType == "30")
            {
                #region 全包护角
                try
                {
                    //判断批次是否存在。
                    if (lot == null || lot.Status == EnumObjectStatus.Disabled)
                    {
                        result.Code = 1001;
                        result.Message = string.Format("批次：（{0}）不存在！", lot.Key);
                        return result;
                    }

                    //取得包装属性
                    Package packageObj = this.PackageDataEngine.Get(packageNo);

                    //判断包装号是否存在。
                    if (packageObj == null)
                    {
                        result.Code = 1002;
                        result.Message = string.Format("包装号：（{0}）不存在！", packageNo);
                        return result;
                    }

                    //包装是否包装状态
                    if (packageObj.PackageState != EnumPackageState.Packaging)
                    {
                        result.Code = 1000;
                        result.Message = string.Format("包装号：（{0}）已完成包装！", packageNo);
                        return result;
                    }

                    //判断产品是否一致，必须一致方可入托
                    if (packageObj.MaterialCode != lot.MaterialCode && packageObj.MaterialCode != "")
                    {
                        result.Code = 0;
                        result.Message = string.Format("包装物料({0})与批次物料({1})不一致！",
                                                       packageObj.MaterialCode,
                                                       lot.MaterialCode);
                        return result;
                    }

                    #region 判断工单是否一致及是否可以混工单
                    if (lot.OrderNumber != packageObj.OrderNumber)
                    {
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
                            result.Code = 0;
                            result.Message = string.Format("批次：（{0}）所在工单（{1}）不允许混工单！", lot.Key, lot.OrderNumber);

                            return result;
                        }

                        //托工单混托属性
                        WorkOrderAttribute packageWorkOrderAttribute = this.WorkOrderAttributeDataEngine.Get(new WorkOrderAttributeKey()
                        {
                            OrderNumber = packageObj.OrderNumber,
                            AttributeName = "PackageLimited"
                        });

                        //未设置默认为允许混工单(false)
                        if (packageWorkOrderAttribute == null || !bool.TryParse(packageWorkOrderAttribute.AttributeValue, out isPackageLimitedForWorkOrder))
                        {
                            isPackageLimitedForWorkOrder = false;
                        }

                        if (isPackageLimitedForWorkOrder == true)
                        {
                            result.Code = 0;
                            result.Message = string.Format("托：（{0}）所在工单（{1}）不允许混工单！", packageNo, packageObj.OrderNumber);
                            return result;
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
                                Where = string.Format(@"Key.OrderNumber = '{0}'", packageObj.OrderNumber)
                            };
                            IList<WorkOrderGroupDetail> lstPackageWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                            //托工单设置了混工单组
                            if (lstPackageWorkOrderGroupDetail != null && lstPackageWorkOrderGroupDetail.Count > 0)
                            {
                                if (lstLotWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString() != lstPackageWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString())
                                {
                                    result.Code = 0;
                                    result.Message = string.Format("托：（{0}）所在工单（{1} 设置的混工单组（{2}）与入托批次（{3}）所在工单（{4}）设置的混工单组（{5}）不一致！",
                                                                    packageNo, packageObj.OrderNumber, lstPackageWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString(),
                                                                    lot.Key, lot.OrderNumber, lstLotWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString());
                                    return result;
                                }
                            }
                            //托工单没设混工单组
                            else
                            {
                                result.Code = 0;
                                result.Message = string.Format("托：（{0}）所在工单（{1} 未设置混工单组规则，但要入托批次（{2}）所在工单（{3}）设置了混工单组！",
                                                                packageNo, packageObj.OrderNumber, lot.Key, lot.OrderNumber);
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
                                Where = string.Format(@"Key.OrderNumber = '{0}'", packageObj.OrderNumber)
                            };
                            IList<WorkOrderGroupDetail> lstPackageWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                            //托工单设置了混工单组
                            if (lstPackageWorkOrderGroupDetail != null && lstPackageWorkOrderGroupDetail.Count > 0)
                            {
                                result.Code = 0;
                                result.Message = string.Format("托：（{0}）所在工单（{1} 已设置混工单组规则，但要入托批次（{2}）所在工单（{3}）未设置混工单组！",
                                                                packageNo, packageObj.OrderNumber, lot.Key, lot.OrderNumber);
                                return result;
                            }
                            #endregion
                        }
                        #endregion
                    }
                    #endregion

                    #region 检查电池片供应商是否可以混装
                    //取得是否允许不同的电池片供应商是否可以混装
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
                        //取得批次电池片厂商
                        string strSupplierCodeForLot = "";      //批次电池片厂商

                        cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format(@"Key.LotNumber='{0}' and MaterialCode like '%{1}%'",
                                                    lot.Key,
                                                    "110"),
                            OrderBy = ""
                        };

                        IList<LotBOM> lstLotBom = this.LotBOMDataEngine.Get(cfg);
                        if (lstLotBom != null && lstLotBom.Count > 0)
                        {
                            //取得批次对应电池片供应商
                            strSupplierCodeForLot = lstLotBom[0].SupplierCode;
                        }

                        //判断电池片供应商是否一致
                        if (string.Compare(packageObj.SupplierCode, strSupplierCodeForLot, true) != 0)
                        {
                            result.Code = 0;
                            result.Message = string.Format("托（{0}）电池片供应商（{1}）与批次（{2}）电池片供应商（{3}）不一致。",
                                                            packageNo,
                                                            packageObj.SupplierCode,
                                                            lot.Key,
                                                            strSupplierCodeForLot);
                            return result;
                        }
                    }
                    #endregion

                    //设置返回参数
                    isInPackage = true;                                             //可入托标志
                    result.ObjectNo = ((int)packageObj.Quantity + 1).ToString();    //最后包装序列号
                    result.Code = 0;

                    return result;
                }
                catch (Exception ex)
                {
                    result.Code = 1000;
                    result.Message = ex.Message;
                    result.Detail = ex.ToString();

                    isInPackage = false;

                    return result;
                }
                #endregion
            }
            return result;                        
        }

        /// <summary>
        /// 批次入Bin操作
        /// </summary>
        /// <param name="p">1.ScanLotNumber    批次代码
        ///                 2.ScanIP;          设备IP地址
        ///                 3.LineCode;        当前操作线别
        ///                 4.EquipmentCode;   设备代码
        ///                 5.PackageNo        包装号
        /// </param>
        /// <param name="inPackageOrder"></param>
        /// <returns>1.code         -- 执行代码 0 - 成功， 其它 - 失败</returns>
        ///          2.Message      -- 错误信息
        private MethodReturnResult ExecuteInBinEx(InBinParameter p,int inPackageOrder)
        {
            MethodReturnResult result = new MethodReturnResult();
            PagingConfig cfg = null;
            ISession session = null;
            ITransaction transaction = null;

            if (packageType == "31")
            {
                #region 隔块包护角
                try
                {
                    string strLotNumber;                                //批次代码
                    string strPackageNo;                                //托号
                    int iQtyMax = 0;                                    //托最大入托数
                    EnumPackageState IsFinishPackage = 0;               //托是否完成包装 0 - 包装中 1 - 完成包装
                    PackageBin packageBinObj = null;                    //Bin对象
                    PackageBin packageBinObjHis = null;                 //历史Bin对象
                    Package packageObject = null;                       //托包装对象
                    PackageDetail packageDetail = null;                 //托包装明细对象
                    LotTransactionPackage transPackageObj = null;       //托包装事物对象
                    Lot lot = null;                                     //批次对象
                    LotTransaction lotTransObj = null;                  //批次事物对象
                    LotTransactionHistory lotTransHistoryObj = null;    //批次历史事物对象
                    LotTransactionEquipment lotTransactionEquipment = null;     //加工设备事物对象
                    DateTime now = DateTime.Now;                        //当前时间
                    string strSupplierCodeForLot = "";                  //批次电池片供应商
                    string transactionKey = "";                         //操作事务主键
                    IList<LotBOM> lstLotBom = null;                     //批次BOM列表
                    IList<PackageBin> lstPackageBin;                    //Bin对象列表
                    bool isNewEquipmentTran = false;                    //设备事物是否为新建

                    #region 取得批次信息
                    strLotNumber = p.ScanLotNumber;
                    lot = this.LotDataEngine.Get(strLotNumber);

                    //判断批次是否存在。
                    if (lot == null || lot.Status == EnumObjectStatus.Disabled)
                    {
                        result.Code = 1001;
                        result.Message = string.Format("批次：（{0}）不存在！", strLotNumber);
                        return result;
                    }

                    //设置批次属性
                    lot.PackageFlag = true;                     //包装标识
                    lot.OperateComputer = p.ScanIP;             //操作客户端
                    lot.PreLineCode = lot.LineCode;             //上工序线别
                    lot.LineCode = p.LineCode;                  //当前操作线别
                    lot.Editor = p.ScanIP;                      //编辑人
                    lot.EditTime = now;                         //编辑日期
                    lot.EquipmentCode = p.EquipmentCode;        //设备代码                
                    lot.StateFlag = EnumLotState.WaitTrackIn;   //包装出站状态

                    //取得工单设置最大入托数
                    CommonObjectDataEngine<WorkOrderRule, WorkOrderRuleKey> commonObjectDataEngine;
                    commonObjectDataEngine = new CommonObjectDataEngine<WorkOrderRule, WorkOrderRuleKey>(LotDataEngine.SessionFactory);

                    WorkOrderRuleKey workOrderRuleKey = new WorkOrderRuleKey
                    {
                        OrderNumber = lot.OrderNumber,
                        MaterialCode = lot.MaterialCode
                    };

                    WorkOrderRule workOrderRule = commonObjectDataEngine.Get(workOrderRuleKey);
                    string lineCode = p.LineCode;
                    //iQtyMax = 31;

                    if (workOrderRule != null)
                    {
                        int.TryParse(workOrderRule.FullPackageQty.ToString(), out iQtyMax);
                    }
                    else
                    {
                        result.Code = 1002;
                        result.Message = "最大满托数未设置！";
                        return result;
                    }

                    #endregion

                    #region 创建托包装对象
                    //判断托是否完成包装（最后一块入托）
                    if (iQtyMax > inPackageOrder)
                    {
                        IsFinishPackage = EnumPackageState.Packaging;
                    }
                    else
                    {
                        IsFinishPackage = EnumPackageState.Packaged;
                    }

                    //判断是否新托
                    if (inPackageOrder == 1)
                    {
                        //1.取得新托号
                        //strPackageNo = PackageNoGenerate.Generate(p.ScanLotNumber, false);

                        MethodReturnResult<string> resultPackageNo = new MethodReturnResult<string>();
                        resultPackageNo = PackageNoGenerate.CreatePackageNo(p.ScanLotNumber);
                        if (resultPackageNo.Code > 0)
                        {
                            result.Code = resultPackageNo.Code;
                            result.Message = resultPackageNo.Message;
                            return result;
                        }

                        strPackageNo = resultPackageNo.Data;

                        //2.取得批次电池片厂商
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

                        //3.创建托对象
                        packageObject = new Package()
                        {
                            Key = strPackageNo,                     //主键托号
                            ContainerNo = null,                     //容器号
                            OrderNumber = lot.OrderNumber,          //工单号
                            PackageState = IsFinishPackage,         //包装状态
                            MaterialCode = lot.MaterialCode,        //物料代码
                            IsLastPackage = false,                  //是否尾包
                            Quantity = inPackageOrder,              //包装数量
                            PackageType = EnumPackageType.Packet,   //包装类型 Packet=0 ---按包 Box=1 --- 按箱
                            Description = "",                       //描述
                            Checker = null,                         //检验人
                            CheckTime = null,                       //检验时间
                            ToWarehousePerson = null,               //入库人
                            ToWarehouseTime = null,                 //入库时间
                            ShipmentPerson = null,                  //出货人
                            ShipmentTime = null,                    //出货时间
                            Creator = p.EquipmentCode,              //创建人
                            CreateTime = now,                       //创建时间                        
                            Editor = p.EquipmentCode,               //编辑人             
                            EditTime = now,                         //编辑时间 
                            SupplierCode = strSupplierCodeForLot,   //电池片供应商编号
                            PackageMixedType = EnumPackageMixedType.UnMixedType //包装最大类型UnMixedType=0 ---按包 MixedType=1 --- 按箱
                        };
                    }
                    else
                    {
                        //取得托号
                        strPackageNo = p.PackageNo;

                        //取得托信息
                        packageObject = this.PackageDataEngine.Get(strPackageNo);

                        //判断包装号是否存在。
                        if (packageObject == null)
                        {
                            result.Code = 1002;
                            result.Message = string.Format("包装号：（{0}）不存在！", strPackageNo);
                            return result;
                        }

                        //包装是否包装状态
                        if (packageObject.PackageState != EnumPackageState.Packaging)
                        {
                            result.Code = 0;
                            result.Message = string.Format("包装号：（{0}）已完成包装！", strPackageNo);
                            return result;
                        }

                        //设置托包装属性
                        packageObject.Quantity = inPackageOrder;        //包装数量
                        packageObject.PackageState = IsFinishPackage;   //包装状态
                        packageObject.Editor = p.EquipmentCode;         //编辑人（设备代码）
                        packageObject.EditTime = now;                   //编辑日期
                    }
                    #endregion

                    #region 创建托包装明细对象
                    packageDetail = new PackageDetail()
                    {
                        Key = new PackageDetailKey()
                        {
                            PackageNo = strPackageNo,               //托号
                            ObjectNumber = strLotNumber,            //批次代码
                            ObjectType = EnumPackageObjectType.Lot  //Lot=0 批次 Packet=1 小包
                        },
                        ItemNo = inPackageOrder,                    //入托项目号（入托顺序）
                        Creator = p.EquipmentCode,                  //创建人
                        CreateTime = now,                           //创建时间                   
                        MaterialCode = lot.MaterialCode,            //物料编码
                        OrderNumber = lot.OrderNumber               //工单代码
                    };
                    #endregion

                    #region 设置批次包装属性
                    lot.PackageNo = strPackageNo;                   //托号
                    #endregion

                    #region 创建Bin对象
                    //取得当前Bin信息
                    cfg = new PagingConfig()
                    {
                        PageNo = 0,
                        PageSize = 1,
                        Where = string.Format("Key.BinNo='{0}' and Key.PackageLine='{1}' ",
                                            p.BinNo,
                                            p.LineCode),
                        OrderBy = " EditTime desc"
                    };

                    lstPackageBin = PackageBinDataEngine.Get(cfg);

                    //是否新入托
                    if (inPackageOrder == 1)
                    {
                        //原Bin对象
                        if (lstPackageBin != null && lstPackageBin.Count > 0)
                        {
                            packageBinObjHis = lstPackageBin.FirstOrDefault();

                            packageBinObjHis.BinState = 1;   //Bin状态 0 - 当前Bin 1 - 非当前Bin数据
                        }

                        //新增Bin对象
                        PackageBinKey packageBinKey = new PackageBinKey
                        {
                            PackageLine = p.LineCode,       //线别
                            BinNo = p.BinNo,                //Bin代码
                            PackageNo = strPackageNo        //托包装号
                        };

                        packageBinObj = new PackageBin
                        {
                            Key = packageBinKey,            //Bin主键
                            BinQty = inPackageOrder,        //Bin数量
                            BinMaxQty = iQtyMax,            //Bin内托最大包装数量
                            BinPackaged = (EnumBinPackaged)IsFinishPackage,   //Bin包装状态与包装状态相同（UnFinished = 0---未完成，Finished = 1---完成）
                            BinState = 0,                   //Bin状态
                            Creator = p.EquipmentCode,      //创建人（设备代码）
                            CreateTime = now,               //创建日期         
                            Editor = p.EquipmentCode,       //编辑人（设备代码）
                            EditTime = now                  //编辑日期
                        };
                    }
                    else
                    {
                        if (lstPackageBin == null || lstPackageBin.Count == 0)
                        {
                            result.Code = 1000;
                            result.Message = string.Format("包装线（{1}）Bin({0}对应的数据异常，未找到历史数据！) ",
                                                            p.BinNo,
                                                            p.LineCode);

                            return result;
                        }

                        //取得当前Bin对象
                        packageBinObj = lstPackageBin.FirstOrDefault();

                        //设置属性
                        packageBinObj.BinPackaged = (EnumBinPackaged)IsFinishPackage;   //Bin包装状态与包装状态相同（UnFinished = 0---未完成，Finished = 1---完成）
                        packageBinObj.BinQty = inPackageOrder;                          //Bin数量
                        packageBinObj.Editor = p.EquipmentCode;                         //编辑人（设备代码）
                        packageBinObj.EditTime = now;                                   //编辑日期
                    }

                    #endregion

                    #region ***创建事物对象***
                    //生成操作事务主键。
                    transactionKey = Guid.NewGuid().ToString();

                    #region 批次事物对象
                    //批次事物对象LotTransaction（表WIP_TRANSACTION）
                    lotTransObj = new LotTransaction()
                    {
                        Key = transactionKey,                   //事物主键
                        Activity = EnumLotActivity.Package,     //批次操作类型（包装）                    
                        InQuantity = lot.Quantity,              //操作数量
                        LotNumber = lot.Key,                    //批次代码                    
                        OrderNumber = lot.OrderNumber,          //工单号
                        OutQuantity = lot.Quantity,             //操作后数量
                        LocationName = lot.LocationName,        //生产车间
                        LineCode = p.LineCode,                  //线别
                        RouteEnterpriseName = lot.RouteEnterpriseName,  //工艺流程组
                        RouteName = lot.RouteName,              //工艺流程
                        RouteStepName = lot.RouteStepName,      //工步
                        ShiftName = "",                         //班别
                        UndoFlag = false,                       //撤销标识
                        UndoTransactionKey = null,              //撤销记录主键
                        Description = "",                       //备注
                        Color = lot.Color,
                        Attr1 = lot.Attr1,
                        Attr2 = lot.Attr2,
                        Attr3 = lot.Attr3,
                        Attr4 = lot.Attr4,
                        Attr5 = lot.Attr5,
                        OperateComputer = p.ScanIP,             //操作客户端
                        Creator = p.ScanIP,                     //创建人
                        CreateTime = now,                       //创建时间                  
                        Editor = p.ScanIP,                      //编辑人
                        EditTime = now                          //编辑时间
                    };

                    //新增批次事物历史记录TransactionHistory（表WIP_TRANSACTION_LOT）  
                    lotTransHistoryObj = new LotTransactionHistory(transactionKey, lot);

                    //新增工艺下一步记录。
                    //nextLotStep = new LotTransactionStep()
                    //{
                    //    Key = transactionKey,                               //事物主键
                    //    ToRouteEnterpriseName = lot.RouteEnterpriseName,    //工艺流程组
                    //    ToRouteName = lot.RouteName,                        //工艺流程
                    //    ToRouteStepName = lot.RouteStepName,                //工步
                    //    Editor = p.ScanIP,                                  //编辑人
                    //    EditTime = now                                      //编辑日期
                    //};
                    #endregion

                    #region 托包装事物对象
                    //记录包装操作事物对象                
                    transPackageObj = new LotTransactionPackage()
                    {
                        Key = transactionKey,                           //事物主键
                        PackageNo = packageObject.Key,                  //托包装号
                        Editor = p.EquipmentCode,                       //编辑人（设备代码）
                        EditTime = now                                  //编辑时间
                    };
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
                                                    strLotNumber,
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
                                LotNumber = strLotNumber,                       //加工批次
                                Quantity = 1,                                   //加工数量（默认1后期优化）
                                StartTime = now,                                //加工开始时间
                                EndTime = now,                                  //加工结束时间
                                Creator = p.EquipmentCode,                      //创建人（设备代码）
                                CreateTime = now,                               //创建时间
                                Editor = p.EquipmentCode,                       //编辑人（设备代码）
                                EditTime = now,                                 //编辑时间
                                State = EnumLotTransactionEquipmentState.End    //事物状态（1 - 结束）
                            };

                            isNewEquipmentTran = true;                          //设备事物对象状态-NEW
                        }
                    }
                    #endregion

                    #endregion

                    #region 开始事物处理
                    session = this.LotDataEngine.SessionFactory.OpenSession();
                    transaction = session.BeginTransaction();

                    #region 1.更新批次基本信息
                    lot.PackageNo = strPackageNo;                   //托包装号

                    this.LotDataEngine.Update(lot, session);

                    //更新批次事物LotTransaction（表WIP_TRANSACTION）信息
                    this.LotTransactionDataEngine.Insert(lotTransObj, session);

                    //更新批次历史事物TransactionHistory（表WIP_TRANSACTION_LOT）信息
                    this.LotTransactionHistoryDataEngine.Insert(lotTransHistoryObj, session);

                    #endregion

                    #region 2.包装数据
                    if (inPackageOrder == 1)        //新托号
                    {
                        this.PackageDataEngine.Insert(packageObject, session);
                    }
                    else
                    {
                        this.PackageDataEngine.Update(packageObject, session);
                    }

                    //2.1.包装明细数据
                    this.PackageDetailDataEngine.Insert(packageDetail, session);

                    //2.2.包装事物
                    this.LotTransactionPackageDataEngine.Insert(transPackageObj, session);
                    #endregion

                    #region 3.Bin数据
                    if (inPackageOrder == 1)        //新托号
                    {
                        //新Bin数据
                        this.PackageBinDataEngine.Insert(packageBinObj, session);

                        //Bin历史数据,当对象为NULL即不存在历史对象不做处理
                        if (packageBinObjHis != null)
                        {
                            this.PackageBinDataEngine.Update(packageBinObjHis, session);
                        }
                    }
                    else
                    {
                        this.PackageBinDataEngine.Update(packageBinObj, session);
                    }
                    #endregion

                    #region 4.更新设备信息 , 设备的Event ,设备的Transaction
                    //4.1
                    if (isNewEquipmentTran == true)
                    {
                        this.LotTransactionEquipmentDataEngine.Insert(lotTransactionEquipment, session);
                    }
                    else
                    {
                        this.LotTransactionEquipmentDataEngine.Update(lotTransactionEquipment, session);
                    }

                    #endregion

                    //transaction.Rollback();
                    transaction.Commit();
                    session.Close();
                    #endregion

                    #region 自动成柜
                    using (PackageInChestCommom packageInChestService = new PackageInChestCommom())
                    {
                        //获取带属性的托号
                        MethodReturnResult<Package> haveAttrPackage = new MethodReturnResult<Package>();
                        haveAttrPackage = packageInChestService.GetAttrOfPackage(packageObject);
                        //获取产品成柜规则
                        MaterialChestParameter mcp = null;
                        mcp = this.MaterialChestParameterDataEngine.Get(haveAttrPackage.Data.MaterialCode);
                        if (mcp != null && mcp.IsPackagedChest)
                        {
                            if (IsFinishPackage == EnumPackageState.Packaged)
                            {
                                //ISessionFactory SessionFactory = this.PackageDataEngine.SessionFactory;
                                //PackageInChestService packageInChestService = new PackageInChestService(SessionFactory);                        
                                //获取最佳柜号
                                MethodReturnResult<string> chestNo = packageInChestService.GetChestNo(packageObject.Key, "", false, false);
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
                                    Editor = p.EquipmentCode,
                                    ChestNo = chestNo.Data,
                                    IsLastestPackageInChest = false,
                                    ChestFullQty = chestFullQty,
                                    StoreLocation = "",
                                    PackageNo = haveAttrPackage.Data.Key,
                                    isManual = false,
                                    ModelType = 1
                                };
                                if (currentQty == chestFullQty)
                                {
                                    chestParameter.IsFinishPackageInChest = true;
                                }
                                //成柜
                                result = packageInChestService.Chest(chestParameter);
                            }
                        }
                    }                    
                    #endregion

                    return result;
                }
                catch (Exception ex)
                {
                    result.Code = 1000;
                    result.Message = ex.Message;
                    result.Detail = ex.ToString();

                    transaction.Rollback();
                    session.Close();

                    return result;
                }
                #endregion
            }
            if (packageType == "30")
            {
                #region 全包护角
                try
                {
                    string strLotNumber;                                //批次代码
                    string strPackageNo;                                //托号
                    int iQtyMax = 0;                                    //托最大入托数
                    EnumPackageState IsFinishPackage = 0;               //托是否完成包装 0 - 包装中 1 - 完成包装
                    PackageBin packageBinObj = null;                    //Bin对象
                    PackageBin packageBinObjHis = null;                 //历史Bin对象
                    Package packageObject = null;                       //托包装对象
                    PackageDetail packageDetail = null;                 //托包装明细对象
                    LotTransactionPackage transPackageObj = null;       //托包装事物对象
                    Lot lot = null;                                     //批次对象
                    LotTransaction lotTransObj = null;                  //批次事物对象
                    LotTransactionHistory lotTransHistoryObj = null;    //批次历史事物对象
                    LotTransactionEquipment lotTransactionEquipment = null;     //加工设备事物对象
                    DateTime now = DateTime.Now;                        //当前时间
                    string strSupplierCodeForLot = "";                  //批次电池片供应商
                    string transactionKey = "";                         //操作事务主键
                    IList<LotBOM> lstLotBom = null;                     //批次BOM列表
                    IList<PackageBin> lstPackageBin;                    //Bin对象列表
                    bool isNewEquipmentTran = false;                    //设备事物是否为新建
                    IList<IVTestData> lstLotIVTestData = null;                  //自制组件IV测试数据
                    IList<WorkOrderPowerset> lstWorkOrderPowerset = null;       //自制组件工单分档规则
                    string locationName = string.Empty;                         //界面所选线别所在车间
                    ProductionLine line = new ProductionLine();                 //线别
                    Location location = new Location();                         //区域

                    #region 取得批次信息
                    strLotNumber = p.ScanLotNumber;

                    lot = this.LotDataEngine.Get(strLotNumber);

                    //判断批次是否存在。
                    if (lot == null || lot.Status == EnumObjectStatus.Disabled)
                    {
                        result.Code = 1001;
                        result.Message = string.Format("批次：（{0}）不存在！", strLotNumber);
                        return result;
                    }

                    if (localName == "K01")
                    {
                        //设置批次属性
                        lot.PackageFlag = true;                     //包装标识
                        lot.OperateComputer = p.ScanIP;             //操作客户端
                        lot.PreLineCode = lot.LineCode;             //上工序线别
                        lot.LineCode = p.LineCode;                  //当前操作线别
                        lot.Editor = p.ScanIP;                      //编辑人
                        lot.EditTime = now;                         //编辑日期
                        lot.EquipmentCode = p.EquipmentCode;        //设备代码                
                        lot.StateFlag = EnumLotState.WaitTrackIn;   //包装出站状态
                    }
                    if (localName == "G01")
                    {
                        //设置批次属性
                        lot.PackageFlag = true;                     //包装标识
                        lot.OperateComputer = p.PackageLine;        //操作客户端
                        lot.PreLineCode = lot.LineCode;             //上工序线别
                        lot.LineCode = p.LineCode;                  //当前操作线别
                        lot.Editor = p.PackageLine;                 //编辑人
                        lot.EditTime = now;                         //编辑日期
                        lot.EquipmentCode = p.EquipmentCode;        //设备代码                
                        lot.StateFlag = EnumLotState.WaitTrackIn;   //包装出站状态
                    }

                    //取得工单设置最大入托数
                    CommonObjectDataEngine<WorkOrderRule, WorkOrderRuleKey> commonObjectDataEngine;
                    commonObjectDataEngine = new CommonObjectDataEngine<WorkOrderRule, WorkOrderRuleKey>(LotDataEngine.SessionFactory);

                    WorkOrderRuleKey workOrderRuleKey = new WorkOrderRuleKey
                    {
                        OrderNumber = lot.OrderNumber,
                        MaterialCode = lot.MaterialCode
                    };

                    WorkOrderRule workOrderRule = commonObjectDataEngine.Get(workOrderRuleKey);

                    if (workOrderRule != null)
                    {
                        int.TryParse(workOrderRule.FullPackageQty.ToString(), out iQtyMax);
                    }
                    else
                    {
                        result.Code = 1002;
                        result.Message = "最大满托数未设置！";
                        return result;
                    }
                    #endregion

                    #region 取得自制组件批次IV测试数据
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
                            result.Message = string.Format("批次[{0}]所在工单[{1}]分档规则[{2}-{3}]不存在！",
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

                    #endregion

                    #region 取得读头所在车间
                    line = this.ProductionLineDataEngine.Get(p.PackageLine);
                    if (line == null)
                    {
                        result.Code = 2005;
                        result.Message = string.Format("产线[{0}]不存在！",
                                                        p.PackageLine);
                        return result;
                    }
                    //根据线别所在区域，获取车间名称。
                    location = this.LocationDataEngine.Get(line.LocationName);

                    if (location == null)
                    {
                        result.Code = 2005;
                        result.Message = string.Format("产线[{0}]对应区域[{1}]不存在！",
                                                        p.PackageLine,
                                                        line.LocationName);
                        return result;
                    }

                    locationName = location.ParentLocationName ?? string.Empty;
                    #endregion

                    #region 创建托包装对象
                    //判断托是否完成包装（最后一块入托）
                    if (iQtyMax > inPackageOrder)
                    {
                        IsFinishPackage = EnumPackageState.Packaging;
                    }
                    else
                    {
                        IsFinishPackage = EnumPackageState.Packaged;
                    }

                    //判断是否新托
                    if (inPackageOrder == 1)
                    {
                        //1.取得新托号
                        //strPackageNo = PackageNoGenerate.Generate(p.ScanLotNumber, false);

                        MethodReturnResult<string> resultPackageNo = new MethodReturnResult<string>();

                        resultPackageNo = PackageNoGenerate.CreatePackageNo(p.ScanLotNumber);

                        if (resultPackageNo.Code > 0)
                        {
                            result.Code = resultPackageNo.Code;
                            result.Message = resultPackageNo.Message;

                            return result;
                        }

                        strPackageNo = resultPackageNo.Data;

                        //2.取得批次电池片厂商
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

                        //3.创建托对象
                        packageObject = new Package()
                        {
                            Key = strPackageNo,                     //主键托号
                            ContainerNo = null,                     //容器号
                            OrderNumber = lot.OrderNumber,          //工单号
                            PackageState = IsFinishPackage,         //包装状态
                            MaterialCode = lot.MaterialCode,        //物料代码
                            IsLastPackage = false,                  //是否尾包
                            Quantity = inPackageOrder,              //包装数量
                            PackageType = EnumPackageType.Packet,   //包装类型 Packet=0 ---按包 Box=1 --- 按箱
                            Description = "",                       //描述
                            Checker = null,                         //检验人
                            CheckTime = null,                       //检验时间
                            ToWarehousePerson = null,               //入库人
                            ToWarehouseTime = null,                 //入库时间
                            ShipmentPerson = null,                  //出货人
                            ShipmentTime = null,                    //出货时间
                            Creator = p.EquipmentCode,              //创建人
                            CreateTime = now,                       //创建时间                        
                            Editor = p.EquipmentCode,               //编辑人             
                            EditTime = now,                         //编辑时间 
                            SupplierCode = strSupplierCodeForLot,   //电池片供应商编号
                            PackageMixedType = EnumPackageMixedType.UnMixedType,    //包装最大类型UnMixedType=0 ---按包 MixedType=1 --- 按箱
                            Color = lot.Color,                                      //花色
                            Grade = lot.Grade,                                      //等级
                            LineCode = p.PackageLine,                                  //线别
                            PowerSubCode = lstLotIVTestData[0].PowersetSubCode,     //子分挡代码
                            PowerName = lstWorkOrderPowerset[0].PowerName,          //分档名称
                            Location = locationName                                 //车间
                        };
                    }
                    else
                    {
                        //取得托号
                        strPackageNo = p.PackageNo;

                        //取得托信息
                        packageObject = this.PackageDataEngine.Get(strPackageNo);

                        //判断包装号是否存在。
                        if (packageObject == null)
                        {
                            result.Code = 1002;
                            result.Message = string.Format("包装号：（{0}）不存在！", strPackageNo);
                            return result;
                        }

                        //包装是否包装状态
                        if (packageObject.PackageState != EnumPackageState.Packaging)
                        {
                            result.Code = 0;
                            result.Message = string.Format("包装号：（{0}）已完成包装！", strPackageNo);
                            return result;
                        }

                        //设置托包装属性
                        packageObject.Quantity = inPackageOrder;        //包装数量
                        packageObject.PackageState = IsFinishPackage;   //包装状态
                        packageObject.Editor = p.EquipmentCode;         //编辑人（设备代码）
                        packageObject.EditTime = now;                   //编辑日期
                    }
                    #endregion

                    #region 创建托包装明细对象
                    packageDetail = new PackageDetail()
                    {
                        Key = new PackageDetailKey()
                        {
                            PackageNo = strPackageNo,               //托号
                            ObjectNumber = strLotNumber,            //批次代码
                            ObjectType = EnumPackageObjectType.Lot  //Lot=0 批次 Packet=1 小包
                        },
                        ItemNo = inPackageOrder,                    //入托项目号（入托顺序）
                        Creator = p.EquipmentCode,                  //创建人
                        CreateTime = now,                           //创建时间                   
                        MaterialCode = lot.MaterialCode,            //物料编码
                        OrderNumber = lot.OrderNumber               //工单代码
                    };
                    #endregion

                    #region 设置批次包装属性
                    lot.PackageNo = strPackageNo;                   //托号
                    #endregion

                    #region 创建Bin对象
                    //取得当前Bin信息
                    cfg = new PagingConfig()
                    {
                        PageNo = 0,
                        PageSize = 1,
                        Where = string.Format("Key.BinNo='{0}' and Key.PackageLine='{1}' ",
                                            p.BinNo,
                                            p.PackageLine),
                        OrderBy = " EditTime desc"
                    };

                    lstPackageBin = PackageBinDataEngine.Get(cfg);

                    //是否新入托
                    if (inPackageOrder == 1)
                    {
                        //原Bin对象
                        if (lstPackageBin != null && lstPackageBin.Count > 0)
                        {
                            packageBinObjHis = lstPackageBin.FirstOrDefault();

                            packageBinObjHis.BinState = 1;   //Bin状态 0 - 当前Bin 1 - 非当前Bin数据
                        }

                        //新增Bin对象
                        PackageBinKey packageBinKey = new PackageBinKey
                        {
                            PackageLine = p.PackageLine,       //线别
                            BinNo = p.BinNo,                //Bin代码
                            PackageNo = strPackageNo        //托包装号
                        };

                        packageBinObj = new PackageBin
                        {
                            Key = packageBinKey,            //Bin主键
                            BinQty = inPackageOrder,        //Bin数量
                            BinMaxQty = iQtyMax,            //Bin内托最大包装数量
                            BinPackaged = (EnumBinPackaged)IsFinishPackage,   //Bin包装状态与包装状态相同（UnFinished = 0---未完成，Finished = 1---完成）
                            BinState = 0,                   //Bin状态
                            Creator = p.EquipmentCode,      //创建人（设备代码）
                            CreateTime = now,               //创建日期         
                            Editor = p.EquipmentCode,       //编辑人（设备代码）
                            EditTime = now                  //编辑日期
                        };
                    }
                    else
                    {
                        if (lstPackageBin == null || lstPackageBin.Count == 0)
                        {
                            result.Code = 1000;
                            result.Message = string.Format("包装线（{1}）Bin({0}对应的数据异常，未找到历史数据！) ",
                                                            p.BinNo,
                                                            p.LineCode);

                            return result;
                        }

                        //取得当前Bin对象
                        packageBinObj = lstPackageBin.FirstOrDefault();

                        //设置属性
                        packageBinObj.BinPackaged = (EnumBinPackaged)IsFinishPackage;   //Bin包装状态与包装状态相同（UnFinished = 0---未完成，Finished = 1---完成）
                        packageBinObj.BinQty = inPackageOrder;                          //Bin数量
                        packageBinObj.Editor = p.EquipmentCode;                         //编辑人（设备代码）
                        packageBinObj.EditTime = now;                                   //编辑日期
                    }

                    #endregion

                    #region ***创建事物对象***
                    //生成操作事务主键。
                    transactionKey = Guid.NewGuid().ToString();

                    #region 批次事物对象
                    if (localName == "K01")
                    {
                        //批次事物对象LotTransaction（表WIP_TRANSACTION）
                        lotTransObj = new LotTransaction()
                        {
                            Key = transactionKey,                   //事物主键
                            Activity = EnumLotActivity.Package,     //批次操作类型（包装）                    
                            InQuantity = lot.Quantity,              //操作数量
                            LotNumber = lot.Key,                    //批次代码                    
                            OrderNumber = lot.OrderNumber,          //工单号
                            OutQuantity = lot.Quantity,             //操作后数量
                            LocationName = lot.LocationName,        //生产车间
                            LineCode = p.LineCode,                  //线别
                            RouteEnterpriseName = lot.RouteEnterpriseName,  //工艺流程组
                            RouteName = lot.RouteName,              //工艺流程
                            RouteStepName = lot.RouteStepName,      //工步
                            ShiftName = "",                         //班别
                            UndoFlag = false,                       //撤销标识
                            UndoTransactionKey = null,              //撤销记录主键
                            Description = "",                       //备注
                            Color = lot.Color,
                            Attr1 = lot.Attr1,
                            Attr2 = lot.Attr2,
                            Attr3 = lot.Attr3,
                            Attr4 = lot.Attr4,
                            Attr5 = lot.Attr5,
                            OperateComputer = p.ScanIP,             //操作客户端
                            Creator = p.ScanIP,                     //创建人
                            CreateTime = now,                       //创建时间                  
                            Editor = p.ScanIP,                      //编辑人
                            EditTime = now                          //编辑时间
                        };
                    }
                    if (localName == "G01")
                    {
                        //批次事物对象LotTransaction（表WIP_TRANSACTION）
                        lotTransObj = new LotTransaction()
                        {
                            Key = transactionKey,                   //事物主键
                            Activity = EnumLotActivity.Package,     //批次操作类型（包装）                    
                            InQuantity = lot.Quantity,              //操作数量
                            LotNumber = lot.Key,                    //批次代码                    
                            OrderNumber = lot.OrderNumber,          //工单号
                            OutQuantity = lot.Quantity,             //操作后数量
                            LocationName = lot.LocationName,        //生产车间
                            LineCode = p.PackageLine,               //线别
                            RouteEnterpriseName = lot.RouteEnterpriseName,  //工艺流程组
                            RouteName = lot.RouteName,              //工艺流程
                            RouteStepName = lot.RouteStepName,      //工步
                            ShiftName = "",                         //班别
                            UndoFlag = false,                       //撤销标识
                            UndoTransactionKey = null,              //撤销记录主键
                            Description = "",                       //备注
                            Color = lot.Color,
                            Attr1 = lot.Attr1,
                            Attr2 = lot.Attr2,
                            Attr3 = lot.Attr3,
                            Attr4 = lot.Attr4,
                            Attr5 = lot.Attr5,
                            OperateComputer = p.PackageLine,             //操作客户端
                            Creator = p.PackageLine,                     //创建人
                            CreateTime = now,                       //创建时间                  
                            Editor = p.PackageLine,                      //编辑人
                            EditTime = now                          //编辑时间
                        };
                    }


                    //新增批次事物历史记录TransactionHistory（表WIP_TRANSACTION_LOT）  
                    lotTransHistoryObj = new LotTransactionHistory(transactionKey, lot);

                    //新增工艺下一步记录。
                    //nextLotStep = new LotTransactionStep()
                    //{
                    //    Key = transactionKey,                               //事物主键
                    //    ToRouteEnterpriseName = lot.RouteEnterpriseName,    //工艺流程组
                    //    ToRouteName = lot.RouteName,                        //工艺流程
                    //    ToRouteStepName = lot.RouteStepName,                //工步
                    //    Editor = p.ScanIP,                                  //编辑人
                    //    EditTime = now                                      //编辑日期
                    //};
                    #endregion

                    #region 托包装事物对象
                    //记录包装操作事物对象                
                    transPackageObj = new LotTransactionPackage()
                    {
                        Key = transactionKey,                           //事物主键
                        PackageNo = packageObject.Key,                  //托包装号
                        Editor = p.EquipmentCode,                       //编辑人（设备代码）
                        EditTime = now                                  //编辑时间
                    };
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
                                                    strLotNumber,
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
                                LotNumber = strLotNumber,                       //加工批次
                                Quantity = 1,                                   //加工数量（默认1后期优化）
                                StartTime = now,                                //加工开始时间
                                EndTime = now,                                  //加工结束时间
                                Creator = p.EquipmentCode,                      //创建人（设备代码）
                                CreateTime = now,                               //创建时间
                                Editor = p.EquipmentCode,                       //编辑人（设备代码）
                                EditTime = now,                                 //编辑时间
                                State = EnumLotTransactionEquipmentState.End    //事物状态（1 - 结束）
                            };

                            isNewEquipmentTran = true;                          //设备事物对象状态-NEW
                        }
                    }
                    #endregion

                    #endregion

                    #region 开始事物处理
                    session = this.LotDataEngine.SessionFactory.OpenSession();
                    transaction = session.BeginTransaction();

                    #region 1.更新批次基本信息
                    lot.PackageNo = strPackageNo;                   //托包装号

                    this.LotDataEngine.Update(lot, session);

                    //更新批次事物LotTransaction（表WIP_TRANSACTION）信息
                    this.LotTransactionDataEngine.Insert(lotTransObj, session);

                    //更新批次历史事物TransactionHistory（表WIP_TRANSACTION_LOT）信息
                    this.LotTransactionHistoryDataEngine.Insert(lotTransHistoryObj, session);

                    #endregion

                    #region 2.包装数据
                    if (inPackageOrder == 1)        //新托号
                    {
                        this.PackageDataEngine.Insert(packageObject, session);
                    }
                    else
                    {
                        this.PackageDataEngine.Update(packageObject, session);
                    }

                    //2.1.包装明细数据
                    this.PackageDetailDataEngine.Insert(packageDetail, session);

                    //2.2.包装事物
                    this.LotTransactionPackageDataEngine.Insert(transPackageObj, session);
                    #endregion

                    #region 3.Bin数据
                    if (inPackageOrder == 1)        //新托号
                    {
                        //新Bin数据
                        this.PackageBinDataEngine.Insert(packageBinObj, session);

                        //Bin历史数据,当对象为NULL即不存在历史对象不做处理
                        if (packageBinObjHis != null)
                        {
                            this.PackageBinDataEngine.Update(packageBinObjHis, session);
                        }
                    }
                    else
                    {
                        this.PackageBinDataEngine.Update(packageBinObj, session);
                    }
                    #endregion

                    #region 4.更新设备信息 , 设备的Event ,设备的Transaction
                    //4.1
                    if (isNewEquipmentTran == true)
                    {
                        this.LotTransactionEquipmentDataEngine.Insert(lotTransactionEquipment, session);
                    }
                    else
                    {
                        this.LotTransactionEquipmentDataEngine.Update(lotTransactionEquipment, session);
                    }

                    #endregion

                    //transaction.Rollback();
                    //LogHelper.WriteLogError(string.Format("准备提交"));
                    transaction.Commit();
                    session.Close();
                    //LogHelper.WriteLogError(string.Format("提交完成"));
                    #endregion

                    #region 自动成柜
                    using (PackageInChestCommom packageInChestService = new PackageInChestCommom())
                    {
                        //获取带属性的托号
                        MethodReturnResult<Package> haveAttrPackage = new MethodReturnResult<Package>();
                        haveAttrPackage = packageInChestService.GetAttrOfPackage(packageObject);
                        //获取产品成柜规则
                        MaterialChestParameter mcp = null;
                        mcp = this.MaterialChestParameterDataEngine.Get(haveAttrPackage.Data.MaterialCode);
                        if (mcp != null && mcp.IsPackagedChest)
                        {
                            if (IsFinishPackage == EnumPackageState.Packaged)
                            {
                                //ISessionFactory SessionFactory = this.PackageDataEngine.SessionFactory;
                                //PackageInChestService packageInChestService = new PackageInChestService(SessionFactory);                        
                                //获取最佳柜号
                                MethodReturnResult<string> chestNo = packageInChestService.GetChestNo(packageObject.Key, "", false, false);
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
                                    Editor = p.EquipmentCode,
                                    ChestNo = chestNo.Data,
                                    IsLastestPackageInChest = false,
                                    ChestFullQty = chestFullQty,
                                    StoreLocation = "",
                                    PackageNo = haveAttrPackage.Data.Key,
                                    isManual = false,
                                    ModelType = 1
                                };
                                if (currentQty == chestFullQty)
                                {
                                    chestParameter.IsFinishPackageInChest = true;
                                }
                                //成柜
                                result = packageInChestService.Chest(chestParameter);
                            }
                        }
                    }
                    #endregion

                    return result;
                }
                catch (Exception ex)
                {
                    result.Code = 1000;
                    result.Message = ex.Message;
                    result.Detail = ex.ToString();

                    transaction.Rollback();
                    session.Close();

                    return result;
                }
                #endregion
            }
            return result;           
        }

        /// <summary>
        /// 立即将批次状态置为锁定状态
        /// </summary>
        /// <param name="lots">批次对象列表</param>
        /// <param name="bLock">锁定标识</param>
        /// <returns></returns>
        public MethodReturnResult SetPackageStateForLock(PackageCorner packageCorner, bool bLock)
        {
            MethodReturnResult result = new MethodReturnResult();
            ITransaction transactioneqp = null;
            ISession session = null;

            try
            {
                session = this.LotDataEngine.SessionFactory.OpenSession();
                transactioneqp = session.BeginTransaction();
                if (bLock)
                {
                    if (packageCorner.LockFlag == 1)
                    {
                        result.Code = 3000;
                        result.Message = string.Format("{0}托已经锁定，请稍后操作。"
                                                        , packageCorner.Key);

                        return result;
                    }
                    packageCorner.LockFlag = 1;           //锁定该托
                }
                else
                {
                    packageCorner.LockFlag = 0;           //取消锁定该托
                }

                //更新设备Transaction信息
                this.PackageCornerDataEngine.Update(packageCorner, session);

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

#region 入Bin顺序排序后的算法
//if (lstBinRules.Count > 0)
//                {
//                    //循环判断是否入Bin,循环次数为最新然后从最后编辑数据开始回溯
//                    int i, n = 0, icount = 0;
//                    bool isInPackage = false;

//                    for (i = 0; i < lstBinRules.Count; i++)
//                    {
//                        int j = n;      //取得当前的数组序号

//                        //取得包装Bin信息
//                        cfg = new PagingConfig()
//                        {
//                            PageNo = 0,
//                            PageSize = 1,
//                            Where = string.Format("Key.BinNo='{0}' and Key.PackageLine='{1}' and BinState=1",
//                                                    lstBinRules[j].Key.BinNo,
//                                                    lot.LineCode),
//                            OrderBy = " EditTime desc"
//                        };

//                        lstPackageBin = PackageBinDataEngine.Get(cfg);
                        
//                        if (lstPackageBin == null || lstPackageBin.Count == 0)      //非bin包装号
//                        {
//                            result.Code = 1009;
//                            result.Message = string.Format("生产线（{0}）中{1}Bin信息未找到，请与系统管理员联系！", lot.LineCode, lstBinRules[j].Key.BinNo);
//                            return result;
//                        }
                        
//                        //判断Bin是否包装完成
//                        if (lstPackageBin[0].BinPackaged == EnumBinPackaged.Finished)
//                        {
//                            //取得Bin信息
//                            strBinNo = lstPackageBin[0].Key.BinNo;              //Bin号
//                            intInBinOrder = 1;                                  //入Bin序列号

//                            //满足条件计数器加一
//                            icount++;
//                        }
//                        else
//                        {
//                            //判断是否满足入托规则 
//                            result = CheckLotInPackageRule(p.ScanLotNumber, lstPackageBin[0].Key.PackageNo, out isInPackage);
//                            if (result.Code > 0)        //产生错误
//                            {
//                                return result;
//                            }
//                            else
//                            {
//                                if(isInPackage)
//                                {
//                                    icount++;

//                                    //取得Bin信息
//                                    strBinNo = lstPackageBin[0].Key.BinNo;          //Bin号
//                                    intInBinOrder = int.Parse(result.ObjectNo);     //将入托序列号
//                                    strPackageNo = lstPackageBin[0].Key.PackageNo;  //将入托包装号
//                                }
//                            }
//                        }                               

//                        //计算下一个数组序号(第一次计算完成后（序号为0），从序列最后一位向前递减)
//                        n = lstBinRules.Count - i - 1;

//                        //第二次满足条件Bin即为可用Bin
//                        if (icount == 2)
//                        { 
//                            //结束循环
//                            i = lstBinRules.Count;
//                        }
//                    }

//                    if (icount == 0)    //未找到符合条件Bin
//                    {
//                        result.Code = 1010;
//                        result.Message = string.Format("批次：（{0}） 无对应Bin！", lotNumber);
//                        return result;
//                    }                    
//                }
//                else
//                {
//                    result.Code = 1010;
//                    result.Message = string.Format("批次：（{0}） 无对应Bin！", lotNumber);
//                    return result;
//                }
#endregion