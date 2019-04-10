// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.LSM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="MaterialScrapServiceClient.cs" company="">
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
    /// 定义报废单管理契约调用方式。
    /// </summary>
    public class MaterialScrapServiceClient : ClientBase<IMaterialScrapContract>, IMaterialScrapContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialScrapServiceClient" /> class.
        /// </summary>
        public MaterialScrapServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialScrapServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public MaterialScrapServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialScrapServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public MaterialScrapServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialScrapServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public MaterialScrapServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialScrapServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public MaterialScrapServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 添加报废单。
        /// </summary>
        /// <param name="obj">报废单数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(MaterialScrap obj, IList<MaterialScrapDetail> lstDetail)
        {
            return base.Channel.Add(obj, lstDetail);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(MaterialScrap obj, IList<MaterialScrapDetail> lstDetail)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj, lstDetail);
            });
        }

        /// <summary>
        /// 获取报废单数据。
        /// </summary>
        /// <param name="key">报废单标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialScrap&gt;" />,报废单数据.</returns>
        public MethodReturnResult<MaterialScrap> Get(string key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;MaterialScrap&gt;&gt;.</returns>
        public async Task<MethodReturnResult<MaterialScrap>> GetAsync(string key)
        {
            return await Task.Run<MethodReturnResult<MaterialScrap>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取报废单数据集合。
        /// </summary>
        /// <param name="cfg">查询报废单.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;MaterialScrap&gt;&gt;，报废单数据集合.</returns>
        public MethodReturnResult<IList<MaterialScrap>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }


        /// <summary>
        /// 获取报废单明细数据。
        /// </summary>
        /// <param name="key">报废单明细标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialScrapDetail&gt;" />,报废单明细数据.</returns>
        public MethodReturnResult<MaterialScrapDetail> GetDetail(MaterialScrapDetailKey key)
        {
            return base.Channel.GetDetail(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;MaterialScrapDetail&gt;&gt;.</returns>
        public async Task<MethodReturnResult<MaterialScrapDetail>> GetDetailAsync(MaterialScrapDetailKey key)
        {
            return await Task.Run<MethodReturnResult<MaterialScrapDetail>>(() =>
            {
                return base.Channel.GetDetail(key);
            });
        }

        /// <summary>
        /// 获取报废单明细数据集合。
        /// </summary>
        /// <param name="cfg">查询报废单明细.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;MaterialScrapDetail&gt;&gt;，报废单明细数据集合.</returns>
        public MethodReturnResult<IList<MaterialScrapDetail>> GetDetail(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.GetDetail(ref cfg);
        }
        public MethodReturnResult<MaterialScrap> Delete(string ScrapNo)
        {
            return base.Channel.Delete(ScrapNo);
        }
    }
}
