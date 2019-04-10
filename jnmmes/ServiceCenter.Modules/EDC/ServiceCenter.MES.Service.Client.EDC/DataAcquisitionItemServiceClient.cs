// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.FMM
// Author           : fangjun
// Created          : 06-04-2017
//
// Last Modified By : 
// Last Modified On : 
// ***********************************************************************
// <copyright file="DataAcquisitionItemServiceClient.cs" company="">
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
using ServiceCenter.MES.Service.Contract.EDC;
using ServiceCenter.MES.Model.EDC;
using ServiceCenter.Model;

/// <summary>
/// The EDC namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.EDC
{
    /// <summary>
    /// 定义采集项目管理契约调用方式。
    /// </summary>
    public class DataAcquisitionItemServiceClient : ClientBase<IDataAcquisitionItemContract>, IDataAcquisitionItemContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataAcquisitionItemServiceClient" /> class.
        /// </summary>
        public DataAcquisitionItemServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAcquisitionItemServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public DataAcquisitionItemServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAcquisitionItemServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public DataAcquisitionItemServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAcquisitionItemServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public DataAcquisitionItemServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAcquisitionItemServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public DataAcquisitionItemServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 添加采集项目。
        /// </summary>
        /// <param name="obj">采集项目数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(DataAcquisitionItem obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(DataAcquisitionItem obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }
        /// <summary>
        /// 修改采集项目。
        /// </summary>
        /// <param name="obj">采集项目数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(DataAcquisitionItem obj)
        {
            return base.Channel.Modify(obj);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(DataAcquisitionItem obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }
        /// <summary>
        /// 删除采集项目。
        /// </summary>
        /// <param name="key">采集项目标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">采集项目标识符.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(string key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }

        /// <summary>
        /// 获取采集项目数据。
        /// </summary>
        /// <param name="key">采集项目标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;DataAcquisitionItem&gt;" />,采集项目数据.</returns>
        public MethodReturnResult<DataAcquisitionItem> Get(string key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;DataAcquisitionItem&gt;&gt;.</returns>
        public async Task<MethodReturnResult<DataAcquisitionItem>> GetAsync(string key)
        {
            return await Task.Run<MethodReturnResult<DataAcquisitionItem>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取采集项目数据集合。
        /// </summary>
        /// <param name="cfg">查询采集项目.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;DataAcquisitionItem&gt;&gt;，采集项目数据集合.</returns>
        public MethodReturnResult<IList<DataAcquisitionItem>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}
