using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.EDC;
using ServiceCenter.MES.Model.EDC;
using ServiceCenter.MES.Service.Contract.EDC;
using ServiceCenter.MES.Service.EDC.Resources;
using ServiceCenter.Model;
using System.ServiceModel.Activation;
using System.Transactions;
using NHibernate;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.SqlClient;

namespace ServiceCenter.MES.Service.EDC
{
    /// <summary>
    /// 实现采集数据事务管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class DataAcquisitionTransService : IDataAcquisitionTransContract
    {
        protected Database _db;

        public DataAcquisitionTransService()
        {
            this._db = DatabaseFactory.CreateDatabase();
        }

        /// <summary>
        /// 采集字段数据访问读写。
        /// </summary>
        /// <value>The DataAcquisitionTrans data engine.</value>
        public IDataAcquisitionTransDataEngine DataAcquisitionTransDataEngine { get; set; }
        
        /// <summary>
        /// 添加采集字段。
        /// </summary>
        /// <param name="obj">采集字段数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(DataAcquisitionTrans obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            
            try
            {
                if (this.DataAcquisitionTransDataEngine.IsExists(obj.Key))
                {
                    result.Code = 1001;
                    result.Message = String.Format(StringResource.DataAcquisitionTransService_IsExists, obj.Key);
                    return result;
                }

                this.DataAcquisitionTransDataEngine.Insert(obj);
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
        /// 修改采集字段。
        /// </summary>
        /// <param name="obj">采集字段数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(DataAcquisitionTrans obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.DataAcquisitionTransDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.DataAcquisitionTransService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.DataAcquisitionTransDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 删除采集字段。
        /// </summary>
        /// <param name="key">采集字段标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(DataAcquisitionTransKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.DataAcquisitionTransDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.DataAcquisitionTransService_IsNotExists, key);
                return result;
            }
            try
            {
                this.DataAcquisitionTransDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取采集字段数据。
        /// </summary>
        /// <param name="key">采集字段标识符。</param>
        /// <returns><see cref="MethodReturnResult&lt;DataAcquisitionTrans&gt;" />,采集字段数据.</returns>
        public MethodReturnResult<DataAcquisitionTrans> Get(DataAcquisitionTransKey key)
        {
            MethodReturnResult<DataAcquisitionTrans> result = new MethodReturnResult<DataAcquisitionTrans>();
            if (!this.DataAcquisitionTransDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.DataAcquisitionTransService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.DataAcquisitionTransDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取采集字段数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;DataAcquisitionTrans&gt;" />,采集字段数据集合。</returns>
        public MethodReturnResult<IList<DataAcquisitionTrans>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<DataAcquisitionTrans>> result = new MethodReturnResult<IList<DataAcquisitionTrans>>();
            try
            {
                result.Data = this.DataAcquisitionTransDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 批量增加采集数据事务（某日期所有数据）
        /// </summary>
        /// <param name="lst">数据数组</param>
        /// <returns></returns>
        public MethodReturnResult Add(IList<DataAcquisitionTrans> lst)
        {
            MethodReturnResult result = new MethodReturnResult();
            ISession session = null;
            ITransaction transaction = null;

            try
            {
                if (lst != null)
                {         
                    //开始事务
                    session = this.DataAcquisitionTransDataEngine.SessionFactory.OpenSession();
                    transaction = session.BeginTransaction();

                    
                    foreach (DataAcquisitionTrans obj in lst)
                    {
                        this.DataAcquisitionTransDataEngine.Insert(obj, session);
                    }
                     
                    //提交事务       
                    transaction.Commit();
                    session.Close();                    
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
                result.Detail = ex.ToString();

                //关闭事务
                if ( transaction != null )
                {
                    transaction.Rollback();
                    session.Close();
                }                
            }
            return result;
        }

        /// <summary>
        /// 根据采集时间、采集项目、车间、线别、设备删除对相应的数据
        /// </summary>
        /// <param name="eDCTime">采集时间</param>
        /// <param name="itemCode">项目代码</param>
        /// <param name="lineCode">线别</param>
        /// <param name="equipmentCode">设备代码</param>
        /// <param name="locationName">车间</param>
        /// <returns></returns>
        public MethodReturnResult Delete(DateTime eDCTime, string itemCode, string lineCode, string equipmentCode, string locationName)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                string condition = string.Format(@"Key.EDCTime = '{0}' 
                                                    AND Key.ItemCode = '{1}'
                                                    AND Key.LocationName = '{2}'
                                                    AND Key.LineCode = '{3}'
                                                    AND Key.EquipmentCode = '{4}'",
                                                  eDCTime, 
                                                  itemCode,
                                                  locationName,
                                                  lineCode,
                                                  equipmentCode);

                this.DataAcquisitionTransDataEngine.DeleteByCondition(condition);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 修改数据采集信息（列表）
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        public MethodReturnResult Modify(IList<DataAcquisitionTrans> lst)
        {
            MethodReturnResult result = new MethodReturnResult();            
            ISession session = null;
            ITransaction transaction = null;

            try
            {
                if (lst != null)
                {       
                    //开始事务
                    session = this.DataAcquisitionTransDataEngine.SessionFactory.OpenSession();
                    transaction = session.BeginTransaction();
                    
                    foreach (DataAcquisitionTrans obj in lst)
                    {
                        this.DataAcquisitionTransDataEngine.Modify(obj, session);
                    }
                    
                    //提交事务
                    transaction.Commit();
                    session.Close();                    
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);

                //关闭事务
                if (transaction != null)
                {
                    transaction.Rollback();
                    session.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// 通过存储过程取得采集数据事务结果列表
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>        
        public MethodReturnResult<DataSet> GetData(ref DataAcquisitionTransGetParameter p)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                DataSet dsResult = new DataSet();
                DataTable dt = new DataTable();

                using (DbConnection con = this._db.CreateConnection())
                {
                    int iReturn;
                    string strErrorMessage = string.Empty;
                    DbCommand cmd = con.CreateCommand();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_EDC_DataAcquisitionTrans_list";
                    this._db.AddInParameter(cmd, "EDCTime", DbType.DateTime, p.EDCTime);                    //采集时间
                    this._db.AddInParameter(cmd, "ItemCode", DbType.String, string.IsNullOrEmpty(p.ItemCode) ? "%" : p.ItemCode);                       //项目代码
                    this._db.AddInParameter(cmd, "LocationName", DbType.String, string.IsNullOrEmpty(p.LocationName) ? "" : p.LocationName);            //车间
                    this._db.AddInParameter(cmd, "LineCode", DbType.String, string.IsNullOrEmpty(p.LineCode) ? "" : p.LineCode);                        //线别
                    this._db.AddInParameter(cmd, "EquipmentCode", DbType.String, string.IsNullOrEmpty(p.EquipmentCode) ? "" : p.EquipmentCode);         //设备代码

                    this._db.AddInParameter(cmd, "@pageSize", DbType.Int32, p.PageSize);      //单页数据大小
                    this._db.AddInParameter(cmd, "@pageNo", DbType.Int32, p.PageNo);          //页号
                    this._db.AddOutParameter(cmd, "@Records", DbType.Int32, int.MaxValue);    //数据总记录数

                    //设置返回错误信息
                    cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 512));
                    cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

                    //设置返回值
                    SqlParameter parReturn = new SqlParameter("@return", SqlDbType.Int);
                    parReturn.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(parReturn);

                    //执行存储过程
                    dsResult = this._db.ExecuteDataSet(cmd);

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
