// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.FMM
// Author           : fangjun
// Created          : 2017-07-19 13:28:00
//
// Last Modified By : 
// Last Modified On : 
// ***********************************************************************
// <copyright file="MaterialPrintSetServiceClient.cs" company="">
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
    /// 定义产品产品标签设置管理契约调用方式
    /// </summary>
    public class MaterialPrintSetServiceClient : ClientBase<IMaterialPrintSetContract>, IMaterialPrintSetContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialPrintSetServiceClient" /> class.
        /// </summary>
        public MaterialPrintSetServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialPrintSetServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public MaterialPrintSetServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialPrintSetServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public MaterialPrintSetServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialPrintSetServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public MaterialPrintSetServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialPrintSetServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public MaterialPrintSetServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 添加产品产品标签设置。
        /// </summary>
        /// <param name="obj">产品产品标签设置数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(MaterialPrintSet obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(MaterialPrintSet obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }
        /// <summary>
        /// 修改产品产品标签设置。
        /// </summary>
        /// <param name="obj">产品产品标签设置数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(MaterialPrintSet obj)
        {
            return base.Channel.Modify(obj);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(MaterialPrintSet obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }
        /// <summary>
        /// 删除产品产品标签设置。
        /// </summary>
        /// <param name="key">产品产品标签设置标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(MaterialPrintSetKey key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">产品产品标签设置标识符.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(MaterialPrintSetKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }

        /// <summary>
        /// 获取产品产品标签设置数据。
        /// </summary>
        /// <param name="key">产品产品标签设置标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialPrintSet&gt;" />,产品产品标签设置数据.</returns>
        public MethodReturnResult<MaterialPrintSet> Get(MaterialPrintSetKey key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;MaterialPrintSet&gt;&gt;.</returns>
        public async Task<MethodReturnResult<MaterialPrintSet>> GetAsync(MaterialPrintSetKey key)
        {
            return await Task.Run<MethodReturnResult<MaterialPrintSet>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取产品产品标签设置数据集合。
        /// </summary>
        /// <param name="cfg">查询产品产品标签设置.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;MaterialPrintSet&gt;&gt;，产品产品标签设置数据集合.</returns>
        public MethodReturnResult<IList<MaterialPrintSet>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}
