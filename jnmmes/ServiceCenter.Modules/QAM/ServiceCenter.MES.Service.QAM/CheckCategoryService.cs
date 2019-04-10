using ServiceCenter.MES.DataAccess.Interface.QAM;
using ServiceCenter.MES.Model.QAM;
using ServiceCenter.MES.Service.Contract.QAM;
using ServiceCenter.MES.Service.QAM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.QAM
{
    /// <summary>
    /// 实现检验参数组管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class CheckCategoryService : ICheckCategoryContract
    {
        /// <summary>
        /// 检验参数组数据访问读写。
        /// </summary>
        public ICheckCategoryDataEngine CheckCategoryDataEngine { get; set; }


        /// <summary>
        /// 添加检验参数组。
        /// </summary>
        /// <param name="obj">检验参数组数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(CheckCategory obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.CheckCategoryDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.CheckCategoryService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.CheckCategoryDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改检验参数组。
        /// </summary>
        /// <param name="obj">检验参数组数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(CheckCategory obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.CheckCategoryDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckCategoryService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.CheckCategoryDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除检验参数组。
        /// </summary>
        /// <param name="key">检验参数组标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.CheckCategoryDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckCategoryService_IsNotExists, key);
                return result;
            }
            try
            {
                this.CheckCategoryDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取检验参数组数据。
        /// </summary>
        /// <param name="key">检验参数组标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;CheckCategory&gt;" />,检验参数组数据.</returns>
        public MethodReturnResult<CheckCategory> Get(string key)
        {
            MethodReturnResult<CheckCategory> result = new MethodReturnResult<CheckCategory>();
            if (!this.CheckCategoryDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckCategoryService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.CheckCategoryDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取检验参数组数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;CheckCategory&gt;" />,检验参数组数据集合。</returns>
        public MethodReturnResult<IList<CheckCategory>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<CheckCategory>> result = new MethodReturnResult<IList<CheckCategory>>();
            try
            {
                result.Data = this.CheckCategoryDataEngine.Get(cfg);
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
