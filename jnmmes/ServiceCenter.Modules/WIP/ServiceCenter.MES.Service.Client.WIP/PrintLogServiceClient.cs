// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.FMM
// Author           : fangjun
// Created          : 2017-07-17
//
// Last Modified By : 
// Last Modified On : 
// ***********************************************************************
// <copyright file="PrintLogServiceClient.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.Model;

/// <summary>
/// The WIP namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.WIP
{
    /// <summary>
    /// 定义打印操作日志管理契约调用方式。
    /// </summary>
    public class PrintLogServiceClient : ClientBase<IPrintLogContract>, IPrintLogContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrintLogServiceClient" /> class.
        /// </summary>
        public PrintLogServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintLogServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public PrintLogServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintLogServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public PrintLogServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintLogServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public PrintLogServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintLogServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public PrintLogServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 添加打印操作日志。
        /// </summary>
        /// <param name="obj">打印操作日志数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(PrintLog obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(PrintLog obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }

        /// <summary>
        /// 修改打印操作日志。
        /// </summary>
        /// <param name="obj">打印操作日志数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(PrintLog obj)
        {
            return base.Channel.Modify(obj);
        }

        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(PrintLog obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }

        /// <summary>
        /// 删除打印操作日志。
        /// </summary>
        /// <param name="key">打印操作日志标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">打印操作日志标识符.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(string key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }

        /// <summary>
        /// 获取打印操作日志数据。
        /// </summary>
        /// <param name="key">打印操作日志标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;PrintLog&gt;" />,打印操作日志数据.</returns>
        public MethodReturnResult<PrintLog> Get(string key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;PrintLog&gt;&gt;.</returns>
        public async Task<MethodReturnResult<PrintLog>> GetAsync(string key)
        {
            return await Task.Run<MethodReturnResult<PrintLog>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取打印操作日志数据集合。
        /// </summary>
        /// <param name="cfg">查询打印操作日志.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;PrintLog&gt;&gt;，打印操作日志数据集合.</returns>
        public MethodReturnResult<IList<PrintLog>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}
