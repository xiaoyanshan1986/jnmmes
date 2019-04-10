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
    /// 实现打印标签管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class PrintLabelService : IPrintLabelContract
    {
        /// <summary>
        /// 打印标签数据访问读写。
        /// </summary>
        public IPrintLabelDataEngine PrintLabelDataEngine { get; set; }


        /// <summary>
        /// 添加打印标签。
        /// </summary>
        /// <param name="obj">打印标签数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(PrintLabel obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.PrintLabelDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.PrintLabelService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.PrintLabelDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改打印标签。
        /// </summary>
        /// <param name="obj">打印标签数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(PrintLabel obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.PrintLabelDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PrintLabelService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.PrintLabelDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除打印标签。
        /// </summary>
        /// <param name="key">打印标签标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.PrintLabelDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PrintLabelService_IsNotExists, key);
                return result;
            }
            try
            {
                this.PrintLabelDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取打印标签数据。
        /// </summary>
        /// <param name="key">打印标签标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;PrintLabel&gt;" />,打印标签数据.</returns>
        public MethodReturnResult<PrintLabel> Get(string key)
        {
            MethodReturnResult<PrintLabel> result = new MethodReturnResult<PrintLabel>();
            if (!this.PrintLabelDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PrintLabelService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.PrintLabelDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取打印标签数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;PrintLabel&gt;" />,打印标签数据集合。</returns>
        public MethodReturnResult<IList<PrintLabel>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<PrintLabel>> result = new MethodReturnResult<IList<PrintLabel>>();
            try
            {
                result.Data = this.PrintLabelDataEngine.Get(cfg);
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
