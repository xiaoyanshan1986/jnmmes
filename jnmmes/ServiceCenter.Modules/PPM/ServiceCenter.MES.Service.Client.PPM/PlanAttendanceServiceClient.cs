// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.PPM
// Author           : 方军
// Created          : 03-02-206
//
// Last Modified By : 
// Last Modified On : 
// ***********************************************************************
// <copyright file="PlanAttendanceServiceClient.cs" company="">
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
    /// 定义日排班计划管理契约调用方式。
    /// </summary>
    public class PlanAttendanceServiceClient : ClientBase<IPlanAttendanceContract>, IPlanAttendanceContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlanAttendanceServiceClient" /> class.
        /// </summary>
        public PlanAttendanceServiceClient()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanAttendanceServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public PlanAttendanceServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanAttendanceServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public PlanAttendanceServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanAttendanceServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public PlanAttendanceServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanAttendanceServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public PlanAttendanceServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }
        
        /// <summary>
        /// 添加日排班计划。
        /// </summary>
        /// <param name="obj">日排班计划数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(PlanAttendance obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(PlanAttendance obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }

        /// <summary>
        /// 修改日排班计划。
        /// </summary>
        /// <param name="obj">日排班计划数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(PlanAttendance obj)
        {
            return base.Channel.Modify(obj);
        }

        public ServiceCenter.Model.MethodReturnResult Modify(IList<PlanAttendance> lst)
        {
            return base.Channel.Modify(lst);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(PlanAttendance obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }

        public async Task<MethodReturnResult> ModifyAsync(IList<PlanAttendance> lst)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(lst);
            });
        }

        /// <summary>
        /// 删除日排班计划。
        /// </summary>
        /// <param name="key">日排班计划标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(PlanAttendanceKey key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">日排班计划标识符.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(PlanAttendanceKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }

        /// <summary>
        /// 获取日排班计划数据。
        /// </summary>
        /// <param name="key">日排班计划标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;PlanAttendance&gt;" />,日排班计划数据.</returns>
        public MethodReturnResult<PlanAttendance> Get(PlanAttendanceKey key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;PlanAttendance&gt;&gt;.</returns>
        public async Task<MethodReturnResult<PlanAttendance>> GetAsync(PlanAttendanceKey key)
        {
            return await Task.Run<MethodReturnResult<PlanAttendance>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取日排班计划数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;PlanAttendance&gt;&gt;，日排班计划数据集合.</returns>
        public MethodReturnResult<IList<PlanAttendance>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}
