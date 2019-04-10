// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.ZPVM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="RuleGradeServiceClient.cs" company="">
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
    /// 定义规则-等级设置数据管理契约调用方式。
    /// </summary>
    public class RuleGradeServiceClient : ClientBase<IRuleGradeContract>, IRuleGradeContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleGradeServiceClient" /> class.
        /// </summary>
        public RuleGradeServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleGradeServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public RuleGradeServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleGradeServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public RuleGradeServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleGradeServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public RuleGradeServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleGradeServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public RuleGradeServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 添加规则-等级设置数据。
        /// </summary>
        /// <param name="obj">规则-等级设置数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(RuleGrade obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(RuleGrade obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }
        /// <summary>
        /// 修改规则-等级设置数据。
        /// </summary>
        /// <param name="obj">规则-等级设置数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(RuleGrade obj)
        {
            return base.Channel.Modify(obj);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(RuleGrade obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }
        /// <summary>
        /// 删除规则-等级设置数据。
        /// </summary>
        /// <param name="key">规则-等级设置数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(RuleGradeKey key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">规则-等级设置数据标识符.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(RuleGradeKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }

        /// <summary>
        /// 获取规则-等级设置数据。
        /// </summary>
        /// <param name="key">规则-等级设置数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;RuleGrade&gt;" />,规则-等级设置数据.</returns>
        public MethodReturnResult<RuleGrade> Get(RuleGradeKey key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;RuleGrade&gt;&gt;.</returns>
        public async Task<MethodReturnResult<RuleGrade>> GetAsync(RuleGradeKey key)
        {
            return await Task.Run<MethodReturnResult<RuleGrade>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取规则-等级设置数据集合。
        /// </summary>
        /// <param name="cfg">查询规则-等级设置数据.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;RuleGrade&gt;&gt;，规则-等级设置数据集合.</returns>
        public MethodReturnResult<IList<RuleGrade>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}
