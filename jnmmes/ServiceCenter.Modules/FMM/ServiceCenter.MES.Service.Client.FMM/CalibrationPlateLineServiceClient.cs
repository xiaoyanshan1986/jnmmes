// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.FMM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="CalibrationPlateLineServiceClient.cs" company="">
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
using ServiceCenter.MES.Service.Contract.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.Model;

/// <summary>
/// The FMM namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.FMM
{
    /// <summary>
    /// 定义Bin规则属性管理契约调用方式。
    /// </summary>
    public class CalibrationPlateLineServiceClient : ClientBase<ICalibrationPlateLineContract>, ICalibrationPlateLineContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalibrationPlateLineServiceClient" /> class.
        /// </summary>
        public CalibrationPlateLineServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalibrationPlateLineServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public CalibrationPlateLineServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalibrationPlateLineServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public CalibrationPlateLineServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalibrationPlateLineServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public CalibrationPlateLineServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalibrationPlateLineServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public CalibrationPlateLineServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 添加Bin规则属性。
        /// </summary>
        /// <param name="obj">Bin规则属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(CalibrationPlateLine obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(CalibrationPlateLine obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }
        /// <summary>
        /// 修改Bin规则属性。
        /// </summary>
        /// <param name="obj">Bin规则属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(CalibrationPlateLine obj)
        {
            return base.Channel.Modify(obj);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(CalibrationPlateLine obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }
        /// <summary>
        /// 删除Bin规则属性。
        /// </summary>
        /// <param name="key">Bin规则属性标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(CalibrationPlateLineKey key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">Bin规则属性标识符.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(CalibrationPlateLineKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }

        /// <summary>
        /// 获取Bin规则属性数据。
        /// </summary>
        /// <param name="key">Bin规则属性标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;CalibrationPlateLine&gt;" />,Bin规则属性数据.</returns>
        public MethodReturnResult<CalibrationPlateLine> Get(CalibrationPlateLineKey key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;CalibrationPlateLine&gt;&gt;.</returns>
        public async Task<MethodReturnResult<CalibrationPlateLine>> GetAsync(CalibrationPlateLineKey key)
        {
            return await Task.Run<MethodReturnResult<CalibrationPlateLine>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取Bin规则属性数据集合。
        /// </summary>
        /// <param name="cfg">查询Bin规则属性.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;CalibrationPlateLine&gt;&gt;，Bin规则属性数据集合.</returns>
        public MethodReturnResult<IList<CalibrationPlateLine>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }

    }
}
