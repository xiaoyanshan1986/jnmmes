using Microsoft.Practices.EnterpriseLibrary.Data;
using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Service.WIP.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;


namespace ServiceCenter.MES.Service.WIP
{

    /// <summary>
    /// 实现包装查询服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class PackageQueryService : IPackageQueryContract
    {
        protected Database _db;
        protected Database _query_db;       //组件报表数据查询数据库连接

        public ISessionFactory SessionFactory
        {
            get;
            set;
        }

        public PackageQueryService(ISessionFactory sf)
        {
            this.SessionFactory = sf;

            this._db = DatabaseFactory.CreateDatabase();

            _query_db = DatabaseFactory.CreateDatabase("QUERYDATA");
        }

        /// <summary>
        ///  数据入BIN数据访问类。
        /// </summary>
        public IPackageCornerDataEngine PackageCornerDataEngine
        {
            get;
            set;
        }

        /// <summary>
        ///  数据入BIN数据明细访问类。
        /// </summary>
        public IPackageCornerDetailDataEngine PackageCornerDetailDataEngine
        {
            get;
            set;
        }


        /// <summary>
        ///  数据入BIN数据明细访问类。
        /// </summary>
        public IBinRuleDataEngine BinRuleDataEngine
        {
            get;
            set;
        }

     
        /// <summary>
        /// 包装数据访问类。
        /// </summary>

        public IPackageDataEngine PackageDataEngine { get; set; }

        /// <summary>
        /// 包装明细数据访问类。
        /// </summary>
        public IPackageDetailDataEngine PackageDetailDataEngine { get; set; }


        /// <summary>
        /// 包装明细数据访问类。
        /// </summary>
        public IPackageBinDataEngine PackageBinDataEngine { get; set; }

        /// <summary>
        /// 获取包装数据。
        /// </summary>
        /// <param name="key">包装标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;Package&gt;" />,包装数据.</returns>
        public MethodReturnResult<Package> Get(string key)
        {
            MethodReturnResult<Package> result = new MethodReturnResult<Package>();
            if (!this.PackageDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PackageQueryService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.PackageDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取包装数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Package&gt;" />,包装数据集合。</returns>
        public MethodReturnResult<IList<Package>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<Package>> result = new MethodReturnResult<IList<Package>>();
            try
            {
                result.Data = this.PackageDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取包装明细数据。
        /// </summary>
        /// <param name="key">包装明细标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;PackageDetail&gt;" />,包装明细数据.</returns>
        public MethodReturnResult<PackageDetail> GetDetail(PackageDetailKey key)
        {
            MethodReturnResult<PackageDetail> result = new MethodReturnResult<PackageDetail>();
            if (!this.PackageDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PackageQueryService_DetailIsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.PackageDetailDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取包装明细数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;PackageDetail&gt;" />,包装明细数据集合。</returns>
        public MethodReturnResult<IList<PackageDetail>> GetDetail(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<PackageDetail>> result = new MethodReturnResult<IList<PackageDetail>>();

            try
            {
                //取数
                result.Data = this.PackageDetailDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }

            return result;
        }
        
        public MethodReturnResult<DataSet> GetPackageTransaction(string key)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@"select  
		                                                    t2.PACKAGE_NO,t2.ORDER_NUMBER,t2.MATERIAL_CODE,t2.PACKAGE_STATE,t2.LAST_PACKAGE,t2.PACKAGE_QTY,
		                                                    t.EDITOR,t.EDIT_TIME,
		                                                    t1.LOT_NUMBER,t1.ROUTE_NAME,t1.ACTIVITY
			                                           from WIP_PACKAGE t2
				                                            inner join WIP_TRANSACTION_PACKAGE t on t2.PACKAGE_NO=t.PACKAGE_NO
				                                            left join WIP_TRANSACTION t1 on t1.TRANSACTION_KEY=t.TRANSACTION_KEY
				                                            where  t2.PACKAGE_NO ='{0}'
				                                            order by t1.CREATE_TIME", key);
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
        
//        public MethodReturnResult CleanBin(string lineCode,string binNo)
//        {
//            int rowSuccess = 0;
//            int rowSuccessCorner = 0;
            
//            try
//            {
//                using (DbConnection con = this._db.CreateConnection())
//                {
//                    DbCommand cmd = con.CreateCommand();
//                    string sql = null;
//                    string sqlCorner = null;
//                    //PagingConfig cfg=null;
//                    if (lineCode!=null&&binNo != null)
//                    {
//                        sql= string.Format("update [dbo].[WIP_PACKAGE_BIN]  set BIN_PACKAGED=1  where PACKAGE_LINE='{0}'  and BIN_PACKAGED=0 and BIN_NO='{1}'", lineCode,binNo);
//                        sqlCorner = string.Format("update WIP_PACKAGE_CORNER  set BIN_PACKAGED=1  where PACKAGE_LINE='{0}'  and BIN_PACKAGED=0 and BIN_NO='{1}'", lineCode, binNo);
//                        //cfg = new PagingConfig()//更新包护角的数据表
//                        //{
//                        //   IsPaging=false,
//                        //   Where=string.Format("BinNo='{0}' and PackageLine='{1}' and BinPackaged=0",binNo,lineCode)
                           
//                        //};
//                        //IList<PackageCorner> lstPackageCorner = this.PackageCornerDataEngine.Get(cfg);
//                        //foreach (PackageCorner item in lstPackageCorner)
//                        //{
//                        //    lstPackageCornerUpdate.Add(item);
//                        //}
////                        cfg = new PagingConfig() 
////                        {
////                            IsPaging = false,
////                            Where = string.Format("BinNo='{0}' and PackageLine='{1}' and BinPackaged=1", binNo, lineCode),
////                            OrderBy="EditTime desc"
////                        };
////                         IList<PackageCorner> lstPackageCornerMaxTime = this.PackageCornerDataEngine.Get(cfg);
////                         string maxTime = "";
////                         if (lstPackageCornerMaxTime.Count > 0)//查找该BIN最后入BIN的时间
////                         {
////                             maxTime = lstPackageCornerMaxTime[0].EditTime.ToString();
////                         }
////                         cfg = new PagingConfig()
////                         {
////                             IsPaging = false,
////                             Where = string.Format("Key.PackageLine='{0}' and Key.BinNo='{1}'", lineCode, binNo)
////                         };

////                         IList<BinRule> lstBinRules = this.BinRuleDataEngine.Get(cfg);//查找该BIN的BIN规则
////                         if (lstBinRules.Count > 0)
////                         {
////                             //Bin测试规则参数
////                             cfg = new PagingConfig()//查询该规则下对应的BIN,除了现有的BIN
////                             {
////                                 IsPaging = false,
////                                 Where = string.Format(@"Key.PackageLine='{0}'
////                                          AND Key.PsCode='{1}'
////                                          AND Key.PsItemNo='{2}'  
////                                          AND Key.PsSubCode='{3}' 
////                                          AND Key.Color='{4}'
////                                          AND Key.BinNo<>'{5}'
////                                          AND ( Key.WorkOrderNumber='{6}' or Key.WorkOrderNumber='{7}')",
////                                                       lineCode,
////                                                       lstBinRules[0].Key.PsCode,
////                                                       lstBinRules[0].Key.PsItemNo,
////                                                       lstBinRules[0].Key.PsSubCode,
////                                                       lstBinRules[0].Key.Color,
////                                                       binNo,
////                                                       lstBinRules[0].Key.WorkOrderNumber,
////                                                       "*")
////                             };

////                             IList<BinRule> lstBinRulesNew = this.BinRuleDataEngine.Get(cfg);
////                             if (lstBinRulesNew.Count > 0 && maxTime != "")
////                             {

////                                 cfg = new PagingConfig()
////                                 {
////                                     Where = string.Format("PackageLine='{0}' and EditTime>'{1}' and BinNo='{2}'", lineCode, maxTime, lstBinRulesNew[i].Key.BinNo),
////                                     IsPaging = false
////                                 };
////                                 IList<PackageCorner> lstPackageCornerNew = this.PackageCornerDataEngine.Get(cfg);//大于最后该BIN入托时间的对应BIN的明细
////                                 foreach (PackageCorner item in lstPackageCornerNew)
////                                 {
////                                     string packageKey = item.Key;
////                                     PackageCorner packageCorner = this.PackageCornerDataEngine.Get(packageKey);
////                                     cfg = new PagingConfig()
////                                     {
////                                         Where = string.Format("Key.PackageKey='{0}' and PackageLine='{1}' and CreateTime>'{2}'", packageKey, lineCode, maxTime),
////                                         IsPaging = false,
////                                         OrderBy = "ItemNo asc"
////                                     };
////                                     IList<PackageCornerDetail> lstPackageCornerDetailMore = this.PackageCornerDetailDataEngine.Get(cfg);
////                                     if (lstPackageCornerDetailMore.Count > 0)
////                                     {
////                                         int Count = 0;
////                                         foreach (PackageCornerDetail packageCornerDetail in lstPackageCornerDetailMore)
////                                         {
////                                             string lotNo = packageCornerDetail.Key.LotNumber;
////                                             cfg = new PagingConfig()
////                                             {
////                                                 Where = string.Format("Key.ObjectNumber='{0}'", lotNo),
////                                                 IsPaging = false
////                                             };
////                                             IList<PackageDetail> lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);
////                                             if (lstPackageDetail.Count > 0)
////                                             {
////                                                 Count++;
////                                             }
////                                         }
////                                         if (Count == 0)
////                                         {
////                                             if (lstPackageCornerDetailMore[0].ItemNo == 1)//判断是否需要更新还是删除该虚拟BIN
////                                             {
////                                                 lstPackageCornerDelete.Add(item);

////                                             }
////                                             else
////                                             {
////                                                 PackageCorner packageCornerClone = item.Clone() as PackageCorner;
////                                                 packageCornerClone.BinQty = lstPackageCornerDetailMore[0].ItemNo - 1;
////                                                 packageCornerClone.BinPackaged = EnumCornerPackaged.UnFinished;
////                                                 lstPackageCornerUpdate.Add(packageCornerClone);
////                                             }
////                                             foreach (PackageCornerDetail packageCornerDetail in lstPackageCornerDetailMore)
////                                             {
////                                                 lstPackageCornerDetailDelete.Add(packageCornerDetail);
////                                             }
////                                         }
                                         
////                                     }

////                                 }
////                             }

////                         }
                       
//                    }
//                    else if (lineCode!=null&&binNo == null)
//                    {
//                        //cfg = new PagingConfig()//更新包护角的数据表
//                        //{
//                        //    IsPaging = false,
//                        //    Where = string.Format(" PackageLine='{0}' and BinPackaged=0", lineCode)

//                        //};
//                        //IList<PackageCorner> lstPackageCorner = this.PackageCornerDataEngine.Get(cfg);
//                        //foreach (PackageCorner item in lstPackageCorner)
//                        //{
//                        //    lstPackageCornerUpdate.Add(item);
//                        //}
//                        sql=string.Format("update [dbo].[WIP_PACKAGE_BIN]  set BIN_PACKAGED=1   where PACKAGE_LINE='{0}' and BIN_PACKAGED=0", lineCode);
//                        sqlCorner = string.Format("update WIP_PACKAGE_CORNER  set BIN_PACKAGED=1  where PACKAGE_LINE='{0}'  and BIN_PACKAGED=0", lineCode);

//                    }
//                    //List<PackageCorner> lstPackageCornerTransaction = new List<PackageCorner>();
//                    //foreach (PackageCorner packageCorner in lstPackageCornerUpdate)
//                    //{
//                    //    packageCorner.BinPackaged = (EnumCornerPackaged)1;
//                    //    packageCorner.EditTime = DateTime.Now;
//                    //    packageCorner.Editor = lineCode;
//                    //    lstPackageCornerTransaction.Add(packageCorner);
//                    //}
                
                
//                    //foreach (PackageCorner packageCorner in lstPackageCornerTransaction)
//                    //{
//                    //    this.PackageCornerDataEngine.Update(packageCorner, session);
//                    //}

//                    cmd.CommandText =sql;
//                     rowSuccess=_db.ExecuteNonQuery(cmd);
//                    if (i<=0)
//                    {
//                        result.Code = 100;
//                        result.Message = "您输入的数据有误！请选择正确的线别和Bin号";
//                    }
//                    //transaction.Commit();
//                    //session.Close();
//                }
             
//            }
//            catch (Exception ex)
//            {
//                //transaction.Rollback();
//                //session.Close();
//                result.Code = 1000;
//                result.Message = ex.Message;
//                result.Detail = ex.ToString();
//            }
//            return result;
//        }

        public MethodReturnResult CleanBin(string lineCode, string binNo)
        {
            int rowSuccess = 0;
            int rowSuccessCorner = 0;
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    string sql = null;
                    string sqlCorner = null;
                    //PagingConfig cfg=null;
                    if (lineCode != null && binNo != null)
                    {
                        sql = string.Format("update [dbo].[WIP_PACKAGE_BIN]  set BIN_PACKAGED=1  where PACKAGE_LINE='{0}'  and BIN_PACKAGED=0 and BIN_NO='{1}'", lineCode, binNo);
                        sqlCorner = string.Format("update WIP_PACKAGE_CORNER  set BIN_PACKAGED=1  where PACKAGE_LINE='{0}'  and BIN_PACKAGED=0 and BIN_NO='{1}'", lineCode, binNo);
                    }
                    else if (lineCode != null && binNo == null)
                    {
                        sql = string.Format("update [dbo].[WIP_PACKAGE_BIN]  set BIN_PACKAGED=1   where PACKAGE_LINE='{0}' and BIN_PACKAGED=0", lineCode);
                        sqlCorner = string.Format("update WIP_PACKAGE_CORNER  set BIN_PACKAGED=1  where PACKAGE_LINE='{0}'  and BIN_PACKAGED=0", lineCode);

                    }
                    cmd.CommandText = sql;
                    rowSuccess = _db.ExecuteNonQuery(cmd);
                    cmd.CommandText = sqlCorner;
                    rowSuccessCorner = _db.ExecuteNonQuery(cmd);
                    if (rowSuccess <= 0 && rowSuccessCorner<=0)
                    {
                        result.Code = 100;
                        result.Message = "您输入的数据有误！请选择正确的线别和Bin号";
                    }
                    //transaction.Commit();
                    //session.Close();
                }

            }
            catch (Exception ex)
            {
                //transaction.Rollback();
                //session.Close();
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return result;
        }
        public MethodReturnResult<DataSet> GetRPTpackagelist(RPTpackagelistParameter p)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_RPT_Package_list";

                    //增加参数
                    this._db.AddInParameter(cmd, "@PackageList", DbType.String, p.PackageNo);       //包装号
                    this._db.AddInParameter(cmd, "@PageNo", DbType.Int32, 0);                       //页码 0 - 取得所有数据
                    this._db.AddInParameter(cmd, "@PageSize", DbType.Int32, 60);                    //单页数据行数默认设置，在页码为0时不起作用

                    this._db.AddOutParameter(cmd, "@Records", DbType.Int32, int.MaxValue);          //返回总记录数
                    cmd.Parameters["@Records"].Direction = ParameterDirection.Output;

                    //返回错误信息
                    cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));     
                    cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

                    //返回值（0 - 成功 -1 - 失败）
                    SqlParameter parReturn = new SqlParameter("@return", SqlDbType.Int);
                    parReturn.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(parReturn);

                    //取数
                    result.Data = this._db.ExecuteDataSet(cmd);

                    int i = (int)cmd.Parameters["@return"].Value;

                    if (i == -1)
                    {
                        strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();
                        result.Code = 1000;
                        result.Message = strErrorMessage;
                        result.Detail = strErrorMessage;
                    }
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

        public MethodReturnResult<DataSet> GetOEMpackagelist(RPTpackagelistParameter p)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_OEM_Package_list";

                    //增加参数
                    this._db.AddInParameter(cmd, "@PackageList", DbType.String, p.PackageNo);       //包装号
                    this._db.AddInParameter(cmd, "@PageNo", DbType.Int32, 0);                       //页码 0 - 取得所有数据
                    this._db.AddInParameter(cmd, "@PageSize", DbType.Int32, 60);                    //单页数据行数默认设置，在页码为0时不起作用

                    this._db.AddOutParameter(cmd, "@Records", DbType.Int32, int.MaxValue);          //返回总记录数
                    cmd.Parameters["@Records"].Direction = ParameterDirection.Output;

                    //返回错误信息
                    cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));
                    cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

                    //返回值（0 - 成功 -1 - 失败）
                    SqlParameter parReturn = new SqlParameter("@return", SqlDbType.Int);
                    parReturn.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(parReturn);

                    //取数
                    result.Data = this._db.ExecuteDataSet(cmd);

                    int i = (int)cmd.Parameters["@return"].Value;

                    if (i == -1)
                    {
                        strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();
                        result.Code = 1000;
                        result.Message = strErrorMessage;
                        result.Detail = strErrorMessage;
                    }
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

        public MethodReturnResult<DataSet> GetRPTpackagelistQueryDb(ref RPTpackagelistParameter p)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.CommandText = "sp_RPT_Package_list";
                    this._query_db.AddInParameter(cmd, "@PackageList", DbType.String, p.PackageNo);
                    this._query_db.AddInParameter(cmd, "@lotList", DbType.String, p.LotNumber);
                    this._query_db.AddInParameter(cmd, "@PageNo", DbType.Int32, p.PageNo + 1);
                    this._query_db.AddInParameter(cmd, "@PageSize", DbType.Int32, p.PageSize);

                    //返回总记录数
                    this._query_db.AddOutParameter(cmd, "@Records", DbType.Int32, int.MaxValue);
                    cmd.Parameters["@Records"].Direction = ParameterDirection.Output;

                    //错误信息
                    cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));
                    cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

                    //返回参数
                    SqlParameter parReturn = new SqlParameter("@return_value", SqlDbType.Int);
                    parReturn.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(parReturn);

                    cmd.CommandTimeout = 120;

                    //执行
                    result.Data = this._query_db.ExecuteDataSet(cmd);

                    //返回总记录数
                    p.TotalRecords = Convert.ToInt32(cmd.Parameters["@Records"].Value);
                    int i = (int)cmd.Parameters["@return_value"].Value;

                    if (i == -1)
                    {
                        strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();
                        result.Code = 1000;
                        result.Message = strErrorMessage;
                        result.Detail = strErrorMessage;
                    }
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

        /// <summary>存储过程获取包装历史记录数据查询</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetPackageTransactionQueryDb(ref RPTpackagelistParameter p)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    //调用存储过程
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_RPT_PackageOpertion_list";

                    //存储过程传递的参数
                    this._query_db.AddInParameter(cmd, "@PackageList", DbType.String, p.PackageNo);
                    this._query_db.AddInParameter(cmd, "@PageNo", DbType.Int32, p.PageNo + 1);
                    this._query_db.AddInParameter(cmd, "@PageSize", DbType.Int32, p.PageSize);
                    //返回总记录数
                    this._query_db.AddOutParameter(cmd, "@Records", DbType.Int32, int.MaxValue);
                    cmd.Parameters["@Records"].Direction = ParameterDirection.Output;
                    //设置返回错误信息
                    cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));
                    cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

                    //设置返回值
                    SqlParameter parReturn = new SqlParameter("@return", SqlDbType.Int);
                    parReturn.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(parReturn);

                    //执行存储过程
                    result.Data = this._query_db.ExecuteDataSet(cmd);

                    //返回总记录数
                    p.TotalRecords = Convert.ToInt32(cmd.Parameters["@Records"].Value);

                    //取得返回值
                    int i = (int)cmd.Parameters["@return"].Value;

                    //调用失败返回错误信息
                    if (i == -1)
                    {
                        strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();
                        result.Code = 1000;
                        result.Message = strErrorMessage;
                        result.Detail = strErrorMessage;
                    }
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

        /// <summary>
        /// 查询历史包装号数据
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetRPTPackageNoInfo(RPTpackagelistParameter p)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_RPT_GetPackageNoInfo";
                    this._query_db.AddInParameter(cmd, "@PackageNo", DbType.String, p.PackageNo);
                    cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));
                    cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;
                    SqlParameter parReturn = new SqlParameter("@return", SqlDbType.Int);
                    parReturn.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(parReturn);
                    result.Data = this._query_db.ExecuteDataSet(cmd);
                    int i = (int)cmd.Parameters["@return"].Value;
                    if (i == -1)
                    {
                        strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();
                        result.Code = 1000;
                        result.Message = strErrorMessage;
                        result.Detail = strErrorMessage;
                    }
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



        /// <summary>
        /// 更新包装号数据
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public MethodReturnResult UpdateAdd(Package obj, string action)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                if (action == "包装")
                {
                    this.PackageDataEngine.Update(obj);
                }
                else
                {
                    result.Code = 1000;
                    result.Message = String.Format("包装号{0}已入库，无法添加描述", obj.Key);
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        } 
    }
}
