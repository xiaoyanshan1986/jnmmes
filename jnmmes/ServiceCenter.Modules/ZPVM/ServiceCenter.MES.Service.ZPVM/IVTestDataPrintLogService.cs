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
    /// 实现IV测试数据打印日志管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class IVTestDataPrintLogService : IIVTestDataPrintLogContract
    {
        /// <summary>
        /// IV测试数据打印日志数据访问读写。
        /// </summary>
        public IIVTestDataPrintLogDataEngine IVTestDataPrintLogDataEngine { get; set; }


        /// <summary>
        /// 添加IV测试数据打印日志。
        /// </summary>
        /// <param name="obj">IV测试数据打印日志数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(IVTestDataPrintLog obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.IVTestDataPrintLogDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.IVTestDataPrintLogService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.IVTestDataPrintLogDataEngine.Insert(obj);
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
        /// 修改IV测试数据打印日志。
        /// </summary>
        /// <param name="obj">IV测试数据打印日志数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(IVTestDataPrintLog obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.IVTestDataPrintLogDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.IVTestDataPrintLogService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.IVTestDataPrintLogDataEngine.Update(obj);
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
        /// 删除IV测试数据打印日志。
        /// </summary>
        /// <param name="key">IV测试数据打印日志标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(IVTestDataPrintLogKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.IVTestDataPrintLogDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.IVTestDataPrintLogService_IsNotExists, key);
                return result;
            }
            try
            {
                this.IVTestDataPrintLogDataEngine.Delete(key);
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
        /// 获取IV测试数据打印日志数据。
        /// </summary>
        /// <param name="key">IV测试数据打印日志标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;IVTestDataPrintLog&gt;" />,IV测试数据打印日志数据.</returns>
        public MethodReturnResult<IVTestDataPrintLog> Get(IVTestDataPrintLogKey key)
        {
            MethodReturnResult<IVTestDataPrintLog> result = new MethodReturnResult<IVTestDataPrintLog>();
            if (!this.IVTestDataPrintLogDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.IVTestDataPrintLogService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.IVTestDataPrintLogDataEngine.Get(key);
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
        /// 获取IV测试数据打印日志数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;IVTestDataPrintLog&gt;" />,IV测试数据打印日志数据集合。</returns>
        public MethodReturnResult<IList<IVTestDataPrintLog>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<IVTestDataPrintLog>> result = new MethodReturnResult<IList<IVTestDataPrintLog>>();
            try
            {
                result.Data = this.IVTestDataPrintLogDataEngine.Get(cfg);
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
