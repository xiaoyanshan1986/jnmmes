using Microsoft.Practices.EnterpriseLibrary.Data;
// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.RPT
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="WIPMoveService.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************、
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

/// <summary>
/// The RPT namespace.
/// </summary>
namespace ServiceCenter.MES.Service.RPT
{
    /// <summary>
    /// 在制品Move报表契约接口。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class WIPMoveService : IWIPMoveContract
    {
        /// <summary>
        /// 数据库对象。
        /// </summary>
        //protected Database _db;
        protected Database _query_db;       //组件报表数据查询数据库连接

        public WIPMoveService()
        {
            //this._db = DatabaseFactory.CreateDatabase();

            _query_db = DatabaseFactory.CreateDatabase("QUERYDATA");
        }

        /// <summary>
        /// 在制品Move数据获取操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        public MethodReturnResult<DataSet> Get(WIPMoveGetParameter p)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 999999999;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_QRY_WIP_MOVE_DATA";
                    this._query_db.AddInParameter(cmd, "p_startDate", DbType.DateTime, p.StartDate);
                    this._query_db.AddInParameter(cmd, "p_endDate", DbType.DateTime, p.EndDate);
                    this._query_db.AddInParameter(cmd, "p_shiftName", DbType.String, p.ShiftName);
                    this._query_db.AddInParameter(cmd, "p_locationName", DbType.String, p.LocationName);
                    this._query_db.AddInParameter(cmd, "p_orderNumber", DbType.String,  p.OrderNumber);
                    this._query_db.AddInParameter(cmd, "p_materialCode", DbType.String,  p.MaterialCode);
                    result.Data = this._query_db.ExecuteDataSet(cmd);
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
        public MethodReturnResult<DataSet> GetWipMoveForStep(WIPMoveGetParameter p)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 300000;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_QRY_WIP_MOVE_DATA_FOR_STEP";
                    this._query_db.AddInParameter(cmd, "p_startDate", DbType.DateTime, p.StartTime);
                    this._query_db.AddInParameter(cmd, "p_endDate", DbType.DateTime, p.EndTime);
                    this._query_db.AddInParameter(cmd, "p_shiftName", DbType.String, p.ShiftName);
                    this._query_db.AddInParameter(cmd, "p_locationName", DbType.String, p.LocationName);
                    this._query_db.AddInParameter(cmd, "p_StepName", DbType.String, p.StepName);
                    this._query_db.AddInParameter(cmd, "@p_orderNumber", DbType.String, p.OrderNumber);
                    result.Data = this._query_db.ExecuteDataSet(cmd);
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
        /// 在制品Move明细数据获取操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        public MethodReturnResult<DataSet> GetDetail(ref WIPMoveDetailGetParameter p)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_QRY_WIP_MOVE_DETAIL_DATA";
                    this._query_db.AddInParameter(cmd, "@p_startDate", DbType.DateTime, p.StartDate);
                    this._query_db.AddInParameter(cmd, "@p_endDate", DbType.DateTime, p.EndDate);
                    this._query_db.AddInParameter(cmd, "@p_shiftName", DbType.String, p.ShiftName);
                    this._query_db.AddInParameter(cmd, "@p_locationName", DbType.String, p.LocationName);
                    this._query_db.AddInParameter(cmd, "@p_orderNumber", DbType.String, p.OrderNumber);
                    this._query_db.AddInParameter(cmd, "@p_materialCode", DbType.String, p.MaterialCode);
                    this._query_db.AddInParameter(cmd, "@p_routeOperationName", DbType.String, p.RouteOperationName);
                    this._query_db.AddInParameter(cmd, "@p_activity", DbType.Int32, p.Activity);
                    this._query_db.AddInParameter(cmd, "@pageNo", DbType.Int32, p.PageNo);
                    this._query_db.AddInParameter(cmd, "@pageSize", DbType.Int32, p.PageSize);
                    this._query_db.AddOutParameter(cmd, "@records", DbType.Int32, int.MaxValue);
                    result.Data = this._query_db.ExecuteDataSet(cmd);
                    p.TotalRecords = Convert.ToInt32(cmd.Parameters["@records"].Value);
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
        /// 批次号获取简单信息
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        public MethodReturnResult<DataSet> GetLotInformation(string lot)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_LOTNUMBER_INFORMATION";
                    this._query_db.AddInParameter(cmd, "@p_lotNumber", DbType.String, lot);
                    result.Data = this._query_db.ExecuteDataSet(cmd);
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

        public MethodReturnResult<DataSet> GetDailyQuantityOfWIP(QMSemiProductionGetParameter p)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" select t.ROUTE_STEP_NAME, sum(t.QUANTITY) QUANTITY from WIP_LOT t 
                                                        where t.LOCATION_NAME='{1}' and t.CREATE_TIME>=CONVERT(DATETIME,'{0}',121)+' 08:00:00'  
                                                        AND t.CREATE_TIME<CONVERT(DATETIME,'{0}',121)+1+'08:00:00'
                                                        group by t.ROUTE_STEP_NAME", p.StartDate, p.LocationName);
                    result.Data = _query_db.ExecuteDataSet(cmd);
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
