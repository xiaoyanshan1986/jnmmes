﻿// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.FMM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="ProductionLineServiceClient.cs" company="">
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
using ServiceCenter.MES.Service.Contract.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.Model;

/// <summary>
/// The FMM namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.FMM
{
    /// <summary>
    /// 定义生产线管理契约调用方式。
    /// </summary>
    public class ProductionLineServiceClient : ClientBase<IProductionLineContract>, IProductionLineContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionLineServiceClient" /> class.
        /// </summary>
        public ProductionLineServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionLineServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public ProductionLineServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionLineServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public ProductionLineServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionLineServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public ProductionLineServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionLineServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public ProductionLineServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }



        /// <summary>
        /// 添加生产线。
        /// </summary>
        /// <param name="obj">生产线数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(ProductionLine obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(ProductionLine obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }
        /// <summary>
        /// 修改生产线。
        /// </summary>
        /// <param name="obj">生产线数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(ProductionLine obj)
        {
            return base.Channel.Modify(obj);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(ProductionLine obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }
        /// <summary>
        /// 删除生产线。
        /// </summary>
        /// <param name="key">生产线标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">生产线标识符.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(string key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }
        /// <summary>
        /// 获取生产线数据。
        /// </summary>
        /// <param name="key">生产线标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;ProductionLine&gt;" />,生产线数据.</returns>
        public MethodReturnResult<ProductionLine> Get(string key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;ProductionLine&gt;&gt;.</returns>
        public async Task<MethodReturnResult<ProductionLine>> GetAsync(string key)
        {
            return await Task.Run<MethodReturnResult<ProductionLine>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取生产线数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;ProductionLine&gt;&gt;，生产线数据集合.</returns>
        public MethodReturnResult<IList<ProductionLine>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}