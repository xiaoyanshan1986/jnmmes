// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.QAM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="CheckDataDetailServiceClient.cs" company="">
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
using ServiceCenter.MES.Service.Contract.QAM;
using ServiceCenter.MES.Model.QAM;
using ServiceCenter.Model;

/// <summary>
/// The QAM namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.QAM
{
    /// <summary>
    /// 定义检验数据明细管理契约调用方式。
    /// </summary>
    public class CheckDataDetailServiceClient : ClientBase<ICheckDataDetailContract>, ICheckDataDetailContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckDataDetailServiceClient" /> class.
        /// </summary>
        public CheckDataDetailServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckDataDetailServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public CheckDataDetailServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckDataDetailServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public CheckDataDetailServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckDataDetailServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public CheckDataDetailServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckDataDetailServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public CheckDataDetailServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 添加检验数据明细。
        /// </summary>
        /// <param name="obj">检验数据明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(CheckDataDetail obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(CheckDataDetail obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }
        /// <summary>
        /// 修改检验数据明细。
        /// </summary>
        /// <param name="obj">检验数据明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(CheckDataDetail obj)
        {
            return base.Channel.Modify(obj);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(CheckDataDetail obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }
        /// <summary>
        /// 删除检验数据明细。
        /// </summary>
        /// <param name="key">检验数据明细标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(CheckDataDetailKey key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">检验数据明细标识符.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(CheckDataDetailKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }

        /// <summary>
        /// 获取检验数据明细数据。
        /// </summary>
        /// <param name="key">检验数据明细标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;CheckDataDetail&gt;" />,检验数据明细数据.</returns>
        public MethodReturnResult<CheckDataDetail> Get(CheckDataDetailKey key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;CheckDataDetail&gt;&gt;.</returns>
        public async Task<MethodReturnResult<CheckDataDetail>> GetAsync(CheckDataDetailKey key)
        {
            return await Task.Run<MethodReturnResult<CheckDataDetail>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取检验数据明细数据集合。
        /// </summary>
        /// <param name="cfg">查询检验数据明细.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;CheckDataDetail&gt;&gt;，检验数据明细数据集合.</returns>
        public MethodReturnResult<IList<CheckDataDetail>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}
