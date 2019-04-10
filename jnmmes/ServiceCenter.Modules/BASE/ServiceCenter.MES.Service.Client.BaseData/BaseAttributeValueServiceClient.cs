// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.FMM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="BaseAttributeValueServiceClient.cs" company="">
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
    /// 定义基础数据值管理契约调用方式。
    /// </summary>
    public class BaseAttributeValueServiceClient : ClientBase<IBaseAttributeValueContract>, IBaseAttributeValueContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAttributeValueServiceClient" /> class.
        /// </summary>
        public BaseAttributeValueServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAttributeValueServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public BaseAttributeValueServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAttributeValueServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public BaseAttributeValueServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAttributeValueServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public BaseAttributeValueServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAttributeValueServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public BaseAttributeValueServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }



        /// <summary>
        /// 添加基础数据值。
        /// </summary>
        /// <param name="obj">基础数据值数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(BaseAttributeValue obj)
        {
            return base.Channel.Add(obj);
        }

        public async Task<MethodReturnResult> AddAsync(BaseAttributeValue obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }

        /// <summary>
        /// 修改基础数据值。
        /// </summary>
        /// <param name="obj">基础数据值数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(BaseAttributeValue obj)
        {
            return base.Channel.Modify(obj);
        }

        public async Task<MethodReturnResult> ModifyAsync(BaseAttributeValue obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }

        /// <summary>
        /// 删除基础数据值。
        /// </summary>
        /// <param name="key">基础数据值标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(BaseAttributeValueKey key)
        {
            return base.Channel.Delete(key);
        }


        public async Task<MethodReturnResult> DeleteAsync(BaseAttributeValueKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }
        /// <summary>
        /// 获取基础数据值数据。
        /// </summary>
        /// <param name="key">基础数据值标识符。</param>
        /// <returns><see cref="MethodReturnResult&lt;BaseAttributeValue&gt;" />,基础数据值数据.</returns>
        public MethodReturnResult<BaseAttributeValue> Get(BaseAttributeValueKey key)
        {
            return base.Channel.Get(key);
        }

        public async Task<MethodReturnResult<BaseAttributeValue>> GetAsync(BaseAttributeValueKey key)
        {
            return await Task.Run<MethodReturnResult<BaseAttributeValue>>(() =>
            {
                return base.Channel.Get(key);
            });
        }
        /// <summary>
        /// 获取基础数据值数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;BaseAttributeValue&gt;&gt;，基础数据值数据集合.</returns>
        public ServiceCenter.Model.MethodReturnResult<IList<BaseAttributeValue>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }



        public MethodReturnResult Add(IList<BaseAttributeValue> lst)
        {
            return base.Channel.Add(lst);
        }

        public async Task<MethodReturnResult> AddAsync(IList<BaseAttributeValue> lst)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(lst);
            });
        }

        public MethodReturnResult Modify(IList<BaseAttributeValue> lst)
        {
            return base.Channel.Modify(lst);
        }

        public async Task<MethodReturnResult> ModifyAsync(IList<BaseAttributeValue> lst)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(lst);
            });
        }

        public MethodReturnResult Delete(string categoryName, int itemOrder)
        {
            return base.Channel.Delete(categoryName,itemOrder);
        }

        public async Task<MethodReturnResult> DeleteAsync(string categoryName, int itemOrder)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(categoryName,itemOrder);
            });
        }
    }
}
