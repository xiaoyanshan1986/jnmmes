using Microsoft.Practices.EnterpriseLibrary.Data;
using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.LSM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Service.Contract.LSM;
using ServiceCenter.MES.Service.LSM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ServiceCenter.MES.Service.LSM
{
    /// <summary>
    /// 实现上料管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class MaterialLoadingService : IMaterialLoadingContract
    {
        protected Database _query_db;       //组件报表数据查询数据库连接

        public ISessionFactory SessionFactory
        {
            get;
            set;
        }

        public MaterialLoadingService(ISessionFactory sf)
        {
            this.SessionFactory = sf;
            
            _query_db = DatabaseFactory.CreateDatabase("QUERYDATA");
        }

        /// <summary>
        /// 上料数据访问读写。
        /// </summary>
        public IMaterialLoadingDataEngine MaterialLoadingDataEngine { get; set; }
        /// <summary>
        /// 上料明细数据访问类。
        /// </summary>
        public IMaterialLoadingDetailDataEngine MaterialLoadingDetailDataEngine { get; set; }
        /// <summary>
        /// 线上仓物料数据访问类。
        /// </summary>
        public ILineStoreMaterialDataEngine LineStoreMaterialDataEngine { get; set; }
        /// <summary>
        /// 线上仓物料明细数据访问类。
        /// </summary>
        public ILineStoreMaterialDetailDataEngine LineStoreMaterialDetailDataEngine { get; set; }

        /// <summary>
        /// 设备数据访问读写。
        /// </summary>
        public IEquipmentDataEngine EquipmentDataEngine { get; set; }

        /// <summary>
        /// 添加上料。
        /// </summary>
        /// <param name="obj">上料数据。</param>
        /// <param name="lstDetail">退料明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        //public MethodReturnResult Add(MaterialLoading obj, IList<MaterialLoadingDetail> lstDetail)
        //{
        //    MethodReturnResult result = new MethodReturnResult();
        //    try
        //    {
        //        //判断上料单是否存在。
        //        if (this.MaterialLoadingDataEngine.IsExists(obj.Key))
        //        {
        //            result.Code = 1001;
        //            result.Message = String.Format(StringResource.MaterialLoadingService_IsExists, obj.Key);
        //            return result;
        //        }

        //        //using (TransactionScope ts = new TransactionScope())
        //        ISession session = this.MaterialLoadingDataEngine.SessionFactory.OpenSession();
        //        ITransaction transaction = session.BeginTransaction();
        //        {
        //            if (string.IsNullOrEmpty(obj.Key))
        //            {
        //                obj.Key = Convert.ToString(Guid.NewGuid());
        //            }

        //            //新增上料。
        //            this.MaterialLoadingDataEngine.Insert(obj, session);

        //            //新增上料明细。
        //            int itemNo = 1;
        //            foreach (MaterialLoadingDetail item in lstDetail)
        //            {
        //                //新增上料明细。
        //                item.Key = new MaterialLoadingDetailKey()
        //                {
        //                    LoadingKey = obj.Key,
        //                    ItemNo = itemNo
        //                };
        //                this.MaterialLoadingDetailDataEngine.Insert(item, session);

        //                //获取线边仓物料数据。
        //                //LineStoreMaterialKey lsmKey = new LineStoreMaterialKey()
        //                //{
        //                //    LineStoreName = item.LineStoreName,
        //                //    MaterialCode = item.MaterialCode
        //                //};
        //                //LineStoreMaterial lsm = this.LineStoreMaterialDataEngine.Get(lsmKey, session);
        //                //if (lsm == null)
        //                //{
        //                //    result.Code = 2003;
        //                //    result.Message = string.Format(StringResource.MaterialLoadingService_MaterialIsNotExists
        //                //                                   , lsmKey
        //                //                                   , item.Key.ItemNo);
        //                //    return result;
        //                //}

        //                ////如果对应线边仓中有物料数据，更新线边仓物料数据。
        //                //LineStoreMaterial lsmUpadate = lsm.Clone() as LineStoreMaterial;
        //                //lsmUpadate.EditTime = obj.EditTime;
        //                //lsmUpadate.Editor = obj.Editor;
        //                //this.LineStoreMaterialDataEngine.Update(lsmUpadate, session);

        //                //新增线边仓明细数据。
        //                LineStoreMaterialDetailKey lsmdKey = new LineStoreMaterialDetailKey()
        //                {
        //                    LineStoreName = item.LineStoreName,
        //                    OrderNumber = item.OrderNumber,
        //                    MaterialCode = item.MaterialCode,
        //                    MaterialLot = item.MaterialLot
        //                };
        //                LineStoreMaterialDetail lsmd = this.LineStoreMaterialDetailDataEngine.Get(lsmdKey, session);
        //                if (lsmd == null)
        //                {
        //                    result.Code = 2002;
        //                    result.Message = string.Format(StringResource.MaterialLoadingService_MaterialLotIsNotExists
        //                                                   , lsmdKey
        //                                                   , item.Key.ItemNo);
        //                    return result;
        //                }
        //                if (lsmd.CurrentQty < item.LoadingQty)
        //                {
        //                    result.Code = 2001;
        //                    result.Message = string.Format(StringResource.MaterialLoadingService_CurrentQtyLTLoadingQty
        //                                                    , lsmd.Key
        //                                                    , lsmd.CurrentQty
        //                                                    , item.LoadingQty
        //                                                    , item.Key.ItemNo);
        //                    return result;
        //                }
        //                //如果对应线边仓中有物料明细数据，则更新线边仓物料明细数据。

        //                LineStoreMaterialDetail lsmdUpdate = lsmd.Clone() as LineStoreMaterialDetail;
        //                lsmdUpdate.LoadingQty += item.LoadingQty;
        //                lsmdUpdate.CurrentQty -= item.LoadingQty;
        //                lsmdUpdate.Editor = obj.Editor;
        //                lsmdUpdate.EditTime = obj.EditTime;
        //                this.LineStoreMaterialDetailDataEngine.Update(lsmdUpdate, session);
        //                itemNo++;
        //            }

        //            //ts.Complete();
        //            //transaction.Rollback();
        //            transaction.Commit();
        //            session.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Code = 1000;
        //        result.Message = String.Format(StringResource.OtherError, ex.Message);
        //    }
        //    return result;
        //}

        public MethodReturnResult Add(MaterialLoading obj, IList<MaterialLoadingDetail> lstDetail)
        {
            MethodReturnResult result = new MethodReturnResult();
            ISession session = null;
            ITransaction transaction = null;

            try
            {
                IList<MaterialLoadingDetail> lstMaterialLoadingDetailUpdate = new List<MaterialLoadingDetail>();
                IList<LineStoreMaterialDetail> lstLineStoreMaterialDetailUpdate = new List<LineStoreMaterialDetail>();

                #region 判断合理性
                //1. 判断上料单是否存在。
                if (this.MaterialLoadingDataEngine.IsExists(obj.Key))
                {
                    result.Code = 1001;
                    result.Message = String.Format(StringResource.MaterialLoadingService_IsExists, obj.Key);
                    return result;
                }

                //2.判断线别代码与设备代码是否一致
                Equipment ep = this.EquipmentDataEngine.Get(obj.EquipmentCode);

                if (ep.LineCode != obj.ProductionLineCode)
                {
                    result.Code = 2001;
                    result.Message = string.Format("线别代码{0}与设备代码{1}不一致！",obj.ProductionLineCode,obj.EquipmentCode);
                    return result;
                }
                #endregion

                else
                {
                    //处理上料及线边仓数据
                    int itemNo = 1;

                    foreach (MaterialLoadingDetail item in lstDetail)
                    {
                        //1. 创建上料明细主键。
                        item.Key = new MaterialLoadingDetailKey()
                        {
                            LoadingKey = obj.Key,
                            ItemNo = itemNo
                        };

                        itemNo++;

                        #region 2.处理线边仓数据
                        //获取线边仓物料数据
                        LineStoreMaterialDetailKey lsmdKey = new LineStoreMaterialDetailKey()
                        {
                            LineStoreName = item.LineStoreName,
                            OrderNumber = item.OrderNumber,
                            MaterialCode = item.MaterialCode,
                            MaterialLot = item.MaterialLot
                        };

                        LineStoreMaterialDetail lsmd = this.LineStoreMaterialDetailDataEngine.Get(lsmdKey);

                        //如果对应线边仓中无物料数据，则返回错误信息
                        if (lsmd == null)
                        {
                            result.Code = 2001;

                            result.Message = string.Format("线边仓无上料工单[{0}]物料[{1}]的批次号[{2}]！"
                                                        , item.OrderNumber
                                                        , item.MaterialCode
                                                        , item.MaterialLot);
                            return result;
                        }
                        else if (lsmd.CurrentQty < item.LoadingQty)
                        {
                            result.Code = 2001;

                            result.Message = string.Format("线边仓上料工单[{0}]物料[{1}]的批次号[{2}]数量[{3}]不足所要上数量[{4}]！"
                                                        , item.OrderNumber
                                                        , item.MaterialCode
                                                        , item.MaterialLot
                                                        , lsmd.CurrentQty
                                                        , item.LoadingQty);
                            return result;
                        }
                        else
                        {
                            //更新线边仓物料明细数据。
                            lsmd.LoadingQty += item.LoadingQty;
                            lsmd.CurrentQty -= item.LoadingQty;
                            lsmd.Editor = obj.Editor;
                            lsmd.EditTime = obj.EditTime;

                            lstLineStoreMaterialDetailUpdate.Add(lsmd);
                        }

                        #endregion
                    }

                    #region 提交事务处理
                    session = this.MaterialLoadingDataEngine.SessionFactory.OpenSession();
                    transaction = session.BeginTransaction();

                    //1.上料表头
                    this.MaterialLoadingDataEngine.Insert(obj, session);

                    //2.上料明细
                    foreach (MaterialLoadingDetail item in lstDetail)
                    {
                        this.MaterialLoadingDetailDataEngine.Insert(item, session);
                    }

                    //3.线边仓明细
                    foreach (LineStoreMaterialDetail item in lstLineStoreMaterialDetailUpdate)
                    {
                        this.LineStoreMaterialDetailDataEngine.Update(item, session);
                    }

                    transaction.Commit();
                    session.Close();
                    #endregion
                }
                
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取上料数据。
        /// </summary>
        /// <param name="key">上料标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialLoading&gt;" />,上料数据.</returns>
        public MethodReturnResult<MaterialLoading> Get(string key)
        {
            MethodReturnResult<MaterialLoading> result = new MethodReturnResult<MaterialLoading>();
            if (!this.MaterialLoadingDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialLoadingService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.MaterialLoadingDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取上料数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialLoading&gt;" />,上料数据集合。</returns>
        public MethodReturnResult<IList<MaterialLoading>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<MaterialLoading>> result = new MethodReturnResult<IList<MaterialLoading>>();
            try
            {
                result.Data = this.MaterialLoadingDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        
        /// <summary>
        /// 获取上料明细数据。
        /// </summary>
        /// <param name="key">上料明细标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialLoadingDetail&gt;" />,上料明细数据.</returns>
        public MethodReturnResult<MaterialLoadingDetail> GetDetail(MaterialLoadingDetailKey key)
        {
            MethodReturnResult<MaterialLoadingDetail> result = new MethodReturnResult<MaterialLoadingDetail>();
            if (!this.MaterialLoadingDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialLoadingDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.MaterialLoadingDetailDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取上料明细数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialLoadingDetail&gt;" />,上料明细数据集合。</returns>
        public MethodReturnResult<IList<MaterialLoadingDetail>> GetDetail(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<MaterialLoadingDetail>> result = new MethodReturnResult<IList<MaterialLoadingDetail>>();

            try
            {
                ////查询使用报表数据库进行
                //using (DbConnection con = this._query_db.CreateConnection())
                //{
                //    //打开报表数据库连接
                //    con.Open();

                //    //创建链接事物期间
                //    ISession session = this.SessionFactory.OpenSession(con);

                    //取数
                    result.Data = this.MaterialLoadingDetailDataEngine.Get(cfg);
                //}
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }

            return result;

            //MethodReturnResult<IList<MaterialLoadingDetail>> result = new MethodReturnResult<IList<MaterialLoadingDetail>>();
            //try
            //{
            //    result.Data = this.MaterialLoadingDetailDataEngine.Get(cfg);
            //}
            //catch (Exception ex)
            //{
            //    result.Code = 1000;
            //    result.Message = String.Format(StringResource.OtherError, ex.Message);
            //}
            //return result;
        }
    }
}
