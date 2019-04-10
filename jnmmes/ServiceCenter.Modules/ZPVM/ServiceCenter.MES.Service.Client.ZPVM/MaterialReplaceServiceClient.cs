// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.ZPVM
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
using ServiceCenter.MES.Service.Contract.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.Model;

/// <summary>
/// The ZPVM namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.ZPVM
{
    /// <summary>
    /// 定义分档数据管理契约调用方式。
    /// </summary>
    public class MaterialReplaceServiceClient : ClientBase<IMaterialReplaceContract>, IMaterialReplaceContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialReplaceServiceClient" /> class.
        /// </summary>
        public MaterialReplaceServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialReplaceServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public MaterialReplaceServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialReplaceServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public MaterialReplaceServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialReplaceServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public MaterialReplaceServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialReplaceServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public MaterialReplaceServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 添加物料替换规则数据。
        /// </summary>
        /// <param name="obj">物料替换规则数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(MaterialReplace obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(MaterialReplace obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }
        /// <summary>
        /// 修改物料替换规则数据。
        /// </summary>
        /// <param name="obj">物料替换规则数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(MaterialReplace obj)
        {
            return base.Channel.Modify(obj);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(MaterialReplace obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }
        /// <summary>
        /// 删除物料替换规则数据。
        /// </summary>
        /// <param name="key">物料替换规则主键。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(MaterialReplaceKey key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">物料替换规则主键.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(MaterialReplaceKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }

        /// <summary>
        /// 获取物料替换规则数据。
        /// </summary>
        /// <param name="key">物料替换规则主键.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialReplace&gt;" />,物料替换规则数据.</returns>
        public MethodReturnResult<MaterialReplace> Get(MaterialReplaceKey key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;MaterialReplace&gt;&gt;.</returns>
        public async Task<MethodReturnResult<MaterialReplace>> GetAsync(MaterialReplaceKey key)
        {
            return await Task.Run<MethodReturnResult<MaterialReplace>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取物料替换规则数据集合。
        /// </summary>
        /// <param name="cfg">查询物料替换规则数据.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;MaterialReplace&gt;&gt;，物料替换规则数据集合.</returns>
        public MethodReturnResult<IList<MaterialReplace>> Gets(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Gets(ref cfg);
        }
    }
}
