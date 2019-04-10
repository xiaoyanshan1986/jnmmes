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
    /// 实现供应商管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SupplierService : ISupplierContract
    {
        /// <summary>
        /// 供应商数据访问读写。
        /// </summary>
        public ISupplierDataEngine SupplierDataEngine { get; set; }


        /// <summary>
        /// 添加供应商。
        /// </summary>
        /// <param name="obj">供应商数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Supplier obj)
        {
            MethodReturnResult<IList<Supplier>> resultOfList = new MethodReturnResult<IList<Supplier>>();
            MethodReturnResult result = new MethodReturnResult();
            if (this.SupplierDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.SupplierService_IsExists, obj.Key);
                return result;
            }
            try
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@" Name='{0}'",obj.Name)
                };
                resultOfList.Data = this.SupplierDataEngine.Get(cfg);
                if (resultOfList.Data != null && resultOfList.Data.Count > 0)
                {
                    result.Code = 1000;
                    result.Message = string.Format(@"已存在供应名称为[{0}]的供应商代码[{1}]", obj.Name, resultOfList.Data[0].Key);
                    return result;
                }
                this.SupplierDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改供应商。
        /// </summary>
        /// <param name="obj">供应商数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(Supplier obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.SupplierDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.SupplierService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.SupplierDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除供应商。
        /// </summary>
        /// <param name="key">供应商标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.SupplierDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.SupplierService_IsNotExists, key);
                return result;
            }
            try
            {
                this.SupplierDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取供应商数据。
        /// </summary>
        /// <param name="key">供应商标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;Supplier&gt;" />,供应商数据.</returns>
        public MethodReturnResult<Supplier> Get(string key)
        {
            MethodReturnResult<Supplier> result = new MethodReturnResult<Supplier>();
            if (!this.SupplierDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.SupplierService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.SupplierDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取供应商数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Supplier&gt;" />,供应商数据集合。</returns>
        public MethodReturnResult<IList<Supplier>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<Supplier>> result = new MethodReturnResult<IList<Supplier>>();
            try
            {
                result.Data = this.SupplierDataEngine.Get(cfg);
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
