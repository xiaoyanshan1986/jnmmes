// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.PPM
// Author           : junhai
// Created          : 11-06-2017
//
// Last Modified By : junhai
// Last Modified On : 11-06-2017
// ***********************************************************************
// <copyright file="PowersetServiceClient.cs" company="">
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
    /// 定义分档数据管理契约调用方式。
    /// </summary>
    public class WorkOrderGroupDetailServiceClient : ClientBase<IWorkOrderGroupDetailContract>, IWorkOrderGroupDetailContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderGroupDetailServiceClient" /> class.
        /// </summary>
        public WorkOrderGroupDetailServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderGroupDetailServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public WorkOrderGroupDetailServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderGroupDetailServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public WorkOrderGroupDetailServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderGroupDetailServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public WorkOrderGroupDetailServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderGroupDetailServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public WorkOrderGroupDetailServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 添加混工单组规则数据。
        /// </summary>
        /// <param name="obj">混工单组规则数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(WorkOrderGroupDetail obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(WorkOrderGroupDetail obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }
        /// <summary>
        /// 修改混工单组规则数据。
        /// </summary>
        /// <param name="obj">混工单组规则数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(WorkOrderGroupDetail obj)
        {
            return base.Channel.Modify(obj);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(WorkOrderGroupDetail obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }
        /// <summary>
        /// 删除混工单组规则数据。
        /// </summary>
        /// <param name="key">混工单组规则主键。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(WorkOrderGroupDetailKey key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">混工单组规则主键.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(WorkOrderGroupDetailKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }

        /// <summary>
        /// 获取混工单组规则数据。
        /// </summary>
        /// <param name="key">混工单组规则主键.</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderGroupDetail&gt;" />,混工单组规则数据.</returns>
        public MethodReturnResult<WorkOrderGroupDetail> Get(WorkOrderGroupDetailKey key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;WorkOrderGroupDetail&gt;&gt;.</returns>
        public async Task<MethodReturnResult<WorkOrderGroupDetail>> GetAsync(WorkOrderGroupDetailKey key)
        {
            return await Task.Run<MethodReturnResult<WorkOrderGroupDetail>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取混工单组规则数据集合。
        /// </summary>
        /// <param name="cfg">查询混工单组规则数据.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;WorkOrderGroupDetail&gt;&gt;，混工单组规则数据集合.</returns>
        public MethodReturnResult<IList<WorkOrderGroupDetail>> Gets(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Gets(ref cfg);
        }
    }
}
