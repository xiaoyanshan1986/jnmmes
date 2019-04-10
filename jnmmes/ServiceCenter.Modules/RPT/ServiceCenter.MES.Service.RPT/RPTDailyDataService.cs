// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.RPT
// Author           : 方军
// Created          : 2016-01-12 13:43:52.250
//
// Last Modified By : 
// Last Modified On : 
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
    public class RPTDailyDataService : IRPTDailyDataContract
    {
        /// <summary>
        /// 数据库对象。
        /// </summary>
        protected Database report_db;   //组件报表数据库连接

        public RPTDailyDataService()
        {
            this.report_db = DatabaseFactory.CreateDatabase("RPTDB");
        }

        /// <summary>
        /// 组件日运营数据获取操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        public MethodReturnResult<DataSet> Get(ref RPTDailyDataGetParameter p)
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
                    this.report_db.AddInParameter(cmd, "ReportCode", DbType.String, string.IsNullOrEmpty(p.ReportCode) ? "%" : p.ReportCode);               //报表代码
                    this.report_db.AddInParameter(cmd, "StartDate", DbType.String, string.IsNullOrEmpty(p.StartDate) ? "%" : p.StartDate);                  //开始日期
                    this.report_db.AddInParameter(cmd, "EndDate", DbType.String, string.IsNullOrEmpty(p.EndDate) ? "%" : p.EndDate);                        //结束日期
                    this.report_db.AddInParameter(cmd, "LocationName", DbType.String, string.IsNullOrEmpty(p.LocationName) ? "" : p.LocationName);          //车间
                    this.report_db.AddInParameter(cmd, "LineCode", DbType.String, string.IsNullOrEmpty(p.LineCode) ? "" : p.LineCode);                      //线别
                    this.report_db.AddInParameter(cmd, "OrderNumber", DbType.String, string.IsNullOrEmpty(p.OrderNumber) ? "" : p.OrderNumber);             //工单号
                    this.report_db.AddInParameter(cmd, "ProductID", DbType.String, string.IsNullOrEmpty(p.ProductID) ? "" : p.ProductID);                   //产品
                    this.report_db.AddInParameter(cmd, "SchedulingCode", DbType.String, string.IsNullOrEmpty(p.SchedulingCode) ? "" : p.SchedulingCode);    //班别
                    this.report_db.AddInParameter(cmd, "MonthDataNumber", DbType.Int32, p.MonthDataNumber < 0 ? 0 : p.MonthDataNumber);                     //月数据显示数
                    this.report_db.AddInParameter(cmd, "YearDataNumber", DbType.Int32, p.YearDataNumber < 0 ? 0 : p.YearDataNumber);                        //年数据显示数

                    this.report_db.AddInParameter(cmd, "@pageSize", DbType.Int32, p.PageSize);      //单页数据大小
                    this.report_db.AddInParameter(cmd, "@pageNo", DbType.Int32, p.PageNo);          //页号
                    this.report_db.AddOutParameter(cmd, "@Records", DbType.Int32, int.MaxValue);    //数据总记录数

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

                    //返回总记录数
                    p.Records = Convert.ToInt32(cmd.Parameters["@Records"].Value);
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
    }
}
