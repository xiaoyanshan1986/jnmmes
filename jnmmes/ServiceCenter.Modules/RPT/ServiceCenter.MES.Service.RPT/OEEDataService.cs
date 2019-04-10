// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.RPT
// Author           : 方军
// Created          : 2016-01-12 13:43:52.250
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="RPTDailyDataService.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Practices.EnterpriseLibrary.Data;
using ServiceCenter.MES.Service.Contract.RPT;
using ServiceCenter.MES.Service.RPT.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Data.SqlClient;

/// <summary>
/// The RPT namespace.
/// </summary>
namespace ServiceCenter.MES.Service.RPT
{
    /// <summary>
    /// 日运营报表契约接口。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class OEEDataService : IOEEDataContract
    {
        /// <summary>
        /// 数据库对象。
        /// </summary>
        protected Database report_db;
        protected Database query_db;
        private Database _db = null;

        public OEEDataService()
        {
            this.report_db = DatabaseFactory.CreateDatabase("RPTDB");
            this._db = DatabaseFactory.CreateDatabase();
            this.query_db = DatabaseFactory.CreateDatabase("QUERYDATA");
        }

        /// <summary>
        /// 组件日运营数据获取操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        public MethodReturnResult<DataSet> Get(OEEDataGetParameter p)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                DataSet dsResult = new DataSet();
                DataTable dt = new DataTable();

                using (DbConnection con = this.report_db.CreateConnection())
                {
                    int iReturn;
                    string strErrorMessage = string.Empty;
                    DbCommand cmd = con.CreateCommand();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_RPT_ModuleDaily_list";
                    this.report_db.AddInParameter(cmd, "ReportCode", DbType.String, string.IsNullOrEmpty(p.StartDate) ? "%" : p.ReportCode);
                    this.report_db.AddInParameter(cmd, "StartDate", DbType.String, string.IsNullOrEmpty(p.StartDate) ? "%" : p.StartDate);
                    this.report_db.AddInParameter(cmd, "EndDate", DbType.String, string.IsNullOrEmpty(p.EndDate) ? "%" : p.EndDate);
                    this.report_db.AddInParameter(cmd, "LocationName", DbType.String, string.IsNullOrEmpty(p.LocationName) ? "" : p.LocationName);
                    this.report_db.AddInParameter(cmd, "LineCode", DbType.String, string.IsNullOrEmpty(p.LineCode) ? "" : p.LineCode);
                    this.report_db.AddInParameter(cmd, "OrderNumber", DbType.String, string.IsNullOrEmpty(p.OrderNumber) ? "" : p.OrderNumber);
                    this.report_db.AddInParameter(cmd, "ProductID", DbType.String, string.IsNullOrEmpty(p.ProductID) ? "" : p.ProductID);

                    //设置返回错误信息
                    cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 512));
                    cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

                    //设置返回值
                    SqlParameter parReturn = new SqlParameter("@return", SqlDbType.Int);
                    parReturn.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(parReturn);

                    //执行存储过程
                    dsResult = this.report_db.ExecuteDataSet(cmd);

                    //取得返回值
                    iReturn = (int)cmd.Parameters["@return"].Value;

                    if (iReturn == -1)  //调用失败返回错误信息
                    {
                        strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();
                        result.Code = 1000;
                        result.Message = strErrorMessage;
                        result.Detail = strErrorMessage;

                        return result;
                    }
                }
                result.Data = dsResult;
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return result;
        }


        #region 设备OEE报表

        public MethodReturnResult<DataSet> GetOEEDailyData(OEEDataGetParameter p)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                DataSet dsResult = new DataSet();
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                using (DbConnection con = this.report_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_OEE_RATE_OPERATE_DAILY_MONTHLY";
                    this.report_db.AddInParameter(cmd, "p_StartDateTime", DbType.String, p.StartDate);
                    this.report_db.AddInParameter(cmd, "p_EndDateTime", DbType.String, p.EndDate);
                    this.report_db.AddInParameter(cmd, "p_EquName", DbType.String, p.StepName);
                    this.report_db.AddInParameter(cmd, "p_LocationName", DbType.String, p.LocationName);
                    ds = this.report_db.ExecuteDataSet(cmd);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];
                        dt.TableName = "dtQTY";
                        dsResult.Tables.Add(dt.Copy());
                    }

                }
                result.Data = dsResult;
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return result;
        }
        #endregion

        #region 稼动率子报表
        public MethodReturnResult<DataSet> GetWIPDailyDataForActRATE(OEEDataGetParameter p)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                DataSet dsResult = new DataSet();
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                using (DbConnection con = this.report_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_ActRATE_OPERATE_DAILY_MONTHLY";
                    this.report_db.AddInParameter(cmd, "p_StartDateTime", DbType.String, p.StartDate);
                    this.report_db.AddInParameter(cmd, "p_EndDateTime", DbType.String, p.EndDate);
                    this.report_db.AddInParameter(cmd, "p_StepName", DbType.String, p.StepName);
                    ds = this.report_db.ExecuteDataSet(cmd);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];
                        dt.TableName = "dtRATE";
                        dsResult.Tables.Add(dt.Copy());
                    }
                }
                result.Data = dsResult;
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return result;
        }
        #endregion

        #region 稼动率及停机损失比例趋势图
        public MethodReturnResult<DataSet> GetWIPDailyDataForActAndPercentRate(OEEDataGetParameter p)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                DataSet dsResult = new DataSet();
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                using (DbConnection con = this.report_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_ActAndPercentRATE_OPERATE_DAILY_MONTHLY";
                    this.report_db.AddInParameter(cmd, "p_StartDateTime", DbType.String, p.StartDate);
                    this.report_db.AddInParameter(cmd, "p_EndDateTime", DbType.String, p.EndDate);
                    this.report_db.AddInParameter(cmd, "p_StepName", DbType.String, p.StepName);
                    ds = this.report_db.ExecuteDataSet(cmd);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];
                        dt.TableName = "dtRATE";
                        dsResult.Tables.Add(dt.Copy());
                    }
                }
                result.Data = dsResult;
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return result;
        }
        #endregion

        #region 停机分类
        public MethodReturnResult<DataSet> GetWIPDailyDataForShutDownClassification(OEEDataGetParameter p)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                DataSet dsResult = new DataSet();
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                using (DbConnection con = this.report_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_ShutDownClassificationRATE_OPERATE_DAILY_MONTHLY";
                    this.report_db.AddInParameter(cmd, "p_StartDateTime", DbType.String, p.StartDate);
                    this.report_db.AddInParameter(cmd, "p_StepName", DbType.String, p.StepName);
                    ds = this.report_db.ExecuteDataSet(cmd);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];
                        dt.TableName = "dtRATE";
                        dsResult.Tables.Add(dt.Copy());
                    }
                }
                result.Data = dsResult;
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return result;
        }
        #endregion

    }
}
