using Microsoft.Practices.EnterpriseLibrary.Data;
// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.RPT
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="WIPDisplayService.cs" company="">
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
    /// 在制品分布报表契约接口。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class WIPDisplayService : IWIPDisplayContract
    {
        /// <summary>
        /// 数据库对象。
        /// </summary>
        //protected Database _db;
        protected Database _query_db;       //组件报表数据查询数据库连接

        public WIPDisplayService()
        {
            //this._db = DatabaseFactory.CreateDatabase();

            _query_db = DatabaseFactory.CreateDatabase("QUERYDATA");
        }

        /// <summary>
        /// 在制品分布数据获取操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        public MethodReturnResult<DataSet> Get(WIPDisplayGetParameter p)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_QRY_WIP_DATA";
                    this._query_db.AddInParameter(cmd, "p_locationName", DbType.String, string.IsNullOrEmpty(p.LocationName)?"%":p.LocationName);
                    this._query_db.AddInParameter(cmd, "p_orderNumber", DbType.String, string.IsNullOrEmpty(p.OrderNumber) ? "%" : p.OrderNumber);
                    this._query_db.AddInParameter(cmd, "p_materialCode", DbType.String, string.IsNullOrEmpty(p.MaterialCode) ? "%" : p.MaterialCode);
                    this._query_db.AddInParameter(cmd, "p_onlinetime", DbType.Double, p.OnlineTime);
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
    }
}
