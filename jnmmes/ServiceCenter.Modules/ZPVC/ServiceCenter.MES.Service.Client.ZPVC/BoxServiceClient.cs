// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.ZPVC
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="BoxServiceClient.cs" company="">
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
using ServiceCenter.MES.Service.Contract.ZPVC;
using ServiceCenter.MES.Model.ZPVC;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.WIP;

/// <summary>
/// The ZPVC namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.ZPVC
{
    /// <summary>
    /// 定义装箱管理契约调用方式。
    /// </summary>
    public class BoxServiceClient : ClientBase<IBoxContract>, IBoxContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoxServiceClient" /> class.
        /// </summary>
        public BoxServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public BoxServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public BoxServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public BoxServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public BoxServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 装箱。
        /// </summary>
        /// <param name="p">装箱参数类。</param>
        /// <returns><see cref="MethodReturnResult"/></returns>
        public MethodReturnResult Box(BoxParameter p)
        {
            return base.Channel.Box(p);
        }

        /// <summary>
        /// 装箱。
        /// </summary>
        /// <param name="p">装箱参数类。</param>
        /// <returns><see cref="MethodReturnResult"/></returns>
        public async Task<MethodReturnResult> BoxAsync(BoxParameter p)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Box(p);
            });
        }
        /// <summary>
        /// 拆箱。
        /// </summary>
        /// <param name="p">拆箱参数类。</param>
        /// <returns>MethodReturnResult.</returns>
        public ServiceCenter.Model.MethodReturnResult Unbox(UnboxParameter p)
        {
            return base.Channel.Unbox(p);
        }
        /// <summary>
        /// 拆箱。
        /// </summary>
        /// <param name="p">拆箱参数类。</param>
        /// <returns>MethodReturnResult.</returns>
        public async Task<MethodReturnResult> UnboxAsync(UnboxParameter p)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Unbox(p);
            });
        }
    }
}
