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
using ServiceCenter.MES.DataAccess.Interface.LSM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.Model.PPM;
using System.Transactions;

namespace ServiceCenter.MES.Service.WIP.ServiceExtensions
{
    /// <summary>
    /// 扩展批次出站，进行参数物料检查和批次物料记录。
    /// </summary>
    class LotTrackOutForParameter : ILotTrackOut
    {
        /// <summary>
        /// 批次数据访问类。
        /// </summary>
        public ILotTransactionHistoryDataEngine LotTransactionHistoryDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 工步参数数据访问类。
        /// </summary>
        public IRouteStepParameterDataEngine RouteStepParameterDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 工单领料数据访问类。
        /// </summary>
        public IMaterialReceiptDetailDataEngine MaterialReceiptDetailDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 线上仓物料明细数据访问类。
        /// </summary>
        public ILineStoreMaterialDetailDataEngine LineStoreMaterialDetailDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 工单上料数据访问类。
        /// </summary>
        public IMaterialLoadingDetailDataEngine MaterialLoadingDetailDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 工单BOM数据访问类。
        /// </summary>
        public IWorkOrderBOMDataEngine WorkOrderBOMDataEngine
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
        /// 在批次出站时进行参数物料检查和批次物料记录。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult Execute(TrackOutParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            /*
            //result.Code = 1000;
            //result.Message += "100->";
            if (p.Paramters==null || p.Paramters.Count == 0)
            {
                return result;
            }

            foreach (string lotNumber in p.Paramters.Keys)
            {
                string transactionKey = p.TransactionKeys[lotNumber];
                LotTransactionHistory lot = this.LotTransactionHistoryDataEngine.Get(transactionKey);

                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.RouteName='{0}' 
                                            AND Key.RouteStepName='{1}'
                                            AND IsDeleted=0
                                            AND DCType='{2}'"
                                            , lot.RouteName
                                            , lot.RouteStepName
                                            , Convert.ToInt32(EnumDataCollectionAction.TrackOut)),
                    OrderBy = "ParamIndex"
                };
                IList<RouteStepParameter> lstRouteStepParameter = this.RouteStepParameterDataEngine.Get(cfg);
                if (lstRouteStepParameter.Count == 0)
                {
                    continue;
                }
                //检验物料批号。
                foreach (TransactionParameter tp in p.Paramters[lotNumber])
                {
                    RouteStepParameter item = lstRouteStepParameter
                                                    .FirstOrDefault(w => w.Key.ParameterName == tp.Name);

                    if (item==null || item.ValidateRule==EnumValidateRule.None)
                    {
                        continue;
                    }
                    
                    //匹配工单可用物料批号（根据领料记录）。
                    if (item.ValidateRule == EnumValidateRule.FullyWorkOrderMaterialLot)
                    {
                        #region //验证工单领料批号。
                        cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 10,
                            Where = string.Format(@"MaterialLot='{0}'
                                                    AND MaterialCode LIKE '{2}%'
                                                    AND EXISTS(SELECT Key
                                                               FROM MaterialReceipt as p
                                                               WHERE p.OrderNumber='{1}'
                                                               AND p.Key=self.Key.ReceiptNo)"
                                                    , tp.Value
                                                    , lot.OrderNumber
                                                    , item.MaterialType),
                            OrderBy = "CreateTime DESC"
                        };

                        IList<MaterialReceiptDetail> lstMaterialReceiptDetail = this.MaterialReceiptDetailDataEngine.Get(cfg);
                        if (lstMaterialReceiptDetail == null || lstMaterialReceiptDetail.Count == 0)
                        {
                            string message = item.ValidateFailedMessage ?? string.Empty;
                            if (string.IsNullOrEmpty(message))
                            {
                                message = "参数 （{0}） 对应物料类型（{3}）,其值 {1} 非工单（{2}）的领料批号。";
                            }
                            result.Code = 2004;
                            result.Message = string.Format(message
                                                            , item.Key.ParameterName
                                                            , tp.Value
                                                            , lot.OrderNumber
                                                            , item.MaterialType);
                            return result;
                        }
                        #endregion

                        string equipmentCode = lot.EquipmentCode;
                        if (string.IsNullOrEmpty(equipmentCode))
                        {
                            equipmentCode = p.EquipmentCode;
                        }

                        #region 更新线边仓物料记录
                        IDictionary<string, double> dicLineStoreMaterialDetail = new Dictionary<string, double>();
                        //遍历领料记录
                        foreach (MaterialReceiptDetail mrdItem in lstMaterialReceiptDetail)
                        {
                            LineStoreMaterialDetail lsmd = this.LineStoreMaterialDetailDataEngine.Get(new LineStoreMaterialDetailKey()
                            {
                                LineStoreName = mrdItem.LineStoreName,
                                OrderNumber=lot.OrderNumber,
                                MaterialCode=mrdItem.MaterialCode,
                                MaterialLot=mrdItem.MaterialLot
                            });

                            if (lsmd == null 
                                || lsmd.CurrentQty == 0)
                            {
                                continue;
                            }


                            //获取物料
                            Material m = this.MaterialDataEngine.Get(lsmd.Key.MaterialCode ?? string.Empty);
                            //获取供应商
                            Supplier s = this.SupplierDataEngine.Get(lsmd.SupplierCode ?? string.Empty);

                            //获取工单BOM
                            if (!dicLineStoreMaterialDetail.ContainsKey(mrdItem.MaterialCode))
                            {
                                cfg = new PagingConfig()
                                {
                                    PageNo = 0,
                                    PageSize = 1,
                                    Where = string.Format(@"Key.OrderNumber='{0}'
                                                          AND MaterialCode='{1}'"
                                                        , lot.OrderNumber
                                                        , mrdItem.MaterialCode)
                                };
                                IList<WorkOrderBOM> lstWorkOrderBOM = this.WorkOrderBOMDataEngine.Get(cfg);
                                if (lstWorkOrderBOM == null || lstWorkOrderBOM.Count == 0)
                                {
                                    continue;
                                }
                                dicLineStoreMaterialDetail.Add(mrdItem.MaterialCode, lstWorkOrderBOM[0].Qty * lot.Quantity);
                            }
                            //更新线边仓物料记录。
                            double qty = dicLineStoreMaterialDetail[mrdItem.MaterialCode];

                            LineStoreMaterialDetail lsmdUpdate = lsmd.Clone() as LineStoreMaterialDetail;
                            double leftQty = lsmdUpdate.CurrentQty - qty;
                            if (leftQty < 0)
                            {
                                dicLineStoreMaterialDetail[mrdItem.MaterialCode] = Math.Abs(leftQty);
                                lsmdUpdate.CurrentQty = 0;
                            }
                            else
                            {
                                dicLineStoreMaterialDetail[mrdItem.MaterialCode] = 0;//设置数量为0
                                lsmdUpdate.CurrentQty = leftQty;
                            }
                            this.LineStoreMaterialDetailDataEngine.Update(lsmdUpdate);

                            //新增批次用料记录。
                            int lotbomItemNo = 1;
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format("Key.LotNumber='{0}' AND Key.MaterialLot='{1}'"
                                                       , lot.LotNumber
                                                       , mrdItem.MaterialLot),
                                OrderBy = "Key.ItemNo Desc"
                            };

                            IList<LotBOM> lstLotBom = this.LotBOMDataEngine.Get(cfg);
                            if (lstLotBom.Count > 0)
                            {
                                lotbomItemNo = lstLotBom[0].Key.ItemNo + 1;
                            }
                            LotBOM lotbomObj = new LotBOM()
                            {
                                CreateTime = DateTime.Now,
                                Creator = p.Creator,
                                Editor = p.Creator,
                                EditTime = DateTime.Now,
                                EquipmentCode = equipmentCode,
                                LineCode = lot.LineCode,
                                LineStoreName = mrdItem.LineStoreName,
                                MaterialCode = mrdItem.MaterialCode,
                                MaterialName = m != null ? m.Name : string.Empty,
                                SupplierCode = lsmd.SupplierCode,
                                SupplierName = s != null ? s.Name : string.Empty,
                                RouteEnterpriseName = lot.RouteEnterpriseName,
                                RouteName = lot.RouteName,
                                RouteStepName = lot.RouteStepName,
                                TransactionKey = p.TransactionKeys[lotNumber],
                                MaterialFrom = EnumMaterialFrom.LineStore,
                                Qty = leftQty >= 0 ? qty : qty + leftQty,
                                Key = new LotBOMKey()
                                {
                                    LotNumber = lot.LotNumber,
                                    MaterialLot = mrdItem.MaterialLot,
                                    ItemNo = lotbomItemNo
                                }
                            };
                            this.LotBOMDataEngine.Insert(lotbomObj);
                            //如果数量满足，跳出。
                            if (dicLineStoreMaterialDetail[mrdItem.MaterialCode] == 0)
                            {
                                break;
                            }
                        }
                        //线边仓物料数量不足。
                        var lnq = from d in dicLineStoreMaterialDetail
                                  where d.Value > 0
                                  select d;
                        if (lnq.Count() > 0)
                        {
                            string message = item.ValidateFailedMessage ?? string.Empty;
                            if (string.IsNullOrEmpty(message))
                            {
                                message = "参数 （{0}） 值 {1} 对应物料不足。";
                            }
                            result.Code = 2004;
                            result.Message = string.Format(message
                                                            , item.Key.ParameterName
                                                            , tp.Value);
                            return result;
                        }
                        #endregion
                    }
                    //匹配设备上料批号（根据上料记录）。
                    else if (item.ValidateRule == EnumValidateRule.FullyLoadingMaterialLot)
                    {
                        string equipmentCode=lot.EquipmentCode;
                        if(string.IsNullOrEmpty(equipmentCode)){
                            equipmentCode=p.EquipmentCode;
                        }

                        #region //验证设备上料批号。
                        cfg = new PagingConfig()
                        {
                            IsPaging=false,
                            Where = string.Format(@"MaterialLot='{0}'
                                                    AND OrderNumber='{4}'
                                                    AND MaterialCode LIKE '{3}%'
                                                    AND CurrentQty>0
                                                    AND EXISTS(SELECT Key
                                                               FROM MaterialLoading as p
                                                               WHERE p.RouteOperationName='{1}'
                                                               AND p.EquipmentCode='{2}'
                                                               AND p.Key=self.Key.LoadingKey)"
                                                    , tp.Value
                                                    , lot.RouteStepName
                                                    , equipmentCode
                                                    , item.MaterialType
                                                    , lot.OrderNumber)
                        };
                        //获取上料记录。
                        IList<MaterialLoadingDetail> lstMaterialLoadingDetail = this.MaterialLoadingDetailDataEngine.Get(cfg);
                        if (lstMaterialLoadingDetail == null || lstMaterialLoadingDetail.Count == 0)
                        {
                            string message = item.ValidateFailedMessage ?? string.Empty;
                            if (string.IsNullOrEmpty(message))
                            {
                                message = "参数 （{0}） 值 {1} 非工单（{4}）工序（{2}）设备（{3}）上料批号。";
                            }
                            result.Code = 2004;
                            result.Message = string.Format(message
                                                            , item.Key.ParameterName
                                                            , tp.Value
                                                            , lot.RouteStepName
                                                            , equipmentCode
                                                            , lot.OrderNumber);
                            return result;
                        }
                        #endregion

                        #region 更新上料记录
                        IDictionary<string, double> dicMaterialLoadingDetail = new Dictionary<string, double>();
                        string loadingMaterialCode=lstMaterialLoadingDetail[0].MaterialCode;
                         //获取工单BOM
                        if (!dicMaterialLoadingDetail.ContainsKey(loadingMaterialCode))
                        {
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key.OrderNumber='{0}'
                                                        AND MaterialCode='{1}'"
                                                    , lot.OrderNumber
                                                    , loadingMaterialCode)
                            };
                            IList<WorkOrderBOM> lstWorkOrderBOM = this.WorkOrderBOMDataEngine.Get(cfg);
                            if (lstWorkOrderBOM == null || lstWorkOrderBOM.Count == 0)
                            {
                                continue;
                            }
                            dicMaterialLoadingDetail.Add(loadingMaterialCode, lstWorkOrderBOM[0].Qty*lot.Quantity);
                        }
                        //遍历上料记录
                        foreach (MaterialLoadingDetail mldItem in lstMaterialLoadingDetail)
                        {
                            //更新上料记录。
                            double qty = dicMaterialLoadingDetail[loadingMaterialCode];
                            MaterialLoadingDetail mldItemUpdate = mldItem.Clone() as MaterialLoadingDetail;
                            double leftQty=mldItemUpdate.CurrentQty-qty;
                            if (leftQty < 0)
                            {
                                dicMaterialLoadingDetail[loadingMaterialCode] = Math.Abs(leftQty);
                                mldItemUpdate.CurrentQty = 0;
                            }
                            else
                            {
                                dicMaterialLoadingDetail[loadingMaterialCode] = 0;//设置数量为0
                                mldItemUpdate.CurrentQty = leftQty;
                            }
                            this.MaterialLoadingDetailDataEngine.Update(mldItemUpdate);

                            LineStoreMaterialDetail lsmd = this.LineStoreMaterialDetailDataEngine.Get(new LineStoreMaterialDetailKey()
                            {
                                LineStoreName = mldItem.LineStoreName,
                                OrderNumber = lot.OrderNumber,
                                MaterialCode = mldItem.MaterialCode,
                                MaterialLot = mldItem.MaterialLot
                            });

                            Material m = null;
                            Supplier s = null;
                            if(lsmd!=null)
                            {
                                //获取物料
                                m = this.MaterialDataEngine.Get(lsmd.Key.MaterialCode ?? string.Empty);
                                //获取供应商
                                s = this.SupplierDataEngine.Get(lsmd.SupplierCode ?? string.Empty);
                            }

                            //新增批次用料记录。
                            int lotbomItemNo = 1;
                            cfg = new PagingConfig()
                            {
                                PageNo=0,
                                PageSize=1,
                                Where = string.Format("Key.LotNumber='{0}' AND Key.MaterialLot='{1}'"
                                                       ,lot.LotNumber
                                                       , mldItem.MaterialLot),
                                OrderBy="Key.ItemNo Desc"
                            };

                            IList<LotBOM> lstLotBom=this.LotBOMDataEngine.Get(cfg);
                            if (lstLotBom.Count > 0)
                            {
                                lotbomItemNo = lstLotBom[0].Key.ItemNo + 1;
                            }
                            LotBOM lotbomObj = new LotBOM()
                            {
                                CreateTime = DateTime.Now,
                                Creator = p.Creator,
                                Editor = p.Creator,
                                EditTime = DateTime.Now,
                                EquipmentCode = equipmentCode,
                                LineCode = lot.LineCode,
                                LineStoreName = mldItem.LineStoreName,
                                MaterialCode = mldItem.MaterialCode,
                                MaterialName = m != null ? m.Name : string.Empty,
                                SupplierCode = lsmd!=null? lsmd.SupplierCode:string.Empty,
                                SupplierName = s != null ? s.Name : string.Empty,
                                RouteEnterpriseName=lot.RouteEnterpriseName,
                                RouteName=lot.RouteName,
                                RouteStepName=lot.RouteStepName,
                                TransactionKey=p.TransactionKeys[lotNumber],
                                MaterialFrom = EnumMaterialFrom.Loading,
                                LoadingItemNo=mldItem.Key.ItemNo,
                                LoadingKey = mldItem.Key.LoadingKey,
                                Qty = leftQty >= 0 ? qty : qty+leftQty,
                                Key = new LotBOMKey()
                                {
                                    LotNumber=lot.LotNumber,
                                    MaterialLot=mldItem.MaterialLot,
                                    ItemNo = lotbomItemNo
                                }
                            };
                            this.LotBOMDataEngine.Insert(lotbomObj);
                            //如果数量满足，跳出。
                            if (dicMaterialLoadingDetail[loadingMaterialCode] == 0)
                            {
                                break;
                            }
                        }
                        //上料数量不足。
                        var lnq = from d in dicMaterialLoadingDetail
                                  where d.Value > 0
                                  select d;
                        if (lnq.Count() > 0)
                        {
                            string message = item.ValidateFailedMessage ?? string.Empty;
                            if (string.IsNullOrEmpty(message))
                            {
                                message = "参数 （{0}） 值 {1} 在工序（{2}）设备（{3}）上料不足。";
                            }
                            result.Code = 2004;
                            result.Message = string.Format(message
                                                            , item.Key.ParameterName
                                                            , tp.Value
                                                            , lot.RouteStepName
                                                            , equipmentCode);
                            return result;
                        }
                        #endregion
                    }
                    else
                    {
                        #region //验证工单BOM
                        cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format(@"Key.OrderNumber='{0}'"
                                                , lot.OrderNumber)
                        };

                        if (item.ValidateRule == EnumValidateRule.FullyWorkorderBOM)
                        {
                            cfg.Where += string.Format(" AND  MaterialCode='{0}'", tp.Value);
                        }
                        else if (item.ValidateRule == EnumValidateRule.PrefixWorkorderBOM)
                        {
                            cfg.Where += string.Format(" AND  MaterialCode LIKE '{0}%'", tp.Value);
                        }
                        else if (item.ValidateRule == EnumValidateRule.SuffixWorkorderBOM)
                        {
                            cfg.Where += string.Format(" AND  MaterialCode LIKE '%{0}'", tp.Value);
                        }
                        else
                        {
                            cfg.Where += string.Format(" AND  MaterialCode LIKE '%{0}%'", tp.Value);
                        }

                        IList<WorkOrderBOM> lstWorkOrderBOM = this.WorkOrderBOMDataEngine.Get(cfg);
                        if (lstWorkOrderBOM == null || lstWorkOrderBOM.Count == 0)
                        {
                            result.Code = 2005;
                            string message = item.ValidateFailedMessage ?? string.Empty;
                            if (string.IsNullOrEmpty(message))
                            {
                                message = "参数 （{0}） 值 {1} 在工单（{2}）BOM中不存在。";
                            }
                            result.Message = string.Format(message
                                                            , item.Key.ParameterName
                                                            , tp.Value
                                                            , lot.OrderNumber);
                            return result;
                        }
                        #endregion
                    }
                }
            }*/
            return result;
        }

    }
}
