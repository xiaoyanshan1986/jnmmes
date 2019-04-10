// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.FMM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="BaseAttributeServiceClient.cs" company="">
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
using ServiceCenter.MES.Service.Contract.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Service.Client.BaseData
{
    /// <summary>
    /// 定义基础数据管理契约调用方式。
    /// </summary>
    public class BaseAttributeServiceClient : ClientBase<IBaseAttributeContract>, IBaseAttributeContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAttributeServiceClient" /> class.
        /// </summary>
        public BaseAttributeServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAttributeServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public BaseAttributeServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAttributeServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public BaseAttributeServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAttributeServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public BaseAttributeServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAttributeServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public BaseAttributeServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }



        /// <summary>
        /// 添加基础数据。
        /// </summary>
        /// <param name="obj">基础数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(BaseAttribute obj)
        {
            return base.Channel.Add(obj);
        }

        public async Task<MethodReturnResult> AddAsync(BaseAttribute obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }

        /// <summary>
        /// 修改基础数据。
        /// </summary>
        /// <param name="obj">基础数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(BaseAttribute obj)
        {
            return base.Channel.Modify(obj);
        }

        public async Task<MethodReturnResult> ModifyAsync(BaseAttribute obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }

        /// <summary>
        /// 删除基础数据。
        /// </summary>
        /// <param name="key">基础数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(BaseAttributeKey key)
        {
            return base.Channel.Delete(key);
        }


        public async Task<MethodReturnResult> DeleteAsync(BaseAttributeKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }
        /// <summary>
        /// 获取基础数据数据。
        /// </summary>
        /// <param name="key">基础数据标识符。</param>
        /// <returns><see cref="MethodReturnResult&lt;BaseAttribute&gt;" />,基础数据数据.</returns>
        public MethodReturnResult<BaseAttribute> Get(BaseAttributeKey key)
        {
            return base.Channel.Get(key);
        }

        public async Task<MethodReturnResult<BaseAttribute>> GetAsync(BaseAttributeKey key)
        {
            return await Task.Run<MethodReturnResult<BaseAttribute>>(() =>
            {
                return base.Channel.Get(key);
            });
        }
        /// <summary>
        /// 获取基础数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;BaseAttribute&gt;&gt;，基础数据数据集合.</returns>
        public ServiceCenter.Model.MethodReturnResult<IList<BaseAttribute>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }

    }
}
