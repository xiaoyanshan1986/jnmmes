// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.PPM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="PlanDayServiceClient.cs" company="">
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
using ServiceCenter.MES.Service.Contract.PPM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.Model;

/// <summary>
/// The PPM namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.PPM
{
    /// <summary>
    /// 定义日生产计划管理契约调用方式。
    /// </summary>
    public class PlanDayServiceClient : ClientBase<IPlanDayContract>, IPlanDayContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlanDayServiceClient" /> class.
        /// </summary>
        public PlanDayServiceClient()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanDayServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public PlanDayServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanDayServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public PlanDayServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanDayServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public PlanDayServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanDayServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public PlanDayServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }
        
        /// <summary>
        /// 添加日生产计划。
        /// </summary>
        /// <param name="obj">日生产计划数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(PlanDay obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(PlanDay obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }

        /// <summary>
        /// 修改日生产计划。
        /// </summary>
        /// <param name="obj">日生产计划数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(PlanDay obj)
        {
            return base.Channel.Modify(obj);
        }

        public ServiceCenter.Model.MethodReturnResult Modify(IList<PlanDay> lst)
        {
            return base.Channel.Modify(lst);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(PlanDay obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }

        public async Task<MethodReturnResult> ModifyAsync(IList<PlanDay> lst)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(lst);
            });
        }

        /// <summary>
        /// 删除日生产计划。
        /// </summary>
        /// <param name="key">日生产计划标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(PlanDayKey key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">日生产计划标识符.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(PlanDayKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }

        /// <summary>
        /// 获取日生产计划数据。
        /// </summary>
        /// <param name="key">日生产计划标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;PlanDay&gt;" />,日生产计划数据.</returns>
        public MethodReturnResult<PlanDay> Get(PlanDayKey key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;PlanDay&gt;&gt;.</returns>
        public async Task<MethodReturnResult<PlanDay>> GetAsync(PlanDayKey key)
        {
            return await Task.Run<MethodReturnResult<PlanDay>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取日生产计划数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;PlanDay&gt;&gt;，日生产计划数据集合.</returns>
        public MethodReturnResult<IList<PlanDay>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}
