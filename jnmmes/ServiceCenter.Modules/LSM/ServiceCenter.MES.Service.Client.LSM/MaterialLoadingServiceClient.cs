// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.LSM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="MaterialLoadingServiceClient.cs" company="">
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
    /// 定义上料管理契约调用方式。
    /// </summary>
    public class MaterialLoadingServiceClient : ClientBase<IMaterialLoadingContract>, IMaterialLoadingContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialLoadingServiceClient" /> class.
        /// </summary>
        public MaterialLoadingServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialLoadingServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public MaterialLoadingServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialLoadingServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public MaterialLoadingServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialLoadingServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public MaterialLoadingServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialLoadingServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public MaterialLoadingServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 添加上料。
        /// </summary>
        /// <param name="obj">上料数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(MaterialLoading obj, IList<MaterialLoadingDetail> lstDetail)
        {
            return base.Channel.Add(obj, lstDetail);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(MaterialLoading obj, IList<MaterialLoadingDetail> lstDetail)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj, lstDetail);
            });
        }

        /// <summary>
        /// 获取上料数据。
        /// </summary>
        /// <param name="key">上料标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialLoading&gt;" />,上料数据.</returns>
        public MethodReturnResult<MaterialLoading> Get(string key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;MaterialLoading&gt;&gt;.</returns>
        public async Task<MethodReturnResult<MaterialLoading>> GetAsync(string key)
        {
            return await Task.Run<MethodReturnResult<MaterialLoading>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取上料数据集合。
        /// </summary>
        /// <param name="cfg">查询上料.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;MaterialLoading&gt;&gt;，上料数据集合.</returns>
        public MethodReturnResult<IList<MaterialLoading>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }




        /// <summary>
        /// 获取上料明细数据。
        /// </summary>
        /// <param name="key">上料明细标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialLoadingDetail&gt;" />,上料明细数据.</returns>
        public MethodReturnResult<MaterialLoadingDetail> GetDetail(MaterialLoadingDetailKey key)
        {
            return base.Channel.GetDetail(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;MaterialLoadingDetail&gt;&gt;.</returns>
        public async Task<MethodReturnResult<MaterialLoadingDetail>> GetDetailAsync(MaterialLoadingDetailKey key)
        {
            return await Task.Run<MethodReturnResult<MaterialLoadingDetail>>(() =>
            {
                return base.Channel.GetDetail(key);
            });
        }

        /// <summary>
        /// 获取上料明细数据集合。
        /// </summary>
        /// <param name="cfg">查询上料明细.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;MaterialLoadingDetail&gt;&gt;，上料明细数据集合.</returns>
        public MethodReturnResult<IList<MaterialLoadingDetail>> GetDetail(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.GetDetail(ref cfg);
        }
    }
}
