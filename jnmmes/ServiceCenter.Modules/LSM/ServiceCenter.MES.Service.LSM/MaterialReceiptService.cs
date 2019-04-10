using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.LSM;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.PPM;
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
using NHibernate;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;

namespace ServiceCenter.MES.Service.LSM
{
    /// <summary>
    /// 实现领料单管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public partial class MaterialReceiptService : IMaterialReceiptContract
    {
        protected Database _dbCell;         //电池数据库连接
        protected Database _db;             //组件生产数据库连接
        protected Database _query_db;       //组件报表数据查询数据库连接
        
        public ISessionFactory SessionFactory
        {
            get;
            set;
        }

        public MaterialReceiptService(ISessionFactory sf)
        {
            this.SessionFactory = sf;
            _dbCell = DatabaseFactory.CreateDatabase("RPTDBCELL");
            _db = DatabaseFactory.CreateDatabase();
            _query_db = DatabaseFactory.CreateDatabase("QUERYDATA");
        }

        /// <summary>
        /// 领料单数据访问读写。
        /// </summary>
        public IMaterialReceiptDataEngine MaterialReceiptDataEngine { get; set; }
        /// <summary>
        /// 领料单明细数据访问类。
        /// </summary>
        public IMaterialReceiptDetailDataEngine MaterialReceiptDetailDataEngine { get; set; }
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
        /// 工单BOM数据访问类。
        /// </summary>
        public IWorkOrderBOMDataEngine WorkOrderBOMDataEngine { get; set; }
        /// <summary>
        /// 供应商数据访问类。
        /// </summary>
        public ISupplierDataEngine SupplierDataEngine { get; set; }
        /// <summary>
        /// 添加领料单。
        /// </summary>
        /// <param name="obj">领料单数据。</param>
        /// <param name="lstDetail">领料明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(MaterialReceipt obj,IList<MaterialReceiptDetail> lstDetail)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                //判断领料单是否存在。
                if (this.MaterialReceiptDataEngine.IsExists(obj.Key))
                {
                    result.Code = 1001;
                    result.Message = String.Format(StringResource.MaterialReceiptService_IsExists, obj.Key);
                    return result;
                }
                //判断工单号是否存在。
                if(!this.WorkOrderDataEngine.IsExists(obj.OrderNumber))
                {
                    result.Code = 1002;
                    result.Message = String.Format(StringResource.MaterialReceiptService_WorkOrderIsNotExists
                                                    , obj.OrderNumber);
                    return result;
                }
                //using (TransactionScope ts = new TransactionScope())
                ISession session = this.WorkOrderDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    //新增领料单。
                    this.MaterialReceiptDataEngine.Insert(obj, session);
                    //新增领料单明细。
                    int itemNo = 1;
                    foreach(MaterialReceiptDetail item in lstDetail)
                    {
                        //判断物料编码在工单BOM中是否存在。
                        PagingConfig cfg = new PagingConfig()
                        {
                            //IsPaging=false,
                            //PageNo=1,
                            //PageSize=1,
                            //Where = string.Format("Key.OrderNumber='{0}' AND MaterialCode='{1}'"
                            //                      ,obj.OrderNumber
                            //                      ,item.MaterialCode),
                        };
                        IList<WorkOrderBOM> lstBOM = this.WorkOrderBOMDataEngine.Get(cfg, session);
                        if (lstBOM.Count==0)
                        {
                            result.Code = 2003;
                            result.Message = String.Format(StringResource.MaterialReceiptService_MaterialCodeIsNotExists
                                                            , string.Format("{0}-{1}",obj.OrderNumber,item.MaterialCode)
                                                            , item.Key.ItemNo);
                            return result;
                        }
                        //判断供应商代码是否存在。
                        if(!this.SupplierDataEngine.IsExists(item.SupplierCode))
                        {
                            result.Code = 2004;
                            result.Message = String.Format(StringResource.MaterialReceiptService_SupplierIsNotExists
                                                            , item.SupplierCode
                                                            , item.Key.ItemNo);
                            return result;
                        }
                        //新增领料单明细。
                        item.Key = new MaterialReceiptDetailKey()
                        {
                            ReceiptNo=obj.Key.ToUpper(),
                            ItemNo=itemNo
                        };
                        this.MaterialReceiptDetailDataEngine.Insert(item, session);
                        //获取线边仓物料数据。
                        LineStoreMaterialKey lsmKey = new LineStoreMaterialKey()
                        {
                            LineStoreName = item.LineStoreName,
                            MaterialCode = item.MaterialCode
                        };
                        LineStoreMaterial lsm = this.LineStoreMaterialDataEngine.Get(lsmKey, session);
                        //如果对应线边仓中无物料数据，则新增线边仓物料数据。
                        if (lsm == null)
                        {
                            lsm = new LineStoreMaterial()
                            {
                                Key = lsmKey,
                                CreateTime = obj.CreateTime,
                                Creator = obj.Creator,
                                Editor = obj.Editor,
                                EditTime = obj.EditTime
                            };
                            this.LineStoreMaterialDataEngine.Insert(lsm);
                        }
                        else
                        {//更新线边仓物料数据。
                            LineStoreMaterial lsmUpdate = new LineStoreMaterial()
                            {
                                Key = lsm.Key,
                                CreateTime = lsm.CreateTime,
                                Creator = lsm.Creator,
                                Editor = obj.Editor,
                                EditTime = obj.EditTime
                            };
                            this.LineStoreMaterialDataEngine.Update(lsmUpdate, session);
                        }

                        //新增线边仓明细数据。
                        LineStoreMaterialDetailKey lsmdKey = new LineStoreMaterialDetailKey()
                        {
                            LineStoreName = item.LineStoreName,
                            OrderNumber = obj.OrderNumber,
                            MaterialCode = item.MaterialCode,
                            MaterialLot = item.MaterialLot
                        };
                        LineStoreMaterialDetail lsmd = this.LineStoreMaterialDetailDataEngine.Get(lsmdKey, session);
                        //如果对应线边仓中无物料明细数据，则新增线边仓物料明细数据。
                        if (lsmd == null)
                        {
                            lsmd = new LineStoreMaterialDetail()
                            {
                                Key = lsmdKey,
                                CurrentQty = item.Qty,
                                ReceiveQty = item.Qty,
                                LoadingQty=0,
                                UnloadingQty=0,
                                ReturnQty = 0,
                                ScrapQty = 0,
                                Attr1=item.Attr1,
                                Attr2=item.Attr2,
                                Attr3=item.Attr3,
                                Attr4=item.Attr4,
                                Attr5=item.Attr5,
                                SupplierMaterialLot = item.SupplierMaterialLot,
                                SupplierCode = item.SupplierCode,
                                Description=item.Description,
                                CreateTime = obj.CreateTime,
                                Creator = obj.Creator,
                                Editor = obj.Editor,
                                EditTime = obj.EditTime
                            };
                            this.LineStoreMaterialDetailDataEngine.Insert(lsmd, session);
                        }
                        else
                        {
                            LineStoreMaterialDetail lsmdUpdate = lsmd.Clone() as LineStoreMaterialDetail;
                            //更新线边仓物料明细数据。
                            lsmdUpdate.ReceiveQty += item.Qty;
                            lsmdUpdate.CurrentQty += item.Qty;
                            lsmdUpdate.Editor = obj.Editor;
                            lsmdUpdate.EditTime = obj.EditTime;
                            this.LineStoreMaterialDetailDataEngine.Update(lsmdUpdate, session);
                        }

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
                result.Message = String.Format(StringResource.OtherError,ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取领料单数据。
        /// </summary>
        /// <param name="key">领料单标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialReceipt&gt;" />,领料单数据.</returns>
        public MethodReturnResult<MaterialReceipt> Get(string key)
        {
            MethodReturnResult<MaterialReceipt> result = new MethodReturnResult<MaterialReceipt>();
            if (!this.MaterialReceiptDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialReceiptService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.MaterialReceiptDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取领料单数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialReceipt&gt;" />,领料单数据集合。</returns>
        public MethodReturnResult<IList<MaterialReceipt>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<MaterialReceipt>> result = new MethodReturnResult<IList<MaterialReceipt>>();
            try
            {
                result.Data = this.MaterialReceiptDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }
        
        /// <summary>
        /// 获取领料单明细数据。
        /// </summary>
        /// <param name="key">领料单明细标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialReceiptDetail&gt;" />,领料单明细数据.</returns>
        public MethodReturnResult<MaterialReceiptDetail> GetDetail(MaterialReceiptDetailKey key)
        {
            MethodReturnResult<MaterialReceiptDetail> result = new MethodReturnResult<MaterialReceiptDetail>();
            if (!this.MaterialReceiptDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialReceiptDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.MaterialReceiptDetailDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取领料单明细数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialReceiptDetail&gt;" />,领料单明细数据集合。</returns>
        public MethodReturnResult<IList<MaterialReceiptDetail>> GetDetail(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<MaterialReceiptDetail>> result = new MethodReturnResult<IList<MaterialReceiptDetail>>();

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
                    result.Data = this.MaterialReceiptDetailDataEngine.Get(cfg);
                //}                
            }
            catch (Exception ex)
            {
                result.Code = 1000;

                result.Message = String.Format(StringResource.OtherError, ex.Message);
                result.Detail = ex.ToString();
            }

            return result;

            //备份在2016-09-26
            //MethodReturnResult<IList<MaterialReceiptDetail>> result = new MethodReturnResult<IList<MaterialReceiptDetail>>();

            //try
            //{
            //    result.Data = this.MaterialReceiptDetailDataEngine.Get(cfg);
            //}
            //catch (Exception ex)
            //{
            //    result.Code = 1000;
            //    result.Message = String.Format(StringResource.OtherError, ex.Message);
            //    result.Detail = ex.ToString();
            //}

            //return result;
        }

        public MethodReturnResult<MaterialReceiptDetail> GetBoxDetail(string boxLotNumber)
        {
            MethodReturnResult<MaterialReceiptDetail> result = new MethodReturnResult<MaterialReceiptDetail>();
            System.Data.DataSet dsOfBox = null;
            System.Data.DataRow dRowOfBox = null;
            try
            {
                using (DbConnection con = this._dbCell.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" select * from V_Package_Info where PACKAGE_NO='{0}'", boxLotNumber);
                    dsOfBox = this._dbCell.ExecuteDataSet(cmd);
                    if (dsOfBox != null && dsOfBox.Tables.Count > 0)
                    {
                        System.Data.DataTable dt = dsOfBox.Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            dRowOfBox = dt.Rows[0];
                        }
                    }
                }
                if (dRowOfBox == null)
                {
                    result.Code = 1001;
                    result.Message = string.Format("箱号{0}在电池MES中不存在", boxLotNumber);
                    return result;
                }

                MaterialReceiptDetail materialDetail = new MaterialReceiptDetail
                {
                    MaterialCode = dRowOfBox["MATERIAL_CODE"].ToString(),
                    MaterialLot = boxLotNumber,
                    SupplierCode = "000000",
                    Qty = Double.Parse(dRowOfBox["PACKAGE_QTY"].ToString()),
                    //Attr1 = dRowOfBox["LOWER_EFFI"].ToString(),
                    Attr1 = string.Format("{0:F2}", Convert.ToDouble(dRowOfBox["LOWER_EFFI"])).ToString(),
                    Attr2 = dRowOfBox["COLOR"].ToString(),
                };
                result.Data=materialDetail;
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        public MethodReturnResult<DataSet> GetOrderNumberByMaterialLot(string MaterialLot)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" SELECT t2.ORDER_NUMBER FROM dbo.LSM_MATERIAL_RECEIPT_detail t1 
                                                        LEFT JOIN dbo.LSM_MATERIAL_RECEIPT t2 ON t1.RECEIPT_NO=t2.RECEIPT_NO
                                                        WHERE t1.MATERIAL_LOT='{0}'", MaterialLot);
                    result.Data = _db.ExecuteDataSet(cmd);
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }

            return result;
        }
    }
}
