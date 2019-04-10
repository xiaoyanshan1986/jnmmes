using Microsoft.Practices.EnterpriseLibrary.Data;
using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.LSM;
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
    /// 实现下料管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class MaterialUnloadingService : IMaterialUnloadingContract
    {
        protected Database _query_db;       //组件报表数据查询数据库连接

        public ISessionFactory SessionFactory
        {
            get;
            set;
        }

        public MaterialUnloadingService(ISessionFactory sf)
        {
            this.SessionFactory = sf;
            
            _query_db = DatabaseFactory.CreateDatabase("QUERYDATA");
        }

        /// <summary>
        /// 下料数据访问读写。
        /// </summary>
        public IMaterialUnloadingDataEngine MaterialUnloadingDataEngine { get; set; }
        /// <summary>
        /// 下料明细数据访问类。
        /// </summary>
        public IMaterialUnloadingDetailDataEngine MaterialUnloadingDetailDataEngine { get; set; }

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
        /// 添加下料。
        /// </summary>
        /// <param name="obj">下料数据。</param>
        /// <param name="lstDetail">退料明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        //public MethodReturnResult Add(MaterialUnloading obj, IList<MaterialUnloadingDetail> lstDetail)
        //{
        //    MethodReturnResult result = new MethodReturnResult();
        //    try
        //    {
        //        //判断下料是否存在。
        //        if (this.MaterialUnloadingDataEngine.IsExists(obj.Key))
        //        {
        //            result.Code = 1001;
        //            result.Message = String.Format(StringResource.MaterialUnloadingService_IsExists, obj.Key);
        //            return result;
        //        }

        //        ISession session = this.MaterialUnloadingDataEngine.SessionFactory.OpenSession();
        //        ITransaction transaction = session.BeginTransaction();
        //        {
        //            //新增下料。
        //            if (string.IsNullOrEmpty(obj.Key))
        //            {
        //                obj.Key = Convert.ToString(Guid.NewGuid());
        //            }

        //            this.MaterialUnloadingDataEngine.Insert(obj,session);

        //            //新增下料明细。
        //            int itemNo = 1;
        //            foreach (MaterialUnloadingDetail item in lstDetail)
        //            {
        //                //新增下料明细。
        //                item.Key = new MaterialUnloadingDetailKey()
        //                {
        //                    UnloadingKey = obj.Key,
        //                    ItemNo = itemNo
        //                };

        //                this.MaterialUnloadingDetailDataEngine.Insert(item,session);

        //                //修改上料数据。
        //                MaterialLoadingDetailKey mldKey = new MaterialLoadingDetailKey()
        //                {
        //                    LoadingKey = item.LoadingKey,
        //                    ItemNo = item.LoadingItemNo
        //                };

        //                MaterialLoadingDetail mld = this.MaterialLoadingDetailDataEngine.Get(mldKey,session);

        //                //上料数据不存在返回错误信息
        //                if (mld == null)
        //                {
        //                    result.Code = 2002;
        //                    result.Message = string.Format(StringResource.MaterialUnloadingService_LoadingItemIsNotExists
        //                                                 , mldKey
        //                                                 , item.Key.ItemNo);
        //                    return result;
        //                }
                                                
        //                MaterialLoadingDetail mldUpdate = mld.Clone() as MaterialLoadingDetail;

        //                //下料数量大于当前剩余数量
        //                if (mldUpdate.CurrentQty < item.UnloadingQty)
        //                {
        //                    result.Code = 2001;
        //                    result.Message = string.Format(StringResource.MaterialUnloadingService_CurrentQtyLTUnloadingQty
        //                                                    , mldUpdate.Key
        //                                                    , mldUpdate.CurrentQty
        //                                                    , item.UnloadingQty
        //                                                    , item.Key.ItemNo);
        //                    return result;
        //                }

        //                mldUpdate.CurrentQty -= item.UnloadingQty;
        //                mldUpdate.UnloadingQty += item.UnloadingQty;
        //                mldUpdate.EditTime = obj.EditTime;
        //                mldUpdate.Editor = obj.Editor;
        //                this.MaterialLoadingDetailDataEngine.Update(mldUpdate,session);
                        
        //                //获取线边仓物料数据。
        //                LineStoreMaterialKey lsmKey = new LineStoreMaterialKey()
        //                {
        //                    LineStoreName = item.LineStoreName,
        //                    MaterialCode = item.MaterialCode
        //                };

        //                LineStoreMaterial lsm = this.LineStoreMaterialDataEngine.Get(lsmKey,session);

        //                //如果对应线边仓中无物料数据，则新增线边仓物料数据。
        //                if (lsm == null)
        //                {
        //                    lsm = new LineStoreMaterial()
        //                    {
        //                        Key = lsmKey,
        //                        CreateTime = obj.CreateTime,
        //                        Creator = obj.Creator,
        //                        Editor = obj.Editor,
        //                        EditTime = obj.EditTime
        //                    };
        //                    this.LineStoreMaterialDataEngine.Insert(lsm,session);
        //                }
        //                else
        //                {
        //                    //更新线边仓物料数据。
        //                    LineStoreMaterial lsmUpdate = lsm.Clone() as LineStoreMaterial;
        //                    lsmUpdate.EditTime = obj.EditTime;
        //                    lsmUpdate.Editor = obj.Editor;
        //                    this.LineStoreMaterialDataEngine.Update(lsmUpdate,session);
        //                }

        //                //新增线边仓明细数据。
        //                LineStoreMaterialDetailKey lsmdKey = new LineStoreMaterialDetailKey()
        //                {
        //                    LineStoreName = item.LineStoreName,
        //                    OrderNumber = item.OrderNumber,
        //                    MaterialCode = item.MaterialCode,
        //                    MaterialLot = item.MaterialLot
        //                };

        //                LineStoreMaterialDetail lsmd = this.LineStoreMaterialDetailDataEngine.Get(lsmdKey,session);

        //                //如果对应线边仓中无物料明细数据，则新增线边仓物料明细数据。
        //                if (lsmd == null)
        //                {
        //                    lsmd = new LineStoreMaterialDetail()
        //                    {
        //                        Key = lsmdKey,
        //                        CurrentQty = item.UnloadingQty,
        //                        CreateTime = obj.CreateTime,
        //                        Creator = obj.Creator,
        //                        Editor = obj.Editor,
        //                        EditTime = obj.EditTime
        //                    };
        //                    this.LineStoreMaterialDetailDataEngine.Insert(lsmd,session);
        //                }
        //                else
        //                {
        //                    LineStoreMaterialDetail lsmdUpdate = lsmd.Clone() as LineStoreMaterialDetail;
        //                    //更新线边仓物料明细数据。
        //                    lsmdUpdate.CurrentQty += item.UnloadingQty;
        //                    lsmdUpdate.UnloadingQty += item.UnloadingQty;
        //                    lsmdUpdate.Editor = obj.Editor;
        //                    lsmdUpdate.EditTime = obj.EditTime;
        //                    this.LineStoreMaterialDetailDataEngine.Update(lsmdUpdate,session);
        //                }
        //                itemNo++;
        //            }
        //            //ts.Complete();
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
        public MethodReturnResult Add(MaterialUnloading obj, IList<MaterialUnloadingDetail> lstDetail)
        {
            MethodReturnResult result = new MethodReturnResult();
            ISession session = null;
            ITransaction transaction = null;

            try
            {
                IList<MaterialLoadingDetail> lstMaterialLoadingDetailUpdate = new List<MaterialLoadingDetail>();
                IList<LineStoreMaterialDetail> lstLineStoreMaterialDetailUpdate = new List<LineStoreMaterialDetail>();

                //判断下料是否存在。
                if (this.MaterialUnloadingDataEngine.IsExists(obj.Key))
                {
                    result.Code = 1001;
                    result.Message = String.Format(StringResource.MaterialUnloadingService_IsExists, obj.Key);
                    return result;
                }

                //处理上料及线边仓数据
                foreach (MaterialUnloadingDetail item in lstDetail)
                {
                    #region 1.处理上料数据
                    //创建上料数据主键
                    MaterialLoadingDetailKey mldKey = new MaterialLoadingDetailKey()
                    {
                        LoadingKey = item.LoadingKey,
                        ItemNo = item.LoadingItemNo
                    };

                    //取得上料明细对象
                    MaterialLoadingDetail mld = this.MaterialLoadingDetailDataEngine.Get(mldKey);

                    //上料数据不存在返回错误信息
                    if (mld == null)
                    {
                        result.Code = 2002;
                        result.Message = string.Format(StringResource.MaterialUnloadingService_LoadingItemIsNotExists
                                                     , mldKey
                                                     , item.Key.ItemNo);
                        return result;
                    }

                    //判断批次、物料是否一致
                    if ((mld.MaterialLot == item.MaterialLot && 
                        mld.MaterialCode == item.MaterialCode &&
                        mld.OrderNumber == item.OrderNumber) == false)
                    {
                        result.Code = 2002;
                        result.Message = string.Format("下料工单[{4}]物料[{0}]的批次号[{1}]与上料明细中工单[{5}]物料[{2}]的批次号[{3}]不一致！"
                                                    , item.MaterialCode
                                                    , item.MaterialLot
                                                    , mld.MaterialCode
                                                    , mld.MaterialLot
                                                    , item.OrderNumber
                                                    , mld.OrderNumber);
                        return result;
                    }

                    //下料数量大于当前剩余数量
                    if (mld.CurrentQty < item.UnloadingQty)
                    {
                        result.Code = 2001;
                        result.Message = string.Format(StringResource.MaterialUnloadingService_CurrentQtyLTUnloadingQty
                                                        , mld.Key
                                                        , mld.CurrentQty
                                                        , item.UnloadingQty
                                                        , item.Key.ItemNo);
                        return result;
                    }
                    
                    //修改上料数据
                    mld.CurrentQty -= item.UnloadingQty;
                    mld.UnloadingQty += item.UnloadingQty;
                    mld.EditTime = obj.EditTime;
                    mld.Editor = obj.Editor;

                    lstMaterialLoadingDetailUpdate.Add(mld);

                    #endregion

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
                        //lsmd = new LineStoreMaterialDetail()
                        //{
                        //    Key = lsmdKey,
                        //    CurrentQty = item.UnloadingQty,
                        //    CreateTime = obj.CreateTime,
                        //    Creator = obj.Creator,
                        //    Editor = obj.Editor,
                        //    EditTime = obj.EditTime
                        //};
                        //this.LineStoreMaterialDetailDataEngine.Insert(lsmd, session);
                        result.Code = 2001;

                        result.Message = string.Format("线边仓无下料工单[{0}]物料[{1}]的批次号[{2}]！"
                                                    , item.OrderNumber
                                                    , item.MaterialCode
                                                    , item.MaterialLot);
                        return result;
                    }
                    else
                    {                        
                        //更新线边仓物料明细数据。
                        lsmd.CurrentQty += item.UnloadingQty;
                        lsmd.UnloadingQty += item.UnloadingQty;
                        lsmd.Editor = obj.Editor;
                        lsmd.EditTime = obj.EditTime;

                        lstLineStoreMaterialDetailUpdate.Add(lsmd);
                    }

                    #endregion
                }

                #region 提交事务处理
                session = this.MaterialUnloadingDataEngine.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                //1.下料表头
                this.MaterialUnloadingDataEngine.Insert(obj, session);

                //2.下料明细
                foreach (MaterialUnloadingDetail item in lstDetail)
                {
                    this.MaterialUnloadingDetailDataEngine.Insert(item, session);
                }

                //3.上料明细
                foreach (MaterialLoadingDetail item in lstMaterialLoadingDetailUpdate)
                {
                    this.MaterialLoadingDetailDataEngine.Update(item, session);
                }

                //4.线边仓明细
                foreach (LineStoreMaterialDetail item in lstLineStoreMaterialDetailUpdate)
                {
                    this.LineStoreMaterialDetailDataEngine.Update(item, session);
                }

                transaction.Commit();
                session.Close();
                #endregion

            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);

                if (transaction != null)
                {
                    transaction.Rollback();
                    session.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// 获取下料数据。
        /// </summary>
        /// <param name="key">下料标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialUnloading&gt;" />,下料数据.</returns>
        public MethodReturnResult<MaterialUnloading> Get(string key)
        {
            MethodReturnResult<MaterialUnloading> result = new MethodReturnResult<MaterialUnloading>();
            if (!this.MaterialUnloadingDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialUnloadingService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.MaterialUnloadingDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取下料数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialUnloading&gt;" />,下料数据集合。</returns>
        public MethodReturnResult<IList<MaterialUnloading>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<MaterialUnloading>> result = new MethodReturnResult<IList<MaterialUnloading>>();
            try
            {
                result.Data = this.MaterialUnloadingDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        
        /// <summary>
        /// 获取下料明细数据。
        /// </summary>
        /// <param name="key">下料明细标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialUnloadingDetail&gt;" />,下料明细数据.</returns>
        public MethodReturnResult<MaterialUnloadingDetail> GetDetail(MaterialUnloadingDetailKey key)
        {
            MethodReturnResult<MaterialUnloadingDetail> result = new MethodReturnResult<MaterialUnloadingDetail>();
            if (!this.MaterialUnloadingDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialUnloadingDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.MaterialUnloadingDetailDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取下料明细数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialUnloadingDetail&gt;" />,下料明细数据集合。</returns>
        public MethodReturnResult<IList<MaterialUnloadingDetail>> GetDetail(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<MaterialUnloadingDetail>> result = new MethodReturnResult<IList<MaterialUnloadingDetail>>();

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
                    result.Data = this.MaterialUnloadingDetailDataEngine.Get(cfg);
                //}
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
