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
    /// 实现客户端配置属性管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class CalibrationPlateService : ICalibrationPlateContract
    {
        /// <summary>
        /// 客户端配置属性数据访问读写。
        /// </summary>
        public ICalibrationPlateDataEngine CalibrationPlateDataEngine { get; set; }


        /// <summary>
        /// 添加客户端配置属性。
        /// </summary>
        /// <param name="obj">客户端配置属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(CalibrationPlate obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                if (this.CalibrationPlateDataEngine.IsExists(obj.Key))
                {
                    result.Code = 1001;
                    result.Message = String.Format(StringResource.CalibrationPlateService_IsExists, obj.Key);
                    return result;
                }
                this.CalibrationPlateDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改客户端配置属性。
        /// </summary>
        /// <param name="obj">客户端配置属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(CalibrationPlate obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.CalibrationPlateDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CalibrationPlateService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.CalibrationPlateDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除客户端配置属性。
        /// </summary>
        /// <param name="key">客户端配置属性标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.CalibrationPlateDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CalibrationPlateService_IsNotExists, key);
                return result;
            }
            try
            {
                this.CalibrationPlateDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取客户端配置属性数据。
        /// </summary>
        /// <param name="key">客户端配置属性标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;CalibrationPlate&gt;" />,客户端配置属性数据.</returns>
        public MethodReturnResult<CalibrationPlate> Get(string key)
        {
            MethodReturnResult<CalibrationPlate> result = new MethodReturnResult<CalibrationPlate>();
            if (!this.CalibrationPlateDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CalibrationPlateService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.CalibrationPlateDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取客户端配置属性数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;CalibrationPlate&gt;" />,客户端配置属性数据集合。</returns>
        public MethodReturnResult<IList<CalibrationPlate>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<CalibrationPlate>> result = new MethodReturnResult<IList<CalibrationPlate>>();
            try
            {
                result.Data = this.CalibrationPlateDataEngine.Get(cfg);
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
