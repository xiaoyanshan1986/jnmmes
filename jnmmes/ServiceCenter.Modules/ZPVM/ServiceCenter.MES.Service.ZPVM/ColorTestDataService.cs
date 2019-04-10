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

namespace ServiceCenter.MES.Service.ZPVM
{
    /// <summary>
    /// 实现花色测试数据数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ColorTestDataService : IColorTestDataContract
    {
        /// <summary>
        /// 花色测试数据数据数据访问读写。
        /// </summary>
        public IColorTestDataDataEngine ColorTestDataDataEngine { get; set; }


        /// <summary>
        /// 添加花色测试数据数据。
        /// </summary>
        /// <param name="obj">花色测试数据数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(ColorTestData obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.ColorTestDataDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.IVTestDataService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.ColorTestDataDataEngine.Insert(obj);
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
        /// 修改花色测试数据数据。
        /// </summary>
        /// <param name="obj">花色测试数据数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(ColorTestData obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ColorTestDataDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.IVTestDataService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.ColorTestDataDataEngine.Update(obj);
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
        /// 删除花色测试数据数据。
        /// </summary>
        /// <param name="key">花色测试数据数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(ColorTestDataKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ColorTestDataDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.IVTestDataService_IsNotExists, key);
                return result;
            }
            try
            {
                this.ColorTestDataDataEngine.Delete(key);
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
        /// 获取花色测试数据数据数据。
        /// </summary>
        /// <param name="key">花色测试数据数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;IVTestData&gt;" />,花色测试数据数据数据.</returns>
        public MethodReturnResult<ColorTestData> Get(ColorTestDataKey key)
        {
            MethodReturnResult<ColorTestData> result = new MethodReturnResult<ColorTestData>();
            if (!this.ColorTestDataDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.IVTestDataService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.ColorTestDataDataEngine.Get(key);
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
        /// 获取花色测试数据数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;IVTestData&gt;" />,花色测试数据数据数据集合。</returns>
        public MethodReturnResult<IList<ColorTestData>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<ColorTestData>> result = new MethodReturnResult<IList<ColorTestData>>();
            try
            {
                result.Data = this.ColorTestDataDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

    }
}
