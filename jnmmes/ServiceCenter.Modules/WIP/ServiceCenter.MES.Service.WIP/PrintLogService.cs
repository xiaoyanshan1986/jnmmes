using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Service.WIP.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.WIP
{
    /// <summary>
    /// 实现打印操作日志管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class PrintLogService : IPrintLogContract
    {
        /// <summary>
        /// 打印操作日志数据访问读写。
        /// </summary>
        public IPrintLogDataEngine PrintLogDataEngine { get; set; }
        
        /// <summary>
        /// 添加打印操作日志。
        /// </summary>
        /// <param name="obj">打印操作日志数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(PrintLog obj)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                //if (this.PrintLogDataEngine.IsExists(obj.Key))                
                //{
                //    result.Code = 1001;
                //    result.Message = String.Format(StringResource.PrintLogService_IsExists, obj.Key);
                //    return result;
                //}

                this.PrintLogDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }
        
        /// <summary>
        /// 修改打印操作日志。
        /// </summary>
        /// <param name="obj">打印操作日志数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(PrintLog obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.PrintLogDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PrintLogService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.PrintLogDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 删除打印操作日志。
        /// </summary>
        /// <param name="key">打印操作日志标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.PrintLogDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PrintLogService_IsNotExists, key);
                return result;
            }
            try
            {
                this.PrintLogDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取打印操作日志数据。
        /// </summary>
        /// <param name="key">打印操作日志标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;PrintLog&gt;" />,打印操作日志数据.</returns>
        public MethodReturnResult<PrintLog> Get(string key)
        {
            MethodReturnResult<PrintLog> result = new MethodReturnResult<PrintLog>();
            if (!this.PrintLogDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PrintLogService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.PrintLogDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取打印操作日志数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;PrintLog&gt;" />,打印操作日志数据集合。</returns>
        public MethodReturnResult<IList<PrintLog>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<PrintLog>> result = new MethodReturnResult<IList<PrintLog>>();
            try
            {
                result.Data = this.PrintLogDataEngine.Get(cfg);
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
