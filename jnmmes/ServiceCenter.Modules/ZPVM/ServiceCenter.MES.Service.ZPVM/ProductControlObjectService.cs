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
    /// 实现产品控制参数设置数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ProductControlObjectService : IProductControlObjectContract
    {
        /// <summary>
        /// 产品控制参数设置数据数据访问读写。
        /// </summary>
        public IProductControlObjectDataEngine ProductControlObjectDataEngine { get; set; }


        /// <summary>
        /// 添加产品控制参数设置数据。
        /// </summary>
        /// <param name="obj">产品控制参数设置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(ProductControlObject obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.ProductControlObjectDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.ProductControlObjectService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.ProductControlObjectDataEngine.Insert(obj);
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
        /// 修改产品控制参数设置数据。
        /// </summary>
        /// <param name="obj">产品控制参数设置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(ProductControlObject obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ProductControlObjectDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ProductControlObjectService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.ProductControlObjectDataEngine.Update(obj);
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
        /// 删除产品控制参数设置数据。
        /// </summary>
        /// <param name="key">产品控制参数设置数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(ProductControlObjectKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ProductControlObjectDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ProductControlObjectService_IsNotExists, key);
                return result;
            }
            try
            {
                this.ProductControlObjectDataEngine.Delete(key);
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
        /// 获取产品控制参数设置数据数据。
        /// </summary>
        /// <param name="key">产品控制参数设置数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;ProductControlObject&gt;" />,产品控制参数设置数据数据.</returns>
        public MethodReturnResult<ProductControlObject> Get(ProductControlObjectKey key)
        {
            MethodReturnResult<ProductControlObject> result = new MethodReturnResult<ProductControlObject>();
            if (!this.ProductControlObjectDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ProductControlObjectService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.ProductControlObjectDataEngine.Get(key);
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
        /// 获取产品控制参数设置数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;ProductControlObject&gt;" />,产品控制参数设置数据数据集合。</returns>
        public MethodReturnResult<IList<ProductControlObject>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<ProductControlObject>> result = new MethodReturnResult<IList<ProductControlObject>>();
            try
            {
                result.Data = this.ProductControlObjectDataEngine.Get(cfg);
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
