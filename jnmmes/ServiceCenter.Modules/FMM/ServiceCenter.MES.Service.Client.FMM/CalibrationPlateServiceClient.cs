// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.FMM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="CalibrationPlateServiceClient.cs" company="">
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
    /// 定义校准板规则属性管理契约调用方式。
    /// </summary>
    public class CalibrationPlateServiceClient : ClientBase<ICalibrationPlateContract>, ICalibrationPlateContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalibrationPlateServiceClient" /> class.
        /// </summary>
        public CalibrationPlateServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalibrationPlateServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public CalibrationPlateServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalibrationPlateServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public CalibrationPlateServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalibrationPlateServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public CalibrationPlateServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalibrationPlateServiceClient" /> class.
        /// </summary>
        /// <param name="校准板ding">The 校准板ding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public CalibrationPlateServiceClient(System.ServiceModel.Channels.Binding Binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(Binding, remoteAddress)
        {
        }

        /// <summary>
        /// 添加校准板规则属性。
        /// </summary>
        /// <param name="obj">校准板规则属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(CalibrationPlate obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(CalibrationPlate obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }
        /// <summary>
        /// 修改校准板规则属性。
        /// </summary>
        /// <param name="obj">校准板规则属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(CalibrationPlate obj)
        {
            return base.Channel.Modify(obj);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(CalibrationPlate obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }
        /// <summary>
        /// 删除校准板规则属性。
        /// </summary>
        /// <param name="key">校准板规则属性标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">校准板规则属性标识符.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(string key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }

        /// <summary>
        /// 获取校准板规则属性数据。
        /// </summary>
        /// <param name="key">校准板规则属性标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;CalibrationPlate&gt;" />,校准板规则属性数据.</returns>
        public MethodReturnResult<CalibrationPlate> Get(string key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;CalibrationPlate&gt;&gt;.</returns>
        public async Task<MethodReturnResult<CalibrationPlate>> GetAsync(string key)
        {
            return await Task.Run<MethodReturnResult<CalibrationPlate>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取校准板规则属性数据集合。
        /// </summary>
        /// <param name="cfg">查询校准板规则属性.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;CalibrationPlate&gt;&gt;，校准板规则属性数据集合.</returns>
        public MethodReturnResult<IList<CalibrationPlate>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}
