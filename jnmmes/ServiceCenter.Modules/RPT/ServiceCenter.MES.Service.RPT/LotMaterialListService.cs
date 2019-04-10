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
using System.Data.SqlClient;
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
    /// 批次物料数据查询服务。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LotMaterialListService : ILotMaterialListContract
    {
        //private Database _db = null;            //工作数据库
        private Database _query_db = null;       //报表数据库

        public LotMaterialListService()
        {
            //this._db = DatabaseFactory.CreateDatabase();
            this._query_db = DatabaseFactory.CreateDatabase("QUERYDATA");
        }

        public MethodReturnResult<DataSet> Get(ref LotMaterialListQueryParameter p)
        {
            MethodReturnResult<DataSet> result=new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_QRY_LOT_MATERIAL_DATA";

                    this._query_db.AddInParameter(cmd, "@locationName", DbType.String, p.LocationName);
                    this._query_db.AddInParameter(cmd, "@orderNumber", DbType.String, p.OrderNumber);
                    this._query_db.AddInParameter(cmd, "@startCreateTime", DbType.DateTime, p.StartCreateTime);
                    this._query_db.AddInParameter(cmd, "@endCreateTime", DbType.DateTime, p.EndCreateTime);
                    this._query_db.AddInParameter(cmd, "@lotNumber", DbType.String, p.LotNumber);
                    this._query_db.AddInParameter(cmd, "@lotNumber1", DbType.String, p.LotNumber1);
                    this._query_db.AddInParameter(cmd, "@pageSize", DbType.Int32, p.PageSize);
                    this._query_db.AddInParameter(cmd, "@pageNo", DbType.Int32, p.PageNo);
                    this._query_db.AddOutParameter(cmd, "@totalRecords",DbType.Int32,int.MaxValue);
                    result.Data = this._query_db.ExecuteDataSet(cmd);
                    p.TotalRecords = Convert.ToInt32(cmd.Parameters["@totalRecords"].Value);
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

        #region//组件车间原物料耗用明细表
        public MethodReturnResult<DataSet> GetMaterialConsume(RPTDailyDataGetParameter p)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                DataSet dsResult = new DataSet();
                DataTable dt = new DataTable();

                using (DbConnection con = this._query_db.CreateConnection())
                {
                    int iReturn;
                    string strErrorMessage = string.Empty;
                    DbCommand cmd = con.CreateCommand();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_MATERIAL_FILE";
                    this._query_db.AddInParameter(cmd, "START_TIME", DbType.String, p.StartDate);
                    this._query_db.AddInParameter(cmd, "END_TIME", DbType.String, p.EndDate);
                    this._query_db.AddInParameter(cmd, "store", DbType.String, p.LocationName);
                    this._query_db.AddInParameter(cmd, "line", DbType.String, p.LineCode);
                    this._query_db.AddInParameter(cmd, "ordernumber", DbType.String, p.OrderNumber);

                    //设置返回错误信息
                    cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 512));
                    cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

                    //设置返回值
                    SqlParameter parReturn = new SqlParameter("@return", SqlDbType.Int);
                    parReturn.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(parReturn);

                    //执行存储过程
                    dsResult = this._query_db.ExecuteDataSet(cmd);

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
        #endregion

        /// <summary>
        /// 获取物料出货信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetRPTMaterialData(ref  MaterialDataParameter p)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 600;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_RPT_GetMaterialSQLDataList";
                    //存储过程传递的参数
                    this._query_db.AddInParameter(cmd, "@out_packageNo", DbType.String, p.OutPackageNo);
                    this._query_db.AddInParameter(cmd, "@bom_materialCode", DbType.String, p.BomMaterialCode);
                    this._query_db.AddInParameter(cmd, "@material_materialName", DbType.String, p.BomMaterialName);
                    this._query_db.AddInParameter(cmd, "@product_materialCode", DbType.String, p.ProductMaterialCode);
                    this._query_db.AddInParameter(cmd, "@out_startDate", DbType.DateTime, p.OutStartTime);
                    this._query_db.AddInParameter(cmd, "@out_endDate", DbType.DateTime, p.OutEndTime);
                    this._query_db.AddInParameter(cmd, "@pageSize", DbType.Int32, p.PageSize);
                    this._query_db.AddInParameter(cmd, "@pageNo", DbType.Int32, p.PageNo);
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

                    //返回总数据
                    //p.AllData = result.Data.Tables[0];

                    //返回总记录数
                    p.Records = Convert.ToInt32(cmd.Parameters["@Records"].Value);

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
    }
}
