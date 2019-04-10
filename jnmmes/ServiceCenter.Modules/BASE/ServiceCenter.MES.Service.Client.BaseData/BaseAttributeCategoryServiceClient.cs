// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.FMM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="BaseAttributeCategoryServiceClient.cs" company="">
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
    /// 定义基础数据分类管理契约调用方式。
    /// </summary>
    public class BaseAttributeCategoryServiceClient : ClientBase<IBaseAttributeCategoryContract>, IBaseAttributeCategoryContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAttributeCategoryServiceClient" /> class.
        /// </summary>
        public BaseAttributeCategoryServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAttributeCategoryServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public BaseAttributeCategoryServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAttributeCategoryServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public BaseAttributeCategoryServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAttributeCategoryServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public BaseAttributeCategoryServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAttributeCategoryServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public BaseAttributeCategoryServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }



        /// <summary>
        /// 添加基础数据分类。
        /// </summary>
        /// <param name="obj">基础数据分类数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(BaseAttributeCategory obj)
        {
            return base.Channel.Add(obj);
        }

        public async Task<MethodReturnResult> AddAsync(BaseAttributeCategory obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }

        /// <summary>
        /// 修改基础数据分类。
        /// </summary>
        /// <param name="obj">基础数据分类数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(BaseAttributeCategory obj)
        {
            return base.Channel.Modify(obj);
        }

        public async Task<MethodReturnResult> ModifyAsync(BaseAttributeCategory obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }

        /// <summary>
        /// 删除基础数据分类。
        /// </summary>
        /// <param name="key">基础数据分类标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            return base.Channel.Delete(key);
        }


        public async Task<MethodReturnResult> DeleteAsync(string key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }
        /// <summary>
        /// 获取基础数据分类数据。
        /// </summary>
        /// <param name="key">基础数据分类标识符。</param>
        /// <returns><see cref="MethodReturnResult&lt;BaseAttributeCategory&gt;" />,基础数据分类数据.</returns>
        public MethodReturnResult<BaseAttributeCategory> Get(string key)
        {
            return base.Channel.Get(key);
        }

        public async Task<MethodReturnResult<BaseAttributeCategory>> GetAsync(string key)
        {
            return await Task.Run<MethodReturnResult<BaseAttributeCategory>>(() =>
            {
                return base.Channel.Get(key);
            });
        }
        /// <summary>
        /// 获取基础数据分类数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;BaseAttributeCategory&gt;&gt;，基础数据分类数据集合.</returns>
        public ServiceCenter.Model.MethodReturnResult<IList<BaseAttributeCategory>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }

    }
}
