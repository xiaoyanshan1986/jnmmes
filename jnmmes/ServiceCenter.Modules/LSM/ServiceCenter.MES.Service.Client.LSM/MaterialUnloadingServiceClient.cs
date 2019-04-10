// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.LSM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="MaterialUnloadingServiceClient.cs" company="">
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
using ServiceCenter.MES.Service.Contract.LSM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.Model;

/// <summary>
/// The LSM namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.LSM
{
    /// <summary>
    /// 定义下料管理契约调用方式。
    /// </summary>
    public class MaterialUnloadingServiceClient : ClientBase<IMaterialUnloadingContract>, IMaterialUnloadingContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialUnloadingServiceClient" /> class.
        /// </summary>
        public MaterialUnloadingServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialUnloadingServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public MaterialUnloadingServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialUnloadingServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public MaterialUnloadingServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialUnloadingServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public MaterialUnloadingServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialUnloadingServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public MaterialUnloadingServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 添加下料。
        /// </summary>
        /// <param name="obj">下料数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(MaterialUnloading obj, IList<MaterialUnloadingDetail> lstDetail)
        {
            return base.Channel.Add(obj, lstDetail);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(MaterialUnloading obj, IList<MaterialUnloadingDetail> lstDetail)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj, lstDetail);
            });
        }

        /// <summary>
        /// 获取下料数据。
        /// </summary>
        /// <param name="key">下料标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialUnloading&gt;" />,下料数据.</returns>
        public MethodReturnResult<MaterialUnloading> Get(string key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;MaterialUnloading&gt;&gt;.</returns>
        public async Task<MethodReturnResult<MaterialUnloading>> GetAsync(string key)
        {
            return await Task.Run<MethodReturnResult<MaterialUnloading>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取下料数据集合。
        /// </summary>
        /// <param name="cfg">查询下料.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;MaterialUnloading&gt;&gt;，下料数据集合.</returns>
        public MethodReturnResult<IList<MaterialUnloading>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }


        /// <summary>
        /// 获取下料明细数据。
        /// </summary>
        /// <param name="key">下料明细标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialUnloadingDetail&gt;" />,下料明细数据.</returns>
        public MethodReturnResult<MaterialUnloadingDetail> GetDetail(MaterialUnloadingDetailKey key)
        {
            return base.Channel.GetDetail(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;MaterialUnloadingDetail&gt;&gt;.</returns>
        public async Task<MethodReturnResult<MaterialUnloadingDetail>> GetDetailAsync(MaterialUnloadingDetailKey key)
        {
            return await Task.Run<MethodReturnResult<MaterialUnloadingDetail>>(() =>
            {
                return base.Channel.GetDetail(key);
            });
        }

        /// <summary>
        /// 获取下料明细数据集合。
        /// </summary>
        /// <param name="cfg">查询下料明细.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;MaterialUnloadingDetail&gt;&gt;，下料明细数据集合.</returns>
        public MethodReturnResult<IList<MaterialUnloadingDetail>> GetDetail(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.GetDetail(ref cfg);
        }
    }
}
