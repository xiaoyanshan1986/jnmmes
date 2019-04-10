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
    /// 实现区域管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LocationService : ILocationContract
    {
        /// <summary>
        /// 区域数据访问读写。
        /// </summary>
        public ILocationDataEngine LocationDataEngine { get; set; }


        /// <summary>
        /// 添加区域。
        /// </summary>
        /// <param name="obj">区域数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Location obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.LocationDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.LocationService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.LocationDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改区域。
        /// </summary>
        /// <param name="obj">区域数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(Location obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.LocationDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.LocationService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.LocationDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除区域。
        /// </summary>
        /// <param name="key">区域标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.LocationDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.LocationService_IsNotExists, key);
                return result;
            }
            try
            {
                this.LocationDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取区域数据。
        /// </summary>
        /// <param name="key">区域标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;Location&gt;" />,区域数据.</returns>
        public MethodReturnResult<Location> Get(string key)
        {
            MethodReturnResult<Location> result = new MethodReturnResult<Location>();
            if (!this.LocationDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.LocationService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.LocationDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取区域数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Location&gt;" />,区域数据集合。</returns>
        public MethodReturnResult<IList<Location>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<Location>> result = new MethodReturnResult<IList<Location>>();
            try
            {
                result.Data = this.LocationDataEngine.Get(cfg);
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
