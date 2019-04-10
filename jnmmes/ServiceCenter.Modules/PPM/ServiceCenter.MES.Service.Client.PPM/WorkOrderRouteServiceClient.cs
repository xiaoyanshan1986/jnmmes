// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.PPM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="WorkOrderRouteServiceClient.cs" company="">
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
    /// 定义工单工艺管理契约调用方式。
    /// </summary>
    public class WorkOrderRouteServiceClient : ClientBase<IWorkOrderRouteContract>, IWorkOrderRouteContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderRouteServiceClient" /> class.
        /// </summary>
        public WorkOrderRouteServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderRouteServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public WorkOrderRouteServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderRouteServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public WorkOrderRouteServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderRouteServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public WorkOrderRouteServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderRouteServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public WorkOrderRouteServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 添加工单工艺。
        /// </summary>
        /// <param name="obj">工单工艺数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(WorkOrderRoute obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(WorkOrderRoute obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }
        /// <summary>
        /// 修改工单工艺。
        /// </summary>
        /// <param name="obj">工单工艺数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(WorkOrderRoute obj)
        {
            return base.Channel.Modify(obj);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(WorkOrderRoute obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }
        /// <summary>
        /// 删除工单工艺。
        /// </summary>
        /// <param name="key">工单工艺标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(WorkOrderRouteKey key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">工单工艺标识符.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(WorkOrderRouteKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }

        /// <summary>
        /// 获取工单工艺数据。
        /// </summary>
        /// <param name="key">工单工艺标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderRoute&gt;" />,工单工艺数据.</returns>
        public MethodReturnResult<WorkOrderRoute> Get(WorkOrderRouteKey key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;WorkOrderRoute&gt;&gt;.</returns>
        public async Task<MethodReturnResult<WorkOrderRoute>> GetAsync(WorkOrderRouteKey key)
        {
            return await Task.Run<MethodReturnResult<WorkOrderRoute>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取工单工艺数据集合。
        /// </summary>
        /// <param name="cfg">查询工单工艺.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;WorkOrderRoute&gt;&gt;，工单工艺数据集合.</returns>
        public MethodReturnResult<IList<WorkOrderRoute>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}
