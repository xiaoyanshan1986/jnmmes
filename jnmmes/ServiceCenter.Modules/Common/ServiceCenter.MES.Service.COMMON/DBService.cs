using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Service.Contract.COMMON;
using ServiceCenter.Model;
using System.ServiceModel.Activation;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;

namespace ServiceCenter.MES.Service.COMMON
{
    /// <summary>
    /// 在制品Move报表契约接口。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class DBService : IDBContract
    {
        /// <summary>
        /// 数据库对象。
        /// </summary>
        protected Database _db;

        public DBService()
        {
            this._db = DatabaseFactory.CreateDatabase();
        }

        public MethodReturnResult<DataTable> ExecuteQuery(string strSql)
        {
            MethodReturnResult<DataTable> result = new MethodReturnResult<DataTable>()
            {
                Code=0
            };

            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = strSql;
                    DataSet ds= this._db.ExecuteDataSet(cmd);
                    result.Data =ds.Tables[0];
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
