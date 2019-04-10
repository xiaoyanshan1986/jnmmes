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
    /// 实现采集数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class DataAcquisitionDataService : IDataAcquisitionDataContract
    {
        protected Database _db;

        public DataAcquisitionDataService()
        {
            this._db = DatabaseFactory.CreateDatabase();
        }

        /// <summary>
        /// 采集字段数据访问读写。
        /// </summary>
        /// <value>The DataAcquisitionData data engine.</value>
        public IDataAcquisitionDataDataEngine DataAcquisitionDataDataEngine { get; set; }

        /// <summary>
        /// 采集字段数据事务访问读写。
        /// </summary>
        public IDataAcquisitionTransDataEngine DataAcquisitionTransDataEngine { get; set; }

        /// <summary>
        /// 添加采集字段。
        /// </summary>
        /// <param name="obj">采集字段数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(DataAcquisitionData obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            
            try
            {
                if (this.DataAcquisitionDataDataEngine.IsExists(obj.Key))
                {
                    result.Code = 1001;
                    result.Message = String.Format(StringResource.DataAcquisitionDataService_IsExists, obj.Key);
                    return result;
                }

                this.DataAcquisitionDataDataEngine.Insert(obj);
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
        public MethodReturnResult Modify(DataAcquisitionData obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.DataAcquisitionDataDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.DataAcquisitionDataService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.DataAcquisitionDataDataEngine.Update(obj);
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
        public MethodReturnResult Delete(DataAcquisitionDataKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.DataAcquisitionDataDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.DataAcquisitionDataService_IsNotExists, key);
                return result;
            }
            try
            {
                this.DataAcquisitionDataDataEngine.Delete(key);
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
        /// <returns><see cref="MethodReturnResult&lt;DataAcquisitionData&gt;" />,采集字段数据.</returns>
        public MethodReturnResult<DataAcquisitionData> Get(DataAcquisitionDataKey key)
        {
            MethodReturnResult<DataAcquisitionData> result = new MethodReturnResult<DataAcquisitionData>();
            if (!this.DataAcquisitionDataDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.DataAcquisitionDataService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.DataAcquisitionDataDataEngine.Get(key);
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
        /// <returns><see cref="MethodReturnResult&lt;DataAcquisitionData&gt;" />,采集字段数据集合。</returns>
        public MethodReturnResult<IList<DataAcquisitionData>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<DataAcquisitionData>> result = new MethodReturnResult<IList<DataAcquisitionData>>();
            try
            {
                result.Data = this.DataAcquisitionDataDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 批量增加采集数据（某日期所有数据）
        /// </summary>
        /// <param name="lst">数据数组</param>
        /// <returns></returns>
        public MethodReturnResult Add(IList<DataAcquisitionData> lst)
        {
            MethodReturnResult result = new MethodReturnResult();
            ISession session = null;
            ITransaction transaction = null;
            List<DataAcquisitionTrans> lstDataAcquisitionTransForInsert = new List<DataAcquisitionTrans>();

            try
            {
                if (lst != null)
                {
                    //取得事物主键
                    string transactionKey = Guid.NewGuid().ToString();

                    //1.判断数据合规性
                    foreach (DataAcquisitionData obj in lst)
                    {
                        //1.1判断数据是否存在
                        if (this.DataAcquisitionDataDataEngine.IsExists(obj.Key))
                        {
                            result.Code = 1001;
                            result.Message = String.Format(StringResource.DataAcquisitionDataService_IsExists, obj.Key);
                            return result;
                        }

                        //1.2创建事务对象
                        DataAcquisitionTrans dataTrans = new DataAcquisitionTrans(transactionKey, obj, EnumAcquisitionTransActivity.New);

                        lstDataAcquisitionTransForInsert.Add(dataTrans);
                    }

                    //2.开始事务
                    session = this.DataAcquisitionDataDataEngine.SessionFactory.OpenSession();
                    transaction = session.BeginTransaction();
                    
                    //2.1保存采集数据
                    foreach (DataAcquisitionData obj in lst)
                    {                        
                        this.DataAcquisitionDataDataEngine.Insert(obj, session);
                    }

                    //2.2保存采集数据事物
                    foreach (DataAcquisitionTrans trans in lstDataAcquisitionTransForInsert)
                    {
                        this.DataAcquisitionTransDataEngine.Insert(trans, session);
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

                this.DataAcquisitionDataDataEngine.DeleteByCondition(condition);
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
        public MethodReturnResult Modify(IList<DataAcquisitionData> lst)
        {
            MethodReturnResult result = new MethodReturnResult();            
            ISession session = null;
            ITransaction transaction = null;
            List<DataAcquisitionTrans> lstDataAcquisitionTransForInsert = new List<DataAcquisitionTrans>();

            try
            {
                if (lst != null)
                {
                    //取得事物主键
                    string transactionKey = Guid.NewGuid().ToString();

                    //1.判断数据合规性
                    foreach (DataAcquisitionData obj in lst)
                    {                        
                        //1.2创建事务对象
                        DataAcquisitionTrans dataTrans = new DataAcquisitionTrans(transactionKey, obj, EnumAcquisitionTransActivity.Modify);

                        lstDataAcquisitionTransForInsert.Add(dataTrans);
                    }

                    //开始事务
                    session = this.DataAcquisitionDataDataEngine.SessionFactory.OpenSession();
                    transaction = session.BeginTransaction();
                    
                    //1.更新采集数据
                    foreach (DataAcquisitionData obj in lst)
                    {
                        this.DataAcquisitionDataDataEngine.Modify(obj, session);
                    }

                    //2.2保存采集数据事物
                    foreach (DataAcquisitionTrans trans in lstDataAcquisitionTransForInsert)
                    {
                        this.DataAcquisitionTransDataEngine.Insert(trans, session);
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
        /// 通过存储过程取得采集数据结果列表
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        public MethodReturnResult<DataSet> GetData(ref DataAcquisitionDataGetParameter p)
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
                    cmd.CommandText = "sp_EDC_DataAcquisitionData_list";
                    this._db.AddInParameter(cmd, "ItemCode", DbType.String, string.IsNullOrEmpty(p.ItemCode) ? "%" : p.ItemCode);                       //项目代码
                    this._db.AddInParameter(cmd, "StartDate", DbType.String, string.IsNullOrEmpty(p.StartDate) ? "%" : p.StartDate);                    //开始日期
                    this._db.AddInParameter(cmd, "EndDate", DbType.String, string.IsNullOrEmpty(p.EndDate) ? "%" : p.EndDate);                          //结束日期
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
