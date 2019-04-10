using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Contract.FMM;
using ServiceCenter.MES.Service.FMM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.FMM
{
    /// <summary>
    /// 实现生产厂商管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ManufacturerService : IManufacturerContract
    {
        /// <summary>
        /// 生产厂商数据访问读写。
        /// </summary>
        public IManufacturerDataEngine ManufacturerDataEngine { get; set; }


        /// <summary>
        /// 添加生产厂商。
        /// </summary>
        /// <param name="obj">生产厂商数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Manufacturer obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.ManufacturerDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.ManufacturerService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.ManufacturerDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改生产厂商。
        /// </summary>
        /// <param name="obj">生产厂商数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(Manufacturer obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ManufacturerDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ManufacturerService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.ManufacturerDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除生产厂商。
        /// </summary>
        /// <param name="key">生产厂商标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ManufacturerDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ManufacturerService_IsNotExists, key);
                return result;
            }
            try
            {
                this.ManufacturerDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取生产厂商数据。
        /// </summary>
        /// <param name="key">生产厂商标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;Manufacturer&gt;" />,生产厂商数据.</returns>
        public MethodReturnResult<Manufacturer> Get(string key)
        {
            MethodReturnResult<Manufacturer> result = new MethodReturnResult<Manufacturer>();
            if (!this.ManufacturerDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ManufacturerService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.ManufacturerDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取生产厂商数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Manufacturer&gt;" />,生产厂商数据集合。</returns>
        public MethodReturnResult<IList<Manufacturer>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<Manufacturer>> result = new MethodReturnResult<IList<Manufacturer>>();
            try
            {
                result.Data = this.ManufacturerDataEngine.Get(cfg);
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
