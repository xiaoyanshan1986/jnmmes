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

namespace ServiceCenter.MES.Service.WIP
{
    /// <summary>
    /// 实现批次包装服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LotPackageService : ILotPackageContract, ILotPackageCheck, ILotPackage, IPackageNoGenerate
    {
        /// <summary>
        /// 操作前检查事件。
        /// </summary>
        public event Func<PackageParameter, MethodReturnResult> CheckEvent;
        /// <summary>
        /// 执行操作时事件。
        /// </summary>
        public event Func<PackageParameter, MethodReturnResult> ExecutingEvent;
        /// <summary>
        /// 操作执行完成事件。
        /// </summary>
        public event Func<PackageParameter, MethodReturnResult> ExecutedEvent;

        /// <summary>
        /// 自定义操作前检查的清单列表。
        /// </summary>
        private IList<ILotPackageCheck> CheckList { get; set; }
        /// <summary>
        /// 自定义执行中操作的清单列表。
        /// </summary>
        private IList<ILotPackage> ExecutingList { get; set; }
        /// <summary>
        /// 自定义执行后操作的清单列表。
        /// </summary>
        private IList<ILotPackage> ExecutedList { get; set; }


        /// <summary>
        /// 注册自定义检查的操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterCheckInstance(ILotPackageCheck obj)
        {
            if (this.CheckList == null)
            {
                this.CheckList = new List<ILotPackageCheck>();
            }
            this.CheckList.Add(obj);
        }
        /// <summary>
        /// 注册执行中的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutingInstance(ILotPackage obj)
        {
            if (this.ExecutingList == null)
            {
                this.ExecutingList = new List<ILotPackage>();
            }
            this.ExecutingList.Add(obj);
        }

        /// <summary>
        /// 注册执行完成后的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutedInstance(ILotPackage obj)
        {
            if (this.ExecutedList == null)
            {
                this.ExecutedList = new List<ILotPackage>();
            }
            this.ExecutedList.Add(obj);
        }


        /// <summary>
        /// 操作前检查。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 操作执行中。
        /// </summary>
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
        /// <summary>
        /// 执行完成。
        /// </summary>
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

        /// <summary>
        /// 构造函数。
        /// </summary>
        public LotPackageService()
        {
            this.RegisterCheckInstance(this);
            this.RegisterExecutedInstance(this);
            this.PackageNoGenerate = this;

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
        /// 包装数据访问对象。
        /// </summary>
        public IPackageDataEngine PackageDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 包装明细数据访问对象。
        /// </summary>
        public IPackageDetailDataEngine PackageDetailDataEngine
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
        /// 批次包装操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>
        /// 代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult ILotPackageContract.Package(PackageParameter p)
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
                using (TransactionScope ts = new TransactionScope())
                {
                    result = this.OnExecuting(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                    result = this.OnExecuted(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                    ts.Complete();
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

        /// <summary>
        /// 操作前检查。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        MethodReturnResult ILotPackageCheck.Check(PackageParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            p.PackageNo = p.PackageNo.Trim().ToUpper();
            if (string.IsNullOrEmpty(p.LineCode))
            {
                result.Code = 1001;
                result.Message =string.Format("{0} {1}"
                                                ,"线别代码"
                                                ,StringResource.ParameterIsNull);
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

            if (p.IsFinishPackage==false 
                && (p.LotNumbers == null || p.LotNumbers.Count==0))
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
            RouteOperationAttribute roAttr=this.RouteOperationAttributeDataEngine.Get(new RouteOperationAttributeKey()
            {
                RouteOperationName = p.RouteOperationName,
                AttributeName = "IsPackageOperation"
            });

            //如果没有设置为包装工序，则直接返回。
            if (roAttr == null 
                || !bool.TryParse(roAttr.Value,out isPackageOperation)
                || isPackageOperation==false)
            {
                result.Code = 1009;
                result.Message = string.Format("{0} 非包装工序，请确认。"
                                                , p.RouteOperationName);
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
            }
            return result;
        }
        /// <summary>
        /// 执行操作。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        MethodReturnResult ILotPackage.Execute(PackageParameter p)
        {
            DateTime now = DateTime.Now;
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            p.PackageNo = p.PackageNo.Trim().ToUpper();
            p.TransactionKeys = new Dictionary<string, string>();
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
                this.PackageDataEngine.Update(packageUpdate);
            }
            //循环批次。
            foreach (string lotNo in p.LotNumbers)
            {
                string lotNumber = lotNo.Trim().ToUpper();
                Lot lot = this.LotDataEngine.Get(lotNumber);

                //生成操作事务主键。
                string transaciontKey = Guid.NewGuid().ToString();
                p.TransactionKeys.Add(lotNumber, transaciontKey);

                //更新批次记录。
                Lot lotUpdate = lot.Clone() as Lot;
                lotUpdate.PackageFlag = true;
                lotUpdate.PackageNo = p.PackageNo;
                lotUpdate.OperateComputer = p.OperateComputer;
                lotUpdate.Editor = p.Creator;
                lotUpdate.EditTime = now;
                this.LotDataEngine.Update(lotUpdate);

                #region//记录操作历史。
                LotTransaction transObj = new LotTransaction()
                {
                    Key = transaciontKey,
                    Activity = EnumLotActivity.Package,
                    CreateTime = now,
                    Creator = p.Creator,
                    Description = p.Remark,
                    Editor = p.Creator,
                    EditTime = now,
                    InQuantity = lot.Quantity,
                    LotNumber = lotNumber,
                    LocationName = lot.LocationName,
                    LineCode = p.LineCode,
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
                this.LotTransactionDataEngine.Insert(transObj);
                //新增批次历史记录。
                LotTransactionHistory lotHistory = new LotTransactionHistory(transaciontKey, lot);
                this.LotTransactionHistoryDataEngine.Insert(lotHistory);

                LotTransactionPackage transPackage = new LotTransactionPackage()
                {
                    Key = transaciontKey,
                    PackageNo = p.PackageNo,
                    Editor = p.Creator,
                    EditTime = now
                };
                this.LotTransactionPackageDataEngine.Insert(transPackage);
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
                                TransactionKey = transaciontKey,
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

                //记录包装数据。
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
                        PackageState = p.IsFinishPackage?EnumPackageState.Packaged:EnumPackageState.Packaging,
                        PackageType = EnumPackageType.Packet,
                        Quantity = lot.Quantity,
                        ShipmentPerson = null,
                        ShipmentTime = null,
                        ToWarehousePerson = null,
                        ToWarehouseTime = null
                    };
                    this.PackageDataEngine.Insert(packageObj);
                }
                else
                {
                    //更新包装数据。
                    Package packageUpdateNew = null;
                    if (packageUpdate != null)
                    {
                        packageUpdateNew=packageUpdate.Clone() as Package;
                    }
                    else
                    {
                        packageUpdateNew = packageObj.Clone() as Package;
                    }
                    if (packageUpdateNew.Quantity == 0)
                    {
                        packageUpdateNew.OrderNumber = lot.OrderNumber;
                        packageUpdateNew.MaterialCode = lot.MaterialCode;
                    }
                    packageUpdateNew.Quantity += lot.Quantity;
                    this.PackageDataEngine.Update(packageUpdateNew);
                }
                //记录包装明细数据。
                int itemNo = 1;
                PagingConfig cfg = new PagingConfig()
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
                this.PackageDetailDataEngine.Insert(packageDetail);

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


        MethodReturnResult<string> ILotPackageContract.Generate(string lotNumber, bool isLastestPackage)
        {
            MethodReturnResult<string> result = new MethodReturnResult<string>();
            result.Data = this.PackageNoGenerate.Generate(lotNumber, isLastestPackage);
            return result;
        }
    }
}
