using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Service.ZPVC.Resources;
using ServiceCenter.Model;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.DataAccess.Interface.LSM;
using ServiceCenter.MES.Model.LSM;

namespace ServiceCenter.MES.Service.ZPVC.ServiceExtensions
{
    /// <summary>
    /// 扩展批次不良，将不良片存入线边仓等。
    /// </summary>
    class LotDefectForLineStore : ILotDefect
    {
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
        /// 批次物料数据访问类。
        /// </summary>
        public ILotBOMDataEngine LotBOMDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 线边仓数据访问类。
        /// </summary>
        public ILineStoreDataEngine LineStoreDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 线边仓物料数据访问类。
        /// </summary>
        public ILineStoreMaterialDataEngine LineStoreMaterialDataEngine
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
       
        public MethodReturnResult Execute(DefectParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();
            
            foreach (string lotNumber in p.LotNumbers)
            {
                //获取批次数据。
                Lot obj = this.LotDataEngine.Get(lotNumber);
                //获取不良数量。
                double qty = 0;
                if(p.ReasonCodes[lotNumber].Count>0)
                {
                    qty = p.ReasonCodes[lotNumber].Sum(m => m.Quantity);
                }

                //更新批次当前数量。更新批次数据。
                Lot objUpdate = obj.Clone() as Lot;
                objUpdate.Quantity -= qty;
                objUpdate.Editor = p.Creator;
                objUpdate.EditTime = DateTime.Now;
                if (objUpdate.Quantity <= 0)
                {
                    objUpdate.DeletedFlag = true;
                    objUpdate.Description = "不良结束";
                }
                this.LotDataEngine.Update(objUpdate);
                //更新事务记录。
                string transactionKey = p.TransactionKeys[lotNumber];
                LotTransaction transObj = this.LotTransactionDataEngine.Get(transactionKey);
                if (transObj != null)
                {
                    LotTransaction transObjUpdate = transObj.Clone() as LotTransaction;
                    transObjUpdate.OutQuantity -= qty;
                    this.LotTransactionDataEngine.Update(transObjUpdate);
                }
                //获取车间对应的不良线边仓。
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Status=1 AND LocationName='{0}' AND Type='{1}'"
                                          ,obj.LocationName
                                          ,Convert.ToInt32(EnumLineStoreType.Defect)),
                    OrderBy = "Key"
                };

                IList<LineStore> lstLineStore = this.LineStoreDataEngine.Get(cfg);
                if (lstLineStore.Count == 0)
                {
                    result.Code = 3001;
                    result.Message = string.Format("请在系统中维护{0}车间的不良线边仓。", obj.LocationName);
                    return result;
                }
                LineStore lsObj = lstLineStore[0];
                //获取批次创建时的用料记录。
                cfg.Where = string.Format("Key.LotNumber='{0}' AND MaterialFrom='{1}'"
                                           ,obj.Key
                                           ,Convert.ToInt32(EnumMaterialFrom.LineStore));
                cfg.OrderBy = "Key.ItemNo";
                IList<LotBOM> lstBOM = this.LotBOMDataEngine.Get(cfg);
                LotBOM lotBOMObj = lstBOM[0];
                //判断批次料号和线边仓名称是否存在数据
                LineStoreMaterialKey lsmKey=new LineStoreMaterialKey()
                {
                    LineStoreName=lsObj.Key,
                    MaterialCode = lotBOMObj.MaterialCode
                };
                LineStoreMaterial lsmObj = this.LineStoreMaterialDataEngine.Get(lsmKey);
                //如果存在数据更新。
                if (lsmObj != null)
                {
                    LineStoreMaterial lsmObjUpdate = lsmObj.Clone() as LineStoreMaterial;
                    lsmObjUpdate.Editor = p.Creator;
                    lsmObjUpdate.EditTime = DateTime.Now;
                    this.LineStoreMaterialDataEngine.Update(lsmObjUpdate);
                }
                //如果不存在，新增。
                else
                {
                    lsmObj = new LineStoreMaterial()
                    {
                        Key=lsmKey,
                        CreateTime=DateTime.Now,
                        EditTime=DateTime.Now,
                        Editor=p.Creator,
                        Creator=p.Creator
                    };
                    this.LineStoreMaterialDataEngine.Insert(lsmObj);
                }
                //判断线边仓物料明细数据是否存在。
                LineStoreMaterialDetailKey lsmdKey = new LineStoreMaterialDetailKey()
                {
                    LineStoreName = lsObj.Key,
                    MaterialCode = lotBOMObj.MaterialCode,
                    OrderNumber=obj.OrderNumber,
                    MaterialLot = lotBOMObj.Key.MaterialLot
                };
                LineStoreMaterialDetail lsmdObj = this.LineStoreMaterialDetailDataEngine.Get(lsmdKey);
                //如果存在数据更新数量。
                if (lsmdObj != null)
                {
                    LineStoreMaterialDetail lsmdObjUpdate = lsmdObj.Clone() as LineStoreMaterialDetail;
                    lsmdObjUpdate.ReceiveQty += qty;
                    lsmdObjUpdate.CurrentQty += qty;
                    lsmdObjUpdate.Editor = p.Creator;
                    lsmdObjUpdate.EditTime=DateTime.Now;
                    this.LineStoreMaterialDetailDataEngine.Update(lsmdObjUpdate);
                }
                //如果不存在，新增。
                else
                {
                    lsmdObj = new LineStoreMaterialDetail()
                    {
                        Key = lsmdKey,
                        CreateTime = DateTime.Now,
                        EditTime = DateTime.Now,
                        Editor = p.Creator,
                        CurrentQty = qty,
                        ReceiveQty = qty,
                        Creator = p.Creator
                    };
                    this.LineStoreMaterialDetailDataEngine.Insert(lsmdObj);
                }
            }

            return result;
        }
    }
}
