// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.QAM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="CheckSettingServiceClient.cs" company="">
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
using ServiceCenter.MES.Service.Contract.QAM;
using ServiceCenter.MES.Model.QAM;
using ServiceCenter.Model;

/// <summary>
/// The QAM namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.QAM
{
    /// <summary>
    /// 定义检验设置管理契约调用方式。
    /// </summary>
    public class CheckSettingServiceClient : ClientBase<ICheckSettingContract>, ICheckSettingContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckSettingServiceClient" /> class.
        /// </summary>
        public CheckSettingServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckSettingServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public CheckSettingServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckSettingServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public CheckSettingServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckSettingServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public CheckSettingServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckSettingServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public CheckSettingServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 添加检验设置。
        /// </summary>
        /// <param name="obj">检验设置数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(CheckSetting obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(CheckSetting obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }
        /// <summary>
        /// 修改检验设置。
        /// </summary>
        /// <param name="obj">检验设置数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(CheckSetting obj)
        {
            return base.Channel.Modify(obj);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(CheckSetting obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }
        /// <summary>
        /// 删除检验设置。
        /// </summary>
        /// <param name="key">检验设置标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">检验设置标识符.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(string key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }

        /// <summary>
        /// 获取检验设置数据。
        /// </summary>
        /// <param name="key">检验设置标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;CheckSetting&gt;" />,检验设置数据.</returns>
        public MethodReturnResult<CheckSetting> Get(string key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;CheckSetting&gt;&gt;.</returns>
        public async Task<MethodReturnResult<CheckSetting>> GetAsync(string key)
        {
            return await Task.Run<MethodReturnResult<CheckSetting>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取检验设置数据集合。
        /// </summary>
        /// <param name="cfg">查询检验设置.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;CheckSetting&gt;&gt;，检验设置数据集合.</returns>
        public MethodReturnResult<IList<CheckSetting>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}
