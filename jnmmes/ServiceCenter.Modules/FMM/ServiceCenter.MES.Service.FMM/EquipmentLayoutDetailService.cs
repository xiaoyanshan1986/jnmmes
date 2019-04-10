using Microsoft.Practices.EnterpriseLibrary.Data;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Contract.FMM;
using ServiceCenter.MES.Service.FMM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.FMM
{
    /// <summary>
    /// 实现设备布局明细管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class EquipmentLayoutDetailService : IEquipmentLayoutDetailContract
    {

        protected Database _db;
        /// <summary>
        /// 设备布局明细数据访问读写。
        /// </summary>
        public IEquipmentLayoutDetailDataEngine EquipmentLayoutDetailDataEngine { get; set; }

        public EquipmentLayoutDetailService()
        {
            this._db = DatabaseFactory.CreateDatabase();
        }
        /// <summary>
        /// 添加设备布局明细。
        /// </summary>
        /// <param name="obj">设备布局明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(EquipmentLayoutDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.EquipmentLayoutDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.EquipmentLayoutDetailService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.EquipmentLayoutDetailDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改设备布局明细。
        /// </summary>
        /// <param name="obj">设备布局明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(EquipmentLayoutDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.EquipmentLayoutDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentLayoutDetailService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.EquipmentLayoutDetailDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除设备布局明细。
        /// </summary>
        /// <param name="key">设备布局明细标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(EquipmentLayoutDetailKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.EquipmentLayoutDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentLayoutDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                this.EquipmentLayoutDetailDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取设备布局明细数据。
        /// </summary>
        /// <param name="key">设备布局明细标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;EquipmentLayoutDetail&gt;" />,设备布局明细数据.</returns>
        public MethodReturnResult<EquipmentLayoutDetail> Get(EquipmentLayoutDetailKey key)
        {
            MethodReturnResult<EquipmentLayoutDetail> result = new MethodReturnResult<EquipmentLayoutDetail>();
            if (!this.EquipmentLayoutDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentLayoutDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.EquipmentLayoutDetailDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取设备布局明细数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;EquipmentLayoutDetail&gt;" />,设备布局明细数据集合。</returns>
        public MethodReturnResult<IList<EquipmentLayoutDetail>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<EquipmentLayoutDetail>> result = new MethodReturnResult<IList<EquipmentLayoutDetail>>();
            try
            {
                result.Data = this.EquipmentLayoutDetailDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        public MethodReturnResult<DataTable> GetEQPInfo(string LayoutName)
        {
            MethodReturnResult<DataTable> result = new MethodReturnResult<DataTable>();
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@"select t1.LAYOUT_NAME,
                                                             t1.EQUIPMENT_CODE,
                                                             t3.STATE_COLOR 
                                                      from [dbo].[FMM_EQUIPMENT_LAYOUT_DETAIL] t1 
                                                      inner join [dbo].[FMM_EQUIPMENT] t2 on t2.EQUIPMENT_CODE = t1.EQUIPMENT_CODE 
                                                      inner join [dbo].[FMM_EQUIPMENT_STATE] t3 on t3.EQUIPMENT_STATE_NAME = t2.EQUIPMENT_STATE_NAME
                                                      where t1.LAYOUT_NAME = '{0}'", LayoutName);
                    DataSet ds = _db.ExecuteDataSet(cmd);
                    result.Data = ds.Tables[0];
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

        public MethodReturnResult<DataTable> GetParameByEqpCode(string EqpCode)
        {
            MethodReturnResult<DataTable> result = new MethodReturnResult<DataTable>();
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@"select T.EQUIPMENT_CODE,T.EQUIPMENT_NAME,T.LINE_CODE,T.EQUIPMENT_STATE_NAME,T1.ROUTE_OPERATION_NAME
                                                         FROM FMM_EQUIPMENT T 
                                                      LEFT JOIN FMM_ROUTE_OPERATION_EQUIPMENT T1 ON T1.EQUIPMENT_CODE=T.EQUIPMENT_CODE
                                                      where t.EQUIPMENT_CODE = '{0}'", EqpCode);
                    DataSet ds = _db.ExecuteDataSet(cmd);
                    result.Data = ds.Tables[0];
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
        /// 获取设备及设备最后一次状态改变的信息
        /// </summary>
        /// <param name="EqpCode"></param>
        /// <returns></returns>
        public MethodReturnResult<DataTable> GetEquipmentInfo(string EqpCode)
        {
            MethodReturnResult<DataTable> result = new MethodReturnResult<DataTable>();
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" select  t1.EQUIPMENT_CODE,
                    t1.EQUIPMENT_NAME,t1.EQUIPMENT_STATE_NAME,t1.CREATOR,
                    t2.CREATOR,
                    t2.Create_Time,DATEDIFF(MI,t2.Create_Time,GETDATE()) as MinutesQty,
                    t2.Description 
                    from FMM_EQUIPMENT t1  
                    left join     [dbo].[V_EMS_STATE_EVENT] t2
                    on t1.EQUIPMENT_CODE =t2.EQUIPMENT_CODE
                    and t1.EQUIPMENT_STATE_NAME =t2.EQUIPMENT_TO_STATE_NAME
                    where t1.EQUIPMENT_CODE='{0}'", EqpCode);
                    DataSet ds = _db.ExecuteDataSet(cmd);
                    result.Data = ds.Tables[0];
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
