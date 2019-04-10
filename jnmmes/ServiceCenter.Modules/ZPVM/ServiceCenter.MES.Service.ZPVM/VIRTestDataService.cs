using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Contract.ZPVM;
using ServiceCenter.MES.Service.ZPVM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using NHibernate;
//using ServiceCenter.Common.DataAccess.NHibernate;

using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
namespace ServiceCenter.MES.Service.ZPVM
{
    /// <summary>
    /// 实现安规测试数据数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class VIRTestDataService : IVIRTestDataContract
    {
        protected Database query_db;

        public VIRTestDataService()
        {
            this.query_db = DatabaseFactory.CreateDatabase("QUERYDATA");
        }

        /// <summary>
        /// 安规测试数据数据数据访问读写。
        /// </summary>
        public IVIRTestDataDataEngine VIRTestDataDataEngine { get; set; }

        /// <summary>
        /// 添加安规测试数据数据。
        /// </summary>
        /// <param name="obj">安规测试数据数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(VIRTestData obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.VIRTestDataDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.VIRTestDataService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.VIRTestDataDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error,ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 修改安规测试数据数据。
        /// </summary>
        /// <param name="obj">安规测试数据数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(VIRTestData obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.VIRTestDataDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.VIRTestDataService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.VIRTestDataDataEngine.Update(obj);
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
        /// 删除安规测试数据数据。
        /// </summary>
        /// <param name="key">安规测试数据数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(VIRTestDataKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.VIRTestDataDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.VIRTestDataService_IsNotExists, key);
                return result;
            }
            try
            {
                this.VIRTestDataDataEngine.Delete(key);
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
        /// 获取安规测试数据数据数据。
        /// </summary>
        /// <param name="key">安规测试数据数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;VIRTestData&gt;" />,安规测试数据数据数据.</returns>
        public MethodReturnResult<VIRTestData> Get(VIRTestDataKey key)
        {
            MethodReturnResult<VIRTestData> result = new MethodReturnResult<VIRTestData>();
            if (!this.VIRTestDataDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.VIRTestDataService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.VIRTestDataDataEngine.Get(key);
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
        /// 获取安规测试数据数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;VIRTestData&gt;" />,安规测试数据数据数据集合。</returns>
        public MethodReturnResult<IList<VIRTestData>> Get(ref PagingConfig cfg)
        {
            //DbConnection con = this.query_db.CreateConnection();
            //ISession session;
            
            MethodReturnResult<IList<VIRTestData>> result = new MethodReturnResult<IList<VIRTestData>>();

            try
            {
                //con.Open();
                //session = this.VIRTestDataDataEngine.SessionFactory.OpenSession(con);

                result.Data = this.VIRTestDataDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>安规数据查询（从存储过程获取）</summary>
        /// <param name="p"></param>
        /// <returns></returns>
        /// 
        //public MethodReturnResult<DataSet> GetIVdata(ref LotIVdataParameter p)
        //{
        //    string strErrorMessage = string.Empty;
        //    MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
        //    try
        //    {
        //        using (DbConnection con = this.query_db.CreateConnection())
        //        {
        //            //调用存储过程
        //            DbCommand cmd = con.CreateCommand();
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.CommandText = "sp_RPT_IVTest";

        //            //存储过程传递的参数
        //            this.query_db.AddInParameter(cmd, "@LotList", DbType.String, p.Lotlist);
        //            this.query_db.AddInParameter(cmd, "@StratTime", DbType.String, p.StratTime);
        //            this.query_db.AddInParameter(cmd, "@EndTime", DbType.String, p.EndTime);
        //            this.query_db.AddInParameter(cmd, "@IsDefault", DbType.Int32, p.IsDefault);
        //            this.query_db.AddInParameter(cmd, "@IsPrint", DbType.Int32, p.IsPrint);
        //            this.query_db.AddInParameter(cmd, "@lineCode", DbType.String, p.LineCode);
        //            this.query_db.AddInParameter(cmd, "@PageNo", DbType.Int32, p.PageNo + 1);
        //            this.query_db.AddInParameter(cmd, "@PageSize", DbType.Int32, p.PageSize);
        //            //返回总记录数
        //            this.query_db.AddOutParameter(cmd, "@Records", DbType.Int32, int.MaxValue);
        //            cmd.Parameters["@Records"].Direction = ParameterDirection.Output;
        //            //设置返回错误信息
        //            cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));
        //            cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

        //            //设置返回值
        //            SqlParameter parReturn = new SqlParameter("@return", SqlDbType.Int);
        //            parReturn.Direction = ParameterDirection.ReturnValue;
        //            cmd.Parameters.Add(parReturn);

        //            //执行存储过程
        //            result.Data = this.query_db.ExecuteDataSet(cmd);

        //            //返回总记录数
        //            p.TotalRecords = Convert.ToInt32(cmd.Parameters["@Records"].Value);

        //            //取得返回值
        //            int i = (int)cmd.Parameters["@return"].Value;

        //            //调用失败返回错误信息
        //            if (i == -1)
        //            {
        //                strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();
        //                result.Code = 1000;
        //                result.Message = strErrorMessage;
        //                result.Detail = strErrorMessage;
        //            }
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

        
    }
}
