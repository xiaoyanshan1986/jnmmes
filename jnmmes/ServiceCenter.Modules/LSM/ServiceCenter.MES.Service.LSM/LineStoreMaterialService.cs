using Microsoft.Practices.EnterpriseLibrary.Data;
using NHibernate;
using ServiceCenter.Common;
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

namespace ServiceCenter.MES.Service.LSM
{
    /// <summary>
    /// 实现线边仓物料管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LineStoreMaterialService : ILineStoreMaterialContract
    {
        protected Database _query_db;       //组件报表数据查询数据库连接

        public ISessionFactory SessionFactory
        {
            get;
            set;
        }

        public LineStoreMaterialService(ISessionFactory sf)
        {
            this.SessionFactory = sf;
            
            _query_db = DatabaseFactory.CreateDatabase("QUERYDATA");
        }

        /// <summary>
        /// 线上仓物料数据访问类。
        /// </summary>
        public ILineStoreMaterialDataEngine LineStoreMaterialDataEngine { get; set; }
        /// <summary>
        /// 线上仓物料明细数据访问类。
        /// </summary>
        public ILineStoreMaterialDetailDataEngine LineStoreMaterialDetailDataEngine { get; set; }
        
        /// <summary>
        /// 获取线边仓物料数据。
        /// </summary>
        /// <param name="key">线边仓物料标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LineStoreMaterial&gt;" />,线边仓物料数据.</returns>
        public MethodReturnResult<LineStoreMaterial> Get(LineStoreMaterialKey key)
        {
            MethodReturnResult<LineStoreMaterial> result = new MethodReturnResult<LineStoreMaterial>();
            if (!this.LineStoreMaterialDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.LineStoreMaterialService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.LineStoreMaterialDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取线边仓物料数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LineStoreMaterial&gt;" />,线边仓物料数据集合。</returns>
        public MethodReturnResult<IList<LineStoreMaterial>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<LineStoreMaterial>> result = new MethodReturnResult<IList<LineStoreMaterial>>();

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
                    result.Data = this.LineStoreMaterialDataEngine.Get(cfg);
                //};
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }

            return result;

            //MethodReturnResult<IList<LineStoreMaterial>> result = new MethodReturnResult<IList<LineStoreMaterial>>();
            //try
            //{
            //    result.Data = this.LineStoreMaterialDataEngine.Get(cfg);
            //}
            //catch (Exception ex)
            //{
            //    result.Code = 1000;
            //    result.Message = String.Format(StringResource.OtherError, ex.Message);
            //}
            //return result;
        }

        /// <summary>
        /// 获取线边仓物料明细数据。
        /// </summary>
        /// <param name="key">线边仓物料明细标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LineStoreMaterialDetail&gt;" />,线边仓物料明细数据.</returns>
        public MethodReturnResult<LineStoreMaterialDetail> GetDetail(LineStoreMaterialDetailKey key)
        {
            MethodReturnResult<LineStoreMaterialDetail> result = new MethodReturnResult<LineStoreMaterialDetail>();
            if (!this.LineStoreMaterialDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.LineStoreMaterialDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.LineStoreMaterialDetailDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取线边仓物料明细数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LineStoreMaterialDetail&gt;" />,线边仓物料明细数据集合。</returns>
        public MethodReturnResult<IList<LineStoreMaterialDetail>> GetDetail(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<LineStoreMaterialDetail>> result = new MethodReturnResult<IList<LineStoreMaterialDetail>>();

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
                    result.Data = this.LineStoreMaterialDetailDataEngine.Get(cfg);
                //};
            }
            catch (Exception ex)
            {
                result.Code = 1000;

                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 获取线边仓物料明细数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LineStoreMaterialDetail&gt;" />,线边仓物料明细数据集合。</returns>
        public MethodReturnResult SplitMaterialLot(SplitMaterialLotParameter p)
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
                result = ExecuteMaterialSplitLot(p);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLogError("SplitLot>", ex);
                result.Code = 1000;
                result.Message += string.Format(StringResource.OtherError, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        protected MethodReturnResult ExecuteMaterialSplitLot(SplitMaterialLotParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();

            if (p == null)
            {
                result.Code = 1001;
                result.Message = StringResource.ParameterIsNull;
                return result;
            }

            #region define List of DataEngine
            List<LineStoreMaterialDetail> lstLotDataEngineForUpdate = new List<LineStoreMaterialDetail>();
            List<LineStoreMaterialDetail> lstLotDataEngineForInsert = new List<LineStoreMaterialDetail>();
            #endregion

            if (p.ChildMaterialLot == null || p.ChildMaterialLot.Count == 0)
            {
                result.Code = 1001;
                result.Message = string.Format("{0} {1}"
                                                , "子批次号"
                                                , StringResource.ParameterIsNull);
                return result;
            }
            #region //Check ParentLotNumber

            if (p.ParentMaterialLotNumber == null || p.ParentMaterialLotNumber.Length == 0)
            {
                result.Code = 1001;
                result.Message = string.Format("{0} {1}"
                                                , "父批次号"
                                                , StringResource.ParameterIsNull);
                return result;
            }

            PagingConfig cfg = new PagingConfig()
            {
                Where = string.Format("  Key.MaterialLot LIKE  '{0}%' AND Key.OrderNumber='{1}'"
                            , p.ParentMaterialLotNumber,p.OrderNumber)
            };

            IList<LineStoreMaterialDetail> parentMaterialLotList = this.LineStoreMaterialDetailDataEngine.Get(cfg);
            if (parentMaterialLotList == null || parentMaterialLotList.Count <= 0 || parentMaterialLotList[0]==null)
            {
                result.Code = 1001;
                result.Message = string.Format("物料批号({0})不存在", p.ParentMaterialLotNumber);
                return result;
            }

            LineStoreMaterialDetail parentMaterialLot = parentMaterialLotList[0];
            if (parentMaterialLot.Attr2!=null&&parentMaterialLot.Attr2.Length>0)
            {
                result.Code = 1001;
                result.Message = string.Format("物料批号({0})已为子批次，禁止拆分", p.ParentMaterialLotNumber);
                return result;
            }
            #endregion

            double sumQtyForChildLot = 0;
            foreach (ChildMaterialLotParameter childMaterialLot in p.ChildMaterialLot)
            {
                sumQtyForChildLot = sumQtyForChildLot + childMaterialLot.Quantity;
            }
            if (Math.Round(sumQtyForChildLot,6) > parentMaterialLot.CurrentQty)
            {
                result.Code = 1001;
                result.Message = string.Format("子批次的合计数量{0}大于父批次的数量{1}"
                                                , sumQtyForChildLot
                                                , parentMaterialLot.CurrentQty);
                return result;
            }
            //更新父批次
            LineStoreMaterialDetail parentMaterialLotForUpdate = parentMaterialLot.Clone() as LineStoreMaterialDetail;
            parentMaterialLotForUpdate.CurrentQty = parentMaterialLot.CurrentQty - sumQtyForChildLot;
            //加入父批次List
            lstLotDataEngineForUpdate.Add(parentMaterialLotForUpdate);



            //插入子批次
            for (int i = 0; i < p.count; i++)
            {
                LineStoreMaterialDetailKey lsmdKey =  new LineStoreMaterialDetailKey ()
                {
                  LineStoreName = parentMaterialLotForUpdate.Key.LineStoreName,
                  MaterialCode = parentMaterialLotForUpdate.Key.MaterialCode,
                  OrderNumber = parentMaterialLotForUpdate.Key.OrderNumber,
                  MaterialLot = p.ChildMaterialLot[i].MaterialLotNumber
                };
                LineStoreMaterialDetail childMaterialLotForInsert = new LineStoreMaterialDetail()
                {
                    Key = lsmdKey,
                    SupplierCode = parentMaterialLotForUpdate.SupplierCode,
                    SupplierMaterialLot = parentMaterialLotForUpdate.SupplierMaterialLot,
                    ReceiveQty = p.ChildMaterialLot[i].Quantity,
                    LoadingQty = 0,
                    UnloadingQty = 0,
                    ReturnQty = 0,
                    ScrapQty = 0,
                    CurrentQty = p.ChildMaterialLot[i].Quantity,
                    Description = parentMaterialLotForUpdate.Description,
                    Creator = p.Creator,
                    CreateTime = System.DateTime.Now,
                    Editor = p.Operator,
                    EditTime = System.DateTime.Now,
                    Attr1=parentMaterialLotForUpdate.Attr1,
                    Attr2=parentMaterialLotForUpdate.Key.MaterialLot,
                    Attr3=parentMaterialLotForUpdate.Attr3,
                    Attr4=parentMaterialLotForUpdate.Attr4,
                    Attr5=parentMaterialLotForUpdate.Attr5
                };
                lstLotDataEngineForInsert.Add(childMaterialLotForInsert);
            }




            ISession session = this.LineStoreMaterialDetailDataEngine.SessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();
            try
            {
                #region//记录操作历史
                foreach (LineStoreMaterialDetail lot in lstLotDataEngineForUpdate)
                {
                    this.LineStoreMaterialDetailDataEngine.Update(lot, session);
                }

                foreach (LineStoreMaterialDetail lot in lstLotDataEngineForInsert)
                {
                    this.LineStoreMaterialDetailDataEngine.Insert(lot, session);
                }
                transaction.Commit();
                session.Close();
                #endregion
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                LogHelper.WriteLogError("ExecuteSplitLot>", ex);
                result.Code = 1000;
                result.Message += string.Format(StringResource.OtherError, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }


    }
}
