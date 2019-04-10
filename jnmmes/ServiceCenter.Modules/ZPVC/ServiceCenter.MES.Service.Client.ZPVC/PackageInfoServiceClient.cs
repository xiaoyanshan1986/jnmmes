// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.ZPVC
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="PackageInfoServiceClient.cs" company="">
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
using ServiceCenter.MES.Service.Contract.ZPVC;
using ServiceCenter.MES.Model.ZPVC;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.WIP;

/// <summary>
/// The ZPVC namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.ZPVC
{
    /// <summary>
    /// 定义包装信息数据管理契约调用方式。
    /// </summary>
    public class PackageInfoServiceClient : ClientBase<IPackageInfoContract>, IPackageInfoContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PackageInfoServiceClient" /> class.
        /// </summary>
        public PackageInfoServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageInfoServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public PackageInfoServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageInfoServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public PackageInfoServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageInfoServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public PackageInfoServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageInfoServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public PackageInfoServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 添加包装信息数据。
        /// </summary>
        /// <param name="obj">包装信息数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Package p, PackageInfo obj)
        {
            return base.Channel.Add(p,obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(Package p, PackageInfo obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(p,obj);
            });
        }
        /// <summary>
        /// 修改包装信息数据。
        /// </summary>
        /// <param name="obj">包装信息数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(PackageInfo obj)
        {
            return base.Channel.Modify(obj);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(PackageInfo obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }
        /// <summary>
        /// 删除包装信息数据。
        /// </summary>
        /// <param name="key">包装信息数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">包装信息数据标识符.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(string key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }

        /// <summary>
        /// 获取包装信息数据数据。
        /// </summary>
        /// <param name="key">包装信息数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;PackageInfo&gt;" />,包装信息数据数据.</returns>
        public MethodReturnResult<PackageInfo> Get(string key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;PackageInfo&gt;&gt;.</returns>
        public async Task<MethodReturnResult<PackageInfo>> GetAsync(string key)
        {
            return await Task.Run<MethodReturnResult<PackageInfo>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取包装信息数据数据集合。
        /// </summary>
        /// <param name="cfg">查询包装信息数据.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;PackageInfo&gt;&gt;，包装信息数据数据集合.</returns>
        public MethodReturnResult<IList<PackageInfo>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}
