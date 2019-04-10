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
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Configuration;
using System.Data.Common;

namespace ServiceCenter.MES.Service.LSM
{
    /// <summary>
    /// 实现退料单管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class MaterialReturnService : IMaterialReturnContract
    {
        /// <summary>
        /// 退料单数据访问读写。
        /// </summary>
        /// 

        string ErpDBName = string.Empty;
        string ErpAccount = string.Empty;
        string ErpGroupCode = string.Empty;
        string ErpORGCode = string.Empty;
        protected Database _db;

        protected Database Ora_db;
        protected Database _query_db;       //组件报表数据查询数据库连接

        public ISessionFactory SessionFactory
        {
            get;
            set;
        }

        public MaterialReturnService(ISessionFactory sf)
        {
            //ErpDBName = ConfigurationSettings.AppSettings["ErpDBName"].ToString();
            ErpDBName = ConfigurationManager.AppSettings["ErpDBName"].ToString();
            ErpAccount = ConfigurationManager.AppSettings["ErpAccount"].ToString();
            ErpGroupCode = ConfigurationManager.AppSettings["ErpGroupCode"].ToString();
            ErpORGCode = ConfigurationManager.AppSettings["ErpORGCode"].ToString();
            this._db = DatabaseFactory.CreateDatabase();
            this.Ora_db = DatabaseFactory.CreateDatabase("ERPDB");
            _query_db = DatabaseFactory.CreateDatabase("QUERYDATA");
            this.SessionFactory = sf;
        }

        public IMaterialReturnDataEngine MaterialReturnDataEngine { get; set; }
        /// <summary>
        /// 退料单明细数据访问类。
        /// </summary>
        public IMaterialReturnDetailDataEngine MaterialReturnDetailDataEngine { get; set; }
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
        /// 添加退料单。
        /// </summary>
        /// <param name="obj">退料单数据。</param>
        /// <param name="lstDetail">退料明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(MaterialReturn obj, IList<MaterialReturnDetail> lstDetail)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                //判断退料单是否存在。
                if (this.MaterialReturnDataEngine.IsExists(obj.Key))
                {
                    result.Code = 1001;
                    result.Message = String.Format(StringResource.MaterialReturnService_IsExists, obj.Key);
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
                    //新增退料单。
                    this.MaterialReturnDataEngine.Insert(obj);
                    //新增退料单明细。
                    int itemNo = 1;
                    foreach (MaterialReturnDetail item in lstDetail)
                    {
                        //新增退料单明细。
                        item.Key = new MaterialReturnDetailKey()
                        {
                            ReturnNo  = obj.Key,
                            ItemNo = itemNo
                        };
                        this.MaterialReturnDetailDataEngine.Insert(item,session);
                        //获取线边仓物料数据。
                        LineStoreMaterialKey lsmKey = new LineStoreMaterialKey()
                        {
                            LineStoreName = item.LineStoreName,
                            MaterialCode = item.MaterialCode
                        };
                        LineStoreMaterial lsm = this.LineStoreMaterialDataEngine.Get(lsmKey,session);
                        if (lsm == null)
                        {
                            //result.Code = 2003;
                            //result.Message = string.Format(StringResource.MaterialReturnService_MaterialIsNotExists
                            //                             , lsmKey
                            //                             , item.Key.ItemNo);
                            //return result;
                        }
                        else
                        {
                            //如果对应线边仓中有物料数据，更新线边仓物料数据。
                            LineStoreMaterial lsmUpdate = lsm.Clone() as LineStoreMaterial;
                            lsmUpdate.EditTime = obj.EditTime;
                            lsmUpdate.Editor = obj.Editor;
                            this.LineStoreMaterialDataEngine.Update(lsmUpdate, session);
                        }                      

                        //获取线边仓明细数据。
                        LineStoreMaterialDetailKey lsmdKey = new LineStoreMaterialDetailKey()
                        {
                            LineStoreName = item.LineStoreName,
                            OrderNumber=obj.OrderNumber,
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
                            result.Message = string.Format(StringResource.MaterialReturnService_CurrentQtyLTReturnQty
                                                            , lsmd.Key
                                                            , lsmd.CurrentQty
                                                            , item.Qty
                                                            , item.Key.ItemNo);
                            return result;
                        }
                        //如果对应线边仓中有物料明细数据，则更新线边仓物料明细数据。
                        LineStoreMaterialDetail lsmdUpdate = lsmd.Clone() as LineStoreMaterialDetail;
                        
                        lsmdUpdate.CurrentQty -= item.Qty;
                        lsmdUpdate.ReturnQty += item.Qty;
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
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取退料单数据。
        /// </summary>
        /// <param name="key">退料单标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialReturn&gt;" />,退料单数据.</returns>
        public MethodReturnResult<MaterialReturn> Get(string key)
        {
            MethodReturnResult<MaterialReturn> result = new MethodReturnResult<MaterialReturn>();
            if (!this.MaterialReturnDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialReturnService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.MaterialReturnDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取退料单数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialReturn&gt;" />,退料单数据集合。</returns>
        public MethodReturnResult<IList<MaterialReturn>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<MaterialReturn>> result = new MethodReturnResult<IList<MaterialReturn>>();
            try
            {
                result.Data = this.MaterialReturnDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取退料单明细数据。
        /// </summary>
        /// <param name="key">退料单明细标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialReturnDetail&gt;" />,退料单明细数据.</returns>
        public MethodReturnResult<MaterialReturnDetail> GetDetail(MaterialReturnDetailKey key)
        {
            MethodReturnResult<MaterialReturnDetail> result = new MethodReturnResult<MaterialReturnDetail>();
            if (!this.MaterialReturnDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialReturnDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.MaterialReturnDetailDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取退料单明细数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialReturnDetail&gt;" />,退料单明细数据集合。</returns>
        public MethodReturnResult<IList<MaterialReturnDetail>> GetDetail(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<MaterialReturnDetail>> result = new MethodReturnResult<IList<MaterialReturnDetail>>();

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
                    result.Data = this.MaterialReturnDetailDataEngine.Get(cfg);
                //}
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;

            //MethodReturnResult<IList<MaterialReturnDetail>> result = new MethodReturnResult<IList<MaterialReturnDetail>>();
            //try
            //{
            //    result.Data = this.MaterialReturnDetailDataEngine.Get(cfg);
            // }
            //catch (Exception ex)
            //{
            //    result.Code = 1000;
            //    result.Message = String.Format(StringResource.OtherError, ex.Message);
            //}
            //return result;
        }

        public MethodReturnResult<DataSet> GetDetailByReturnNo(string key)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@"SELECT a.*,b.ORDER_NUMBER FROM [dbo].[LSM_MATERIAL_RETURN_DETAIL] a 
                                                      INNER  JOIN [dbo].[LSM_MATERIAL_RETURN] b ON a.RETURN_NO=b.RETURN_NO
                                                      WHERE a.RETURN_NO ='{0}' ", key);
                    

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
        
        public MethodReturnResult WO(MaterialReturnParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            MaterialReturn materialReturn = this.MaterialReturnDataEngine.Get(p.ReturnNo);

            if (materialReturn != null)
            {
                materialReturn.State = EnumReturnState.Approved;
                materialReturn.Key = p.ReturnNo;
                materialReturn.Editor = p.Editor;
                materialReturn.EditTime = DateTime.Now;
                materialReturn.ErpCode = p.ErpCode;
                materialReturn.Store = p.Store;
            }

            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                OrderBy = "ItemNo",
                Where = string.Format(" Key.BillCode = '{0}'"
                                            , p.ReturnNo)
            };

            ISession db = this.SessionFactory.OpenSession();
            ITransaction transaction = db.BeginTransaction();
            try
            {
                this.MaterialReturnDataEngine.Update(materialReturn, db);
                transaction.Commit();
                db.Close();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                db.Close();
                result.Code = 1000;
                result.Message += string.Format("报工单回填状态失败！", ex.Message);
                result.Detail = ex.ToString();
            }
            return result;

        }

        public MethodReturnResult<DataSet> GetStore()
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    //cmd.CommandText = string.Format(@" select * from " + ErpDBName + 
                    //                                 ".v_bd_stordoc t where t.groupcode = '{0}' and t.orgcode='{1}' and t.storname like '%成品%'",
                    //                                ErpGroupCode,
                    //                                ErpORGCode);
                    cmd.CommandText = string.Format(@" select * from " + ErpDBName +
                                                     ".v_bd_stordoc t where t.groupcode = '{0}' and t.orgcode='{1}' and storcode in ('FL001','CP001','ZZ002','ZZ003','YL003','CP005','YP002') order by storcode",
                                                    ErpGroupCode,
                                                    ErpORGCode);

                    result.Data = Ora_db.ExecuteDataSet(cmd);
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

        public MethodReturnResult<DataSet> GetEffiByMaterialLot(string materialLot)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            //string erpCode = "";

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                  
                    //select distinct vchangerate from v_ic_material_b where vbatchcode = '2345678' and groupcode = '001' and orgcode = 'K01'
                    cmd.CommandText = string.Format(@" select distinct vchangerate from " + ErpDBName + ".v_ic_material_b where groupcode = '{0}' and orgcode='{1}' and vbatchcode = '{2}' ",
                                                    ErpGroupCode,
                                                    ErpORGCode,
                                                    materialLot);


                    result.Data = Ora_db.ExecuteDataSet(cmd);
                }

                //if (Code.StartsWith("1101"))
                //{
                //    using (DbConnection con = this._db.CreateConnection())
                //    {
                //        DbCommand cmd = con.CreateCommand();
                //        cmd.CommandText = string.Format(@" SELECT * FROM [dbo].[LSM_MATERIAL_RECEIPT_DETAIL] where MATERIAL_LOT = '{0}'", Code);
                //        result.Data = _db.ExecuteDataSet(cmd);
                //    }

                //}
                //else
                //{

                //    using (DbConnection con = this.Ora_db.CreateConnection())
                //    {
                //        DbCommand cmd = con.CreateCommand();
                //        //cmd.CommandText = string.Format(@" select * from " + ErpDBName + ".v_materialconvert where code = '{0}' ", Code);
                //        cmd.CommandText = string.Format(@" select * from " + ErpDBName + ".v_materialconvert where groupcode = '{0}' and orgcode='{1}' and code = '{2}' ",
                //                                        ErpGroupCode,
                //                                        ErpORGCode,
                //                                        Code);

                //        cmd.CommandText = string.Format(@" select * from " + ErpDBName + ".v_bd_stordoc t where t.groupcode = '{0}' and t.orgcode='{1}' and t.storname like '%成品%'",
                //                                    ErpGroupCode,
                //                                    ErpORGCode);

                //        result.Data = Ora_db.ExecuteDataSet(cmd);
                //    }

                //}
               
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return result;
        }


        public MethodReturnResult<DataSet> GetERPMaterialReceiptDetail(string LotNo)//根据批次号得到ERP中物料的颜色，等级，效率
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" select t1.materialcode,
                                                               t1.vbatchcode,
                                                               sum(t1.nnum) as nnum,
                                                               t1.suppliercode,
                                                               t1.suppliername,
                                                               t1.ZJGL,
                                                               t1.ZJDL,
                                                               t1.ZJWGDJ,
                                                               t1.battransrate,
                                                               t1.batcolor,
                                                               t1.batlvl
                                                        from " + ErpDBName + ".v_ic_material_b t1 " + 
                                                       "left join " + ErpDBName + ".v_ic_material_h t2 " +
                                                       "  on t2.groupcode = '{0}' and t2.orgcode='{1}' and t1.imhcode = t2.vbillcode " +
                                                       "where t1.groupcode = '{0}' and t1.orgcode='{1}' and t1.vbatchcode = '{2}'" + 
                                                       "group by t1.materialcode, t1.vbatchcode, t1.suppliercode,t1.suppliername,t1.battransrate,t1.batcolor,t1.batlvl,t1.ZJGL,t1.ZJDL,t1.ZJWGDJ",
                                                       ErpGroupCode,
                                                       ErpORGCode,
                                                       LotNo);
                    
                    result.Data = Ora_db.ExecuteDataSet(cmd);
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


        public MethodReturnResult<DataSet> GetERPWorkStock(string OrderNumber)//根据工单号获取备料单号
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@"  select * from " + ErpDBName + ".v_mm_pickm where  groupcode = '{0}' and orgcode='{1}' and vsourcemocode = '{2}'",
                                                        ErpGroupCode,
                                                        ErpORGCode,                        
                                                        OrderNumber);
                    result.Data = Ora_db.ExecuteDataSet(cmd);
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


        //public MethodReturnResult<DataSet> GetERPWorkStockInfo(string BLNumber)//根据工单号获取备料单号
        //{
        //    MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

        //    try
        //    {
        //        using (DbConnection con = this.Ora_db.CreateConnection())
        //        {
        //            DbCommand cmd = con.CreateCommand();
        //            cmd.CommandText = string.Format(@"  select * from " + ErpDBName + ".v_mm_pickm_b where pickcode  = '{0}'", BLNumber);
        //            result.Data = Ora_db.ExecuteDataSet(cmd);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Code = 1000;
        //        result.Message = ex.Message;
        //        result.Detail = ex.ToString();
        //    }
        //    return result;
        //}

        public MethodReturnResult<DataSet> GetERPWorkStockInfo(string BLNumber, string materialcode)//根据工单号获取备料单号
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    if (string.IsNullOrEmpty(materialcode) || materialcode == "")
                    {
                        cmd.CommandText = string.Format(@"  select * from " + ErpDBName + ".v_mm_pickm_b where groupcode = '{0}' and orgcode='{1}' and pickcode  = '{2}'",
                                                        ErpGroupCode,
                                                        ErpORGCode,
                                                        BLNumber);
                    }
                    else
                    {
                        cmd.CommandText = string.Format(@"  select * from " + ErpDBName + ".v_mm_pickm_b where groupcode = '{0}' and orgcode='{1}' and pickcode = '{2}' and materialcode = '{3}'",
                                                        ErpGroupCode,
                                                        ErpORGCode, 
                                                        BLNumber, materialcode);
                    }



                    result.Data = Ora_db.ExecuteDataSet(cmd);
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

        public MethodReturnResult<DataSet> GetWOReportFromDB(string ReturnNo)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            DataSet dsResult = new DataSet();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" SELECT [BILL_CODE]
                      ,[BILL_DATE],[NOTE],[BILL_MAKER],[BILL_MAKEDDATE]
                      ,[MIXTYPE],[ORDER_NUMBER],[STORE],[MATERIAL_CODE]
                      ,[TOTAL_QTY] ,[BILL_STATE] ,[ERP_IN_CODE],[ERP_WR_CODE]
                      ,[CREATOR],[CREATE_TIME],[EDITOR] ,[EDIT_TIME],[SCRAP_TYPE]
                      FROM [dbo].[ERP_WO_REPORT] where BILL_CODE='{0}' and BILL_STATE>0 ", ReturnNo);
                    ds = _db.ExecuteDataSet(cmd);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];
                        dt.TableName = "StgIn";
                        dsResult.Tables.Add(dt.Copy());
                        cmd = con.CreateCommand();
                        //if (dt.Rows[0]["SCRAP_TYPE"].ToString() == "1")
                        //{
                            cmd.CommandText = string.Format(
                                @"select t2.MATERIAL_CODE,t2.ORDER_NUMBER, t2.MATERIAL_NAME, t2.LOT_NUMBER PACKAGE_NO,t2.GRADE ,
                        		                                                t2.SPM_VALUE ,t2.PM_NAME ,t2.PS_SUBCODE_Name,t2.[COEF_PMAX] sumCOEF_PMAX,count(t2.Lot_Number) as QTY
                                                                                from ERP_WO_REPORT_DETAIL t1
                                                                                inner join [dbo].[V_LotNumber_Detail] t2 on t1.OBJECT_NUMBER=t2.LOT_NUMBER 
                                                                                WHERE t1.BILL_CODE='{0}' group by
                                                                                t2.MATERIAL_CODE, t2.MATERIAL_NAME, t2.LOT_NUMBER,t2.GRADE ,
                        		                                                t2.SPM_VALUE ,t2.PM_NAME ,t2.PS_SUBCODE_Name,t2.ORDER_NUMBER,t2.COEF_PMAX ", ReturnNo);
                        //}
//                        else if (dt.Rows[0]["SCRAP_TYPE"].ToString() == "0")
//                        {

//                            cmd.CommandText = string.Format(
//                                @"select t2.MATERIAL_CODE,t2.ORDER_NUMBER, t2.MATERIAL_NAME, t2.PACKAGE_NO ,t2.GRADE ,
//		                      t2.SPM_VALUE ,t2.PM_NAME ,t2.PS_SUBCODE_Name,SUM(t2.[COEF_PMAX]) sumCOEF_PMAX,count(t2.Lot_Number) as QTY
//                              from ERP_WO_REPORT_DETAIL t1
//                              inner join [dbo].[V_Package_Detail] t2 on t1.OBJECT_NUMBER=t2.PACKAGE_NO
//                              WHERE t1.BILL_CODE='{0}' group by
//                              t2.MATERIAL_CODE, t2.MATERIAL_NAME, t2.PACKAGE_NO ,t2.GRADE ,
//		                      t2.SPM_VALUE ,t2.PM_NAME ,t2.PS_SUBCODE_Name,t2.ORDER_NUMBER ", BillCode);
//                        }


                        ds = _db.ExecuteDataSet(cmd);
                        if (ds != null && ds.Tables.Count > 0)
                        {
                            dt = ds.Tables[0];
                            dt.TableName = "StgInDetail";
                            dsResult.Tables.Add(dt.Copy());
                        }
                    }
                    result.Data = dsResult;
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return result;
        }//获取打印模板数据


        public MethodReturnResult<DataSet> GetERPReportCodeById(string strId)//视图中需要通过回执单号查询备料计划单号以及出库单号
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" select * from " + ErpDBName + ".V_IC_MATERIAL_H where groupcode = '{0}' and orgcode='{1}' and cgeneralhid = '{2}' ",
                                                    ErpGroupCode,
                                                    ErpORGCode,
                                                    strId);

                    result.Data = Ora_db.ExecuteDataSet(cmd);
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


        public MethodReturnResult<DataSet> GetReturnReportFromDB(string ReturnNo)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            DataSet dsResult = new DataSet();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format( @" SELECT * FROM [dbo].[LSM_MATERIAL_RETURN] WHERE RETURN_NO='{0}' AND RETURN_STATE=10", ReturnNo);
                    ds = _db.ExecuteDataSet(cmd);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];
                        dt.TableName = "StgIn";
                        dsResult.Tables.Add(dt.Copy());
                        cmd = con.CreateCommand();

                            cmd.CommandText = string.Format(
                                @"SELECT * FROM [dbo].[LSM_MATERIAL_RETURN_detail] WHERE RETURN_NO='{0}'", ReturnNo);
//                            cmd.CommandText = string.Format(
//                                @"select t2.MATERIAL_CODE,t2.ORDER_NUMBER, t2.MATERIAL_NAME, t2.PACKAGE_NO ,t2.GRADE ,
//		                      t2.SPM_VALUE ,t2.PM_NAME ,t2.PS_SUBCODE_Name,SUM(t2.[COEF_PMAX]) sumCOEF_PMAX,count(t2.Lot_Number) as QTY
//                              from ERP_WO_REPORT_DETAIL t1
//                              inner join [dbo].[V_Package_Detail] t2 on t1.OBJECT_NUMBER=t2.PACKAGE_NO
//                              WHERE t1.BILL_CODE='{0}' group by
//                              t2.MATERIAL_CODE, t2.MATERIAL_NAME, t2.PACKAGE_NO ,t2.GRADE ,
//		                      t2.SPM_VALUE ,t2.PM_NAME ,t2.PS_SUBCODE_Name,t2.ORDER_NUMBER ", ReturnNo);



                        ds = _db.ExecuteDataSet(cmd);
                        if (ds != null && ds.Tables.Count > 0)
                        {
                            dt = ds.Tables[0];
                            dt.TableName = "StgInDetail";
                            dsResult.Tables.Add(dt.Copy());
                        }
                    }
                    result.Data = dsResult;
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return result;
        }//根据退料号得到退料明细数据


        public MethodReturnResult<DataSet> GetMaterialInfo(string MaterialCode)//根据物料编码得到物料名称，规格等信息
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@"  select * from " + ErpDBName + ".V_MATERIAL_CONVERT where groupcode = '{0}' and orgcode='{1}' and invcode  = '{2}'",
                                                    ErpGroupCode,
                                                    ErpORGCode,                             
                                                    MaterialCode);

                    result.Data = Ora_db.ExecuteDataSet(cmd);
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


        public MethodReturnResult<DataSet> GetStoreName(string Store)//根据仓库代码得到仓库名称
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@"  select * from " + ErpDBName + ".v_bd_stordoc where groupcode = '{0}' and orgcode='{1}' and storcode  = '{2}'",
                                                    ErpGroupCode,
                                                    ErpORGCode,
                                                    Store);

                    result.Data = Ora_db.ExecuteDataSet(cmd);
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
