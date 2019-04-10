// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.QAM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="CheckCategoryDetailServiceClient.cs" company="">
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
    /// 定义采集参数管理契约调用方式。
    /// </summary>
    public class CheckCategoryDetailServiceClient : ClientBase<ICheckCategoryDetailContract>, ICheckCategoryDetailContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckCategoryDetailServiceClient" /> class.
        /// </summary>
        public CheckCategoryDetailServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckCategoryDetailServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public CheckCategoryDetailServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckCategoryDetailServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public CheckCategoryDetailServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckCategoryDetailServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public CheckCategoryDetailServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckCategoryDetailServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public CheckCategoryDetailServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 添加采集参数。
        /// </summary>
        /// <param name="obj">采集参数数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(CheckCategoryDetail obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(CheckCategoryDetail obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }
        /// <summary>
        /// 修改采集参数。
        /// </summary>
        /// <param name="obj">采集参数数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(CheckCategoryDetail obj)
        {
            return base.Channel.Modify(obj);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(CheckCategoryDetail obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }
        /// <summary>
        /// 删除采集参数。
        /// </summary>
        /// <param name="key">采集参数标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(CheckCategoryDetailKey key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">采集参数标识符.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(CheckCategoryDetailKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }

        /// <summary>
        /// 获取采集参数数据。
        /// </summary>
        /// <param name="key">采集参数标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;CheckCategoryDetail&gt;" />,采集参数数据.</returns>
        public MethodReturnResult<CheckCategoryDetail> Get(CheckCategoryDetailKey key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;CheckCategoryDetail&gt;&gt;.</returns>
        public async Task<MethodReturnResult<CheckCategoryDetail>> GetAsync(CheckCategoryDetailKey key)
        {
            return await Task.Run<MethodReturnResult<CheckCategoryDetail>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取采集参数数据集合。
        /// </summary>
        /// <param name="cfg">查询采集参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;CheckCategoryDetail&gt;&gt;，采集参数数据集合.</returns>
        public MethodReturnResult<IList<CheckCategoryDetail>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}
