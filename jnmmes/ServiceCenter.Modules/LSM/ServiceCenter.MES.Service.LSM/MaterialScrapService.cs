using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.LSM;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Service.Contract.LSM;
using ServiceCenter.MES.Service.LSM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ServiceCenter.MES.Service.LSM
{
    /// <summary>
    /// 实现物料报废管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class MaterialScrapService : IMaterialScrapContract
    {
        /// <summary>
        /// 物料报废数据访问读写。
        /// </summary>
        public IMaterialScrapDataEngine MaterialScrapDataEngine { get; set; }
        /// <summary>
        /// 物料报废明细数据访问类。
        /// </summary>
        public IMaterialScrapDetailDataEngine MaterialScrapDetailDataEngine { get; set; }
        /// <summary>
        /// 线上仓物料数据访问类。
        /// </summary>
        public ILineStoreMaterialDataEngine LineStoreMaterialDataEngine { get; set; }
        /// <summary>
        /// 线上仓物料明细数据访问类。
        /// </summary>
        public ILineStoreMaterialDetailDataEngine LineStoreMaterialDetailDataEngine { get; set; }
        /// <summary>
        /// 工单数据访问类。
        /// </summary>
        public IWorkOrderDataEngine WorkOrderDataEngine { get; set; }
        /// <summary>
        /// 添加物料报废。
        /// </summary>
        /// <param name="obj">物料报废数据。</param>
        /// <param name="lstDetail">退料明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(MaterialScrap obj, IList<MaterialScrapDetail> lstDetail)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                //判断物料报废是否存在。
                if (this.MaterialScrapDataEngine.IsExists(obj.Key))
                {
                    result.Code = 1001;
                    result.Message = String.Format(StringResource.MaterialScrapService_IsExists, obj.Key);
                    return result;
                }
                //判断工单号是否存在。
                if (!this.WorkOrderDataEngine.IsExists(obj.OrderNumber))
                {
                    result.Code = 1002;
                    result.Message = String.Format(StringResource.MaterialReceiptService_WorkOrderIsNotExists, obj.OrderNumber);
                    return result;
                }

                //using (TransactionScope ts = new TransactionScope())
                ISession session = this.WorkOrderDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();

                {
                    //新增物料报废。
                    this.MaterialScrapDataEngine.Insert(obj,session);
                    //新增物料报废明细。
                    int itemNo = 1;
                    foreach (MaterialScrapDetail item in lstDetail)
                    {
                        //新增物料报废明细。
                        item.Key = new MaterialScrapDetailKey()
                        {
                            ScrapNo = obj.Key,
                            ItemNo = itemNo
                        };
                        this.MaterialScrapDetailDataEngine.Insert(item,session);
                        //获取线边仓物料数据。
                        LineStoreMaterialKey lsmKey = new LineStoreMaterialKey()
                        {
                            LineStoreName = item.LineStoreName,
                            MaterialCode = item.MaterialCode
                        };
                        LineStoreMaterial lsm = this.LineStoreMaterialDataEngine.Get(lsmKey,session);
                        if (lsm == null)
                        {
                            result.Code = 2003;
                            result.Message = string.Format(StringResource.MaterialReturnService_MaterialIsNotExists
                                                         , lsmKey
                                                         , item.Key.ItemNo);
                            return result;
                        }
                        //如果对应线边仓中有物料数据，更新线边仓物料数据。
                        LineStoreMaterial lsmUpdate = lsm.Clone() as LineStoreMaterial;
                        lsmUpdate.EditTime = obj.EditTime;
                        lsmUpdate.Editor = obj.Editor;
                        this.LineStoreMaterialDataEngine.Update(lsmUpdate,session);
                        //新增线边仓明细数据。
                        LineStoreMaterialDetailKey lsmdKey = new LineStoreMaterialDetailKey()
                        {
                            LineStoreName = item.LineStoreName,
                            OrderNumber = item.OrderNumber,
                            MaterialCode = item.MaterialCode,
                            MaterialLot = item.MaterialLot
                        };
                        LineStoreMaterialDetail lsmd = this.LineStoreMaterialDetailDataEngine.Get(lsmdKey,session);
                        if (lsmd == null)
                        {
                            result.Code = 2002;
                            result.Message = string.Format(StringResource.MaterialReturnService_MaterialLotIsNotExists
                                                            , lsmdKey
                                                            , item.Key.ItemNo);
                            return result;
                        }
                        if (lsmd.CurrentQty < item.Qty)
                        {
                            //lsmdUpdate.CurrentQty = 0;
                            result.Code = 2001;
                            result.Message = string.Format(StringResource.MaterialScrapService_CurrentQtyLTReturnQty
                                                            , lsmd.Key
                                                            , lsmd.CurrentQty
                                                            , item.Qty
                                                            , item.Key.ItemNo);
                            return result;
                        }
                        //如果对应线边仓中有物料明细数据，则更新线边仓物料明细数据。
                        LineStoreMaterialDetail lsmdUpdate = lsmd.Clone() as LineStoreMaterialDetail;
                        lsmdUpdate.CurrentQty -= item.Qty;
                        lsmdUpdate.ScrapQty += item.Qty;
                        lsmdUpdate.Editor = obj.Editor;
                        lsmdUpdate.EditTime = obj.EditTime;
                        this.LineStoreMaterialDetailDataEngine.Update(lsmdUpdate,session);

                        itemNo++;
                    }
                    //ts.Complete();
                    transaction.Commit();
                    session.Close();
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        public MethodReturnResult Modify(MaterialScrap obj, IList<MaterialScrapDetail> lstDetail)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                //判断物料报废是否存在。
                if (!this.MaterialScrapDataEngine.IsExists(obj.Key))
                {
                    result.Code = 1001;
                    result.Message = String.Format(StringResource.MaterialScrapService_IsNotExists, obj.Key);
                    return result;
                }
                //判断工单号是否存在。
                if (!this.WorkOrderDataEngine.IsExists(obj.OrderNumber))
                {
                    result.Code = 1002;
                    result.Message = String.Format(StringResource.MaterialReceiptService_WorkOrderIsNotExists, obj.OrderNumber);
                    return result;
                }

                //using (TransactionScope ts = new TransactionScope())
                ISession session = this.WorkOrderDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();

                {
                    //新增物料报废。
                    this.MaterialScrapDataEngine.Update(obj, session);
                    //新增物料报废明细。
                    int itemNo = 1;
                    foreach (MaterialScrapDetail item in lstDetail)
                    {
                        //新增物料报废明细。
                        item.Key = new MaterialScrapDetailKey()
                        {
                            ScrapNo = obj.Key,
                            ItemNo = itemNo
                        };
                        this.MaterialScrapDetailDataEngine.Update(item, session);
                        //获取线边仓物料数据。
                        LineStoreMaterialKey lsmKey = new LineStoreMaterialKey()
                        {
                            LineStoreName = item.LineStoreName,
                            MaterialCode = item.MaterialCode
                        };
                        LineStoreMaterial lsm = this.LineStoreMaterialDataEngine.Get(lsmKey, session);
                        if (lsm == null)
                        {
                            result.Code = 2003;
                            result.Message = string.Format(StringResource.MaterialReturnService_MaterialIsNotExists
                                                         , lsmKey
                                                         , item.Key.ItemNo);
                            return result;
                        }
                        //如果对应线边仓中有物料数据，更新线边仓物料数据。
                        LineStoreMaterial lsmUpdate = lsm.Clone() as LineStoreMaterial;
                        lsmUpdate.EditTime = obj.EditTime;
                        lsmUpdate.Editor = obj.Editor;
                        this.LineStoreMaterialDataEngine.Update(lsmUpdate, session);
                        //新增线边仓明细数据。
                        LineStoreMaterialDetailKey lsmdKey = new LineStoreMaterialDetailKey()
                        {
                            LineStoreName = item.LineStoreName,
                            OrderNumber = item.OrderNumber,
                            MaterialCode = item.MaterialCode,
                            MaterialLot = item.MaterialLot
                        };
                        LineStoreMaterialDetail lsmd = this.LineStoreMaterialDetailDataEngine.Get(lsmdKey, session);
                        if (lsmd == null)
                        {
                            result.Code = 2002;
                            result.Message = string.Format(StringResource.MaterialReturnService_MaterialLotIsNotExists
                                                            , lsmdKey
                                                            , item.Key.ItemNo);
                            return result;
                        }
                        if (lsmd.CurrentQty < item.Qty)
                        {
                            //lsmdUpdate.CurrentQty = 0;
                            result.Code = 2001;
                            result.Message = string.Format(StringResource.MaterialScrapService_CurrentQtyLTReturnQty
                                                            , lsmd.Key
                                                            , lsmd.CurrentQty
                                                            , item.Qty
                                                            , item.Key.ItemNo);
                            return result;
                        }
                        //如果对应线边仓中有物料明细数据，则更新线边仓物料明细数据。
                        LineStoreMaterialDetail lsmdUpdate = lsmd.Clone() as LineStoreMaterialDetail;
                        lsmdUpdate.CurrentQty -= item.Qty;
                        lsmdUpdate.ScrapQty += item.Qty;
                        lsmdUpdate.Editor = obj.Editor;
                        lsmdUpdate.EditTime = obj.EditTime;
                        this.LineStoreMaterialDetailDataEngine.Update(lsmdUpdate, session);

                        itemNo++;
                    }
                    //ts.Complete();
                    transaction.Commit();
                    session.Close();
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        public MethodReturnResult<MaterialScrap> Delete(string ScrapNo)
        {
            ISession db = this.WorkOrderDataEngine.SessionFactory.OpenSession();
            ITransaction transaction = db.BeginTransaction();
            MethodReturnResult<MaterialScrap> result = new MethodReturnResult<MaterialScrap>()
            {
                Code = 0,
                Message = ""
            };

            //判断领料单是否存在。
            MaterialScrap materialScrap = this.MaterialScrapDataEngine.Get(ScrapNo);
            if (materialScrap == null)
            {
                result.Code = 1001;
                result.Message = String.Format(@"报废单{0}不存在.", ScrapNo);
                return result;
            }

            PagingConfig cfg = new PagingConfig()
            {
                Where = string.Format("Key.ScrapNo='{0}' ", ScrapNo),
                IsPaging = false
            };
            IList<MaterialScrapDetail> lstMaterialScrapDetail = this.MaterialScrapDetailDataEngine.Get(cfg);


            foreach(var item in lstMaterialScrapDetail)
            {
             LineStoreMaterialKey lsmKey = new LineStoreMaterialKey()
                        {
                            LineStoreName = item.LineStoreName,
                            MaterialCode = item.MaterialCode
                        };
             LineStoreMaterial lsm = this.LineStoreMaterialDataEngine.Get(lsmKey, db);
                        if (lsm == null)
                        {
                            result.Code = 2003;
                            result.Message = string.Format(StringResource.MaterialReturnService_MaterialIsNotExists
                                                         , lsmKey
                                                         , item.Key.ItemNo);
                            return result;
                        }
                        //如果对应线边仓中有物料数据，更新线边仓物料数据。
                        LineStoreMaterial lsmUpdate = lsm.Clone() as LineStoreMaterial;
                        lsmUpdate.EditTime = DateTime.Now;
                        this.LineStoreMaterialDataEngine.Update(lsmUpdate, db);
                        //新增线边仓明细数据。
                        LineStoreMaterialDetailKey lsmdKey = new LineStoreMaterialDetailKey()
                        {
                            LineStoreName = item.LineStoreName,
                            OrderNumber = item.OrderNumber,
                            MaterialCode = item.MaterialCode,
                            MaterialLot = item.MaterialLot
                        };
                        LineStoreMaterialDetail lsmd = this.LineStoreMaterialDetailDataEngine.Get(lsmdKey, db);
                        if (lsmd == null)
                        {
                            result.Code = 2002;
                            result.Message = string.Format(StringResource.MaterialReturnService_MaterialLotIsNotExists
                                                            , lsmdKey
                                                            , item.Key.ItemNo);
                            return result;
                        }
                        if (lsmd.CurrentQty < item.Qty)
                        {
                            result.Code = 2001;
                            result.Message = string.Format(StringResource.MaterialScrapService_CurrentQtyLTReturnQty
                                                            , lsmd.Key
                                                            , lsmd.CurrentQty
                                                            , item.Qty
                                                            , item.Key.ItemNo);
                            return result;
                        }
                        //如果对应线边仓中有物料明细数据，则更新线边仓物料明细数据。
                        LineStoreMaterialDetail lsmdUpdate = lsmd.Clone() as LineStoreMaterialDetail;
                        lsmdUpdate.CurrentQty += item.Qty;
                        lsmdUpdate.ScrapQty -= item.Qty;
                        lsmdUpdate.EditTime = DateTime.Now;
                        this.LineStoreMaterialDetailDataEngine.Update(lsmdUpdate, db);
                    }
            
            #region //开始事物处理
            
            try
            {
                foreach (MaterialScrapDetail materialScrapDetail in lstMaterialScrapDetail)
                {
                    this.MaterialScrapDetailDataEngine.Delete(materialScrapDetail.Key, db);
                }
                this.MaterialScrapDataEngine.Delete(ScrapNo, db);
                transaction.Commit();
                db.Close();
            }
            catch (Exception err)
            {
                transaction.Rollback();
                db.Close();
                result.Code = 1000;
                result.Message += err.Message;
                return result;
            }
            #endregion

            return result;
        }


        /// <summary>
        /// 获取物料报废数据。
        /// </summary>
        /// <param name="key">物料报废标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialScrap&gt;" />,物料报废数据.</returns>
        public MethodReturnResult<MaterialScrap> Get(string key)
        {
            MethodReturnResult<MaterialScrap> result = new MethodReturnResult<MaterialScrap>();
            if (!this.MaterialScrapDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialScrapService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.MaterialScrapDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取物料报废数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialScrap&gt;" />,物料报废数据集合。</returns>
        public MethodReturnResult<IList<MaterialScrap>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<MaterialScrap>> result = new MethodReturnResult<IList<MaterialScrap>>();
            try
            {
                result.Data = this.MaterialScrapDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取物料报废明细数据。
        /// </summary>
        /// <param name="key">物料报废明细标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialScrapDetail&gt;" />,物料报废明细数据.</returns>
        public MethodReturnResult<MaterialScrapDetail> GetDetail(MaterialScrapDetailKey key)
        {
            MethodReturnResult<MaterialScrapDetail> result = new MethodReturnResult<MaterialScrapDetail>();
            if (!this.MaterialScrapDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialScrapDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.MaterialScrapDetailDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取物料报废明细数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialScrapDetail&gt;" />,物料报废明细数据集合。</returns>
        public MethodReturnResult<IList<MaterialScrapDetail>> GetDetail(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<MaterialScrapDetail>> result = new MethodReturnResult<IList<MaterialScrapDetail>>();
            try
            {
                result.Data = this.MaterialScrapDetailDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
    }
}
