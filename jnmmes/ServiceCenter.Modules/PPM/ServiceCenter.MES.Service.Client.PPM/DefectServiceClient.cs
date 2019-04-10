﻿// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.PPM
// Author           : 方军
// Created          : 03-11-206
//
// Last Modified By : 
// Last Modified On : 
// ***********************************************************************
// <copyright file="DefectServiceClient.cs" company="">
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
    /// 定义日不良信息管理契约调用方式。
    /// </summary>
    public class DefectServiceClient : ClientBase<IDefectContract>, IDefectContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefectServiceClient" /> class.
        /// </summary>
        public DefectServiceClient()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefectServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public DefectServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefectServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public DefectServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefectServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public DefectServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefectServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public DefectServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }
        
        /// <summary>
        /// 添加日不良信息。
        /// </summary>
        /// <param name="obj">日不良数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Defect obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(Defect obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }

        /// <summary>
        /// 修改日不良数据。
        /// </summary>
        /// <param name="obj">日不良数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(Defect obj)
        {
            return base.Channel.Modify(obj);
        }

        public ServiceCenter.Model.MethodReturnResult Modify(IList<Defect> lst)
        {
            return base.Channel.Modify(lst);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(Defect obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }

        public async Task<MethodReturnResult> ModifyAsync(IList<Defect> lst)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(lst);
            });
        }

        /// <summary>
        /// 删除日不良数据。
        /// </summary>
        /// <param name="key">日不良标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(DefectKey key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">日不良标识符.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(DefectKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }

        /// <summary>
        /// 获取日不良数据。
        /// </summary>
        /// <param name="key">日不良标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;Defect&gt;" />,日不良数据.</returns>
        public MethodReturnResult<Defect> Get(DefectKey key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;Defect&gt;&gt;.</returns>
        public async Task<MethodReturnResult<Defect>> GetAsync(DefectKey key)
        {
            return await Task.Run<MethodReturnResult<Defect>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取日不良数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;Defect&gt;&gt;，日不良数据集合.</returns>
        public MethodReturnResult<IList<Defect>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}