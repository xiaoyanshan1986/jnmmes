// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.PPM
// Author           : 方军
// Created          : 03-02-206
//
// Last Modified By : 
// Last Modified On : 
// ***********************************************************************
// <copyright file="TargetParameterServiceClient.cs" company="">
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
using ServiceCenter.MES.Service.Contract.PPM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.Model;

/// <summary>
/// The PPM namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.PPM
{
    /// <summary>
    /// 定义日目标参数管理契约调用方式。
    /// </summary>
    public class TargetParameterServiceClient : ClientBase<ITargetParameterContract>, ITargetParameterContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetParameterServiceClient" /> class.
        /// </summary>
        public TargetParameterServiceClient()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetParameterServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public TargetParameterServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetParameterServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public TargetParameterServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetParameterServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public TargetParameterServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetParameterServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public TargetParameterServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }
        
        /// <summary>
        /// 添加日目标参数。
        /// </summary>
        /// <param name="obj">日目标参数数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(TargetParameter obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(TargetParameter obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }

        /// <summary>
        /// 修改日目标参数。
        /// </summary>
        /// <param name="obj">日目标参数数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(TargetParameter obj)
        {
            return base.Channel.Modify(obj);
        }

        public ServiceCenter.Model.MethodReturnResult Modify(IList<TargetParameter> lst)
        {
            return base.Channel.Modify(lst);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(TargetParameter obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }

        public async Task<MethodReturnResult> ModifyAsync(IList<TargetParameter> lst)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(lst);
            });
        }

        /// <summary>
        /// 删除日目标参数。
        /// </summary>
        /// <param name="key">日目标参数标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(TargetParameterKey key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">日目标参数标识符.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(TargetParameterKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }

        /// <summary>
        /// 获取日目标参数数据。
        /// </summary>
        /// <param name="key">日目标参数标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;TargetParameter&gt;" />,日目标参数数据.</returns>
        public MethodReturnResult<TargetParameter> Get(TargetParameterKey key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;TargetParameter&gt;&gt;.</returns>
        public async Task<MethodReturnResult<TargetParameter>> GetAsync(TargetParameterKey key)
        {
            return await Task.Run<MethodReturnResult<TargetParameter>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取日目标参数数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;TargetParameter&gt;&gt;，日目标参数数据集合.</returns>
        public MethodReturnResult<IList<TargetParameter>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}
