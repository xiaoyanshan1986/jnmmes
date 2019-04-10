// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.ZPVM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="IVTestDataTransferServiceClient.cs" company="">
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
    /// 定义IV测试数据移转契约调用方式。
    /// </summary>
    public class IVTestDataTransferServiceClient : ClientBase<IIVTestDataTransferContract>, IIVTestDataTransferContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IVTestDataTransferServiceClient" /> class.
        /// </summary>
        public IVTestDataTransferServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IVTestDataTransferServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public IVTestDataTransferServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IVTestDataTransferServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public IVTestDataTransferServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IVTestDataTransferServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public IVTestDataTransferServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IVTestDataTransferServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public IVTestDataTransferServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 移转IV测试数据。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Transfer(IVTestDataTransferParameter p)
        {
            return base.Channel.Transfer(p);
        }
        /// <summary>
        /// 移转IV测试数据。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> TransferAsync(IVTestDataTransferParameter p)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Transfer(p);
            });
        }

    }
}
