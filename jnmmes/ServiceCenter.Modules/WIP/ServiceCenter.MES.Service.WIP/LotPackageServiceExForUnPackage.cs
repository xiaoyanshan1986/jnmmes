using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Service.WIP.Resources;
using ServiceCenter.Model;
using ServiceCenter.MES.DataAccess.Interface.EMS;
using ServiceCenter.MES.Model.EMS;
using NHibernate;
using ServiceCenter.Common;

namespace ServiceCenter.MES.Service.WIP
{
    /// <summary>
    /// 扩展批次包装，进行批次进站操作。
    /// </summary>
    public partial class LotPackageService
    {
        public IPackageBinDataEngine PackageBinDataEngine
        {
            get;
            set;
        }
        public IPackageCornerDetailDataEngine PackageCornerDetailDataEngine
        {
            get;
            set;
        }
        public IPackageCornerDataEngine PackageCornerDataEngine
        {
            get;
            set;
        }

        MethodReturnResult ExecuteUnPackage(PackageParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (p == null)
            {
                result.Code = 1001;
                result.Message = StringResource.ParameterIsNull;
                return result;
            }
            DateTime now = DateTime.Now;
            Package packageObj = null;
            p.TransactionKeys = new Dictionary<string, string>();
            
            packageObj = this.PackageDataEngine.Get(p.PackageNo);

            #region 1.托号合规性检查

            if (packageObj==null)
            {
                result.Code = 1001;
                result.Message = string.Format("包{0}不存在。", p.PackageNo);
                return result;
            }
            if(packageObj.InOrder == 1)
            {
                result.Code = 1003;
                result.Message = string.Format("包{0}已在入库单据中，不允许拆包。", p.PackageNo);
                return result;
            }

            //if (packageObj.PackageState == EnumPackageState.ToWarehouse || packageObj.PackageState == EnumPackageState.Shipped)
            //{
            //    result.Code = 1002;
            //    result.Message = string.Format("包{0}已入库，不允许拆包。", p.PackageNo);
            //    return result;
            //}

            if (packageObj.PackageState != EnumPackageState.Packaged && packageObj.PackageState != EnumPackageState.Packaging)
            {
                result.Code = 1002;
                result.Message = string.Format("托[{0}]当前状态[{1}]不允许出托操作！", p.PackageNo, packageObj.PackageState.GetDisplayName());

                return result;
            }
            if (packageObj.ContainerNo != null && packageObj.ContainerNo != "")
            {
                result.Code = 1002;
                result.Message = string.Format("托[{0}]在柜[{1}]中，不允许出托操作，请先执行出柜操作！", p.PackageNo, packageObj.ContainerNo);

                return result;
            }

            PagingConfig cfg = new PagingConfig()
            {
                IsPaging=false,
                Where = string.Format("Key.PackageNo='{0}'", p.PackageNo),
                OrderBy = "ItemNo ASC"
            };
            IList<PackageDetail> lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);
            if (lstPackageDetail == null || lstPackageDetail.Count==0)
            {
                result.Code = 1003;
                result.Message = string.Format("包{0}的明细为空。", p.PackageNo);
                return result;
            }

            #endregion

            #region 2.定义事务操作列表
            lstLotDataEngineForUpdate = new List<Lot>();
            lstLotTransactionForInsert = new List<LotTransaction>();
            lstLotTransactionHistoryForInsert = new List<LotTransactionHistory>();
            lstLotTransactionParameterDataEngineForInsert = new List<LotTransactionParameter>();
   
            //Package
            lstPackageDataForUpdate = new List<Package>();
            lstPackageDetailForDelete = new List<PackageDetail>();
            lstPackageDetailForInsert = new List<PackageDetail>();
            lstPackageDetailForUpdate = new List<PackageDetail>();
            IList<PackageCornerDetail> lstPackageCornerDetailForUpdateObj = new List<PackageCornerDetail>();
            lstLotTransactionPackageForInsert = new List<LotTransactionPackage>();

            lstOemDataEngineForUpdate = new List<OemData>();
            #endregion

            #region 3.更新包装数据(一)
            Package packageUpdate = null;          
            packageUpdate = packageObj.Clone() as Package;
            packageUpdate.Editor = p.Creator;
            packageUpdate.EditTime = now;   
            packageUpdate.PackageState = EnumPackageState.Packaging;
            #endregion

            #region 4.循环批次
            PackageDetail packageDetail = null;
            OemData oemData = null;
            foreach (string lotNumber in p.LotNumbers)
            {
                #region 4.1.校验LOT_NUMBER是否在包装明细项中
                var lstLotPackageDetail = (from item in lstPackageDetail
                                           where item.Key.ObjectNumber == lotNumber
                                           select item);
                if (lstLotPackageDetail == null || lstLotPackageDetail.Count() == 0)
                {
                    result.Code = 1003;
                    result.Message = string.Format("包号（{0}）明细项中不存在批次号（{1}）。", p.PackageNo,lotNumber);
                    return result;
                }
                else
                {
                    packageDetail = lstLotPackageDetail.FirstOrDefault();
                }
                #endregion
                
                oemData = this.OemDataEngine.Get(packageDetail.Key.ObjectNumber);
                if (oemData != null)
                {
                    #region OEM组件拆包过程

                    #region 4.2.获取OEM更新批次记录(一)
                    OemData oemDataUpdate = oemData.Clone() as OemData;
                    oemDataUpdate.Status = EnumOemStatus.Import;
                    oemDataUpdate.PackageNo = "";
                    oemDataUpdate.Editor = p.Creator;
                    oemDataUpdate.EditTime = now;
                    #endregion

                    #region 4.3.记录包装数据
                    packageUpdate.Quantity -= 1;
                    if (packageUpdate.Quantity < 0)
                    {
                        packageUpdate.Quantity = 0;
                    }
                    #endregion

                    #region 4.4.增加删除批次包装明细记录
                    this.lstPackageDetailForDelete.Add(packageDetail);
                    #endregion

                    #region 4.5.新增OEM更新批次记录(二)
                    lstOemDataEngineForUpdate.Add(oemDataUpdate);
                    #endregion

                    #endregion
                }                              
                else
                {
                    #region 自制组件拆包过程

                    #region 4.2.获取自制更新批次记录(一)
                    Lot lot = this.LotDataEngine.Get(lotNumber);
                    //生成操作事务主键。
                    string transactionKey = Guid.NewGuid().ToString();
                    p.TransactionKeys.Add(lotNumber, transactionKey);

                    Lot lotUpdate = lot.Clone() as Lot;
                    lotUpdate.PackageFlag = false;
                    lotUpdate.PackageNo = null;
                    lotUpdate.OperateComputer = p.OperateComputer;
                    lotUpdate.Editor = p.Creator;
                    lotUpdate.EditTime = now;
                    #endregion

                    #region 4.3.记录包装数据
                    packageUpdate.Quantity -= lot.Quantity;
                    if (packageUpdate.Quantity < 0)
                    {
                        packageUpdate.Quantity = 0;
                    }
                    #endregion

                    #region 4.4.增加删除批次包装明细记录
                    this.lstPackageDetailForDelete.Add(packageDetail);
                    #endregion

                    #region 4.5.判断是否要更新表头的SupplierCode字段
                    if (packageUpdate.SupplierCode != null)
                    {
                        cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format(@"Key.ObjectNumber <>'{0}' AND  Key.PackageNo='{1}' and EXISTS(FROM LotBOM as p WHERE p.Key.LotNumber=self.Key.ObjectNumber AND p.SupplierCode='{2}')"
                                                    , lotUpdate.Key
                                                    , packageUpdate.Key
                                                    , packageUpdate.SupplierCode)
                        };
                        IList<PackageDetail> lstPackageDetailCheck = this.PackageDetailDataEngine.Get(cfg);
                        if (lstPackageDetailCheck == null || lstPackageDetailCheck.Count == 0)
                        {
                            packageUpdate.SupplierCode = null;
                        }
                    }
                    #endregion

                    #region 4.6.判断是否需要更新Package-Bin中的Qty
                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"Key.PackageNo='{0}' and BinPackaged='0' "
                                                , lot.PackageNo)
                    };
                    IList<PackageBin> lstPackageBin = PackageBinDataEngine.Get(cfg);
                    if (lstPackageBin != null && lstPackageBin.Count > 0)
                    {
                        PackageBin packageBin = lstPackageBin.FirstOrDefault();
                        if (packageBin != null)
                        {
                            PackageBin packageBinForUpdate = packageBin.Clone() as PackageBin;
                            packageBinForUpdate.BinQty = packageBinForUpdate.BinQty - 1;
                            if (packageBinForUpdate.BinQty < 0)
                            {
                                packageBinForUpdate.BinQty = 0;
                            }
                            //packageBinForUpdate.BinPackaged = EnumBinPackaged.UnFinished;
                            lstPackageBinForUpdate.Add(packageBinForUpdate);
                        }
                    }
                    #endregion

                    #region 4.7.判断是否需要更新PackageCorner中的Qty

                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"Key.LotNumber='{0}'"
                                                , lotNumber),
                        OrderBy = "CreateTime desc"
                    };
                    IList<PackageCornerDetail> lstPackageCornerDetail = this.PackageCornerDetailDataEngine.Get(cfg);

                    if (lstPackageCornerDetail != null && lstPackageCornerDetail.Count > 0)
                    {
                        PackageCornerDetail packageCornerDetail = lstPackageCornerDetail.FirstOrDefault();
                        if (packageCornerDetail != null)
                        {
                            string packageKey = packageCornerDetail.Key.PackageKey;
                            cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format(@"Key.PackageKey='{0}'"
                                                        , packageKey),
                                OrderBy = "ItemNo ASC"
                            };
                            IList<PackageCornerDetail> lstPackageCornerDetailForUpdate = this.PackageCornerDetailDataEngine.Get(cfg);
                            PackageCorner packageCorner = this.PackageCornerDataEngine.Get(packageKey);
                            EnumCornerPackaged enumCornerPackaged = new EnumCornerPackaged();
                            if (packageCorner != null)
                            {
                                enumCornerPackaged = packageCorner.BinPackaged;
                                if (enumCornerPackaged == EnumCornerPackaged.UnFinished)
                                {
                                    PackageCorner packageCornerForUpdate = packageCorner.Clone() as PackageCorner;
                                    packageCornerForUpdate.BinQty = packageCornerForUpdate.BinQty - 1;
                                    if (packageCornerForUpdate.BinQty < 0)
                                    {
                                        packageCornerForUpdate.BinQty = 0;
                                    }
                                    //packageBinForUpdate.BinPackaged = EnumBinPackaged.UnFinished;

                                    lstPackageCornerForUpdate.Add(packageCornerForUpdate);
                                }
                            }

                            foreach (PackageCornerDetail item in lstPackageCornerDetailForUpdate)
                            {
                                if (item.Key.LotNumber == lotNumber)
                                {
                                    lstPackageCornerDetailForDelete.Add(item);
                                }
                            }
                            foreach (PackageCornerDetail item in lstPackageCornerDetailForDelete)
                            {

                                lstPackageCornerDetailForUpdate.Remove(item);
                            }

                            int itemNoCorner = 0;
                            foreach (PackageCornerDetail packageCornerDetailObj in lstPackageCornerDetailForUpdate)
                            {
                                itemNoCorner++;
                                if (packageCornerDetailObj.ItemNo == itemNoCorner)
                                {
                                    continue;
                                }
                                PackageCornerDetail packageCornerDetailObjUpdate = packageCornerDetailObj.Clone() as PackageCornerDetail;
                                packageCornerDetailObjUpdate.ItemNo = itemNoCorner;
                                lstPackageCornerDetailForUpdateObj.Add(packageCornerDetailObjUpdate);
                            }


                        }
                    }
                    #endregion

                    #region 4.8.记录(LotTransaction/LotTransactionHistory/LotTransactionPackage)操作历史
                    LotTransaction transObj = null;
                    LotTransactionHistory lotHistory = null;
                    transactionKey = Guid.NewGuid().ToString();
                    now = System.DateTime.Now;
                    transObj = new LotTransaction()
                    {
                        Key = transactionKey,
                        Activity = EnumLotActivity.UnPackage,
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
                        UndoTransactionKey = null,
                        LocationName = lot.LocationName,
                        LineCode = lot.LineCode
                    };
                    lstLotTransactionForInsert.Add(transObj);

                    //新增批次历史记录。
                    lotHistory = new LotTransactionHistory(transactionKey, lot);
                    lstLotTransactionHistoryForInsert.Add(lotHistory);

                    //包装事物对象
                    LotTransactionPackage transPackage = new LotTransactionPackage()
                    {
                        Key = transactionKey,
                        PackageNo = p.PackageNo,
                        Editor = p.Creator,
                        EditTime = now
                    };
                    lstLotTransactionPackageForInsert.Add(transPackage);
                    #endregion

                    #region 4.9.有附加参数记录附加参数数据。
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
                        }
                    }
                    #endregion

                    #region 4.10.新增更新批次记录(二)
                    lstLotDataEngineForUpdate.Add(lotUpdate);
                    #endregion

                    #endregion
                }            
            }
            #endregion

            #region 5.移除拆包的批次
            foreach (PackageDetail item in lstPackageDetailForDelete)
            {
                lstPackageDetail.Remove(item);
            }
            #endregion

            #region 6.重新定义Item次序
            int itemNo = 0;
            foreach (PackageDetail packageDetailObj in lstPackageDetail)
            {
                itemNo++;
                if (packageDetailObj.ItemNo == itemNo)
                {
                    continue;
                }
                PackageDetail packageDetailObjUpdate = packageDetailObj.Clone() as PackageDetail;
                packageDetailObjUpdate.ItemNo = itemNo;
                this.lstPackageDetailForUpdate.Add(packageDetailObjUpdate);
            }
            #endregion

            #region 7.新增更新包装数据(二)
            if (packageUpdate != null)
            {
                lstPackageDataForUpdate.Add(packageUpdate);
            }
            #endregion

            #region 8.开始事物处理
            ISession session = this.LotDataEngine.SessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();
            try
            {
                if (oemData == null)
                {
                    #region 8.1更新批次LOT 的信息
                    //更新批次基本信息
                    foreach (Lot lot in lstLotDataEngineForUpdate)
                    {
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
                        this.LotTransactionHistoryDataEngine.Insert(lotTransactionHistory, session);
                    }

                    //LotTransactionParameter
                    foreach (LotTransactionParameter lotTransactionParameter in lstLotTransactionParameterDataEngineForInsert)
                    {
                        this.LotTransactionParameterDataEngine.Insert(lotTransactionParameter, session);
                    }
                    #endregion

                    #region 8.2更新Package基本信息
                    foreach (Package package in lstPackageDataForUpdate)
                    {
                        this.PackageDataEngine.Update(package, session);
                    }

                    foreach (PackageDetail item in lstPackageDetailForDelete)
                    {
                        this.PackageDetailDataEngine.Delete(item.Key, session);
                    }

                    foreach (PackageCornerDetail packageCornerDetail in lstPackageCornerDetailForDelete)
                    {
                        this.PackageCornerDetailDataEngine.Delete(packageCornerDetail.Key, session);
                    }

                    foreach (PackageCornerDetail item in lstPackageCornerDetailForUpdateObj)
                    {
                        this.PackageCornerDetailDataEngine.Update(item, session);
                    }

                    foreach (PackageDetail item in lstPackageDetailForUpdate)
                    {
                        this.PackageDetailDataEngine.Update(item, session);
                    }

                    foreach (PackageBin item in lstPackageBinForUpdate)
                    {
                        this.PackageBinDataEngine.Update(item, session);
                    }

                    foreach (PackageCorner item in lstPackageCornerForUpdate)
                    {
                        this.PackageCornerDataEngine.Update(item, session);
                    }

                    foreach (LotTransactionPackage lotTransactionPackage in lstLotTransactionPackageForInsert)
                    {
                        this.LotTransactionPackageDataEngine.Insert(lotTransactionPackage, session);
                    }

                    #endregion
                }
                else
                {
                    #region 8.1更新OEM_DATA数据
                    foreach (OemData oemLot in lstOemDataEngineForUpdate)
                    {
                        this.OemDataEngine.Update(oemLot, session);
                    }
                    #endregion

                    #region 8.2更新Package基本信息
                    foreach (Package package in lstPackageDataForUpdate)
                    {
                        this.PackageDataEngine.Update(package, session);
                    }

                    foreach (PackageDetail item in lstPackageDetailForDelete)
                    {
                        this.PackageDetailDataEngine.Delete(item.Key, session);
                    }
                   
                    foreach (PackageDetail item in lstPackageDetailForUpdate)
                    {
                        this.PackageDetailDataEngine.Update(item, session);
                    }
                    #endregion
                }                                           

                transaction.Commit();
                session.Close();

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
            #endregion

            return result;
        }


        MethodReturnResult ExecuteUnPackageCheck(PackageParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (p == null)
            {
                result.Code = 1001;
                result.Message = StringResource.ParameterIsNull;
                return result;
            }
          
            return result;
        }
    }
}
