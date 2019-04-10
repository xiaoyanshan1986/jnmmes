using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Contract.FMM;
using ServiceCenter.MES.Service.FMM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.FMM
{
    /// <summary>
    /// 实现设备异常类型耗时管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class EquipmentConsumingService : IEquipmentConsumingContract
    {
        /// <summary>
        /// 设备异常类型耗时管理访问读写。
        /// </summary>
        public IEquipmentConsumingDataEngine EquipmentConsumingDataEngine { get; set; }

        /// <summary>
        /// 添加设备异常类型耗时管理。
        /// </summary>
        /// <param name="obj">设备异常类型耗时管理数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(EquipmentConsuming obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.EquipmentConsumingDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.EquipmentConsumingService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.EquipmentConsumingDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        
        /// <summary>
        /// 修改设备异常类型耗时管理。
        /// </summary>
        /// <param name="obj">设备异常类型耗时管理数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(EquipmentConsuming obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                this.EquipmentConsumingDataEngine.Modify(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 修改设备异常类型耗时管理(列表方式)
        /// </summary>
        /// <param name="lst">设备异常类型耗时管理列表</param>
        /// <returns></returns>
        public MethodReturnResult Modify(IList<EquipmentConsuming> lst)
        {
            MethodReturnResult result = new MethodReturnResult();
            NHibernate.ISession db = null;
            ITransaction transaction = null;
            try
            {
                db = this.EquipmentConsumingDataEngine.SessionFactory.OpenSession();
                transaction = db.BeginTransaction();
                foreach (EquipmentConsuming obj in lst)
                {
                    this.EquipmentConsumingDataEngine.Modify(obj, db);
                }
                transaction.Commit();
                db.Close();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                db.Close();
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 删除设备异常类型耗时管理。
        /// </summary>
        /// <param name="key">设备异常类型耗时管理标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(EquipmentConsumingKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.EquipmentConsumingDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentConsumingService_IsNotExists, key);
                return result;
            }
            try
            {
                this.EquipmentConsumingDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取设备异常类型耗时管理。
        /// </summary>
        /// <param name="key">设备异常类型耗时管理标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;PlanAttendance&gt;" />,设备异常类型耗时管理数据.</returns>
        public MethodReturnResult<EquipmentConsuming> Get(EquipmentConsumingKey key)
        {
            MethodReturnResult<EquipmentConsuming> result = new MethodReturnResult<EquipmentConsuming>();
            if (!this.EquipmentConsumingDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentConsumingService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.EquipmentConsumingDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 设备异常类型耗时管理数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;PlanAttendance&gt;" />,设备异常类型耗时管理数据集合。</returns>
        public MethodReturnResult<IList<EquipmentConsuming>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<EquipmentConsuming>> result = new MethodReturnResult<IList<EquipmentConsuming>>();
            try
            {
                result.Data = this.EquipmentConsumingDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
    }
}
